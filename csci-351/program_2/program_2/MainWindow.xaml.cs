using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace program_2
{

    class MenuItem
    {
        public MenuItem(String type, String name, double cost)
        {
            this.type = type;
            this.name = name;
            this.cost = cost;
        }
        public String type { get; set; }
        public String name { get; set; }
        public double cost { get; set; }
    }

    public partial class MainWindow : Window
    {
        private List<MenuItem> mlist;
        private List<List<MenuItem>> receipt = new List<List<MenuItem>>();

        public MainWindow()
        {
            InitializeComponent();

            mlist = new List<MenuItem>()
            {
                // Starters
                new MenuItem("STARTERS", "KAHUKU PRAWNS (4)", 11),
                new MenuItem("STARTERS", "KAHUKU PRAWNS (8)", 18),
                new MenuItem("STARTERS", "DUNGENESS CRAB COCKTAIL", 12),
                new MenuItem("STARTERS", "ALDER SMOKED SALMON", 11),
                new MenuItem("STARTERS", "PAN FRIED OYSTERS (6)", 10),
                new MenuItem("STARTERS", "PAN FRIED OYSTERS (12)", 18),
                new MenuItem("STARTERS", "PRAWN PLATTER", 14),
                new MenuItem("STARTERS", "HOT CRAB & SHRIMP DIP", 15),
                new MenuItem("STARTERS", "DEVILS ON HORSEBACK", 16),

                // Salad & Soup
                new MenuItem("SALAD&SOUP", "DUNGENESS CRAB LOUIS", 19),
                new MenuItem("SALAD&SOUP", "CAESAR SALAD", 10),
                new MenuItem("SALAD&SOUP", "WHISKEY CRAB SOUP (CUP)", 6),
                new MenuItem("SALAD&SOUP", "WHISKEY CRAB SOUP (BOWL)", 8),
                new MenuItem("SALAD&SOUP", "SOUP & SALAD BAR", 18),
                // Addons for salad
                new MenuItem("CAESAR SALAD", "GRILLED CHICKEN BREAST", 6),
                new MenuItem("CAESAR SALAD", "OREGON BAY SHRIMP", 7),
                new MenuItem("CAESAR SALAD", "GUNGENESS CRAB", 12),

                // Pasta
                new MenuItem("PASTA", "MUSHROOM LINGUINE", 17),
                // Addons for pasta
                new MenuItem("MUSHROOM LINGUINE", "SEARED ALASKAN WEATHERVANE SCALLOPS", 6),

                // Steaks
                new MenuItem("STEAKS", "TOP SIRLOIN", 25),
                new MenuItem("STEAKS", "RIB EYE", 36),
                new MenuItem("STEAKS", "NEW YORK", 32),
                new MenuItem("STEAKS", "TENDERLOIN", 36),

                // Seafood
                new MenuItem("SEAFOOD", "SEAFOOD MEDLEY", 24),
                new MenuItem("SEAFOOD", "PRAWN SAUTE", 22),
                new MenuItem("SEAFOOD", "CATCH OF THE DAY", 25),
                new MenuItem("SEAFOOD", "LOBSTER", 25),

                // Chicken
                new MenuItem("CHICKEN", "CHICKEN DUNGENESS", 25),

                // Desserts
                new MenuItem("DESSERTS", "CHOCOLATE TORTE", 8),
                new MenuItem("DESSERTS", "PEANUT BUTTER PIE", 7.5),
                new MenuItem("DESSERTS", "CREME BRULEE", 7.5),
                new MenuItem("DESSERTS", "COOKIES & CREAM", 7),
                new MenuItem("DESSERTS", "SEASONAL CHEESECAKE", 8),
                new MenuItem("DESSERTS", "MAUREEN'S SPECIAL", 9),
                // Addons for desserts
                new MenuItem("CHOCOLATE TORTE", "MALLARD'S ALA MODE", 3),

                // Entree combo thing
                new MenuItem("ENTREEFREE", "TRIP TO SALAD BAR", 0),
                new MenuItem("ENTREEFREE", "SMALL CAESAR SALAD", 0),
                new MenuItem("ENTREECOMBO", "WHISKEY CRAB SOUP (CUP)", 5),
                new MenuItem("ENTREECOMBO", "WHISKEY CRAB SOUP (BOWL)", 7)
            };

            foreach (String s in new List<String>() { "TOP SIRLOIN", "RIB EYE", "NEW YORK", "TENDERLOIN" })
            {
                mlist.Add(new MenuItem(s, "OSCAR", 8));
                mlist.Add(new MenuItem(s, "PAN SEARED ALASKAN WEATHERVANE SCALLOPS", 6));
                mlist.Add(new MenuItem(s, "DEMI-GLACE", 8));
                mlist.Add(new MenuItem(s, "BLACK PEPPER MARINADE", 8));
                mlist.Add(new MenuItem(s, "DAVE'S MUSHROOM SAUCE", 8));
                mlist.Add(new MenuItem(s, "HERBED BLUE CHEESE BUTTER", 8));
            }

            foreach (String s in new List<String>() { "OSCAR", "PAN SEARED ALASKAN WEATHERVANE SCALLOPS", "DEMI-GLACE", "BLACK PEPPER MARINADE", "DAVE'S MUSHROOM SAUCE", "HERBED BLUE CHEESE BUTTER" })
            {
                mlist.Add(new MenuItem(s, "OSCAR", 8));
                mlist.Add(new MenuItem(s, "PAN SEARED ALASKAN WEATHERVANE SCALLOPS", 6));
                mlist.Add(new MenuItem(s, "DEMI-GLACE", 8));
                mlist.Add(new MenuItem(s, "BLACK PEPPER MARINADE", 8));
                mlist.Add(new MenuItem(s, "DAVE'S MUSHROOM SAUCE", 8));
                mlist.Add(new MenuItem(s, "HERBED BLUE CHEESE BUTTER", 8));
            }

            initMenus(sp_starters, "STARTERS");
            initMenus(sp_saladandsoup, "SALAD&SOUP");
            initMenus(sp_pasta, "PASTA");
            initMenus(sp_steaks, "STEAKS");
            initMenus(sp_seafood, "SEAFOOD");
            initMenus(sp_chicken, "CHICKEN");
            initMenus(sp_dessert, "DESSERTS");
            displayReceipt();
        }

        private void initMenus(StackPanel origin, String type){
            ComboBox cb = makeComboBox(type);
            cb.SelectionChanged += (sender, e) => cb_SelectionChanged(sender, e, origin, new List<ComboBox>(){cb});
            origin.Children.Add(cb);
        }

        void cb_SelectionChanged(object sender, SelectionChangedEventArgs e, StackPanel sp, List<ComboBox> cbl)
        {
            ComboBox cb = (ComboBox)sender;
            String selection = (String)cb.SelectedItem;

            try
            {
                selection = selection.Split('~')[1].Trim();
            }
            catch
            {
                selection = "";
            }

            sp.Children.Clear();
            foreach (ComboBox c in cbl)
            {
                sp.Children.Add(c);
            }
            
            ComboBox addon = makeComboBox(selection);
            if (addon.Items.Count != 1)
            {
                if (sp == sp_steaks)
                {
                    cbl.Add(addon);
                    addon.SelectionChanged += (sender1, e1) => cb_SelectionChanged(sender1, e1, sp, cbl);
                }
                sp.Children.Add(addon);
            }

            if (!String.Equals((String)((ComboBox)sp.Children[0]).SelectedItem, "( NONE )"))
            {
                if (sp == sp_pasta || sp == sp_steaks || sp == sp_seafood || sp == sp_chicken)
                {
                    ComboBox free = makeComboBox("ENTREEFREE");
                    free.SelectedIndex = 0;
                    sp.Children.Add(free);
                    sp.Children.Add(makeComboBox("ENTREECOMBO"));
                }

                StackPanel s = new StackPanel();
                s.Orientation = Orientation.Horizontal;
                s.FlowDirection = FlowDirection.RightToLeft;

                Button b = new Button();
                b.Width = 60;
                b.HorizontalAlignment = HorizontalAlignment.Right;
                b.Margin = new Thickness(10);
                b.Content = "Add";
                b.Click += (sender2, e2) => b_Click(sender2, e2, sp);

                Button b1 = new Button();
                b1.Width = 60;
                b1.HorizontalAlignment = HorizontalAlignment.Right;
                b1.Margin = new Thickness(10);
                b1.Content = "Reset";
                b1.Click += (sender3, e3) => b1_Click(sender3, e3, sp);

                s.Children.Add(b);
                s.Children.Add(b1);
                sp.Children.Add(s);
            }
        }

        void b1_Click(object sender, RoutedEventArgs e, StackPanel sp)
        {
            sp.Children.Clear();
            if (sp == sp_starters)
            {
                initMenus(sp_starters, "STARTERS");
            }
            else if (sp == sp_saladandsoup)
            {
                initMenus(sp_saladandsoup, "SALAD&SOUP");
            }
            else if (sp == sp_pasta)
            {
                initMenus(sp_pasta, "PASTA");
            }
            else if (sp == sp_steaks)
            {
                initMenus(sp_steaks, "STEAKS");
            }
            else if (sp == sp_seafood)
            {
                initMenus(sp_seafood, "SEAFOOD");
            }
            else if (sp == sp_chicken)
            {
                initMenus(sp_chicken, "CHICKEN");
            }
            else if (sp == sp_dessert)
            {
                initMenus(sp_dessert, "DESSERTS");
            }
        }

        void b_Click(object sender, RoutedEventArgs e, StackPanel sp)
        {

            IEnumerable<ComboBox> cbl = sp.Children.OfType<ComboBox>();
            List<MenuItem> prices = new List<MenuItem>();
            foreach (ComboBox cb in cbl)
            {
                String text = (String)cb.SelectedItem;
                if (!String.Equals(text, "( NONE )"))
                {
                    String[] split = text.Split('~');
                    double x;
                    Double.TryParse(split[0].Trim().Substring(1), out x);
                    MenuItem mi = new MenuItem("", split[1].Trim(), x);
                    prices.Add(mi);
                }
            }
            receipt.Add(prices);
            displayReceipt();

            sp.Children.Clear();
            if (sp == sp_starters)
            {
                initMenus(sp_starters, "STARTERS");
            }
            else if (sp == sp_saladandsoup)
            {
                initMenus(sp_saladandsoup, "SALAD&SOUP");
            }
            else if (sp == sp_pasta)
            {
                initMenus(sp_pasta, "PASTA");
            }
            else if (sp == sp_steaks)
            {
                initMenus(sp_steaks, "STEAKS");
            }
            else if (sp == sp_seafood)
            {
                initMenus(sp_seafood, "SEAFOOD");
            }
            else if (sp == sp_chicken)
            {
                initMenus(sp_chicken, "CHICKEN");
            }
            else if (sp == sp_dessert)
            {
                initMenus(sp_dessert, "DESSERTS");
            }
        }

        private static IEnumerable<string> SplitByLength(string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }

        private static int numWhitespace(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != ' ')
                {
                    return i;
                }
            }
            return str.Length;
        }

        private void displayReceipt()
        {
            double totalcost = 0;
            receiptdisplay.Text = "**********WELCOME TO*********\n**THE CLIFF HOUSE RESTRAUNT**\n*****************************\n\nTABLE NUMBER: " + table_textbox.Text + "\n\n";
            foreach (List<MenuItem> order in receipt)
            {
                displayOneLine(order[0].cost.ToString("$0.00") + " ~ " + order[0].name);
                totalcost += order[0].cost;
                foreach (MenuItem o in order.Skip(1))
                {
                    displayOneLine("    " + o.cost.ToString("+ $0.00") + " ~ " + o.name);
                    totalcost += o.cost;
                }
            }
            receiptdisplay.Text += "_____________________________\n\nTOTAL: " + totalcost.ToString("$0.00");
        }

        private void displayOneLine(string str)
        {
            string[] slist = SplitByLength(str, 31).ToArray();
            int whitespace = numWhitespace(slist[0]);
            receiptdisplay.Text += slist[0] + "\n";
            foreach (String s in slist.Skip(1))
            {
                receiptdisplay.Text += String.Concat(Enumerable.Repeat(' ', whitespace)) + s + "\n";
            }
        }

        private ComboBox makeComboBox(String type){
            List<MenuItem> temp = mlist.FindAll(x => x.type == type);
            ComboBox cb = new ComboBox();
            cb.Width = 220;
            cb.HorizontalAlignment = HorizontalAlignment.Right;
            cb.Margin = new Thickness(10);
            if (type != "ENTREEFREE")
            {
                cb.Items.Add("( NONE )");
            }            
            foreach (MenuItem mi in temp)
            {
                cb.Items.Add("$" + mi.cost.ToString() + " ~ " + mi.name);
            }
            cb.SelectedItem = cb.Items[0];
            return cb;
        }

        private void table_textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            Regex regex = new Regex(@"\d{1,2}");
            if (!regex.Match(tb.Text).Success)
            {
                MessageBox.Show("Please enter a number between 1-99");
                tb.Text = "";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (table_textbox.Text == "")
            {
                MessageBox.Show("Please enter a table number");
            }
            else
            {
                displayReceipt();
                File.WriteAllText("receipt.txt", receiptdisplay.Text);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            receipt = new List<List<MenuItem>>();
            displayReceipt();
        }
    }


}
