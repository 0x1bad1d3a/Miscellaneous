using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace program_3
{
    public partial class Form1 : Form
    {
        private Graphics canvas;

        private Shape tempShape;
        private Point prevMouseLoc;
        private List<Shape> shapes = new List<Shape>();
        private List<Shape> redoShapes = new List<Shape>();

        private int fillType = 0;
        private Color fillColor = Color.White;
        private Color lineColor = Color.Black;
        private DashStyle dashStyle = DashStyle.Solid;
        private int lineWidth = 1;

        private bool unsavedChanges = false;
        String imageFile;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            canvas = canvasPanel.CreateGraphics();
            toolstrip_combobox.SelectedIndex = 0;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            canvas = canvasPanel.CreateGraphics();
        }

        private void drawShape(Graphics e, Shape shape)
        {
            Action drawRectangle = () =>
                {
                    Point[] points = new Point[4];
                    points[0] = new Point(shape.startPoint.X, shape.startPoint.Y);
                    points[1] = new Point(shape.endPoint.X, shape.startPoint.Y);
                    points[2] = new Point(shape.endPoint.X, shape.endPoint.Y);
                    points[3] = new Point(shape.startPoint.X, shape.endPoint.Y);
                    if (shape.getFill() == 1)
                    {
                        e.FillPolygon(shape.getSolidBrush(), points);
                    }
                    e.DrawPolygon(shape.getPen(), points);
                };

            Action drawEllipse = () =>
                {
                    Point endPoint = new Point(shape.endPoint.X - shape.startPoint.X, shape.endPoint.Y - shape.startPoint.Y);
                    if (shape.getFill() == 1)
                    {
                        e.FillEllipse(shape.getSolidBrush(), shape.startPoint.X, shape.startPoint.Y, endPoint.X, endPoint.Y);
                    }
                    e.DrawEllipse(shape.getPen(), shape.startPoint.X, shape.startPoint.Y, endPoint.X, endPoint.Y);
                };

            Action drawRightTriangle = () =>
                {
                    Point[] points = new Point[3];
                    points[0] = new Point(shape.startPoint.X, shape.startPoint.Y);
                    points[1] = new Point(shape.startPoint.X, shape.endPoint.Y);
                    points[2] = new Point(shape.endPoint.X, shape.endPoint.Y);
                    if (shape.getFill() == 1)
                    {
                        e.FillPolygon(shape.getSolidBrush(), points);
                    }
                    e.DrawPolygon(shape.getPen(), points);
                };

            Action drawDiamond = () =>
                {
                    Point[] points = new Point[4];
                    points[0] = new Point((shape.startPoint.X + shape.endPoint.X) / 2, shape.startPoint.Y);
                    points[1] = new Point(shape.endPoint.X, (shape.startPoint.Y + shape.endPoint.Y) / 2);
                    points[2] = new Point((shape.startPoint.X + shape.endPoint.X) / 2, shape.endPoint.Y);
                    points[3] = new Point(shape.startPoint.X, (shape.startPoint.Y + shape.endPoint.Y) / 2);
                    if (shape.getFill() == 1)
                    {
                        e.FillPolygon(shape.getSolidBrush(), points);
                    }
                    e.DrawPolygon(shape.getPen(), points);
                };

            Action drawIsocelesTriangle = () =>
                {
                    Point[] points = new Point[3];
                    points[0] = new Point((shape.startPoint.X + shape.endPoint.X) / 2, shape.startPoint.Y);
                    points[1] = new Point(shape.startPoint.X, shape.endPoint.Y);
                    points[2] = new Point(shape.endPoint.X, shape.endPoint.Y);
                    if (shape.getFill() == 1)
                    {
                        e.FillPolygon(shape.getSolidBrush(), points);
                    }
                    e.DrawPolygon(shape.getPen(), points);
                };

            Action drawHexagon = () =>
                {
                    Point[] points = new Point[6];
                    Point c = new Point(shape.startPoint.X + ((shape.endPoint.X - shape.startPoint.X) / 2), shape.startPoint.Y + ((shape.endPoint.Y - shape.startPoint.Y) / 2));
                    double a = Math.PI * 2 / 6;
                    double rotate = Math.PI * 2 / 12;
                    double r = Math.Abs(shape.endPoint.X - shape.startPoint.X) / 2;
                    points[0] = new Point(c.X + (int)(Math.Sin(a * 0 + rotate) * r), c.Y + (int)(Math.Cos(a * 0 + rotate) * r));
                    points[1] = new Point(c.X + (int)(Math.Sin(a * 1 + rotate) * r), c.Y + (int)(Math.Cos(a * 1 + rotate) * r));
                    points[2] = new Point(c.X + (int)(Math.Sin(a * 2 + rotate) * r), c.Y + (int)(Math.Cos(a * 2 + rotate) * r));
                    points[3] = new Point(c.X + (int)(Math.Sin(a * 3 + rotate) * r), c.Y + (int)(Math.Cos(a * 3 + rotate) * r));
                    points[4] = new Point(c.X + (int)(Math.Sin(a * 4 + rotate) * r), c.Y + (int)(Math.Cos(a * 4 + rotate) * r));
                    points[5] = new Point(c.X + (int)(Math.Sin(a * 5 + rotate) * r), c.Y + (int)(Math.Cos(a * 5 + rotate) * r));
                    if (shape.getFill() == 1)
                    {
                        e.FillPolygon(shape.getSolidBrush(), points);
                    }
                    e.DrawPolygon(shape.getPen(), points);
                };

            Action drawPentagon = () =>
                {
                    Point[] points = new Point[5];
                    Point c = new Point(shape.startPoint.X + ((shape.endPoint.X - shape.startPoint.X) / 2), shape.startPoint.Y + ((shape.endPoint.Y - shape.startPoint.Y) / 2));
                    double a = Math.PI * 2 / 5;
                    double rotate = Math.PI * 2 / 10;
                    double r = Math.Abs(shape.endPoint.X - shape.startPoint.X) / 2;
                    points[0] = new Point(c.X + (int)(Math.Sin(a * 0 + rotate) * r), c.Y + (int)(Math.Cos(a * 0 + rotate) * r));
                    points[1] = new Point(c.X + (int)(Math.Sin(a * 1 + rotate) * r), c.Y + (int)(Math.Cos(a * 1 + rotate) * r));
                    points[2] = new Point(c.X + (int)(Math.Sin(a * 2 + rotate) * r), c.Y + (int)(Math.Cos(a * 2 + rotate) * r));
                    points[3] = new Point(c.X + (int)(Math.Sin(a * 3 + rotate) * r), c.Y + (int)(Math.Cos(a * 3 + rotate) * r));
                    points[4] = new Point(c.X + (int)(Math.Sin(a * 4 + rotate) * r), c.Y + (int)(Math.Cos(a * 4 + rotate) * r));
                    if (shape.getFill() == 1)
                    {
                        e.FillPolygon(shape.getSolidBrush(), points);
                    }
                    e.DrawPolygon(shape.getPen(), points);
                };

            switch (shape.getType())
            {
                case 0:
                    drawRectangle(); break;
                case 1:
                    drawEllipse(); break;
                case 2:
                    drawRightTriangle(); break;
                case 3:
                    drawDiamond(); break;
                case 4:
                    drawIsocelesTriangle(); break;
                case 5:
                    drawHexagon(); break;
                case 6:
                    drawPentagon(); break;
            }
        }

        private void canvasPanel_Paint(object sender, PaintEventArgs e)
        {
            foreach (Shape s in shapes)
            {
                drawShape(e.Graphics, s);
            }
            if (tempShape != null)
            {
                drawShape(e.Graphics, tempShape);
            }
        }

        private void canvasPanel_MouseDown(object sender, MouseEventArgs e)
        {
            prevMouseLoc = new Point(e.X, e.Y);
            tempShape = new Shape(toolstrip_combobox.SelectedIndex, fillType, prevMouseLoc, prevMouseLoc, fillColor, lineColor, dashStyle, lineWidth);

            toolstrip_undo.Visible = true;
            undoToolStripMenuItem.Enabled = true;
            toolstrip_redo.Visible = false;
            redoToolStripMenuItem.Enabled = false;
            redoShapes = new List<Shape>();
            unsavedChanges = true;
        }

        private void canvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel2.Text = String.Format("Shapes: {0}", shapes.Count);
            toolStripStatusLabel3.Text = String.Format("Mouse: ({0}, {1})", e.X, e.Y);
            toolStripStatusLabel4.BackColor = fillColor;
            if (e.Button == MouseButtons.Left && tempShape != null)
            {
                tempShape.endPoint = new Point(e.X, e.Y);
                this.Refresh();
            }
        }

        private void canvasPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (prevMouseLoc.X != e.X && prevMouseLoc.Y != e.Y && tempShape != null){
                shapes.Add(tempShape);
                tempShape = null;
                unsavedChanges = true;
            }
        }

        private void toolstrip_fill1_Click(object sender, EventArgs e)
        {
            fillType = fillType == 0 ? 1 : 0;
            if (fillType == 0)
            {
                toolstrip_fill1.Checked = false;
                toolstrip_fill1.Text = "Drawn";
            }
            else
            {
                toolstrip_fill1.Checked = true;
                toolstrip_fill1.Text = "Fill";
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Shape Files (*.shp)|*.shp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                String[] text = File.ReadAllLines(openFileDialog.FileName);

                int[] tColor = Array.ConvertAll(text[0].Split(' '), s => int.Parse(s));
                canvasPanel.BackColor = Color.FromArgb(tColor[0], tColor[1], tColor[2]);

                String[] tImage = text[1].Split(new Char[] {' '}, 2);
                if (Int32.Parse(tImage[0]) == 1)
                {
                    Image image = Image.FromFile(tImage[1]);
                    imageFile = tImage[1];
                    canvasPanel.BackgroundImage = image;
                }
                else
                {
                    canvasPanel.BackgroundImage = null;
                }

                for (int i = 3; i < text.Length; i++)
                {
                    int[] ts = Array.ConvertAll(text[i].Split(' '), s => int.Parse(s));
                    DashStyle ds;
                    switch (ts[12])
                    {
                        case 1: ds = DashStyle.Solid; break;
                        case 2: ds = DashStyle.DashDot; break;
                        case 3: ds = DashStyle.Dot; break;
                        default: ds = DashStyle.Custom; break;
                    }
                    shapes.Add(new Shape(ts[0], ts[1], new Point(ts[2], ts[3]), new Point(ts[4], ts[5]), Color.FromArgb(ts[6], ts[7], ts[8]), Color.FromArgb(ts[9], ts[10], ts[11]), ds, ts[13]));
                }
            }
        }
        
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String text = "";

            text += String.Format("{0} {1} {2}\n", canvasPanel.BackColor.R, canvasPanel.BackColor.G, canvasPanel.BackColor.B);
            text += imageFile == null ? "0\n" : "1 " + imageFile + "\n";
            text += String.Format("{0}\n", shapes.Count);
            foreach (Shape s in shapes)
            {
                text += s.toString() + "\n";
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Shape Files (*.shp)|*.shp";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, text);
            }

            unsavedChanges = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (unsavedChanges)
            {
                saveToolStripMenuItem_Click(sender, e);
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to quit?", "Confirm Exit", MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to quit?", "Confirm Exit", MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (shapes.Count > 0)
            {
                redoShapes.Add(shapes[shapes.Count - 1]);
                shapes.RemoveAt(shapes.Count - 1);
                this.Refresh();
            }
            if (shapes.Count == 0)
            {
                toolstrip_undo.Visible = false;
                undoToolStripMenuItem.Enabled = false;
            }
            toolstrip_redo.Visible = true;
            redoToolStripMenuItem.Enabled = true;
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoShapes.Count > 0)
            {
                shapes.Add(redoShapes[redoShapes.Count - 1]);
                redoShapes.RemoveAt(redoShapes.Count - 1);
                this.Refresh();
            }
            if (redoShapes.Count == 0)
            {
                toolstrip_redo.Visible = false;
                redoToolStripMenuItem.Enabled = false;
            }
            toolstrip_undo.Visible = true;
            undoToolStripMenuItem.Enabled = true;
        }

        private void clearScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shapes = new List<Shape>();
            redoShapes = new List<Shape>();
            canvasPanel.BackgroundImage = null;
            canvasPanel.BackColor = Color.White;
            toolstrip_undo.Visible = false;
            undoToolStripMenuItem.Enabled = false;
            toolstrip_redo.Visible = false;
            redoToolStripMenuItem.Enabled = false;
            this.Refresh();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.gif;*.jpeg;*.jpg;*.png;*.tiff";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Image image = Image.FromFile(openFileDialog.FileName);
                imageFile = openFileDialog.FileName;
                canvasPanel.BackgroundImage = image;
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image File (*.bmp)|*.bmp";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(canvasPanel.Width, canvasPanel.Height);
                canvasPanel.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.Save(saveFileDialog.FileName);
            }
        }

        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                fillColor = colorDialog.Color;
            }
        }

        private void lineToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                lineColor = colorDialog.Color;
            }
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                canvasPanel.BackColor = colorDialog.Color;
            }
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LineDialog dialog = new LineDialog();
            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dashStyle = dialog.dashStyle;
                lineWidth = dialog.lineWidth;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialog dialog = new AboutDialog();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
            {
                if (e.Control && e.Shift && e.KeyCode == Keys.O)
                {
                    openToolStripMenuItem1_Click(sender, e);
                }
                else
                {
                    openToolStripMenuItem_Click(sender, e);
                }                
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                if (e.Control && e.Shift && e.KeyCode == Keys.S)
                {
                    saveToolStripMenuItem1_Click(sender, e);
                }
                else
                {
                    saveToolStripMenuItem_Click(sender, e);
                }                
            }

            if (e.Control && e.KeyCode == Keys.X)
            {
                Application.Exit();
            }

            if (e.Control && e.KeyCode == Keys.Z)
            {
                undoToolStripMenuItem_Click(sender, e);
            }

            if (e.Control && e.KeyCode == Keys.Y)
            {
                redoToolStripMenuItem_Click(sender, e);
            }

            if (e.Control && e.Shift && e.KeyCode == Keys.C)
            {
                clearScreenToolStripMenuItem_Click(sender, e);
            }

            if (e.Control && e.KeyCode == Keys.L)
            {
                if (e.Control && e.Shift && e.KeyCode == Keys.L)
                {
                    lineToolStripMenuItem1_Click(sender, e);
                }
                else
                {
                    lineToolStripMenuItem_Click(sender, e);
                }                
            }

            if (e.Control && e.Shift && e.KeyCode == Keys.F)
            {
                fillToolStripMenuItem_Click(sender, e);
            }



            if (e.Control && e.Shift && e.KeyCode == Keys.B)
            {
                backgroundToolStripMenuItem_Click(sender, e);
            }

            if (e.Control && e.KeyCode == Keys.H)
            {
                aboutToolStripMenuItem_Click(sender, e);
            }
        }
    }
}
