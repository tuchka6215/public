using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CacheServer
{
    class Program
    {
        private static Cache cache = new Cache();

        static void Main(string[] args)
        {
            var server = new TcpListener(IPAddress.Any, 9000);
            server.Start();

            for (; ;)
            {
                var client = server.AcceptTcpClient();
                var clientThread = new Thread(new ThreadStart(() =>
                {
                    using (var stream = client.GetStream())
                    {
                        var bytes = new byte[1024];
                        for (; client.Connected;)
                        {
                            var bytesRead = stream.Read(bytes, 0, bytes.Length);
                            bytes[bytesRead] = 0;
                            if (bytesRead > 0)
                            {
                                var text = Encoding.UTF8.GetString(bytes);
                                text = text.Substring(0, text.IndexOf('\0'));
                                Console.WriteLine(text);

                                //TODO:
                                //#1 parse command
                                //#2 call appropreate cache.Set... method
                            }
                        }
                    }
                }));
                clientThread.Start();
                

            }
        }
    }
}
