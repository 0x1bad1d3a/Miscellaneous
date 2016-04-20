using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace program_final
{
    class Person
    {
        private Graph graph;
        private Image image;
        private Canvas canvas;
        private Point location;
        private Polyline pl;
        private bool moving;

        public delegate void CurrentPerson(Person person);
        private CurrentPerson cp;

        public Person(Canvas canvas, Graph graph, Point point, CurrentPerson cp, ToolTip tp)
        {
            image = new Image();
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,,/program_final;component/Resources/miku.gif");
            img.EndInit();
            ImageBehavior.SetAnimatedSource(image, img);

            image.MouseLeftButtonUp += Person_MouseLeftButtonUp;
            image.ToolTip = tp;
            ToolTipService.SetInitialShowDelay(image, 0);
            ToolTipService.SetShowDuration(image, 60000);

            canvas.Children.Add(image);
            Canvas.SetZIndex(image, 1);
            point = graph.closestNode(point).getPoint();
            Canvas.SetLeft(image, point.X - 16);
            Canvas.SetTop(image, point.Y - 34);

            this.location = point;
            this.graph = graph;
            this.canvas = canvas;
            this.moving = false;
            this.cp = cp;
        }

        public void movePath(List<Point> points)
        {
            moving = true;
            canvas.Children.Remove(pl);

            List<Point> p = new List<Point>(points);
            p.Insert(0, location);
            List<Point[]> pathPoints = getPaths(p, new List<Point[]>());

            List<List<Node>> paths = new List<List<Node>>();
            foreach (Point[] pathPoint in pathPoints){
                paths.Add(findPath(pathPoint[0], pathPoint[1]));
            }

            List<Node> fullpath = paths.Aggregate((a, b) => a.Concat(b).ToList());

            pl = new Polyline();
            SolidColorBrush red = new SolidColorBrush();
            red.Color = Colors.Red;
            foreach (Node n in fullpath)
            {
                pl.Points.Add(n.getPoint());
            }
            pl.Stroke = red;
            pl.StrokeThickness = 3;
            Canvas.SetZIndex(pl, 0);
            canvas.Children.Add(pl);

            animatePath(fullpath);
        }

        private List<Point[]> getPaths(List<Point> points, List<Point[]> paths)
        {
            if (points.Count > 1)
            {
                paths.Add(new Point[2] { points[0], points[1] });
                points.RemoveAt(0);
                return getPaths(points, paths);
            }
            return paths;
        }

        public List<Node> findPath(Point p1, Point p2)
        {
            p1 = graph.closestNode(p1).getPoint();
            p2 = graph.closestNode(p2).getPoint();
            return graph.findPath(p1, p2);
        }

        private void animatePath(List<Node> path)
        {
            if (path.Count > 1)
            {
                Node n1 = path[0];
                Node n2 = path[1];
                path.RemoveAt(0);

                DoubleAnimation horizontal = new DoubleAnimation();
                horizontal.From = n1.getPoint().X - image.ActualWidth / 2;
                horizontal.To = n2.getPoint().X - image.ActualWidth / 2;
                horizontal.Duration = new Duration(TimeSpan.Parse("0:0:0.1"));
                image.BeginAnimation(Canvas.LeftProperty, horizontal);

                DoubleAnimation vertical = new DoubleAnimation();
                vertical.From = n1.getPoint().Y - image.ActualHeight;
                vertical.To = n2.getPoint().Y - image.ActualHeight;
                vertical.Duration = new Duration(TimeSpan.Parse("0:0:0.1"));
                vertical.Completed += (s, e) => { animatePath(path); };
                image.BeginAnimation(Canvas.TopProperty, vertical);
            }
            else
            {
                if (path.Count > 0)
                {
                    location = path[0].getPoint();
                }
                moving = false;
            }
        }

        public void remove()
        {
            canvas.Children.Remove(pl);
            canvas.Children.Remove(image);
        }

        public bool isMoving()
        {
            return moving;
        }

        private void Person_MouseLeftButtonUp(Object sender, RoutedEventArgs args)
        {
            cp(this);
        }
    }
}
