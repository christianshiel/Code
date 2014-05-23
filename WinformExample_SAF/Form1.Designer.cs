namespace WinformExample_SAF
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.deviceCombobox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.intervalTextbox = new System.Windows.Forms.TextBox();
            this.portTextbox = new System.Windows.Forms.TextBox();
            this.broadcastButton = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.entityComboBox = new System.Windows.Forms.ComboBox();
            this.entityPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.mapClickModeCheckbox = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.routePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.waypointCheckbox = new System.Windows.Forms.CheckBox();
            this.routeNameTextBox = new System.Windows.Forms.TextBox();
            this.createRouteButton = new System.Windows.Forms.Button();
            this.mousePositionLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.subSurface = new Diswerx.Surface2D.SubSurface();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(619, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(234, 402);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.deviceCombobox);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.intervalTextbox);
            this.tabPage1.Controls.Add(this.portTextbox);
            this.tabPage1.Controls.Add(this.broadcastButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(226, 373);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Setup";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Device";
            // 
            // deviceCombobox
            // 
            this.deviceCombobox.FormattingEnabled = true;
            this.deviceCombobox.Location = new System.Drawing.Point(9, 76);
            this.deviceCombobox.Name = "deviceCombobox";
            this.deviceCombobox.Size = new System.Drawing.Size(211, 24);
            this.deviceCombobox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Interval (milis)";
            // 
            // intervalTextbox
            // 
            this.intervalTextbox.Location = new System.Drawing.Point(120, 127);
            this.intervalTextbox.Name = "intervalTextbox";
            this.intervalTextbox.Size = new System.Drawing.Size(100, 22);
            this.intervalTextbox.TabIndex = 2;
            this.intervalTextbox.Text = "200";
            // 
            // portTextbox
            // 
            this.portTextbox.Location = new System.Drawing.Point(120, 19);
            this.portTextbox.Name = "portTextbox";
            this.portTextbox.Size = new System.Drawing.Size(100, 22);
            this.portTextbox.TabIndex = 1;
            this.portTextbox.Text = "3000";
            // 
            // broadcastButton
            // 
            this.broadcastButton.Location = new System.Drawing.Point(120, 172);
            this.broadcastButton.Name = "broadcastButton";
            this.broadcastButton.Size = new System.Drawing.Size(100, 37);
            this.broadcastButton.TabIndex = 0;
            this.broadcastButton.Text = "Broadcast";
            this.broadcastButton.UseVisualStyleBackColor = true;
            this.broadcastButton.Click += new System.EventHandler(this.broadcastButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.entityComboBox);
            this.tabPage2.Controls.Add(this.entityPropertyGrid);
            this.tabPage2.Controls.Add(this.mapClickModeCheckbox);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(226, 373);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SAF";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // entityComboBox
            // 
            this.entityComboBox.FormattingEnabled = true;
            this.entityComboBox.Location = new System.Drawing.Point(6, 343);
            this.entityComboBox.Name = "entityComboBox";
            this.entityComboBox.Size = new System.Drawing.Size(214, 24);
            this.entityComboBox.TabIndex = 8;
            this.entityComboBox.SelectedIndexChanged += new System.EventHandler(this.entityComboBox_SelectedIndexChanged);
            // 
            // entityPropertyGrid
            // 
            this.entityPropertyGrid.Location = new System.Drawing.Point(6, 46);
            this.entityPropertyGrid.Name = "entityPropertyGrid";
            this.entityPropertyGrid.Size = new System.Drawing.Size(215, 291);
            this.entityPropertyGrid.TabIndex = 7;
            this.entityPropertyGrid.ToolbarVisible = false;
            this.entityPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.entityPropertyGrid_PropertyValueChanged);
            // 
            // mapClickModeCheckbox
            // 
            this.mapClickModeCheckbox.AutoSize = true;
            this.mapClickModeCheckbox.Location = new System.Drawing.Point(6, 19);
            this.mapClickModeCheckbox.Name = "mapClickModeCheckbox";
            this.mapClickModeCheckbox.Size = new System.Drawing.Size(194, 21);
            this.mapClickModeCheckbox.TabIndex = 0;
            this.mapClickModeCheckbox.Text = "Check to enable map click";
            this.mapClickModeCheckbox.UseVisualStyleBackColor = true;
            this.mapClickModeCheckbox.CheckedChanged += new System.EventHandler(this.mapClickModeCheckbox_CheckedChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.routePropertyGrid);
            this.tabPage3.Controls.Add(this.waypointCheckbox);
            this.tabPage3.Controls.Add(this.routeNameTextBox);
            this.tabPage3.Controls.Add(this.createRouteButton);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(226, 373);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Routes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // routePropertyGrid
            // 
            this.routePropertyGrid.Location = new System.Drawing.Point(6, 105);
            this.routePropertyGrid.Name = "routePropertyGrid";
            this.routePropertyGrid.Size = new System.Drawing.Size(215, 262);
            this.routePropertyGrid.TabIndex = 14;
            this.routePropertyGrid.ToolbarVisible = false;
            // 
            // waypointCheckbox
            // 
            this.waypointCheckbox.AutoSize = true;
            this.waypointCheckbox.Location = new System.Drawing.Point(6, 78);
            this.waypointCheckbox.Name = "waypointCheckbox";
            this.waypointCheckbox.Size = new System.Drawing.Size(189, 21);
            this.waypointCheckbox.TabIndex = 13;
            this.waypointCheckbox.Text = "Check to place waypoints";
            this.waypointCheckbox.UseVisualStyleBackColor = true;
            this.waypointCheckbox.CheckedChanged += new System.EventHandler(this.waypointCheckbox_CheckedChanged);
            // 
            // routeNameTextBox
            // 
            this.routeNameTextBox.Location = new System.Drawing.Point(6, 14);
            this.routeNameTextBox.Name = "routeNameTextBox";
            this.routeNameTextBox.ReadOnly = true;
            this.routeNameTextBox.Size = new System.Drawing.Size(214, 22);
            this.routeNameTextBox.TabIndex = 12;
            this.routeNameTextBox.Text = "Route";
            // 
            // createRouteButton
            // 
            this.createRouteButton.Location = new System.Drawing.Point(6, 42);
            this.createRouteButton.Name = "createRouteButton";
            this.createRouteButton.Size = new System.Drawing.Size(215, 30);
            this.createRouteButton.TabIndex = 11;
            this.createRouteButton.Text = "Create New";
            this.createRouteButton.UseVisualStyleBackColor = true;
            this.createRouteButton.Click += new System.EventHandler(this.createRouteButton_Click);
            // 
            // mousePositionLabel
            // 
            this.mousePositionLabel.AutoSize = true;
            this.mousePositionLabel.Location = new System.Drawing.Point(3, 408);
            this.mousePositionLabel.Name = "mousePositionLabel";
            this.mousePositionLabel.Size = new System.Drawing.Size(108, 17);
            this.mousePositionLabel.TabIndex = 2;
            this.mousePositionLabel.Text = "Mouse Position:";
            // 
            // timer1
            // 
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
            this.subSurface.Size = new System.Drawing.Size(610, 402);
            this.subSurface.TabIndex = 0;
            this.subSurface.ZoomSpeed = 0.05F;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel1.Controls.Add(this.mousePositionLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.subSurface, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(856, 448);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(226, 373);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Data";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 448);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Diswerx SAF";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Diswerx.Surface2D.SubSurface subSurface;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox intervalTextbox;
        private System.Windows.Forms.TextBox portTextbox;
        private System.Windows.Forms.Button broadcastButton;
        private System.Windows.Forms.CheckBox mapClickModeCheckbox;
        private System.Windows.Forms.Label mousePositionLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox deviceCombobox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PropertyGrid entityPropertyGrid;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox entityComboBox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox waypointCheckbox;
        private System.Windows.Forms.TextBox routeNameTextBox;
        private System.Windows.Forms.Button createRouteButton;
        private System.Windows.Forms.PropertyGrid routePropertyGrid;
        private System.Windows.Forms.TabPage tabPage4;
    }
}

