using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Util
{
    public class DatabaseUtil
    {
        public static DataTable Query(string sql, string connectionString)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            conn.Open();
            OleDbDataAdapter mydb = new OleDbDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            mydb.Fill(dt);
            conn.Close();
            conn.Dispose();
            return dt;
        }
    }
}
