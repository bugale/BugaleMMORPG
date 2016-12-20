using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace BugaleMMORPGServer {
    public class Server {
        private readonly TcpListener _listener;
        private readonly List<ConnectedClient> _clients;
        private readonly bool _running;

        public Server(IPAddress address, int port) {
            this._listener = new TcpListener(address, port);
            this._clients = new List<ConnectedClient>();
            this._running = true;
        }

        public void Listen() {
            this._listener.Start();
            while (this._running) {
                var client = new ConnectedClient(this._listener.AcceptTcpClient(), this);
                client.StartCommunicating();
                this._clients.Add(client);
            }
        }
    }

    public class ConnectedClient {
        private readonly TcpClient _client;
        private readonly Server _server;
        private Thread _thread;

        public ConnectedClient(TcpClient client, Server server) {
            this._client = client;
            this._server = server;
        }

        public void StartCommunicating() {
            this._thread = new Thread(this.CommunicationThread);
            this._thread.Start();
        }

        private void CommunicationThread() {
            var bytes = new byte[256];
            this._client.GetStream().Read(bytes, 0, bytes.Length);
        }
    }
}
