namespace WinformExample_Mapview3D
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
            this.connectButton = new System.Windows.Forms.Button();
            this.mousePosLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadTerrainFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subTerrain = new Diswerx.SubControls.SubTerrain();
            this.msgTextbox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectButton.Enabled = false;
            this.connectButton.Location = new System.Drawing.Point(753, 446);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(214, 34);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Find Entities";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // mousePosLabel
            // 
            this.mousePosLabel.AutoSize = true;
            this.mousePosLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mousePosLabel.Location = new System.Drawing.Point(3, 443);
            this.mousePosLabel.Name = "mousePosLabel";
            this.mousePosLabel.Size = new System.Drawing.Size(744, 40);
            this.mousePosLabel.TabIndex = 2;
            this.mousePosLabel.Text = "Mouse Position:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadTerrainFilesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(970, 28);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadTerrainFilesToolStripMenuItem
            // 
            this.loadTerrainFilesToolStripMenuItem.Name = "loadTerrainFilesToolStripMenuItem";
            this.loadTerrainFilesToolStripMenuItem.Size = new System.Drawing.Size(137, 24);
            this.loadTerrainFilesToolStripMenuItem.Text = "Load Terrain Files";
            this.loadTerrainFilesToolStripMenuItem.Click += new System.EventHandler(this.loadTerrainFilesToolStripMenuItem_Click);
            // 
            // subTerrain
            // 
            this.subTerrain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.subTerrain.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("subTerrain.BackgroundImage")));
            this.subTerrain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.subTerrain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subTerrain.Location = new System.Drawing.Point(3, 3);
            this.subTerrain.Name = "subTerrain";
            this.subTerrain.Size = new System.Drawing.Size(744, 437);
            this.subTerrain.TabIndex = 0;
            // 
            // msgTextbox
            // 
            this.msgTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.msgTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.msgTextbox.Location = new System.Drawing.Point(753, 3);
            this.msgTextbox.Multiline = true;
            this.msgTextbox.Name = "msgTextbox";
            this.msgTextbox.Size = new System.Drawing.Size(214, 437);
            this.msgTextbox.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanel1.Controls.Add(this.subTerrain, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.connectButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.mousePosLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.msgTextbox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 28);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(970, 483);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 511);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Diswerx Mapview 3D";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Diswerx.SubControls.SubTerrain subTerrain;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label mousePosLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadTerrainFilesToolStripMenuItem;
        private System.Windows.Forms.TextBox msgTextbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}

