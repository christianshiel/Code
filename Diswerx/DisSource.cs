using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diswerx
{
    public class DisSource
    {
        private string hostname;

        public string Hostname
        {
            get { return hostname; }
            set { hostname = value; }
        }

        private ushort port;

        public ushort Port
        {
            get { return port; }
            set { port = value; }
        }

        private int deviceIndex;

        public int DeviceIndex
        {
            get { return deviceIndex; }
            set { deviceIndex = value; }
        }

        public DisSource()
        {
        }

        /// <summary>
        /// Represents an address from which DIS is available.
        /// </summary>
        /// <param name="hostname">Who is sending DIS</param>
        /// <param name="port">Port DIS is coming out on</param>
        /// <param name="deviceIndex">Zero based index on which device DIS is coming out of</param>
        public DisSource(string hostname, ushort port, int deviceIndex)
        {
            this.deviceIndex = deviceIndex;
            this.hostname = hostname;
            this.port = port;
        }
    }
}
