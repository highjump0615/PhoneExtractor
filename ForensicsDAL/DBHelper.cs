using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.DAL
{
    class DBHelper
    {
        static ILog log = LogManager.GetLogger(typeof(CaseService));
        public static String _conString = String.Empty;

        private static SQLiteConnection _con;

        public static SQLiteConnection Con
        {
            get
            {
                try
                {
                    if (_con == null)
                    {
                        _con = new SQLiteConnection(_conString);
                        _con.Open();
                    }
                    else if (_con.State == System.Data.ConnectionState.Broken)
                    {
                        _con.Close();
                        _con.Open();
                    }
                    else if (_con.State == System.Data.ConnectionState.Closed)
                    {
                        _con.Open();
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("打开数据库连接失败");
                }
                return DBHelper._con;

            }
            set
            {
                _con = value;
            }
        }

        public static DataTable GetDataTable(String sql, params SQLiteParameter[] sqliteParameter)
        {
            DataTable dt = new DataTable();

            SQLiteCommand cmd = new SQLiteCommand(sql, Con);
            if (sqliteParameter != null)
                cmd.Parameters.AddRange(sqliteParameter);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            try
            {
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("查询失败");
            }
            cmd.Dispose();
            Con = null;
            return dt;
        }
        public static int ExecuteCommand(String sql, params SQLiteParameter[] sqliteParameter)
        {
            int result = 0;
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, Con);
                if (sqliteParameter != null)
                    cmd.Parameters.AddRange(sqliteParameter);
                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("执行语句出错");
            }
            finally
            {
                Con.Close();
                Con = null;
            }

            return result;
        }

        public static Object ExecuteScalar(String sql, params SQLiteParameter[] sqliteParameter)
        {
            Object obj = null;
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, Con);
                if (sqliteParameter != null)
                    cmd.Parameters.AddRange(sqliteParameter);
                obj = cmd.ExecuteScalar();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("执行语句出错");
            }
            finally
            {
                Con.Close();
                Con = null;
            }
            return obj;
        }

        /// <summary>
        /// 获取开启事务的命令对象
        /// </summary>
        /// <returns></returns>
        public static SQLiteCommand GetCommand()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteTransaction tran = Con.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Connection = Con;
            return cmd;
        }
        /// <summary>
        /// 执行带事务的命令对象
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="sql"></param>
        /// <param name="sqliteParameter"></param>
        /// <returns></returns>
        public static int ExecuteCommand(SQLiteCommand cmd, String sql, params SQLiteParameter[] sqliteParameter)
        {
            cmd.CommandText = sql;
            if (sqliteParameter != null)
                cmd.Parameters.AddRange(sqliteParameter);
            try
            {
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("执行语句出错");
            }
        }
        /// <summary>
        /// 事务提交
        /// </summary>
        /// <param name="cmd"></param>
        public static void Commit(SQLiteCommand cmd)
        {
            cmd.Transaction.Commit();
            cmd.Connection.Close();
            cmd.Dispose();
            Con = null;
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        /// <param name="cmd"></param>
        public static void Rollback(SQLiteCommand cmd)
        {
            cmd.Transaction.Rollback();
            cmd.Connection.Close();
            cmd.Dispose();
            Con = null;
        }
    }
}
