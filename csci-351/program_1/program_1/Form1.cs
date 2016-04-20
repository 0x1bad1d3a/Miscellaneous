using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace program_1
{
    public struct StudentGrade
    {
        public TextBox Name;
        public TextBox Grade;
    }

    public partial class Form1 : Form
    {

        List<StudentGrade> tlist = new List<StudentGrade>();
        bool toggle = false;

        public Form1()
        {
            InitializeComponent();

            this.MinimumSize = new System.Drawing.Size(500, 400);

            int tb_size = panel1.Size.Width / 2 - 10;
            for (int i = 0; i < 2; i++)
            {
                addRow();
            }

            panel1.Controls.Remove(listBox1);

            checkScrollBar();
        }

        private void addRow()
        {
            int tb_size = panel1.Size.Width / 2 - 10;
            TextBox tb1 = new TextBox();
            tb1.Size = new Size(tb_size, 20);
            tb1.Location = new Point(0, tlist.Count * 20 + panel1.AutoScrollPosition.Y);
            panel1.Controls.Add(tb1);

            TextBox tb2 = new TextBox();
            tb2.Size = new Size(tb_size, 20);
            tb2.Location = new Point(tb_size, tlist.Count * 20 + panel1.AutoScrollPosition.Y);
            tb2.Validating += new CancelEventHandler(TextBox_Validating);
            tb2.Validated += new EventHandler(TextBox_Validated);
            panel1.Controls.Add(tb2);

            StudentGrade sg = new StudentGrade();
            sg.Name = tb1;
            sg.Grade = tb2;
            tlist.Add(sg);
            checkScrollBar();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Control nextControl;
            if (e.KeyCode == Keys.Enter) 
            {
                nextControl = GetNextControl(ActiveControl, !e.Shift);
                if (nextControl == null)
                {
                    nextControl = GetNextControl(null, true);
                }
                nextControl.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void Form1_Resize(Object sender, EventArgs e)
        {
            checkScrollBar();
 
            int tb_size = panel1.Size.Width / 2 - 10;
            foreach (StudentGrade sg in tlist)
            {
                Size newsize = new Size(tb_size, 20);
                sg.Name.Size = newsize;
                sg.Grade.Size = newsize;
                sg.Grade.Location = new Point(tb_size, sg.Grade.Location.Y);
            }
        }

        private void checkScrollBar()
        {
            if (panel1.VerticalScroll.Visible == true || listBox1.Parent != null)
            {
                vScrollBar1.Visible = false;
            }
            else
            {
                vScrollBar1.Visible = true;
            }
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int x;
            if (int.TryParse(tb.Text, out x) || tb.Text == ""){
                if (x < 0 || x > 100)
                {
                    tb.BackColor = Color.Yellow;
                    System.Media.SystemSounds.Beep.Play();
                    e.Cancel = true;
                }
            }
            else
            {
                tb.BackColor = Color.Yellow;
                System.Media.SystemSounds.Beep.Play();
                e.Cancel = true;
            }
        }

        private void TextBox_Validated(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.BackColor = Color.White;
            int count = 0;
            foreach (StudentGrade sg in tlist)
            {
                count += sg.Grade.Text == "" ? 0 : 1;
            }
            if (count >= tlist.Count - 1)
            {
                addRow();
            }
        }

        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            toggleView();
        }

        // Removes everything but the scrollbar
        private void removeAll()
        {
            foreach (Control c in panel1.Controls)
            {
                if (c.GetType() != typeof(VScrollBar))
                {
                    panel1.Controls.Remove(c);
                }
            }
            vScrollBar1.Visible = false;
        }

        private bool listFull()
        {
            foreach (StudentGrade sg in tlist)
            {
                if (sg.Grade.Text.Trim() == "")
                {
                    return false;
                }
            }
            return true;
        }
        private void redrawTextBoxes()
        {
            foreach (StudentGrade sg in tlist)
            {
                sg.Name.Location = new Point(0, tlist.IndexOf(sg) * 20);
                sg.Grade.Location = new Point(sg.Grade.Location.X, tlist.IndexOf(sg) * 20);
                panel1.Controls.Add(sg.Name);
                panel1.Controls.Add(sg.Grade);
            }
            if (tlist.Count == 0 || listFull())
            {
                addRow();
                addRow();
            }
        }

        private void toggleView()
        {
            panel1.VerticalScroll.Value = 0;
            removeAll();

            if (toggle)
            {
                toggle = false;
                panel1.Controls.Remove(listBox1);
                button2.Text = "Compute";
                redrawTextBoxes();
                checkScrollBar();
            }
            else
            {
                toggle = true;
                panel1.Controls.Add(listBox1);
                button2.Text = "RETURN";
                foreach (StudentGrade sg in tlist)
                {
                    panel1.Controls.Remove(sg.Name);
                    panel1.Controls.Remove(sg.Grade);
                }
                checkScrollBar();
            }
        }

        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("NAME", "About");
        }

        private void button4_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        int hoveredIndex = -1;
        ToolTip tt = new ToolTip();
        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int newHoveredIndex = listBox1.IndexFromPoint(e.Location);

            if (hoveredIndex != newHoveredIndex)
            {
                hoveredIndex = newHoveredIndex;
                if (hoveredIndex > -1)
                {
                    string t = listBox1.GetItemText(listBox1.Items[hoveredIndex]);
                    switch (t)
                    {
                        case "up":
                            t = "sort grades in ascending order"; break;
                        case "down":
                            t = "sort grades in descending order"; break;
                        case "max":
                            t = "get the maximum grade(s)"; break;
                        case "min":
                            t = "get the minimum grade(s)"; break;
                        case "sum":
                            t = "get the sum of all the grades"; break;
                        case "median":
                            t = "get the median grade"; break;
                        case "mean":
                            t = "get the mean grade"; break;
                        case "stdev":
                            t = "get the standard deviation"; break;
                        case "new":
                            t = "start a new grading sheet"; break;
                        case "curve":
                            t = "curve all the grades by 1 to 10"; break;
                        default: break;
                    }
                    tt.SetToolTip(listBox1, t);
                    tt.Active = true;
                }
            }
        }

        private int getInt(TextBox tb)
        {
            try
            {
                return Int32.Parse(tb.Text);
            }
            catch
            {
                return -1;
            }            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string t = listBox1.GetItemText(listBox1.SelectedItem);
            tlist.RemoveAll(x => getInt(x.Grade) == -1);
            removeAll();

            Label l = new Label();
            l.Dock = DockStyle.Fill;
            l.Font = new Font(Font.FontFamily, 20);
            l.TextAlign = ContentAlignment.MiddleCenter;
            int a;

            switch (t)
            {
                case "up":
                    tlist = tlist.OrderByDescending(o => getInt(o.Grade)).ToList();
                    toggleView();
                    break;
                case "down":
                    tlist = tlist.OrderBy(o => getInt(o.Grade)).ToList();
                    toggleView();
                    break;
                case "max":
                    if (tlist.Count == 0)
                    {
                        l.Text = "MAX: NONE";
                    }
                    else
                    {
                        a = tlist.Max(x => getInt(x.Grade));
                        l.Text = String.Format("MAX: {0}", a);
                    }
                    panel1.Controls.Add(l);
                    break;
                case "min":
                    if (tlist.Count == 0)
                    {
                        l.Text = "MIN: NONE";
                    }
                    else
                    {
                        a = tlist.Min(x => getInt(x.Grade));
                        l.Text = String.Format("MIN: {0}", a);
                    }
                    panel1.Controls.Add(l);
                    break;
                case "sum":
                    if (tlist.Count == 0)
                    {
                        l.Text = "SUM: NONE";
                    }
                    else
                    {
                        a = tlist.Sum(x => getInt(x.Grade));
                        l.Text = String.Format("SUM: {0}", a);
                    }
                    panel1.Controls.Add(l);
                    break;
                case "median":
                    if (tlist.Count == 0)
                    {
                        l.Text = "MEDIAN: NONE";
                    }
                    else
                    {
                        double b = 0;
                        if (tlist.Count % 2 == 1)
                        {
                            b = Math.Ceiling((double)tlist.Count / 2.0) - 1;
                            b = getInt(tlist[(int)b].Grade);
                        }
                        else
                        {
                            int b1 = tlist.Count / 2 - 1;
                            int b2 = b1 + 1;
                            b = (double) (getInt(tlist[b1].Grade) + getInt(tlist[b2].Grade)) / 2.0;
                        }
                        l.Text = String.Format("MEDIAN: {0}", b);
                    }
                    panel1.Controls.Add(l);
                    break;
                case "mean":
                    if (tlist.Count == 0)
                    {
                        l.Text = "MEAN: NONE";
                    }
                    else
                    {
                        double c = tlist.Sum(x => getInt(x.Grade)) / (double) tlist.Count;
                        l.Text = String.Format("MEAN: {0}", c);
                    }
                    panel1.Controls.Add(l);
                    break;
                case "stdev":
                    if (tlist.Count == 0)
                    {
                        l.Text = "STANDARD DEVIATION: NONE";
                    }
                    else
                    {
                        double mean = tlist.Sum(x => getInt(x.Grade)) / (double)tlist.Count;
                        List<double> stdev = new List<double>();
                        foreach (StudentGrade sg in tlist)
                        {
                            stdev.Add(Math.Pow((double)getInt(sg.Grade) - mean, 2));
                        }
                        double var = stdev.Sum() / (double)stdev.Count;
                        double sd = Math.Sqrt(var);
                        l.Text = String.Format("STANDARD DEVIATION: {0}", sd);
                    }
                    panel1.Controls.Add(l);
                    break;
                case "new":
                    tlist.Clear();
                    toggleView();
                    break;
                case "curve":
                    Form2 dialog = new Form2();
                    var result = dialog.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        int curveValue = dialog.curveValue;
                        foreach (StudentGrade sg in tlist)
                        {
                            int grade = getInt(sg.Grade);
                            grade = grade + curveValue > 100 ? 100 : grade + curveValue;
                            sg.Grade.Text = String.Format("{0}", grade);
                        }
                        toggleView();
                    }
                    else
                    {
                        toggleView();
                    }
                    break;
                default: break;
            }
        }
    }
}
