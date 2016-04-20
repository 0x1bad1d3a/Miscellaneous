using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace program_final
{
    class Node : PriorityQueueNode
    {
        private Point point;
        private List<Node> list;
        public Node parent { get; set; }
        public int gvalue { get; set; }

        public Node(Point point)
        {
            this.list = new List<Node>();
            this.point = point;
        }

        public Point getPoint() {
            return point;
        }

        public List<Node> getLinks()
        {
            return list;
        }
        
        public void removeLink(Node node){
            list.Remove(node);
        }

        public void addLink(Node node)
        {
            this.list.Add(node);
        }
    }
}
