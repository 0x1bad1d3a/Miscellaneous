using System;
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

namespace program_4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AboutWindow aboutWindow;
        Dictionary<int, Window> detailWindows = new Dictionary<int, Window>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            switch (mi.Name)
            {
                case "_mitem1":
                    launchDetailChildWindow(1); break;
                case "_mitem2":
                    launchDetailChildWindow(2); break;
                case "_mitem3":
                    launchDetailChildWindow(3); break;
                case "_mitem4":
                    launchDetailChildWindow(4); break;
                case "_mitem5":
                    launchDetailChildWindow(5); break;
                case "_mitem6":
                    launchDetailChildWindow(6); break;
                case "_mitem7":
                    launchDetailChildWindow(7); break;
                default:
                    break;
            }
        }

        private void MenuItem_Click_Cascade(object sender, RoutedEventArgs e)
        {
            IEnumerable<Window> l = detailWindows.Values;
            l = l.OrderBy(w => w.Title);

            double inc = 30;
            foreach (Window w in l)
            {
                w.Left = this.Left + inc;
                w.Top = this.Top + inc;
                w.Focus();
                inc += 30;
            }
        }

        private void MenuItem_Click_Title_Horizontal(object sender, RoutedEventArgs e)
        {
            int numWide = (int)Math.Floor(System.Windows.SystemParameters.PrimaryScreenWidth / 300);

            IEnumerable<Window> l = detailWindows.Values;
            l = l.OrderBy(w => w.Title);

            int count = 0;
            int row = 0;
            int column = 0;

            foreach (Window w in l)
            {
                if (count == numWide)
                {
                    row += 1;
                    column = 0;
                    count = 0;
                }
                w.Left = column * 300;
                w.Top = 60 + row * 300;
                count++;
                column++;
            }
        }

        private void MenuItem_Click_Title_Vertical(object sender, RoutedEventArgs e)
        {
            int numTall = (int)Math.Floor(System.Windows.SystemParameters.PrimaryScreenWidth / 300);
            numTall = 3;

            IEnumerable<Window> l = detailWindows.Values;
            l = l.OrderBy(w => w.Title);

            int count = 0;
            int row = 0;
            int column = 0;

            foreach (Window w in l)
            {
                if (count == numTall)
                {
                    column += 1;
                    row = 0;
                    count = 0;
                }
                w.Left = column * 300;
                w.Top = 60 + row * 300;
                count++;
                row++;
            }
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            if (aboutWindow == null)
            {
                aboutWindow = new AboutWindow();
                aboutWindow.Closed += new EventHandler(aboutWindow_Closed);
                aboutWindow.Owner = this;
                aboutWindow.ShowDialog();
            }
            else
            {
                aboutWindow.Focus();
            }
        }

        private void aboutWindow_Closed(object sender, EventArgs e)
        {
            aboutWindow = null;
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            Rectangle rec = (Rectangle)sender;
            rec.Opacity = 0.3;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            Rectangle rec = (Rectangle)sender;
            rec.Opacity = 0.1;
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = (Rectangle)sender;
            switch (rec.Name)
            {
                case "_rec1":
                    launchDetailChildWindow(1); break;
                case "_rec2":
                    launchDetailChildWindow(2); break;
                case "_rec3":
                    launchDetailChildWindow(3); break;
                case "_rec4":
                    launchDetailChildWindow(4); break;
                case "_rec5":
                    launchDetailChildWindow(5); break;
                case "_rec6":
                    launchDetailChildWindow(6); break;
                case "_rec7":
                    launchDetailChildWindow(7); break;
                default:
                    break;
            }
        }

        private void launchDetailChildWindow(int win)
        {
            switch (win)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                default:
                    break;
            }
            if (detailWindows.ContainsKey(win))
            {
                detailWindows[win].Focus();
            }
            else
            {
                DetailWindow detail = new DetailWindow();
                detail.Closed += new EventHandler(childWindow_Closed);
                detail.person = win;
                detail.Owner = this;
                detail.Show();
                detail.Owner = null;
                detailWindows.Add(win, detail);
            }            
        }

        private void childWindow_Closed(object sender, EventArgs e)
        {
            DetailWindow child = (DetailWindow)sender;
            detailWindows.Remove(child.person);
        }
    }
}
