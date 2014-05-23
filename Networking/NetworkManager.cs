using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PcapDotNet.Core;
using System.Threading;
using PcapDotNet.Packets;
using System.Net.NetworkInformation;
using PcapDotNet.Packets.Transport;
using System.IO;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.IpV4;

namespace Diswerx.Networking
{
    /// <summary>
    /// A low level yet simple way for connectivity.
    /// </summary>
    public class NetworkManager
    {
        protected static object sync = new object();

        private IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

        private PacketCommunicator communicator;
        private LivePacketDevice selectedDevice;

        public LivePacketDevice SelectedDevice { get { return selectedDevice; } }

        private Thread workerThread;
        // events used to stop worker thread
        private ManualResetEvent eventStopThread;
        private ManualResetEvent eventThreadStopped;

        private delegate void DelegateThreadFinished();
        private DelegateThreadFinished delegateThreadFinished;

        public delegate void PacketIn(Packet packet, PacketDevice device);
        public event PacketIn OnPacketIn;

        private int sleep = 100; // this will be changed dynamically depending on process load

        private ushort listenPort;

        private string defaultFilterValue = "ip and udp";
        private string filterValue; // port 3000";

        protected Control user;

        public Control User { get { return user; } }

        /// <summary>
        ///  Create with this if you want to use this in a winform/control
        /// </summary>
        /// <param name="user">Your main form or control</param>
        public NetworkManager(Control user)
        {
            this.user = user;
            Setup();
        }

        /// <summary>
        /// Create with this if you are not using with a winform/control.
        /// </summary>
        public NetworkManager()
        {
            Setup();
        }

        private void Setup()
        {
            eventStopThread = new ManualResetEvent(false);
            eventThreadStopped = new ManualResetEvent(false);
            delegateThreadFinished = new DelegateThreadFinished(this.ThreadFinished);            
        }

        private void Start(int deviceIndex = 0, int sleep = 100)
        {
            this.sleep = sleep;

            Setup();

            allDevices = LivePacketDevice.AllLocalMachine;
            selectedDevice = allDevices[deviceIndex];            

            // reset events
            eventStopThread.Reset();
            eventThreadStopped.Reset();

            workerThread = new Thread(new ThreadStart(WorkerThreadFunction));
            workerThread.Start();
            
        }

        /// <summary>
        /// Start listening to a device
        /// </summary>
        /// <param name="deviceIndex">Zero based index of devices</param>
        /// <param name="port">Port to connect</param>
        /// <param name="sleep">Sleep time: change to lower for quicker</param>
        public virtual void Listen(int deviceIndex, ushort port, int sleep = 100)
        {
            listenPort = port;
            filterValue = defaultFilterValue + " port " + listenPort;
            Start(deviceIndex, sleep);
        }

        /// <summary>
        /// Start listening to a device
        /// </summary>
        /// <param name="deviceIndex">Zero based device index</param>
        /// <param name="sleep">Change to lower for quicker</param>
        public virtual void Listen(int deviceIndex = 0, int sleep = 100)
        {
            Start(deviceIndex, sleep);
        }

        /// <summary>
        /// Interupt current listen to the port, but don't terminate.
        /// </summary>
        public void Stop()
        {
            communicator.Break();
        }
        
        /// <summary>
        /// Kill all comms.
        /// </summary>
        public void Terminate()
        {
            if (workerThread != null && workerThread.IsAlive)
            {
                Stop();

                eventStopThread.Set();

                // wait when thread will stop or finish
                while (workerThread.IsAlive)
                {
                    if (WaitHandle.WaitAll(
                        (new ManualResetEvent[] { eventThreadStopped }),
                        100,
                        true))
                    {
                        break;
                    }

                    workerThread.Abort();
                    Application.DoEvents();                    
                }

            }
        }

        // Called from worker thread using delegate and Control.Invoke
        private void ThreadFinished()
        {
            // done
            workerThread.Abort();
        }

        // Main code goes here
        private void ProcessStates()
        {
            ListenAsync();
        }

