using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BugaleMMORPGServer {
    [Serializable]
    public class Message {
        public static Message FromStream(Stream stream) {
            var formatter = new BinaryFormatter();
            return (Message)formatter.Deserialize(stream);
        }
    }

    [Serializable]
    public class MapStateMessage : Message {
        private readonly MapState _state;

        public MapStateMessage(MapState state) { this._state = state; }

        public MapState State => this._state;
    }
}
