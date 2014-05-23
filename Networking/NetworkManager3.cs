using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;

namespace Networking
{
    public class NetworkManager3
    {
        private int sleep = 500; 

        protected Control user;

        public Control User { get { return user; } }

        private Thread t;
        private volatile bool running = false;
        private int counter = 0;
        private NetworkStream stream;
        private TcpClient client;
        private TcpListener server = null;

        public NetworkManager3(Control user)
        {
            this.user = user;
        }

        public void Send(byte[] data)
        {
            if (stream != null)
            {
                try
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
                catch (Exception ex)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        private void ListenForClients()
        {
            try
            {
                // Enter the listening loop.
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                client = server.AcceptTcpClient();
                counter++;
                Console.WriteLine("#" + counter + " Connected!");

                // Get a stream object for reading and writing
                stream = client.GetStream();
            }
            catch (Exception ex)
            {

            }
        }

        void Go()
        {
            while (running)
            {
                ListenForClients();
                Thread.Sleep(sleep);
            }
        }
        
        public void Start()
        {            
            try
            {
                // Set the TcpListener on port 3000.
                Int32 port = 3000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();
                
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
                return;
            }

            running = true;

            t = new Thread(Go);
            t.Start();

        }

        public void Terminate()
        {
            running = false;

            Stop();

            if (t != null)
            {
                t.Join(1000);
            }
        }

        public void Dispose()
        {         
        }

        public void Stop()
        {
            if (client != null)
                client.Close();

            server.Stop();
        }
    }
}
