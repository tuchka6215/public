using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CacheClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new TcpClient())
            {
                client.Connect("localhost", 9000);

                using (var stream = client.GetStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var bytes = Encoding.UTF8.GetBytes("SET STRING \"abc\"\n");
                        stream.Write(bytes, 0, bytes.Length);

                        bytes = Encoding.UTF8.GetBytes("SET STRING \"abcd\"\n");
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }
    }
}
