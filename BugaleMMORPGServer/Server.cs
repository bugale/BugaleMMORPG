using System.Net;
using System.Net.Sockets;

namespace BugaleMMORPGServer {
    public class Server {
        private readonly TcpListener _listener;

        public Server(IPAddress address, int port) {
            this._listener = new TcpListener(address, port);
        }

        public void Listen() {
            this._listener.Start();
            while (true) {
                TcpClient client = this._listener.AcceptTcpClient();
                
            }
        }
    }

    public class Client
}
