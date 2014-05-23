using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PcapDotNet.Packets;
using PcapDotNet.Core;
using PcapDotNet.Packets.Transport;
using System.IO;
using Diswerx.Networking;
using OpenDis.Dis1998;

namespace Diswerx
{
    public class DisRouter
    {
        public delegate void DisPduIn(Pdu pdu);
        public event DisPduIn OnDisPduIn;     

        private DisFilterEngine disFilterEngine;
        
        private ushort outPort;
        private int deviceIndex;

        private Control user;
        private DisNetwork disNet;

        /// <summary>
        /// Dis router
        /// </summary>
        /// <param name="user">Parent winform/control</param>
        public DisRouter(DisNetwork disNet)            
        {
            this.user = disNet.User;
            this.disNet = disNet;
            disFilterEngine = new DisFilterEngine();
        }

        void disNet_OnDisPduIn(Pdu pdu)
        {
            if (disFilterEngine.PassesFilters(pdu))
            {                
                byte[] data = DisHelpers.MarshalPdu(pdu);
                
                Packet p = NetworkingHelpers.CreateUdpBroadcastPacket(data, outPort, disNet.SelectedDevice);
                disNet.BroadcastPacket(deviceIndex, p, outPort);
            }
            else
            {
                // leave message
                
            }

            if (OnDisPduIn != null)
            {
                OnDisPduIn.Invoke(pdu);
                //OnDisPduIn.Invoke(pdu);
            }
        }

        /// <summary>
        /// Forward any DIS traffic on specified device and port
        /// </summary>
        /// <param name="deviceIndex">Zero based device index</param>
        /// <param name="outPort">Outgoing port to send on</param>
        public void ForwardTraffic(int deviceIndex, ushort outPort)
        {
            disNet.OnDisPduIn -= new DisNetwork.DisPduIn(disNet_OnDisPduIn);
            disNet.OnDisPduIn += new DisNetwork.DisPduIn(disNet_OnDisPduIn);
            this.outPort = outPort;
            this.deviceIndex = deviceIndex;
        }

        public void AddFilter(DisFilter filter)
        {
            disFilterEngine.AddFilter(filter);
        }

        public void RemoveAllFilters()
        {
            disFilterEngine.ClearFilters();
        }

        public void Terminate()
        {
            disNet.OnDisPduIn -= new DisNetwork.DisPduIn(disNet_OnDisPduIn);
            //disNet.Dispose();
        }


    }
}
