using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BugaleMMORPG
{
    public class Client
    {
        public delegate void ShowGameState(String state);

        private const String _SERVER_IP = "127.0.0.1";
        private const Int32 _SERVER_PORT = 1337;
        private ShowGameState _show_game_state = null;
        private StreamReader _commanding_stream = null;
        private ConcurrentQueue<String> _commanding_queue = null;

        public Client(ShowGameState show_game_state, StreamReader commanding_stream)
        {
            this._show_game_state = show_game_state;
            this._commanding_stream = commanding_stream;
        }

        public Client(ShowGameState show_game_state, ConcurrentQueue<String> commanding_queue)
        {
            this._show_game_state = show_game_state;
            this._commanding_queue = commanding_queue;
        }

        public void Run()
        {
            TcpClient client = new TcpClient();
            StreamReader reader = null;
            StreamWriter writer = null;

            Console.WriteLine("Connection to server...");
            client.Connect(new IPEndPoint(IPAddress.Parse(Client._SERVER_IP), Client._SERVER_PORT));
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            Console.WriteLine("Connected!");

            writer.WriteLine("X");
            writer.Flush();

            try
            {
                while (true)
                {
                    // Receive server response
                    this._show_game_state(reader.ReadLine());

                    if (this._commanding_stream == null)
                    {
                        String command;
                        if (this._commanding_queue.TryDequeue(out command))
                            writer.WriteLine(command);
                        else
                        {
                            writer.WriteLine("X");
                            Thread.Sleep(10);
                        }
                    }
                    else
                        writer.WriteLine(this._commanding_stream.ReadLine());
                    writer.Flush();
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

            Console.WriteLine("Client exiting...");
            reader.Close();
            writer.Close();
            client.Close();
        }
    }
}
