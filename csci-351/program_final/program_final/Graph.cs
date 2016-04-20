using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;

using Bitmap = System.Drawing.Bitmap;
using BitmapImage = System.Windows.Media.Imaging.BitmapImage;
using Point = System.Windows.Point;
using Color = System.Drawing.Color;

using Priority_Queue;

namespace program_final
{
    class Graph
    {
        private Bitmap image_mask { get; set; }
        private Dictionary<Point, Node> dict { get; set; }

        public Graph(BitmapImage bitmapImage)
        {
            this.image_mask = BitmapImage2Bitmap(bitmapImage);
            this.dict = generateMap(generateNodes(4));
        }

        private Dictionary<Point, Node> generateMap(Node[,] nodes)
        {
            Dictionary<Point, Node> dict = new Dictionary<Point, Node>();

            // Build the graph and dictionary
            for (int i = 1; i < nodes.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < nodes.GetLength(1) - 1; j++)
                {
                    nodes[i, j].addLink(nodes[i - 1, j - 1]);
                    nodes[i, j].addLink(nodes[i + 1, j + 1]);
                    nodes[i, j].addLink(nodes[i - 1, j + 1]);
                    nodes[i, j].addLink(nodes[i + 1, j - 1]);
                    nodes[i, j].addLink(nodes[i + 1, j]);
                    nodes[i, j].addLink(nodes[i - 1, j]);
                    nodes[i, j].addLink(nodes[i, j + 1]);
                    nodes[i, j].addLink(nodes[i, j - 1]);
                    dict.Add(nodes[i, j].getPoint(), nodes[i, j]);
                }
            }

            // Find points inside unallowed regions
            List<Point> removeList = new List<Point>();
            foreach (KeyValuePair<Point, Node> d in dict)
            {
                int x = (int)d.Value.getPoint().X;
                int y = (int)d.Value.getPoint().Y;
                Color color = image_mask.GetPixel(x, y);
                if (color.R < 10 && color.G < 10 && color.B < 10)
                {
                    removeList.Add(d.Key);
                }
            }

            // Remove points from graph and dictionary
            foreach (Point p in removeList)
            {
                Node node = dict[p];
                foreach (Node n in node.getLinks())
                {
                    n.removeLink(node);
                }
                dict.Remove(p);
            }

            return dict;
        }

