using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace program_5
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        public AddBookWindow()
        {
            InitializeComponent();
        }

        public string bISBN { get; set; }
        public string bTitle { get; set; }
        public DateTime bPublished { get; set; }
        public string bAuthor { get; set; }
        public int bPages { get; set; }
        public string bPublisher { get; set; }

        private void tb_isbn_LostFocus(object sender, RoutedEventArgs e)
        {
            bool valid = false;

            TextBox tb = (TextBox)sender;
            if (Regex.IsMatch(tb.Text, @"\d+-\d+-\d+-\d+") || Regex.IsMatch(tb.Text, @"\d{10}"))
            {
                string s = Regex.Replace(tb.Text, @"\D", "");
                if (s.Length == 10 || s.Length == 13){
                    valid = true;
                    tb.Background = null;
                    bISBN = tb_isbn.Text;
                }
            }

            if (!valid)
            {
                tb.Background = Brushes.Yellow;
            }
        }

        private void tb_published_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            try
            {
                DateTime date = DateTime.ParseExact(tb.Text, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                tb.Background = null;
                bPublished = date;
            }
            catch
            {
                tb.Background = Brushes.Yellow;
            }
        }

        private void tb_pages_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            int x;
            if (Regex.IsMatch(tb.Text, @"\d+") && Int32.TryParse(tb.Text, out x))
            {
                tb.Background = null;
                bPages = x;
            }
            else
            {
                tb.Background = Brushes.Yellow;
            }
        }

        private void tb_title_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (Regex.IsMatch(tb.Text, @".+"))
            {
                tb.Background = null;
                bTitle = tb_title.Text;
            }
            else
            {
                tb.Background = Brushes.Yellow;
            }
        }

        private void tb_author_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (Regex.IsMatch(tb.Text, @".+"))
            {
                tb.Background = null;
                bAuthor = tb_author.Text;
            }
            else
            {
                tb.Background = Brushes.Yellow;
            }
        }

        private void tb_publisher_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (Regex.IsMatch(tb.Text, @".+"))
            {
                tb.Background = null;
                bPublisher = tb_publisher.Text;
            }
            else
            {
                tb.Background = Brushes.Yellow;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool cont = true;
            foreach (TextBox tb in grid.Children.OfType<TextBox>()){
                if (cont)
                {
                    cont = tb.Background == Brushes.Yellow || tb.Text.Equals("") ? false : true;
                }                
            }
            if (cont)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
