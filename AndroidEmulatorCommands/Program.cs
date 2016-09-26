using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidEmulatorCommands
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient connection = new TcpClient();
            connection.Connect("localhost", 5554);
            var str = connection.GetStream();
            var writer = new StreamWriter(str);
            writer.WriteLine("sms send 691619063 bbb");
            writer.Flush();
            Thread.Sleep(10);
            connection.Close();
        }
    }
}
