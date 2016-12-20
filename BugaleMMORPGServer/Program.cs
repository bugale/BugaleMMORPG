using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BugaleMMORPGServer {
    public class Program {
        private static readonly int _port = 1337;

        public static void Main(string[] args) {
            var server = new Server(IPAddress.Any, Program._port);
            server.Listen();
        }
    }
}