        private void WorkerThreadFunction()
        {
            while (workerThread.IsAlive)
            {
                ProcessStates();
                Thread.Sleep(sleep);
            }
        }

        private void ListenAsync(int readTimeout = 1000) //ip and udp
        {
            using (communicator =
                    selectedDevice.Open(65536,
                                    PacketDeviceOpenAttributes.Promiscuous,
                                    readTimeout))
            {
                // Compile the filter
                using (BerkeleyPacketFilter filter = communicator.CreateFilter(filterValue))
                {
                    // Set the filter
                    communicator.SetFilter(filter);
                }
                                
                communicator.ReceivePackets(0, PacketHandler);

                Stop();
            }
        }

        // Callback function invoked by libpcap for every incoming packet
        private void PacketHandler(Packet packet)
        {
            NotifyPacketIn(packet);
        }

        private void NotifyPacketIn(Packet ip)
        {
            if (OnPacketIn != null)
            {
                if (user != null && !user.IsDisposed && !user.Disposing)
                {
                    user.BeginInvoke(OnPacketIn, new object[] { ip, selectedDevice });
                    //user.Invoke(OnPacketIn, new object[] { ip, selectedDevice });
                }
                else
                {
                    int deviceIndex = GetDeviceIndex(selectedDevice.Name);
                    OnPacketIn(ip, selectedDevice);
                }
            }
        }

        #region Sending

        /// <summary>
        /// Multicast (Not available yet).
        /// </summary>
        /// <param name="deviceIndex">Zero based device index</param>
        /// <param name="data">Data to send</param>
        /// <param name="receivers">Subnet</param>
        /// <param name="readTimeout">Time to read</param>
        public void Multicast(int deviceIndex, byte[] data, List<IpV4SocketAddress> receivers, int readTimeout = 1000)
        {
            throw new Exception("Not implemented yet.");
        }

        /// <summary>
        /// Unicast (Not available yet).
        /// </summary>
        /// <param name="deviceIndex">Zero based device index</param>
        /// <param name="data">Data to send</param>
        /// <param name="receiver">Single receiver</param>
        /// <param name="readTimeout">Time to read</param>
        public void Unicast(int deviceIndex, byte[] data, IpV4SocketAddress receiver, int readTimeout = 1000)
        {
        }

        /// <summary>
        /// Get the mac address of the router (needed to send over internet)
        /// </summary>
        /// <param name="deviceIndex"></param>
        /// <returns></returns>
        public MacAddress GetRouterMacAddress(int deviceIndex) //LivePacketDevice device)
        {
            Packet arp = NetworkingHelpers.CreateArpPacket(allDevices[deviceIndex]);

            PacketDevice device = allDevices[deviceIndex];

            using (communicator =
                    device.Open(65536,
                                    PacketDeviceOpenAttributes.Promiscuous,
                                    1000))
            {
                communicator.SendPacket(arp);
            }

            using (communicator =
                device.Open(65536,                                  // portion of the packet to capture
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    10))                                  // read timeout
            {
                // Compile the filter
                using (BerkeleyPacketFilter filter = communicator.CreateFilter("arp"))
                {
                    // Set the filter
                    communicator.SetFilter(filter);
                }

                // Retrieve the packets
                Packet packet;
                do
                {
                    PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                    switch (result)
                    {
                        case PacketCommunicatorReceiveResult.Timeout:
                            // Timeout elapsed
                            continue;
                        case PacketCommunicatorReceiveResult.Ok:

                            EthernetLayer ethernet = (EthernetLayer)packet.Ethernet.ExtractLayer();
                            //ArpLayer arpLayer = (ArpLayer)packet.Ethernet.Arp.ExtractLayer();
                            //IpV4Layer ipv4 = (IpV4Layer)packet.Ethernet.IpV4.ExtractLayer();
                            string sourceMac = ethernet.Destination.ToString();
                            return new MacAddress(sourceMac);

                            break;
                        default:
                            throw new InvalidOperationException("The result " + result + " should never be reached here");
                    }
                } while (true);
            }

            return new MacAddress();
        }

