using System;
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
    public class Server
    {
        private const Int32 _PORT = 1337;
        private const String _PLAYER_NAME = "bugale";

        public static void Main(string[] args)
        {
            TcpListener server = null;
            NetworkStream client_stream = null;
            StreamReader reader = null;
            StreamWriter writer = null;
            Game game = new Game();

            game.connect(Server._PLAYER_NAME);

            Console.WriteLine("Starting server...");
            server = new TcpListener(IPAddress.Any, Server._PORT);
            server.Start();

            while (true)
            {
                client_stream = server.AcceptTcpClient().GetStream();
                reader = new StreamReader(client_stream);
                writer = new StreamWriter(client_stream);
                Console.WriteLine("Got connection!");

                try
                {
                    while (true)
                    {
                        // Read command
                        switch (reader.ReadLine())
                        {
                            case "L":
                                // Move left
                                game.move(Server._PLAYER_NAME, false);
                                break;
                            case "R":
                                // Move right
                                game.move(Server._PLAYER_NAME, true);
                                break;
                            case "A":
                                // Attack
                                game.attack(Server._PLAYER_NAME);
                                break;
                            default:
                                // Do nothing
                                break;
                        }

                        // Write back current state
                        writer.WriteLine(game.serialize_creatures(Server._PLAYER_NAME));
                        writer.Flush();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }

                reader.Close();
                writer.Close();
            }
        }
    }
}
