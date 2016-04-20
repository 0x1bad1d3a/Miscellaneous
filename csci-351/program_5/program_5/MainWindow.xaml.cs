using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.SQLite;
using System.Data;

namespace program_5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SQLiteConnection db;
        Boolean new_db = false;

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists("pix_database.sqlite"))
            {
                SQLiteConnection.CreateFile("pix_database.sqlite");
                new_db = true;
            }

            db = new SQLiteConnection("Data Source=pix_database.sqlite;Version=3;");
            db.Open();

            SQLiteCommand createTables = db.CreateCommand();

            String bookTable = "CREATE TABLE IF NOT EXISTS Book" +
                "(ISBN TEXT PRIMARY KEY NOT NULL," +
                "Title TEXT NOT NULL," +
                "Published DATETIME NOT NULL," +
                "Author TEXT NOT NULL," +
                "Pages INT NOT NULL," +
                "Publisher TEXT NOT NULL);";

            String courseTable = "CREATE TABLE IF NOT EXISTS CourseBook" +
                "(Course TEXT NOT NULL," +
                "ISBN TEXT NOT NULL," +
                "PRIMARY KEY (Course, ISBN)," +
                "FOREIGN KEY (ISBN) REFERENCES Book(ISBN));";

            createTables.CommandText = bookTable;
            createTables.ExecuteNonQuery();
            createTables.Parameters.Clear();
            createTables.CommandText = courseTable;
            createTables.ExecuteNonQuery();
            createTables.Dispose();

            if (new_db)
            {
                SQLiteCommand insertInitial = db.CreateCommand();

                String book351 = "INSERT INTO Book VALUES ('0-672-33690-1', 'C# Unleashed', '2013-03-23', 'Bart de Smet', 1700, 'Sams Publishing');" +
                    "INSERT INTO Book VALUES ('978-0-672-33690-4', 'C# Unleashed', '2013-03-23', 'Bart de Smet', 1700, 'Sams Publishing');";

                String course351 = "INSERT INTO CourseBook VALUES ('CSCI351', '978-0-672-33690-4');" +
                    "INSERT INTO CourseBook VALUES ('CSCI351', '0-672-33690-1');";

                insertInitial.CommandText = book351;
                insertInitial.ExecuteNonQuery();
                insertInitial.Parameters.Clear();
                insertInitial.CommandText = course351;
                insertInitial.ExecuteNonQuery();
                insertInitial.Dispose();
            }
        }

        private void Button_Search(object sender, RoutedEventArgs e)
        {
            SQLiteCommand search = db.CreateCommand();

            String column;
            switch (search_combobox.Text)
            {
                case "ISBN":
                    column = "ISBN"; break;
                case "Title":
                    column = "Title"; break;
                case "Author":
                    column = "Author"; break;
                case "Publisher":
                    column = "Publisher"; break;
                default:
                    column = "OTHER"; break;
            }

            if (!column.Equals("OTHER"))
            {
                search.CommandText = String.Format("SELECT * FROM Book WHERE {0} LIKE @Value", column);
                search.Prepare();
                search.Parameters.AddWithValue("@Value", String.Format("%{0}%", tb_search.Text));
            }
            else
            {
                search.CommandText = String.Format("SELECT c.Course, b.Title, b.Author, b.Publisher, b.ISBN FROM CourseBook AS c INNER JOIN Book AS b ON c.ISBN == b.ISBN WHERE c.Course LIKE @Value");
                search.Prepare();
                search.Parameters.AddWithValue("@Value", String.Format("%{0}%", tb_search.Text));
            }

            SQLiteDataAdapter da = new SQLiteDataAdapter(search);
            DataSet ds = new DataSet();            
            da.Fill(ds);
            dataGrid.DataContext = ds.Tables[0].DefaultView;

            search.Dispose();
            ds.Dispose();
            da.Dispose();
        }

        private void Button_Add_Book(object sender, RoutedEventArgs e)
        {
            AddBookWindow win = new AddBookWindow();
            win.Closed += new EventHandler(addBook_Window_Closed);
            win.ShowDialog();
        }

        private void addBook_Window_Closed(object sender, EventArgs e)
        {
            AddBookWindow win = (AddBookWindow)sender;
            if (win.DialogResult.Equals(true))
            {
                SQLiteCommand insert = db.CreateCommand();
                insert.CommandText = "INSERT INTO Book (ISBN, Title, Published, Author, Pages, Publisher) VALUES (@ISBN, @Title, @Published, @Author, @Pages, @Publisher)";
                insert.Prepare();
                insert.Parameters.AddWithValue("@ISBN", win.bISBN);
                insert.Parameters.AddWithValue("@Title", win.bTitle);
                insert.Parameters.AddWithValue("@Published", win.bPublished);
                insert.Parameters.AddWithValue("@Author", win.bAuthor);
                insert.Parameters.AddWithValue("@Pages", win.bPages);
                insert.Parameters.AddWithValue("@Publisher", win.bPublisher);
                insert.ExecuteNonQuery();
                insert.Dispose();
            }
        }

        private void Button_Add_Course(object sender, RoutedEventArgs e)
        {
            AddCourseWindow win = new AddCourseWindow();
            win.Closed += new EventHandler(addCourse_Window_Closed);
            win.ShowDialog();
        }

        private void addCourse_Window_Closed(object sender, EventArgs e)
        {
            AddCourseWindow win = (AddCourseWindow)sender;
            if (win.DialogResult.Equals(true))
            {
                SQLiteCommand insert = db.CreateCommand();
                insert.CommandText = "INSERT INTO CourseBook (Course, ISBN) VALUES (@Course, @ISBN)";
                insert.Prepare();
                insert.Parameters.AddWithValue("@Course", win.cCourse);
                insert.Parameters.AddWithValue("@ISBN", win.cISBN);
                insert.ExecuteNonQuery();
                insert.Dispose();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Close();
            db.Dispose();
        }
    }
}
