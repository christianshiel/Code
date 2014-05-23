using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DiswerxSamples
{
    public partial class Form1 : Form
    {
        private string path = @"C:\Diswerx\Examples\";

        private string safPath = @"SAF\bin\Release\WinformExample_SAF.exe";
        private string routerPath = @"DIS_Router\bin\Release\WinformExample_DISRouter.exe";
        private string monitorPath = @"DIS_Monitor\bin\Release\WinformExample_Monitor.exe";
        private string map3dPath = @"Mapview3D\bin\Release\WinformExample_Mapview3D.exe";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = (string)listBox1.SelectedItem;

            if (str != null)
            {
                switch (str)
                {
                    case "SAF Example": System.Diagnostics.Process.Start(path + safPath); break;
                    case "Monitor Example": System.Diagnostics.Process.Start(path + monitorPath); break;
                    case "3D Map Example": System.Diagnostics.Process.Start(path + map3dPath); break;
                    case "Router Example": System.Diagnostics.Process.Start(path + routerPath); break;
                    default: break;
                }
                
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
    }
}
