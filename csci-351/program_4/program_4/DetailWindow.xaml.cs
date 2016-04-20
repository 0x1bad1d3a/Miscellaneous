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
using System.Windows.Shapes;

namespace program_4
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DetailWindow : Window
    {

        public DetailWindow()
        {
            InitializeComponent();
        }

        public int person { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            switch (this.person)
            {
                case 1:
                    this.Title = "Margarita Teresa";
                    this.textbox_name.Text = "Margarita Teresa";
                    this.textbox_description.Text = "Margaret Theresa of Spain was Holy Roman Empress, German Queen, Archduchess consort of Austria, Queen consort of Hungary and Bohemia.";
                    this.image.Source = new BitmapImage(new Uri("pack://application:,,,/program_4;component/Resources/1.png")); break;
                case 2:
                    this.Title = "Maria Agustina Sarmiento";
                    this.textbox_name.Text = "María Agustina Sarmiento";
                    this.textbox_description.Text = "María Agustina Sarmiento de Sotomayor fue una menina de la infanta Margarita de Austria.\n\nNo clue what it says ┐(￣ー￣)┌";
                    this.image.Source = new BitmapImage(new Uri("pack://application:,,,/program_4;component/Resources/2.png")); break;
                case 3:
                    this.Title = "Diego Velazquez";
                    this.textbox_name.Text = "Diego Velázquez";
                    this.textbox_description.Text = "Diego Rodríguez de Silva y Velázquez was a Spanish painter who was the leading artist in the court of King Philip IV and one of the most important painters of the Spanish Golden Age.";
                    this.image.Source = new BitmapImage(new Uri("pack://application:,,,/program_4;component/Resources/3.png")); break;
                case 4:
                    this.Title = "Isabel de Velasco";
                    this.textbox_name.Text = "Isabel de Velasco";
                    this.textbox_description.Text = "Daughter of Don Bernardino López de Ayala y Velasco.";
                    this.image.Source = new BitmapImage(new Uri("pack://application:,,,/program_4;component/Resources/4.png")); break;
                case 5:
                    this.Title = "Maria Barbola";
                    this.textbox_name.Text = "Maria Barbola";
                    this.textbox_description.Text = "Las Meninas, features an achondroplastic dwarf, Maria Barbola, among its subjects.";
                    this.image.Source = new BitmapImage(new Uri("pack://application:,,,/program_4;component/Resources/5.png")); break;
                case 6:
                    this.Title = "Nicolas Pertusato";
                    this.textbox_name.Text = "Nicolas Pertusato";
                    this.textbox_description.Text = "Detalle del lienzo Las Meninas de Velázquez en el que se puede observar a Nicolasito Pertusato.\n\nNo clue what it says ┐(￣ー￣)┌";
                    this.image.Source = new BitmapImage(new Uri("pack://application:,,,/program_4;component/Resources/6.png")); break;
                case 7:
                    this.Title = "Don Jose Nieto Velazquez";
                    this.textbox_name.Text = "Don José Nieto Velazquez";
                    this.textbox_description.Text = "The Queen’s chamberlain and keeper of the royal tapestries.";
                    this.image.Source = new BitmapImage(new Uri("pack://application:,,,/program_4;component/Resources/7.png")); break;
                default:
                    break;
            }
            
        }
    }
}
