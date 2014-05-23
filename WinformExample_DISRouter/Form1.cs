using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Diswerx;
using OpenDis.Dis1998;
using Diswerx.Common.Coordinates;
using Networking;

namespace WinformExample_DISRouter
{
    public partial class Form1 : Form
    {
        private DisNetwork disNet;
        //private DisRouter disRouter;

        private NetworkManager3 net;

        private int deviceIndex; // in
        //private int device2; // out

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            disNet = new DisNetwork(this);
            //disRouter = new DisRouter(disNet);

            deviceCombobox.Items.AddRange(disNet.GetWindowsDeviceNames().ToArray());
            device2Combobox.Items.AddRange(disNet.GetWindowsDeviceNames().ToArray());

            deviceIndex = disNet.GetDefaultBroadcastDevice();
            deviceCombobox.SelectedIndex = deviceIndex;
            device2Combobox.SelectedIndex = deviceIndex;
        }

        private void Setup()
        {
            //disRouter.OnDisPduIn += new DisRouter.DisPduIn(disRouter_OnDisPduIn);

            // Example of premade filter
            //disRouter.AddFilter(new EntityTypeFilter("0.0.0.0.0.0.0"));

            List<PointD> poly = new List<PointD>();
            poly.Add(new PointD(0, 0));

            // Example of roll-your-own filter
            //disRouter.AddFilter(new CustomFilter(poly));

            ushort port1 = ushort.Parse(portTextbox.Text);
            ushort port2 = ushort.Parse(port2Textbox.Text);

            //disRouter.ForwardTraffic(deviceIndex, port2);

            net = new NetworkManager3(this);
            net.Start();

            disNet.OnDisPduIn += new DisNetwork.DisPduIn(disRouter_OnDisPduIn);            
            DisSource source = new DisSource("Unknown", port1, deviceIndex);
            disNet.Connect(source);

            //disRouter.ForwardTraffic(deviceIndex, port2);
        }

        void disRouter_OnDisPduIn(Pdu pdu)
        {
            DisplayMessage("Message received: " + DisHelpers.GetPduType(pdu.PduType));
            byte[] raw = DisHelpers.MarshalPdu(pdu);
            net.Send(raw);
        }

        private void DisplayMessage(string message)
        {
            infoTextbox.AppendText(message + System.Environment.NewLine);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //disRouter.Terminate();
            disNet.Terminate();

            if (disNet != null)
            {
                disNet.Terminate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // begin the routing
            Setup();
        }
    }
}
