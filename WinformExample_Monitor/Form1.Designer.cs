namespace WinformExample_Monitor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.mousePositionLabel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.subSurface = new Diswerx.Surface2D.SubSurface();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.messageTextbox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.deviceCombobox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.portTextbox = new System.Windows.Forms.TextBox();
            this.autoDetectCheckbox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mousePositionLabel
            // 
            this.mousePositionLabel.AutoSize = true;
            this.mousePositionLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mousePositionLabel.Location = new System.Drawing.Point(3, 443);
            this.mousePositionLabel.Name = "mousePositionLabel";
            this.mousePositionLabel.Size = new System.Drawing.Size(710, 17);
            this.mousePositionLabel.TabIndex = 1;
            this.mousePositionLabel.Text = "Mouse Position:";
            // 
            // connectButton
            // 
            this.connectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectButton.Location = new System.Drawing.Point(719, 446);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(214, 34);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Detect Entities";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // subSurface
            // 
            this.subSurface.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.subSurface.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("subSurface.BackgroundImage")));
            this.subSurface.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.subSurface.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.subSurface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subSurface.Location = new System.Drawing.Point(3, 3);
            this.subSurface.MapScale = 0.1F;
            this.subSurface.MoveSpeed = 10F;
            this.subSurface.Name = "subSurface";
            this.subSurface.Size = new System.Drawing.Size(710, 437);
            this.subSurface.TabIndex = 0;
            this.subSurface.ZoomSpeed = 0.05F;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanel1.Controls.Add(this.connectButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.mousePositionLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.subSurface, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(936, 483);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(719, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(214, 437);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.messageTextbox);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(206, 408);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Messages";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // messageTextbox
            // 
            this.messageTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.messageTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageTextbox.Location = new System.Drawing.Point(3, 3);
            this.messageTextbox.Multiline = true;
            this.messageTextbox.Name = "messageTextbox";
            this.messageTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.messageTextbox.Size = new System.Drawing.Size(200, 402);
            this.messageTextbox.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.deviceCombobox);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.portTextbox);
            this.tabPage2.Controls.Add(this.autoDetectCheckbox);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(206, 408);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Setup";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Device";
            // 
            // deviceCombobox
            // 
            this.deviceCombobox.FormattingEnabled = true;
            this.deviceCombobox.Location = new System.Drawing.Point(9, 109);
            this.deviceCombobox.Name = "deviceCombobox";
            this.deviceCombobox.Size = new System.Drawing.Size(191, 24);
            this.deviceCombobox.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Port";
            // 
            // portTextbox
            // 
            this.portTextbox.Location = new System.Drawing.Point(100, 52);
            this.portTextbox.Name = "portTextbox";
            this.portTextbox.Size = new System.Drawing.Size(100, 22);
            this.portTextbox.TabIndex = 7;
            this.portTextbox.Text = "4000";
            // 
            // autoDetectCheckbox
            // 
            this.autoDetectCheckbox.AutoSize = true;
            this.autoDetectCheckbox.Checked = true;
            this.autoDetectCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoDetectCheckbox.Location = new System.Drawing.Point(6, 15);
            this.autoDetectCheckbox.Name = "autoDetectCheckbox";
            this.autoDetectCheckbox.Size = new System.Drawing.Size(130, 21);
            this.autoDetectCheckbox.TabIndex = 0;
            this.autoDetectCheckbox.Text = "Auto Detect DIS";
            this.autoDetectCheckbox.UseVisualStyleBackColor = true;
            this.autoDetectCheckbox.CheckedChanged += new System.EventHandler(this.autoDetectCheckbox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 483);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Diswerx DIS Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Diswerx.Surface2D.SubSurface subSurface;
        private System.Windows.Forms.Label mousePositionLabel;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox messageTextbox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox autoDetectCheckbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox deviceCombobox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox portTextbox;
    }
}

