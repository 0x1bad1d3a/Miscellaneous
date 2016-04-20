using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace program_5
{
    /// <summary>
    /// Interaction logic for AddCourseWindow.xaml
    /// </summary>
    public partial class AddCourseWindow : Window
    {
        public AddCourseWindow()
        {
            InitializeComponent();
        }

        public string cCourse { get; set; }
        public string cISBN { get; set; }

        private void tb_isbn_LostFocus(object sender, RoutedEventArgs e)
        {
            bool valid = false;

            TextBox tb = (TextBox)sender;
            if (Regex.IsMatch(tb.Text, @"\d+-\d+-\d+-\d+") || Regex.IsMatch(tb.Text, @"\d{10}"))
            {
                string s = Regex.Replace(tb.Text, @"\D", "");
                if (s.Length == 10 || s.Length == 13)
                {
                    valid = true;
                    tb.Background = null;
                    cISBN = tb_isbn.Text;
                }
            }

            if (!valid)
            {
                tb.Background = Brushes.Yellow;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tb_isbn.Background != Brushes.Yellow && !tb_isbn.Text.Equals(""))
            {
                cCourse = tb_course.Text.Equals("") ? "OTHER" : tb_course.Text;
                this.DialogResult = true;
                this.Close();
            }            
        }
    }
}
