using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diswerx.Networking;
using OpenDis.Dis1998;
using OpenDis.Core;
using System.Windows.Forms;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Transport;
using System.IO;
using PcapDotNet.Core;
using System.Diagnostics;
using System.Threading;

namespace Diswerx
{
    public class DisNetwork : NetworkManager2
    {
        public delegate void DisPduIn(Pdu pdu);
        public event DisPduIn OnDisPduIn;

        public delegate void DisDetected(DisSource source);
        public event DisDetected OnDisDetected;

        private bool detectingDis = false;

        /// <summary>
        /// Create a new DisNetwork manager
        /// </summary>
        /// <param name="user">Parent winform/control</param>
        public DisNetwork(Control user)
            : base(user)
        {
            base.OnPacketIn += new PacketIn(DisNetwork_OnPacketIn);
        }

        // todo
        // Xaml application
        //public DisRouter(UserControl user)
        //{
        //}

        // todo
        /// <summary>
        /// Set the endian-ess of sendering/receiving
        /// </summary>
        public void SetDisEndian()
        {
        }

        /// <summary>
        /// Auto connect to a DIS source when found (not implemented yet).
        /// </summary>
        public void AutoConnectToDisSource()
        {
            throw new Exception("Not implemented yet.");
        }

        /// <summary>
        /// Connect to a DIS source.
        /// </summary>
        /// <param name="source">The DIS source to listen to</param>
        public void Connect(DisSource source, int sleep = 1000)
        {
            base.Listen(source.DeviceIndex, source.Port, sleep);
        }

        /// <summary>
        /// Detect DIS on a device.
        /// </summary>
        /// <param name="onDisDetected">Callback to notify when DIS arrives</param>
        /// <param name="deviceIndex">Zero based device index to listen on</param>
        public void DetectDis(DisDetected onDisDetected, int deviceIndex)
        {
            detectingDis = true;
            OnDisDetected = onDisDetected;

            base.Listen(deviceIndex);
        }

        //int i = 0;
        void DisNetwork_OnPacketIn(Packet packet, PacketDevice device)
        {
            //i++;
            //Console.WriteLine("DisNet packet in: " + i);

            // DIS In            
            byte[] buffer;

            //lock (sync)
            {
                buffer = GetBuffer(packet);

                if (buffer == null)
                {
                    Console.WriteLine("WARNING: null packet");
                    return;
                }

                if (DisHelpers.IsDis(buffer))
                {
                    if (detectingDis)
                    {
                        int deviceIndex = base.GetDeviceIndex(device.Name);
                        UdpDatagram udp = packet.Ethernet.IpV4.Udp;

                        DisSource source = new DisSource(packet.IpV4.Source.ToString(),
                                                          udp.DestinationPort,
                                                          deviceIndex);
                        
                        if (detectingDis)
                            NotifyDisDetected(source);

                        //Stop();

                        detectingDis = false;
                    }
                    else
                    {
                        ThreadPool.QueueUserWorkItem(o => BuildAndSend(buffer)); 
                    }
                }

            }
        }

        // thread safe?
        private void BuildAndSend(byte[] buffer)
        {
            Pdu pdu = BuildPdu(buffer);                       
            NotifyDisIn(pdu);
        }

        private void NotifyDisDetected(DisSource source)
        {
            if (OnDisDetected != null)
            {
                if (user != null)
                {
                    user.BeginInvoke(OnDisDetected, new object[] { source });
                }
            }
        }

        private void NotifyDisIn(Pdu pdu)
        {
            if (OnDisPduIn != null)
            {
                if (user != null && !user.IsDisposed)
                {
                    user.BeginInvoke(OnDisPduIn, new object[] { pdu });
                }
            }
        }

        public Pdu BuildPdu(byte[] buffer)
        {
            byte pduType = (byte)DisHelpers.GetPduType(buffer);
            Pdu pdu = PduProcessor.UnmarshalRawPdu(pduType, buffer, Endian.Big);
            return pdu;
        }
    }
}