        /// <summary>
        /// Broadcast data over udp.
        /// </summary>
        /// <param name="deviceIndex">Zero based device index</param>
        /// <param name="data">Data to send</param>
        /// <param name="port">Port to broadcast on</param>
        /// <param name="readTimeout">Timeout</param>
        public void Broadcast(int deviceIndex, byte[] data, ushort port, int readTimeout = 1000)
        {
            Packet packet = NetworkingHelpers.CreateUdpBroadcastPacket(data, port, allDevices[deviceIndex]);
            BroadcastPacket(deviceIndex, packet, port, readTimeout);
        }

        /// <summary>
        /// Broadcast string over udp.
        /// </summary>
        /// <param name="deviceIndex">Zero based device index</param>
        /// <param name="data">String to send</param>
        /// <param name="port">Port to send on</param>
        /// <param name="readTimeout">Timeout</param>
        public void Broadcast(int deviceIndex, string data, ushort port, int readTimeout = 1000)
        {
            byte[] payload = new byte[data.Length + 1];

            for (int i = 0; i < data.Length; i++)
            {
                byte b = (byte)data[i];
                payload.SetValue(b, i);
            }

            byte end = 0;

            payload.SetValue(end, data.Length);

            Broadcast(deviceIndex, payload, port, readTimeout);
        }

        /// <summary>
        /// Broadcast a packet over udp.
        /// </summary>
        /// <param name="deviceIndex">Zero based device index</param>
        /// <param name="packet">Libpcap packet</param>
        /// <param name="port">Port to send on</param>
        /// <param name="readTimeout">Timeout</param>
        public void BroadcastPacket(int deviceIndex, Packet packet, ushort port, int readTimeout = 1000)
        {
            PacketDevice device = allDevices[deviceIndex];

            using (communicator =
                    device.Open(65536,
                                    PacketDeviceOpenAttributes.Promiscuous,
                                    readTimeout))
            {
                communicator.SendPacket(packet);
            }
        }

        #endregion

        public byte[] GetBuffer(Packet packet)
        {
            UdpDatagram udp = packet.Ethernet.IpV4.Udp;

            // warning: sometimes udp is null
            if (udp == null)
                return null;

            MemoryStream data = udp.Payload.ToMemoryStream();
            byte[] buffer = data.ToArray();

            return buffer;
        }

        #region Device name

        /// <summary>
        /// Get a list of all devices on this machine.
        /// </summary>
        /// <returns>Winpcap device list</returns>
        public IList<LivePacketDevice> GetAllDevices()
        {
            return allDevices;
        }

        /// <summary>
        /// Get a list of all devices on this machine given as strings.
        /// </summary>
        /// <returns>List of names that windows gives devices</returns>
        public List<string> GetWindowsDeviceNames()
        {
            List<string> names = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (LivePacketDevice device in allDevices)
            {
                string id = device.Name.Split(new char[] { '{', '}' })[1];

                NetworkInterface winDevice = interfaces.First(x => x.Id.Contains(id));

                names.Add(winDevice.Name);
            }

            return names;
        }

        /// <summary>
        /// Given a windows device name, return the zero based index
        /// </summary>
        /// <param name="deviceName">Windows name of device</param>
        /// <returns>Zero based index</returns>
        public int GetDeviceIndex(string deviceName)
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            int index = 0;

            foreach (LivePacketDevice device in allDevices)
            {
                if (device.Name == deviceName)
                    return index;

                index++;
            }

            return 0;
        }

        public int GetDefaultBroadcastDevice()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            int index = 0;

            foreach (LivePacketDevice device in allDevices)
            {
                foreach (DeviceAddress address in device.Addresses)
                {
                    if (!address.ToString().Contains("Internet6") && 
                        address.Address.ToString() != "Internet 0.0.0.0" && 
                        address.Address.ToString() != "Internet 192.168.1.1")
                        return index;
                }
                
                index++;
            }

            return index;
        }

        #endregion
    }
}
