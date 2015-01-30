using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BugaleMMORPG
{
    public class ConsoleClient
    {
        public static void Main(string[] args)
        {
            Client client = new Client(print_game_state, new StreamReader(Console.OpenStandardInput()));
            client.Run();
        }

        public static void print_game_state(String state)
        {
            Char[] map = new Char[30];
            IEnumerable<String> monsters =
                from m in state.Split(';')
                where m.StartsWith("Monsters")
                orderby UInt32.Parse(m.Split(',')[2])
                select m;
            IEnumerable<String> players = 
                from p in state.Split(';')
                where p.StartsWith("Players")
                select p;
            String current_player = state.Split(';').First();

            // Initialize map
            for (int i = 0; i < map.Length; i++) map[i] = '_';
            Console.Clear();

            // Put monsters
            foreach (String monster in monsters)
            {
                Console.Write("{0} ", monster.Split(',')[3]); // Print monster HP
                map[Int32.Parse(monster.Split(',')[2])] = 'X';
            }

            // Put players
            foreach (String player in players)
                if (map[Int32.Parse(player.Split(',')[2])] == '_')
                    map[Int32.Parse(player.Split(',')[2])] = 'P';
                else
                    map[Int32.Parse(player.Split(',')[2])] = 'R';

            // Print map
            Console.WriteLine();
            Console.WriteLine(map);

            // Print player stats
            Console.WriteLine("My stats: Level-{3} HP-{2} WeaponAttack-{1}", current_player.Split(','));
        }
    }
}
