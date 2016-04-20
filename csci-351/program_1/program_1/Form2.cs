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
    public partial class Form2 : Form
    {
        int x = 0;

        public Form2()
        {
            InitializeComponent();

            textBox1.Validating += new CancelEventHandler(TextBox_Validating);
            textBox1.Validated += new EventHandler(TextBox_Validated);
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (int.TryParse(tb.Text, out x) || tb.Text == "")
            {
                if (x < 1 || x > 10)
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
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }

        public int curveValue { get; set; }

        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            this.curveValue = x;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