        private Node[,] generateNodes(int step)
        {
            Node[,] nodes = new Node[image_mask.Width / step + 1, image_mask.Height / step + 1];
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    nodes[i, j] = new Node(new Point(i * step, j * step));
                }
            }
            return nodes;
        }

        // Will StackOverflow if you set generateNodes step too high (>10)
        private Node findClosestNodeInGraph(Dictionary<Point, Node> dict, Point point, char prev, int stride, int step)
        {
            if (dict.ContainsKey(point))
            {
                return dict[point];
            }
            if (step < stride)
            {
                switch (prev)
                {
                    case 'R': point.X += 1; break;
                    case 'D': point.Y -= 1; break;
                    case 'L': point.X -= 1; break;
                    case 'U': point.Y += 1; break;
                }
                return findClosestNodeInGraph(dict, point, prev, stride, step + 1);
            }
            else
            {
                switch (prev)
                {
                    case 'R': return findClosestNodeInGraph(dict, point, 'D', stride, 0);
                    case 'D': return findClosestNodeInGraph(dict, point, 'L', stride + 1, 0);
                    case 'L': return findClosestNodeInGraph(dict, point, 'U', stride, 0);
                    case 'U': return findClosestNodeInGraph(dict, point, 'R', stride + 1, 0);
                }
            }
            return null; // Shouldn't happen as long as you use D L U R
        }

        public Node closestNode(Point point)
        {
            point.X = (int)point.X;
            point.Y = (int)point.Y;
            return findClosestNodeInGraph(dict, point, 'U', 0, 0);
        }

        private List<Node> followPath(Node node, List<Node> list)
        {
            if (node.parent == null)
            {
                list.Reverse();
                return list;
            }
            list.Add(node);

            return followPath(node.parent, list);
        }

        public List<Node> findPathDumb(Point start, Point end)
        {
            foreach (KeyValuePair<Point, Node> k in dict)
            {
                k.Value.gvalue = 0;
                k.Value.parent = null;
            }

            Dictionary<Point, Node> traversed = new Dictionary<Point, Node>();

            Node curr = dict[start];
            while (true)
            {
                Console.WriteLine(curr.getPoint());
                if (curr.getPoint().Equals(end))
                {
                    break;
                }
                Node choice = null;
                foreach (Node n in curr.getLinks())
                {
                    if (!traversed.ContainsKey(n.getPoint()))
                    {
                        if (choice == null)
                        {
                            choice = n;
                        }
                        else
                        {
                            int n_dist = (int)Math.Sqrt(Math.Pow(end.X - n.getPoint().X, 2) + Math.Pow(end.Y - n.getPoint().Y, 2));
                            int choice_dist = (int)Math.Sqrt(Math.Pow(end.X - choice.getPoint().X, 2) + Math.Pow(end.Y - choice.getPoint().Y, 2));
                            if (n_dist < choice_dist)
                            {
                                choice = n;
                            }
                        }
                    }
                }
                traversed[choice.getPoint()] = choice;
                choice.parent = curr;
                curr = choice;
            }

            return followPath(curr, new List<Node>());
        }

        

        public List<Node> findPath(Point start, Point end)
        {
            foreach (KeyValuePair<Point, Node> k in dict)
            {
                k.Value.gvalue = 0;
                k.Value.parent = null;
            }

            HeapPriorityQueue<Node> open_set = new HeapPriorityQueue<Node>(49152);
            Dictionary<Point, Node> closed_set = new Dictionary<Point, Node>();
            dict[start].gvalue = 0;
            open_set.Enqueue(dict[start], 0);

            Node curr = null;
            while (open_set.Count > 0)
            {
                // lowest rank
                curr = open_set.Dequeue();

                if (curr.getPoint().Equals(end))
                {
                    break;
                }

                closed_set[curr.getPoint()] = curr;

                foreach (Node n in curr.getLinks())
                {
                    int cost = curr.gvalue + (int)(Math.Abs(end.X - curr.getPoint().X) + Math.Abs(end.Y - curr.getPoint().Y));
                    if (open_set.Contains(n) && cost < n.gvalue)
                    {
                        open_set.Remove(n);
                    }
                    if (!open_set.Contains(n) && !closed_set.ContainsKey(n.getPoint()))
                    {
                        n.gvalue = cost;
                        open_set.Enqueue(n, cost);
                        n.parent = curr;
                    }
                }
            }

            return followPath(curr, new List<Node>());
        }

        public List<Node> findPathOtherDumb(Point start, Point end)
        {
            foreach (KeyValuePair<Point, Node> k in dict)
            {
                k.Value.gvalue = 0;
                k.Value.parent = null;
            }

            Dictionary<Point, Node> open_set = new Dictionary<Point, Node>();
            Dictionary<Point, Node> closed_set = new Dictionary<Point, Node>();
            dict[start].gvalue = 0;
            open_set.Add(dict[start].getPoint(), dict[start]);

            Node curr = null;
            while (open_set.Count > 0)
            {
                // lowest rank
                curr = open_set.OrderBy(k => k.Value.gvalue).First().Value;
                open_set.Remove(curr.getPoint());

                if (curr.getPoint().Equals(end))
                {
                    break;
                }

                closed_set[curr.getPoint()] = curr;

                foreach (Node n in curr.getLinks())
                {
                    int cost = curr.gvalue + (int)Math.Sqrt(Math.Pow(end.X - curr.getPoint().X, 2) + Math.Pow(end.Y - curr.getPoint().Y, 2));
                    if (open_set.ContainsKey(n.getPoint()) && cost < n.gvalue)
                    {
                        open_set.Remove(n.getPoint());
                    }
                    if (!open_set.ContainsKey(n.getPoint()) && !closed_set.ContainsKey(n.getPoint()))
                    {
                        n.gvalue = cost;
                        open_set[n.getPoint()] = n;
                        n.parent = curr;
                    }
                }
            }

            return followPath(curr, new List<Node>());
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        public BitmapImage getBitmapImage()
        {
            Bitmap copy = new Bitmap(image_mask);
            foreach (KeyValuePair<Point, Node> d in dict)
            {
                Point p = d.Value.getPoint();
                copy.SetPixel((int)p.X, (int)p.Y, Color.Red);
            }
            using (MemoryStream memory = new MemoryStream())
            {
                copy.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
