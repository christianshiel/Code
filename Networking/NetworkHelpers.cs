using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Transport;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Core;
using PcapDotNet.Core.Extensions;
using PcapDotNet.Packets.Arp;
using System.Collections.ObjectModel;

namespace Diswerx.Networking
{
    /// <summary>
    /// Use this helper to assist with verbose tasks.
    /// </summary>
    public static class NetworkingHelpers
    {
        public static byte[] GetPayloadAsByteArray(Packet packet)
        {
            return packet.IpV4.Payload.ToArray();
        }

        public static string GetPayloadAsString(Packet packet)
        {
            return packet.IpV4.Payload.Decode(Encoding.ASCII);
        }

        // todo for non-ethernet
        public static Packet CreateUdpPacket(byte[] payload, string destination, ushort destPort)
        {
            return null;
        }

        public static MacAddress GetDeviceMacAddress(LivePacketDevice device)
        {
            return LivePacketDeviceExtensions.GetMacAddress(device);
        }

        public static Packet CreateArpPacket(LivePacketDevice device)
        {
            string sgatewayIP = "192.168.0.1";
            string sbroadcastIP = "192.168.0.255";
            string stargetMAC = "ff:ff:ff:ff:ff:ff";

            MacAddress tempMyMac = device.GetMacAddress();
            MacAddress tempTargetMAC = new MacAddress(stargetMAC);
            IpV4Address tempBroadcastIP = new IpV4Address(sbroadcastIP);
            IpV4Address tempMyIP = new IpV4Address(sgatewayIP);

            byte[] targetMAC = new byte[6]; //MAC and IP for ARPLayer
            targetMAC.Write(0, tempTargetMAC, Endianity.Big);
            byte[] broadcastIP = new byte[4];
            broadcastIP.Write(0, tempBroadcastIP, Endianity.Big);
            byte[] myMAC = new byte[6];
            myMAC.Write(0, tempMyMac, Endianity.Big);
            byte[] myIP = new byte[4];
            myIP.Write(0, tempMyIP, Endianity.Big);

            EthernetLayer ethernetLayer = new EthernetLayer
            {
                Source = tempMyMac,
                Destination = tempTargetMAC
            };

            //ARP Layer
            ArpLayer arpLayer = new ArpLayer
            {
                SenderHardwareAddress = new ReadOnlyCollection<byte>(myMAC), //specifying address parameters
                TargetHardwareAddress = new ReadOnlyCollection<byte>(targetMAC),
                SenderProtocolAddress = new ReadOnlyCollection<byte>(myIP),
                TargetProtocolAddress = new ReadOnlyCollection<byte>(broadcastIP),
                Operation = ArpOperation.Reply// ARP reply OP Code

            };

            PacketBuilder builder = new PacketBuilder(ethernetLayer, arpLayer);
            return builder.Build(DateTime.Now);
        }

        public static Packet CreateUdpUnicastPacket(byte[] payload, MacAddress router, ushort destPort, string destIp, LivePacketDevice device)
        {
            router = new MacAddress("cc:33:bb:01:40:b2");
            MacAddress macAddress = GetDeviceMacAddress(device);

            EthernetLayer ethernetLayer =
                new EthernetLayer
                {
                    //Destination = new MacAddress("192.168.1.254"),
                    Destination = router,
                    Source = macAddress,
                    EtherType = EthernetType.None, // Will be filled automatically.
                };

            // 192.168.1.254 gateway
            // 192.168.1.64 

            IpV4Layer ipV4Layer =
                new IpV4Layer
                {
                    Source = IpV4Address.AllHostsGroupAddress,
                    CurrentDestination = new IpV4Address(destIp),
                    Fragmentation = IpV4Fragmentation.None,
                    HeaderChecksum = null, // Will be filled automatically.
                    Identification = 123,
                    Options = IpV4Options.None,
                    Protocol = null, // Will be filled automatically.
                    Ttl = 100,
                    TypeOfService = 0,
                };

            UdpLayer udpLayer =
                new UdpLayer
                {
                    DestinationPort = destPort,
                    Checksum = null, // Will be filled automatically.
                    CalculateChecksumValue = true,
                };

            PayloadLayer payloadLayer =
                new PayloadLayer
                {
                    Data = new Datagram(payload)
                };

            return PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, payloadLayer);
        }

        public static Packet CreateUdpBroadcastPacket(byte[] payload, ushort destPort, LivePacketDevice device)
        {
            MacAddress macAddress = GetDeviceMacAddress(device);
            
            MacAddress broadcast = new MacAddress("FF:FF:FF:FF:FF:FF");

            EthernetLayer ethernetLayer =
                new EthernetLayer
                {
                    //Destination = new MacAddress("192.168.1.254"),
                    Destination = broadcast,
                    Source = macAddress,                    
                    EtherType = EthernetType.None, // Will be filled automatically.
                };

            // 192.168.1.254 gateway
            // 192.168.1.64 

            IpV4Layer ipV4Layer =
                new IpV4Layer
                {
                    //Source = new IpV4Address("192.168.1.64"), //IpV4Address.AllHostsGroupAddress,
                    Source = new IpV4Address("192.168.1.64"), //.AllHostsGroupAddress,
                    //CurrentDestination = new IpV4Address("255.255.255.255"),
                    CurrentDestination = new IpV4Address("192.168.1.254"),
                    Fragmentation = IpV4Fragmentation.None,
                    HeaderChecksum = null, // Will be filled automatically.
                    Identification = 1234,
                    Options = IpV4Options.None,
                    Protocol = null, // Will be filled automatically.
                    Ttl = 100,
                    TypeOfService = 17,                    
                };

            UdpLayer udpLayer =
                new UdpLayer
                {                    
                    DestinationPort = destPort,
                    SourcePort = 55480,
                    Checksum = null, // Will be filled automatically.
                    CalculateChecksumValue = true,                    
                };

            PayloadLayer payloadLayer =
                new PayloadLayer
                {                    
                    Data = new Datagram(payload)
                };

            //return PacketBuilder.Build(DateTime.Now, ipV4Layer, udpLayer, payloadLayer);
            return PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, payloadLayer);
        }

        /// <summary>
        /// This function build an UDP over IPv4 over Ethernet with payload packet.
        /// </summary>
        public static Packet CloneUdpPacket(Packet packet, ushort destPort)
        {
            EthernetLayer ethernetLayer =
                new EthernetLayer
                {

                    Source = packet.Ethernet.Source,
                    Destination = packet.Ethernet.Destination,
                    EtherType = EthernetType.None, // Will be filled automatically.
                };

            IpV4Layer ipV4Layer =
                new IpV4Layer
                {
                    Source = packet.IpV4.Source,
                    CurrentDestination = packet.IpV4.CurrentDestination,
                    Fragmentation = IpV4Fragmentation.None,
                    HeaderChecksum = null, // Will be filled automatically.
                    Identification = 123,
                    Options = IpV4Options.None,
                    Protocol = null, // Will be filled automatically.
                    Ttl = 100,
                    TypeOfService = 0,
                };

            UdpLayer udpLayer =
                new UdpLayer
                {
                    SourcePort = packet.IpV4.Udp.SourcePort,
                    DestinationPort = destPort,
                    Checksum = null, // Will be filled automatically.
                    CalculateChecksumValue = true,
                };

            PayloadLayer payloadLayer =
                new PayloadLayer
                {
                    Data = packet.IpV4.Udp.Payload
                };

            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, udpLayer, payloadLayer);

            return builder.Build(DateTime.Now);
        }

    }
}
