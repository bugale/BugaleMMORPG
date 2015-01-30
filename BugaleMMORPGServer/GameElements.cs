using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugaleMMORPG
{
    public abstract class GameElement
    {
        private String _name = "";

        public GameElement(String name)
        {
            this._name = name;
            this._from_dictionary(DBAdapter.instance.load_attributes(this.get_type(), this._name));
        }

        protected void _flush()
        {
            DBAdapter.instance.save_attributes(this.get_type(), this._name, this._to_dictionary());
        }

        public String name { get { return this._name; } }

        public abstract String get_type();
        protected abstract void _from_dictionary(IDictionary<String, String> dictionary);
        protected abstract IDictionary<String, String> _to_dictionary();
    }

    public abstract class Creature : GameElement
    {
        private UInt32 _max_hp = 0;
        private UInt32 _weapon_attack = 0;
        private UInt32 _hp = 0;
        private UInt32 _position = 0;

        public Creature(String name) : base(name)
        {
            this._hp = this._max_hp;
        }

        protected override void _from_dictionary(IDictionary<String, String> dictionary)
        {
            this._max_hp = UInt32.Parse(dictionary["max_hp"]);
            this._weapon_attack = UInt32.Parse(dictionary["weapon_attack"]);
        }

        protected override IDictionary<String, String> _to_dictionary()
        {
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            dictionary["max_hp"] = this._max_hp.ToString();
            dictionary["weapon_attack"] = this._weapon_attack.ToString();
            return dictionary;
        }

        protected virtual void _die(Creature by_creature) { }
        protected virtual void _kill(Creature creature) { }

        public void attack(Creature creature)
        {
            // If the HP of the enemy is lower then owr attack, it will be set to zero (dead)
            creature.hp -= Math.Min(this.weapon_attack, creature.hp);

            if (creature.dead)
            {
                creature._die(this);
                this._kill(creature);
            }
        }

        public bool dead
        {
            get { return this._hp == 0; }
        }

        public UInt32 max_hp
        {
            get { return this._max_hp; }
            set { this._max_hp = value; base._flush(); }
        }

        public UInt32 weapon_attack
        {
            get { return this._weapon_attack; }
            set { this._weapon_attack = value; base._flush(); }
        }

        public UInt32 hp
        {
            get { return this._hp; }
            set { this._hp = value; }
        }

        public UInt32 position
        {
            get { return this._position; }
            set { this._position = value; }
        }
    }

    public class Monster : Creature
    {
        private const String _TYPE = "Monsters";

        public Monster(String name) : base(name) { }
        public override String get_type() { return Monster._TYPE; }
    }

    public class Player : Creature
    {
        private const String _TYPE = "Players";
        private UInt32 _level = 0;

        public Player(String name) : base(name) { }
        public override String get_type() { return Player._TYPE; }

        protected override void _from_dictionary(IDictionary<String, String> dictionary)
        {
            base._from_dictionary(dictionary);
            this._level = UInt32.Parse(dictionary["level"]);
        }

        protected override IDictionary<String, String> _to_dictionary()
        {
            IDictionary<String, String> dictionary = base._to_dictionary();
            dictionary["level"] = this._level.ToString();
            return dictionary;
        }

        protected override void _die(Creature creature)
        {
            // Decrement the level by 1, and weapon attack by 5, but keep both at least 1
            this.level -= Math.Min(1, (this.level - 1));
            this.weapon_attack -= Math.Min(5, (this.weapon_attack - 1));
        }
        protected override void _kill(Creature creature)
        {
            // Increment level by 1, and weapon attack by a random number between 1 and 10
            this.level++;
            this.weapon_attack += (UInt32)(new Random().Next(1, 10));
        }

        public UInt32 level
        {
            get { return this._level; }
            set { this._level = value; base._flush(); }
        }
    }
}
