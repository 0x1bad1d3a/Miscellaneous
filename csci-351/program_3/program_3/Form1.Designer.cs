namespace program_3
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.parametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolstrip_file = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_save = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_undo = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_redo = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_fill1 = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_line1 = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_line2 = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_fill2 = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_back = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_image1 = new System.Windows.Forms.ToolStripButton();
            this.toolstripe_image2 = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_about = new System.Windows.Forms.ToolStripButton();
            this.toolstrip_combobox = new System.Windows.Forms.ToolStripComboBox();
            this.canvasPanel = new System.Windows.Forms.Panel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripMenuItem,
            this.editToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.parametersToolStripMenuItem,
            this.colorToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(710, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mainToolStripMenuItem
            // 
            this.mainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.mainToolStripMenuItem.Name = "mainToolStripMenuItem";
            this.mainToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.mainToolStripMenuItem.Text = "Main";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearScreenToolStripMenuItem,
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem1});
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.imageToolStripMenuItem.Text = "Image";
            // 
            // clearScreenToolStripMenuItem
            // 
            this.clearScreenToolStripMenuItem.Name = "clearScreenToolStripMenuItem";
            this.clearScreenToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.clearScreenToolStripMenuItem.Text = "Clear Screen";
            this.clearScreenToolStripMenuItem.Click += new System.EventHandler(this.clearScreenToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(139, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // parametersToolStripMenuItem
            // 
            this.parametersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineToolStripMenuItem});
            this.parametersToolStripMenuItem.Name = "parametersToolStripMenuItem";
            this.parametersToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.parametersToolStripMenuItem.Text = "Parameters";
            // 
            // lineToolStripMenuItem
            // 
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            this.lineToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.lineToolStripMenuItem.Text = "Line";
            this.lineToolStripMenuItem.Click += new System.EventHandler(this.lineToolStripMenuItem_Click);
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fillToolStripMenuItem,
            this.lineToolStripMenuItem1,
            this.backgroundToolStripMenuItem});
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.colorToolStripMenuItem.Text = "Color";
            // 
            // fillToolStripMenuItem
            // 
            this.fillToolStripMenuItem.Name = "fillToolStripMenuItem";
            this.fillToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fillToolStripMenuItem.Text = "Fill";
            this.fillToolStripMenuItem.Click += new System.EventHandler(this.fillToolStripMenuItem_Click);
            // 
            // lineToolStripMenuItem1
            // 
            this.lineToolStripMenuItem1.Name = "lineToolStripMenuItem1";
            this.lineToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.lineToolStripMenuItem1.Text = "Line";
            this.lineToolStripMenuItem1.Click += new System.EventHandler(this.lineToolStripMenuItem1_Click);
            // 
            // backgroundToolStripMenuItem
            // 
            this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
            this.backgroundToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.backgroundToolStripMenuItem.Text = "Background";
            this.backgroundToolStripMenuItem.Click += new System.EventHandler(this.backgroundToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstrip_file,
            this.toolstrip_save,
            this.toolstrip_undo,
            this.toolstrip_redo,
            this.toolstrip_fill1,
            this.toolstrip_line1,
            this.toolstrip_line2,
            this.toolstrip_fill2,
            this.toolstrip_back,
            this.toolstrip_image1,
            this.toolstripe_image2,
            this.toolstrip_about,
            this.toolstrip_combobox});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(710, 38);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolstrip_file
            // 
            this.toolstrip_file.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_file.Image")));
            this.toolstrip_file.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_file.Name = "toolstrip_file";
            this.toolstrip_file.Size = new System.Drawing.Size(40, 35);
            this.toolstrip_file.Text = "Open";
            this.toolstrip_file.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_file.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolstrip_save
            // 
            this.toolstrip_save.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_save.Image")));
            this.toolstrip_save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_save.Name = "toolstrip_save";
            this.toolstrip_save.Size = new System.Drawing.Size(35, 35);
            this.toolstrip_save.Text = "Save";
            this.toolstrip_save.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_save.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolstrip_undo
            // 
            this.toolstrip_undo.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_undo.Image")));
            this.toolstrip_undo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_undo.Name = "toolstrip_undo";
            this.toolstrip_undo.Size = new System.Drawing.Size(40, 35);
            this.toolstrip_undo.Text = "Undo";
            this.toolstrip_undo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_undo.Visible = false;
            this.toolstrip_undo.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // toolstrip_redo
            // 
            this.toolstrip_redo.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_redo.Image")));
            this.toolstrip_redo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_redo.Name = "toolstrip_redo";
            this.toolstrip_redo.Size = new System.Drawing.Size(38, 35);
            this.toolstrip_redo.Text = "Redo";
            this.toolstrip_redo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_redo.Visible = false;
            this.toolstrip_redo.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolstrip_fill1
            // 
            this.toolstrip_fill1.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_fill1.Image")));
            this.toolstrip_fill1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_fill1.Name = "toolstrip_fill1";
            this.toolstrip_fill1.Size = new System.Drawing.Size(45, 35);
            this.toolstrip_fill1.Text = "Drawn";
            this.toolstrip_fill1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_fill1.Click += new System.EventHandler(this.toolstrip_fill1_Click);
            // 
            // toolstrip_line1
            // 
            this.toolstrip_line1.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_line1.Image")));
            this.toolstrip_line1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_line1.Name = "toolstrip_line1";
            this.toolstrip_line1.Size = new System.Drawing.Size(33, 35);
            this.toolstrip_line1.Text = "Line";
            this.toolstrip_line1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_line1.Click += new System.EventHandler(this.lineToolStripMenuItem_Click);
            // 
            // toolstrip_line2
            // 
            this.toolstrip_line2.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_line2.Image")));
            this.toolstrip_line2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_line2.Name = "toolstrip_line2";
            this.toolstrip_line2.Size = new System.Drawing.Size(33, 35);
            this.toolstrip_line2.Text = "Line";
            this.toolstrip_line2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_line2.Click += new System.EventHandler(this.lineToolStripMenuItem1_Click);
            // 
            // toolstrip_fill2
            // 
            this.toolstrip_fill2.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_fill2.Image")));
            this.toolstrip_fill2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_fill2.Name = "toolstrip_fill2";
            this.toolstrip_fill2.Size = new System.Drawing.Size(26, 35);
            this.toolstrip_fill2.Text = "Fill";
            this.toolstrip_fill2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_fill2.Click += new System.EventHandler(this.fillToolStripMenuItem_Click);
            // 
            // toolstrip_back
            // 
            this.toolstrip_back.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_back.Image")));
            this.toolstrip_back.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_back.Name = "toolstrip_back";
            this.toolstrip_back.Size = new System.Drawing.Size(36, 35);
            this.toolstrip_back.Text = "Back";
            this.toolstrip_back.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_back.Click += new System.EventHandler(this.backgroundToolStripMenuItem_Click);
            // 
            // toolstrip_image1
            // 
            this.toolstrip_image1.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_image1.Image")));
            this.toolstrip_image1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_image1.Name = "toolstrip_image1";
            this.toolstrip_image1.Size = new System.Drawing.Size(44, 35);
            this.toolstrip_image1.Text = "Image";
            this.toolstrip_image1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_image1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // toolstripe_image2
            // 
            this.toolstripe_image2.Image = ((System.Drawing.Image)(resources.GetObject("toolstripe_image2.Image")));
            this.toolstripe_image2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstripe_image2.Name = "toolstripe_image2";
            this.toolstripe_image2.Size = new System.Drawing.Size(44, 35);
            this.toolstripe_image2.Text = "Image";
            this.toolstripe_image2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstripe_image2.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // toolstrip_about
            // 
            this.toolstrip_about.Image = ((System.Drawing.Image)(resources.GetObject("toolstrip_about.Image")));
            this.toolstrip_about.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolstrip_about.Name = "toolstrip_about";
            this.toolstrip_about.Size = new System.Drawing.Size(44, 35);
            this.toolstrip_about.Text = "About";
            this.toolstrip_about.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolstrip_about.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolstrip_combobox
            // 
            this.toolstrip_combobox.Items.AddRange(new object[] {
            "Rectangle",
            "Ellipse",
            "Right Triangle",
            "Diamond",
            "Isosceles Triangle",
            "Hexagon",
            "Pentagon"});
            this.toolstrip_combobox.Name = "toolstrip_combobox";
            this.toolstrip_combobox.Size = new System.Drawing.Size(121, 38);
            // 
            // canvasPanel
            // 
            this.canvasPanel.AutoSize = true;
            this.canvasPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.canvasPanel.BackColor = System.Drawing.Color.White;
            this.canvasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasPanel.Location = new System.Drawing.Point(0, 62);
            this.canvasPanel.Margin = new System.Windows.Forms.Padding(0);
            this.canvasPanel.Name = "canvasPanel";
            this.canvasPanel.Size = new System.Drawing.Size(710, 424);
            this.canvasPanel.TabIndex = 2;
            this.canvasPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.canvasPanel_Paint);
            this.canvasPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvasPanel_MouseDown);
            this.canvasPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvasPanel_MouseMove);
            this.canvasPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvasPanel_MouseUp);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(513, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Help Text";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(56, 17);
            this.toolStripStatusLabel2.Text = "Shapes: 0";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusLabel3.RightToLeftAutoMirrorImage = true;
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(72, 17);
            this.toolStripStatusLabel3.Text = "Mouse: (0,0)";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(54, 17);
            this.toolStripStatusLabel4.Text = "Fill Color";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4});
            this.statusStrip1.Location = new System.Drawing.Point(0, 464);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(710, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 486);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.canvasPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Paint Thingy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem parametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem backgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolstrip_file;
        private System.Windows.Forms.ToolStripButton toolstrip_save;
        private System.Windows.Forms.ToolStripButton toolstrip_undo;
        private System.Windows.Forms.ToolStripButton toolstrip_redo;
        private System.Windows.Forms.ToolStripButton toolstrip_fill1;
        private System.Windows.Forms.ToolStripButton toolstrip_line1;
        private System.Windows.Forms.Panel canvasPanel;
        private System.Windows.Forms.ToolStripButton toolstrip_image1;
        private System.Windows.Forms.ToolStripButton toolstripe_image2;
        private System.Windows.Forms.ToolStripButton toolstrip_back;
        private System.Windows.Forms.ToolStripButton toolstrip_fill2;
        private System.Windows.Forms.ToolStripButton toolstrip_line2;
        private System.Windows.Forms.ToolStripButton toolstrip_about;
        private System.Windows.Forms.ToolStripComboBox toolstrip_combobox;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.StatusStrip statusStrip1;
    }
}

