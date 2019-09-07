namespace FurnaceControl
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeviceBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.FurnaceGroup = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ProgramSelector = new System.Windows.Forms.ComboBox();
            this.StartHaltButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.LogBox = new System.Windows.Forms.RichTextBox();
            this.GraphBox = new System.Windows.Forms.PictureBox();
            this.GraphRefresh = new System.Windows.Forms.Timer(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SchedulePicker = new System.Windows.Forms.DateTimePicker();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.FurnaceGroup.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GraphBox)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 692);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1172, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1172, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit);
            // 
            // DeviceBox
            // 
            this.DeviceBox.FormattingEnabled = true;
            this.DeviceBox.Location = new System.Drawing.Point(9, 19);
            this.DeviceBox.Name = "DeviceBox";
            this.DeviceBox.Size = new System.Drawing.Size(106, 21);
            this.DeviceBox.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ConnectButton);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.DeviceBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(440, 51);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Port";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnectButton.Location = new System.Drawing.Point(353, 18);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(76, 23);
            this.ConnectButton.TabIndex = 4;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectDevice);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(121, 18);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.RefreshDevices);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.FurnaceGroup);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(712, 24);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(460, 668);
            this.panel1.TabIndex = 4;
            // 
            // FurnaceGroup
            // 
            this.FurnaceGroup.Controls.Add(this.SchedulePicker);
            this.FurnaceGroup.Controls.Add(this.button4);
            this.FurnaceGroup.Controls.Add(this.button3);
            this.FurnaceGroup.Controls.Add(this.button2);
            this.FurnaceGroup.Controls.Add(this.ProgramSelector);
            this.FurnaceGroup.Controls.Add(this.StartHaltButton);
            this.FurnaceGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.FurnaceGroup.Enabled = false;
            this.FurnaceGroup.Location = new System.Drawing.Point(10, 61);
            this.FurnaceGroup.Name = "FurnaceGroup";
            this.FurnaceGroup.Size = new System.Drawing.Size(440, 94);
            this.FurnaceGroup.TabIndex = 5;
            this.FurnaceGroup.TabStop = false;
            this.FurnaceGroup.Text = "Furnace";
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(71, 61);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(25, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "X";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(40, 61);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(25, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "+";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.AddNewProgram);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(9, 61);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "=";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ProgramSelector
            // 
            this.ProgramSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgramSelector.FormattingEnabled = true;
            this.ProgramSelector.Location = new System.Drawing.Point(9, 28);
            this.ProgramSelector.Name = "ProgramSelector";
            this.ProgramSelector.Size = new System.Drawing.Size(420, 21);
            this.ProgramSelector.TabIndex = 5;
            // 
            // StartHaltButton
            // 
            this.StartHaltButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartHaltButton.Location = new System.Drawing.Point(354, 61);
            this.StartHaltButton.Name = "StartHaltButton";
            this.StartHaltButton.Size = new System.Drawing.Size(75, 23);
            this.StartHaltButton.TabIndex = 0;
            this.StartHaltButton.Text = "START";
            this.StartHaltButton.UseVisualStyleBackColor = true;
            this.StartHaltButton.Click += new System.EventHandler(this.StartHaltNow);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.LogBox);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(10, 332);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.groupBox3.Size = new System.Drawing.Size(440, 326);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Log";
            // 
            // LogBox
            // 
            this.LogBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LogBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.LogBox.Location = new System.Drawing.Point(5, 18);
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.LogBox.Size = new System.Drawing.Size(432, 305);
            this.LogBox.TabIndex = 0;
            this.LogBox.Text = "";
            // 
            // GraphBox
            // 
            this.GraphBox.BackColor = System.Drawing.Color.White;
            this.GraphBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GraphBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GraphBox.Location = new System.Drawing.Point(0, 0);
            this.GraphBox.Name = "GraphBox";
            this.GraphBox.Size = new System.Drawing.Size(712, 663);
            this.GraphBox.TabIndex = 5;
            this.GraphBox.TabStop = false;
            this.GraphBox.SizeChanged += new System.EventHandler(this.UpdateImageNow);
            this.GraphBox.Paint += new System.Windows.Forms.PaintEventHandler(this.UpdateImage);
            // 
            // GraphRefresh
            // 
            this.GraphRefresh.Interval = 5000;
            this.GraphRefresh.Tick += new System.EventHandler(this.RefreshGraph);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.GraphBox);
            this.panel3.Controls.Add(this.splitter1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 24);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(712, 668);
            this.panel3.TabIndex = 6;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 663);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(712, 5);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // SchedulePicker
            // 
            this.SchedulePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SchedulePicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SchedulePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.SchedulePicker.Location = new System.Drawing.Point(102, 62);
            this.SchedulePicker.MinDate = new System.DateTime(2019, 1, 1, 0, 0, 0, 0);
            this.SchedulePicker.Name = "SchedulePicker";
            this.SchedulePicker.Size = new System.Drawing.Size(246, 21);
            this.SchedulePicker.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1172, 714);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Furnace Control | Studio Mlichtronix 2019";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveBeforeClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.FurnaceGroup.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GraphBox)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ComboBox DeviceBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox GraphBox;
        private System.Windows.Forms.Timer GraphRefresh;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox LogBox;
        private System.Windows.Forms.GroupBox FurnaceGroup;
        private System.Windows.Forms.ComboBox ProgramSelector;
        private System.Windows.Forms.Button StartHaltButton;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DateTimePicker SchedulePicker;
    }
}

