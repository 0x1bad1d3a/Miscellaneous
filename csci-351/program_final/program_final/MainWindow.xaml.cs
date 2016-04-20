using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Point = System.Windows.Point;

using Priority_Queue;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfAnimatedGif;

namespace program_final
{

    public partial class MainWindow : Window
    {
        Graph graph;
        Person miku;
        Person.CurrentPerson cp;
        ToolTip tooltip;

        List<Point> points;

        public MainWindow()
        {
            InitializeComponent();

            graph = new Graph(new BitmapImage(new Uri("pack://application:,,,/program_final;component/Resources/overlay.png")));
            cp = new Person.CurrentPerson(Clicked_Person);
            tooltip = new ToolTip();
            tooltip.Content = "( LEFT CLICK TO SELECT )\nRIGHT CLICK TO MOVE ( +SHIFT TO QUEUE MOVEMENT )\nKEY A TO ADD ( +SHIFT TO ADD AT MOUSE LOCATION )\nKEY D TO DELETE SELECTED";
            miku = new Person(canvas, graph, new Point(18, 36), new Person.CurrentPerson(cp), tooltip);

            points = new List<Point>();
        }

        private void Clicked_Person(Person person)
        {
            miku = person;
        }

        private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (miku != null && !miku.isMoving())
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    points.Add(e.GetPosition(screen_map));
                }
                else
                {
                    points.Add(e.GetPosition(screen_map));
                    miku.movePath(points);
                    points = new List<Point>();
                }                
            }            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)
            {
                screen_map.Source = graph.getBitmapImage();
            }
            if (e.Key == Key.N)
            {
                screen_map.Source = new BitmapImage(new Uri("pack://application:,,,/program_final;component/Resources/map.png"));
            }
            if (e.Key == Key.A)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    // Seems that any point outside of screen_map is given a negative value, so we should be fine here
                    Point p = Mouse.GetPosition(screen_map);
                    if (p.X >= 0 && p.Y >= 0)
                    {
                        miku = new Person(canvas, graph, p, new Person.CurrentPerson(cp), tooltip);
                    }
                }
                else
                {
                    miku = new Person(canvas, graph, new Point(18, 36), new Person.CurrentPerson(cp), tooltip);
                }
            }
            if (e.Key == Key.D)
            {
                miku.remove();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (miku != null && !miku.isMoving() && points.Count > 0)
            {
                if (e.Key == Key.LeftShift)
                {
                    miku.movePath(points);
                    points = new List<Point>();
                }
            }
        }
    }
}