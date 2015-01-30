using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BugaleMMORPG
{
    public class Game
    {
        private UInt32 _map_size = 30;
        private String _monster_name = "x_monster";
        private UInt32 _monster_count = 3;

        private Random _random = new Random();
        private List<Creature> _creatures = new List<Creature>();

        public Game()
        {
            for (int i = 0; i < this._monster_count; i++)
                this._spawn_creature(new Monster(this._monster_name));
        }

        private void _spawn_creature(Creature creature)
        {
            // TODO: Solve this: get a true random UInt32
            creature.position = (UInt32)(this._random.Next((int)this._map_size));
            this._creatures.Add(creature);
        }

        private Player _get_active_player(String player_name)
        {
            Player player = (
                from p in this._creatures
                where p is Player && ((Player)p).name == player_name
                select (Player)p
                ).First();

            return player;
        }

        public void connect(String player_name)
        {
            this._spawn_creature(new Player(player_name));
        }

        public void attack(String player_name)
        {
            Player player = this._get_active_player(player_name);

            // This is a hack to get the monsters in range 2: We add 2 to accept range (0, 4) instead
            // of (-2, 2) and because it is usigned, we can only validate it is not above 4.
            Monster[] monsters_in_range = (
                from m in this._creatures
                where m is Monster && (2 + m.position - player.position) <= 4
                select (Monster)m).ToArray();

            foreach (Monster m in monsters_in_range)
            {
                player.attack(m);
                if (m.dead)
                {
                    this._creatures.Remove(m);
                    this._spawn_creature(new Monster(this._monster_name));
                }
            }
        }

        public void move(String player_name, bool right)
        {
            Player player = this._get_active_player(player_name);
            UInt32 new_position = 0;

            if (right)
                new_position = Math.Min(player.position + 1, this._map_size - 1);
            else
                new_position = player.position - Math.Min(1, player.position);

            Monster[] monsters_in_new_position = (
                from m in this._creatures
                where m is Monster && m.position == new_position
                select (Monster)m).ToArray();

            foreach (Monster m in monsters_in_new_position)
            {
                m.attack(player);
                if (player.dead)
                {
                    player.hp = player.max_hp;
                    this._spawn_creature(player);
                }
            }

            if (monsters_in_new_position.Count() == 0) player.position = new_position;
        }

        public String serialize_creatures(String player_name)
        {
            StringBuilder string_builder = new StringBuilder();
            Player player = this._get_active_player(player_name);

            string_builder.Append(String.Format("{0},{1},{2},{3};", new object[] { "Connected", player.weapon_attack, player.hp, player.level }));
            foreach (Creature creature in this._creatures)
            {
                string_builder.Append(String.Format("{0},{1},{2},{3};", creature.get_type(), creature.name, creature.position, creature.hp));
            }

            return string_builder.ToString();
        }
    }
}
