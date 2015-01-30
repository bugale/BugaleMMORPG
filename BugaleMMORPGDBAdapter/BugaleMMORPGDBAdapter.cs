using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BugaleMMORPG
{
    public class DBAdapter
    {
        private const String _DB_ADDRESS = "localhost";
        private const String _DB_PORT = "1337";
        private const String _DB_NAME = "bugalemmorpg";
        private const String _DB_USERNAME = "root";
        private const String _DB_PASSWORD = "root";
        private static DBAdapter _instance;

        private MySqlConnection _connection = new MySqlConnection(String.Format(
                "Address={0};Database={1};Persist Security Info=no;User Name='{2}';Password='{3}';",
                DBAdapter._DB_ADDRESS, DBAdapter._DB_NAME, DBAdapter._DB_USERNAME, DBAdapter._DB_PASSWORD
                ));

        private DBAdapter()
        {
            this._connection.Open();
        }

        public static DBAdapter instance
        {
            get
            {
                if (_instance == null) _instance = new DBAdapter();
                return _instance;
            }
        }

        public IDictionary<String, String> load_attributes(String type, String id)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            MySqlCommand command = new MySqlCommand(
                String.Format("select * from `{0}` where `id`='{1}'", type.ToLower(), id),
                this._connection);
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();

            for (Int32 i = 0; i < reader.FieldCount; i++)
            {
                result[reader.GetName(i)] = reader.GetString(i);
            }
            reader.Close();
            return result;
        }

        public void save_attributes(String type, String id, IDictionary<String, String> attributes)
        {
            MySqlCommand command = null;
            StringBuilder string_builder = new StringBuilder();
            string_builder.AppendFormat("UPDATE {0} SET `id`='{1}'", type.ToLower(), id);
            foreach (var attribute in attributes)
            {
                string_builder.AppendFormat(",`{0}`='{1}'", attribute.Key, attribute.Value);
            }
            string_builder.AppendFormat("WHERE `id`='{0}'", id);

            command = new MySqlCommand(string_builder.ToString(), this._connection);
            command.ExecuteNonQuery();
        }
    }
}
