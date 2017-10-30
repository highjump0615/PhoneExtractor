using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteCore
{
    public static class SQLiteHelper
    {
        public static List<string> GetTablesList(string dbPath)
        {
            List<string> tables = new List<string>();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();

                    using (SQLiteCommand cmd = conn.CreateCommand() as SQLiteCommand)
                    {
                        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(reader["name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return tables;
        }

        public static bool TestDatabase(string dbPath)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;

                    conn.ConnectionString = connstr.ToString();
                    conn.Open();
                    conn.Close();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DataTable ExecuteQuery(string dbPath, string sql, bool arrangeTable = true)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();

                    using (DbDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn))
                    {
                        dataAdapter.Fill(dt);
                    }
                }

                if (arrangeTable) DataTableFilter(dt);
            }
            catch
            {
            }

            return dt;
        }

        public static Int32 ExecuteQueryCount(string dbPath, string sql, bool arrangeTable = true)
        {
            Int32 lit = new Int32();
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();

                    using (DbDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn))
                    {
                        dataAdapter.Fill(dt);
                    }
                }

                if (arrangeTable) DataTableFilter(dt);

                lit = Convert.ToInt32(dt.Rows[0][0].ToString());
                return lit;
            }
            catch
            {
                return 0;
            }

            //return 0;
        }

        public static DataTable ExecuteCleanQuery(string dbPath, string sql, bool arrangeTable = true)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();
                    using (DbDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn))
                    {
                        dataAdapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dt;
        }


        public static DataTable ExecutePassQuery(string dbPath, string sql, string passkey, bool arrangeTable = true)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;
                    connstr.Password = passkey;
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();

                    using (DbDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn))
                    {
                        dataAdapter.Fill(dt);
                    }
                }
            }
            catch
            {
            }

            return dt;
        }

        public static void ExecuteNonQuery(string dbPath, string sql)
        {
            try
            {

                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();

                    using (SQLiteCommand cmd = conn.CreateCommand() as SQLiteCommand)
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
            }
        }

        public static DataTable GetTable(string dbPath, string tableName, bool arrangeTable = true)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();

                    string cmdTxt = string.Format("SELECT * from {0};", tableName);

                    using (DbDataAdapter dataAdapter = new SQLiteDataAdapter(cmdTxt, conn))
                    {
                        dataAdapter.Fill(dt);
                    }
                }

                if (arrangeTable) DataTableFilter(dt);
            }
            catch
            {

            }

            return dt;
        }

        private static void DataTableFilter(DataTable dt)
        {
            List<string> cnl = new List<string>();
            foreach (DataColumn col in dt.Columns)
            {
                if (col.DataType == typeof(byte[]))
                {
                    try
                    {
                        if (dt.Rows.Count > 0)
                        {
                            byte[] buf = (byte[])dt.Rows[0][col];
                            using (MemoryStream sr = new MemoryStream(buf))
                            {
                                Image img = Image.FromStream(sr);
                            }
                        }
                    }
                    catch
                    {
                        cnl.Add(col.ColumnName);
                    }
                }
            }
            foreach (string colName in cnl)
            {
                string cName = string.Format("[{0}]", colName);
                dt.Columns.Add(new DataColumn(cName));

                foreach (DataRow dr in dt.Rows)
                {
                    StringBuilder sb = new StringBuilder();
                    byte[] buf = (byte[])dr[colName];

                    dr[cName] = Encoding.UTF8.GetString(buf);
                }

                dt.Columns.Remove(colName);
            }
        }

        public static void ResaveDb(string dbPath, IEnumerable<DataTable> tables)
        {
            //Create Database
            string datasource = dbPath;
            System.Data.SQLite.SQLiteConnection.CreateFile(datasource);

            using (SQLiteConnection conn = new SQLiteConnection())
            {
                SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                connstr.DataSource = dbPath;
                conn.ConnectionString = connstr.ToString();
                conn.Open();

                using (SQLiteCommand cmd = conn.CreateCommand() as SQLiteCommand)
                {
                    cmd.Connection = conn;

                    foreach (DataTable dt in tables)
                    {
                        // Create Table
                        if (dt.TableName.Contains("sqlite_sequence")) continue;

                        StringBuilder cols = new StringBuilder();
                        StringBuilder colParams = new StringBuilder();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            cols.Append(dc.ColumnName);
                            cols.Append(',');

                            colParams.Append('@');
                            colParams.Append(dc.ColumnName);
                            colParams.Append(',');
                        }
                        string colsName = cols.ToString().Substring(0, cols.Length - 1);
                        string colParamsStr = colParams.ToString().Substring(0, colParams.Length - 1);
                        cmd.CommandText = string.Format("CREATE TABLE {0} ({1});", dt.TableName, colsName);
                        cmd.ExecuteNonQuery();


                        // Insert Data
                        var transaction = conn.BeginTransaction();
                        foreach (DataRow dr in dt.Rows)
                        {
                            cmd.CommandText = string.Format(@"INSERT INTO {0}({1}) VALUES ({2});", dt.TableName, colsName, colParamsStr);
                            cmd.Parameters.Clear();
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                cmd.Parameters.Add(string.Format("@{0}", dt.Columns[i].ColumnName), DbType.String).Value = dr.ItemArray[i].ToString();
                            }
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }
            }
        }

        public static DataTable GetTablebySql(string dbPath, string tableName)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
                    connstr.DataSource = dbPath;
                    conn.ConnectionString = connstr.ToString();
                    conn.Open();

                    string cmdTxt = tableName;

                    using (DbDataAdapter dataAdapter = new SQLiteDataAdapter(cmdTxt, conn))
                    {
                        dataAdapter.Fill(dt);
                    }
                }

                DataTableFilter(dt);
            }
            catch
            {

            }

            return dt;
        }

        public static bool CreateNewDatabase(string dbPath)
        {
            SQLiteConnection.CreateFile(dbPath);
            return true;
        }
    }
}
