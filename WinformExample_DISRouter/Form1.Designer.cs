namespace WinformExample_DISRouter
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
            this.label4 = new System.Windows.Forms.Label();
            this.deviceCombobox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.portTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.device2Combobox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.port2Textbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.infoTextbox = new System.Windows.Forms.TextBox();
            this.methodComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Device";
            // 
            // deviceCombobox
            // 
            this.deviceCombobox.FormattingEnabled = true;
            this.deviceCombobox.Location = new System.Drawing.Point(12, 66);
            this.deviceCombobox.Name = "deviceCombobox";
            this.deviceCombobox.Size = new System.Drawing.Size(201, 24);
            this.deviceCombobox.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Port";
            // 
            // portTextbox
            // 
            this.portTextbox.Location = new System.Drawing.Point(113, 9);
            this.portTextbox.Name = "portTextbox";
            this.portTextbox.Size = new System.Drawing.Size(100, 22);
            this.portTextbox.TabIndex = 7;
            this.portTextbox.Text = "3000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(407, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "Device";
            // 
            // device2Combobox
            // 
            this.device2Combobox.FormattingEnabled = true;
            this.device2Combobox.Location = new System.Drawing.Point(410, 66);
            this.device2Combobox.Name = "device2Combobox";
            this.device2Combobox.Size = new System.Drawing.Size(201, 24);
            this.device2Combobox.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(407, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Port";
            // 
            // port2Textbox
            // 
            this.port2Textbox.Location = new System.Drawing.Point(511, 12);
            this.port2Textbox.Name = "port2Textbox";
            this.port2Textbox.Size = new System.Drawing.Size(100, 22);
            this.port2Textbox.TabIndex = 11;
            this.port2Textbox.Text = "3000";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(253, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 33);
            this.button1.TabIndex = 15;
            this.button1.Text = "Route";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // infoTextbox
            // 
            this.infoTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.infoTextbox.Location = new System.Drawing.Point(12, 110);
            this.infoTextbox.Multiline = true;
            this.infoTextbox.Name = "infoTextbox";
            this.infoTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.infoTextbox.Size = new System.Drawing.Size(605, 66);
            this.infoTextbox.TabIndex = 16;
            // 
            // methodComboBox
            // 
            this.methodComboBox.FormattingEnabled = true;
            this.methodComboBox.Items.AddRange(new object[] {
            "TCP Secure",
            "UDP"});
            this.methodComboBox.Location = new System.Drawing.Point(219, 66);
            this.methodComboBox.Name = "methodComboBox";
            this.methodComboBox.Size = new System.Drawing.Size(185, 24);
            this.methodComboBox.TabIndex = 17;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 188);
            this.Controls.Add(this.methodComboBox);
            this.Controls.Add(this.infoTextbox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.device2Combobox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.port2Textbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.deviceCombobox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.portTextbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Diswerx DIS Router";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox deviceCombobox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox portTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox device2Combobox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox port2Textbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox infoTextbox;
        private System.Windows.Forms.ComboBox methodComboBox;
    }
}

