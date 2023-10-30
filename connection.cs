using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamsapi
{
    public class connection
    {
        MySqlConnection conn;
        public connection() {
            conn = new MySqlConnection();
        }

        public bool DbConnection()
        {
            try {
                var v = "SERVER = 'server'; UID = 'root'; PWD = 'pwd'; PORT = 3306";
                conn.ConnectionString = v;
                conn.Open();
                return true;
            }catch {
                return false;
            }
        }

        public DataTable DataSelect(string sql) {
            var dT = new DataTable();
            using (var cmd = new MySqlCommand() { Connection = conn , CommandText = sql } ) {
                using (var da = new MySqlDataAdapter() { SelectCommand = cmd }) {
                    da.Fill(dT);
                }
            }
            return dT;
        }

        public async void DataExeCute(string sql) {
            var auto = 0;
            using (var cmd = new MySqlCommand() { Connection = conn, CommandText = sql }) {
               await cmd.ExecuteNonQueryAsync();
                auto = (int)cmd.LastInsertedId;
            }
        }

    }
}
