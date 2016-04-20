using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace program_3
{
    class Shape
    {
        private int Type;
        private int Fill;
        
        private Pen Pen;
        private SolidBrush Brush;

        public Shape(int Type, int Fill, Point startPoint, Point endPoint, Color fillColor, Color borderColor, DashStyle borderStyle, int borderWidth )
        {
            this.Type = Type;
            this.Fill = Fill;

            this.startPoint = startPoint;
            this.endPoint = endPoint;

            this.Pen = new Pen(borderColor, borderWidth);
            this.Pen.DashStyle = borderStyle;

            this.Brush = new SolidBrush(fillColor);
        }

        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        public int getType() { return this.Type; }
        public int getFill() { return this.Fill; }
        public Pen getPen() { return this.Pen; }
        public SolidBrush getSolidBrush() { return this.Brush; }

        public String toString()
        {
            int borderType = 1;
            switch (Pen.DashStyle)
            {
                case DashStyle.DashDot:
                    borderType = 2; break;
                case DashStyle.Dot:
                    borderType = 3; break;
            }
            return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13}",
                Type, Fill, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y,
                Brush.Color.R, Brush.Color.G, Brush.Color.B,
                Pen.Color.R, Pen.Color.G, Pen.Color.B,
                borderType, Pen.Width);
        }
    }
}
