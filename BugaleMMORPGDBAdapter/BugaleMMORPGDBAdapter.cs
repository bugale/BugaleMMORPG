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
        private static DBAdapter _instance;

        private DBAdapter() { }

        public static DBAdapter instance
        {
            get
            {
                if (_instance == null) _instance = new DBAdapter();
                return _instance;
            }
        }

        private String _db_filename = @"C:\Users\Public\bugalemmorpgdb.xml";

        public IDictionary<String, String> load_attributes(String type, String id)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();

            foreach (var attribute in XElement.Load(this._db_filename).Element(type).Element(id).Attributes())
            {
                result[attribute.Name.LocalName] = attribute.Value;
            }

            return result;
        }

        public void save_attributes(String type, String id, IDictionary<String, String> attributes)
        {
            XDocument doc = XDocument.Load(this._db_filename);

            doc.Elements().First().Element(type).Element(id).RemoveAttributes();
            foreach (var attribute in attributes)
            {
                doc.Elements().First().Element(type).Element(id).SetAttributeValue(attribute.Key, attribute.Value);
            }

            doc.Save(this._db_filename);
        }
    }
}
