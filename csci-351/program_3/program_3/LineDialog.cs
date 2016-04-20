using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace program_3
{
    public partial class LineDialog : Form
    {
        public LineDialog()
        {
            InitializeComponent();
            dashStyle = DashStyle.Solid;
        }

        public int lineWidth { get; set; }
        public DashStyle dashStyle { get; set; }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = String.Format("{0}", trackBar1.Value);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            Pen p1 = new Pen(Color.Red, 6);
            p1.DashStyle = DashStyle.Solid;
            e.Graphics.DrawLine(p1, 30, 30, 130, 30);

            Pen p2 = new Pen(Color.Red, 6);
            p2.DashStyle = DashStyle.DashDot;
            e.Graphics.DrawLine(p2, 180, 30, 280, 30);

            Pen p3 = new Pen(Color.Red, 6);
            p3.DashStyle = DashStyle.Dot;
            e.Graphics.DrawLine(p3, 330, 30, 430, 30);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                dashStyle = DashStyle.Solid;
            }
            else if (radioButton2.Checked)
            {
                dashStyle = DashStyle.DashDot;
            }
            else if (radioButton3.Checked)
            {
                dashStyle = DashStyle.Dot;
            }
            lineWidth = trackBar1.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
