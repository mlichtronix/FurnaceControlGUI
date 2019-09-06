namespace FurnaceControl
{
    partial class ProgramDesigner
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ProgramValues = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.programBlockBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Temperature = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Wattage = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProgramValues)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.programBlockBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.NameBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8);
            this.groupBox1.Size = new System.Drawing.Size(456, 49);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Program Name";
            // 
            // NameBox
            // 
            this.NameBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.NameBox.Location = new System.Drawing.Point(8, 21);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(440, 20);
            this.NameBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ProgramValues);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(10, 59);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(8);
            this.groupBox2.Size = new System.Drawing.Size(456, 505);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Program Name";
            // 
            // ProgramValues
            // 
            this.ProgramValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ProgramValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ProgramValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Temperature,
            this.Duration,
            this.Wattage});
            this.ProgramValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgramValues.Location = new System.Drawing.Point(8, 21);
            this.ProgramValues.Name = "ProgramValues";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ProgramValues.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.ProgramValues.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ProgramValues.Size = new System.Drawing.Size(440, 476);
            this.ProgramValues.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(10, 564);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(456, 36);
            this.panel1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(8, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(59, 28);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Cancel);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(389, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 28);
            this.button1.TabIndex = 0;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ApplyValues);
            // 
            // programBlockBindingSource
            // 
            this.programBlockBindingSource.DataSource = typeof(FurnaceControl.ProgramBlock);
            // 
            // Temperature
            // 
            this.Temperature.DataPropertyName = "Temperature";
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.Temperature.DefaultCellStyle = dataGridViewCellStyle1;
            this.Temperature.HeaderText = "Temperature [°C]";
            this.Temperature.MaxInputLength = 4;
            this.Temperature.Name = "Temperature";
            // 
            // Duration
            // 
            this.Duration.DataPropertyName = "Duration";
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = null;
            this.Duration.DefaultCellStyle = dataGridViewCellStyle2;
            this.Duration.HeaderText = "Duration [min]";
            this.Duration.MaxInputLength = 4;
            this.Duration.Name = "Duration";
            // 
            // Wattage
            // 
            this.Wattage.DataPropertyName = "Wattage";
            this.Wattage.HeaderText = "Wattage [kW]";
            this.Wattage.Items.AddRange(new object[] {
            "10",
            "30"});
            this.Wattage.Name = "Wattage";
            // 
            // ProgramDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 603);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "ProgramDesigner";
            this.Padding = new System.Windows.Forms.Padding(10, 10, 10, 3);
            this.ShowIcon = false;
            this.Text = "Program designer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ProgramValues)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.programBlockBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView ProgramValues;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.BindingSource programBlockBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Temperature;
        private System.Windows.Forms.DataGridViewTextBoxColumn Duration;
        private System.Windows.Forms.DataGridViewComboBoxColumn Wattage;
    }
}