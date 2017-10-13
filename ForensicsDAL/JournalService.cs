using Forensics.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.DAL
{
    /// <summary>
    /// 日志操作
    /// </summary>
    public class JournalService
    {
        ILog log = LogManager.GetLogger(typeof(JournalService));
        /// <summary>
        /// 添加日志
        /// </summary> 
        /// <param name="journal">journal 添加的日志信息</param>
        /// <returns></returns>
        public int AddJournal(Journal journal)
        {
            if (journal == null)
                throw new Exception("不能添加空对象");
            SQLiteConnection con = null;
            try
            {
                FileInfo file = new FileInfo("journal.db");
                if (!file.Exists)
                {
                    createDataTable();
                }
                if ((DateTime.Now.Year - file.CreationTime.Year) > 1)
                {
                    createDataTable();
                }
                String sql = " insert into TBL_JOURNAL (JOURNAL_GUID,USER_GUID,USER_NAME,ADDTIME,DESCRIPTION,OPERATE) values(@JOURNAL_GUID,@USER_GUID,@USER_NAME,@ADDTIME,@DESCRIPTION,@OPERATE)";
                SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder
                {
                    DataSource = "journal.db"
                };
                con = new SQLiteConnection(conString.ToString());
                SQLiteCommand cmd = new SQLiteCommand(sql, con);
                cmd.Parameters.AddRange(new SQLiteParameter[]
                {
                     new SQLiteParameter("@JOURNAL_GUID",journal.JOURNAL_GUID),
                     new SQLiteParameter("@USER_GUID",journal.USER_GUID),
                     new SQLiteParameter("@USER_NAME",journal.USER_NAME),
                     new SQLiteParameter("@ADDTIME",journal.ADDTIME),
                     new SQLiteParameter("@DESCRIPTION",journal.DESCRIPTION),
                     new SQLiteParameter("@OPERATE", Enum.ToObject(typeof(JournalOperate),journal.OPERATE)),

                });
                con.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("添加日志信息失败");

            }
            finally
            {
                con.Close();
            }

        }
        /// <summary>
        /// 分页获取日志信息
        /// </summary> 
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <returns></returns>
        public List<Journal> GetAllJournal(int pageIndex, int pageSize)
        {
            List<Journal> journalList = null;
            String sql = String.Format(" select  JOURNAL_GUID,USER_GUID,ADDTIME,DESCRIPTION,OPERATE,USER_NAME from TBL_JOURNAL  order by ADDTIME DESC limit {0} offset {0}*{1}", pageSize, (pageIndex - 1));
            SQLiteConnection con = null;
            SQLiteDataReader reader = null;
            try
            {
                SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder
                {
                    DataSource = "journal.db"
                };
                con = new SQLiteConnection(conString.ToString());
                SQLiteCommand cmd = new SQLiteCommand(sql, con);
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    journalList = new List<Journal>();
                    while (reader.Read())
                    {
                        journalList.Add(new Journal
                        {
                            JOURNAL_GUID = reader["JOURNAL_GUID"].ToString(),
                            ADDTIME = Convert.ToDateTime(reader["ADDTIME"]),
                            DESCRIPTION = reader["DESCRIPTION"].ToString(),
                            OPERATE = (JournalOperate)Enum.Parse(typeof(JournalOperate), reader["OPERATE"].ToString()),
                            USER_GUID = reader["USER_GUID"].ToString(),
                            USER_NAME = reader["USER_NAME"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("读取数据失败", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
            }
            return journalList;
        }
        /// <summary>
        /// 批量删除
        /// </summary> 
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DelJournal(String ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return 0;
            }
            SQLiteConnection con = null;
            String sql = " delete from TBL_JOURNAL where JOURNAL_GUID in (" + ids + ")";
            try
            {
                SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder
                {
                    DataSource = "journal.db"
                };
                con = new SQLiteConnection(conString.ToString());
                SQLiteCommand cmd = new SQLiteCommand(sql, con);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("删除日志信息失败", ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
        /// <summary>
        /// 获取指定时间段的日志信息
        /// </summary>
        /// <param name="beginTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">当前第N 页</param>
        /// <param name="pageSize">每页N 条</param>
        /// <returns></returns>
        public List<Journal> GetJournalByTimeSpan(DateTime beginTime, DateTime endTime, int pageIndex, int pageSize)
        {
            if (beginTime.CompareTo(endTime) > 0)
                throw new Exception("起始时间不能大于结束时间");
            List<Journal> journalList = null;
            String sql = String.Format(" select JOURNAL_GUID,USER_GUID,ADDTIME,DESCRIPTION,OPERATE,USER_NAME from TBL_JOURNAL where ADDTIME >=@startTime and ADDTIME<=@endTime  order by ADDTIME DESC limit {0} offset {0}*{1}", pageSize, (pageIndex - 1));
            SQLiteConnection con = null;
            SQLiteDataReader reader = null;
            try
            {
                SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder
                {
                    DataSource = "journal.db"
                };
                con = new SQLiteConnection(conString.ToString());
                SQLiteCommand cmd = new SQLiteCommand(sql, con);
                cmd.Parameters.AddRange(new SQLiteParameter[]
                {
                     new SQLiteParameter("@startTime",beginTime.ToString("yyyy-MM-dd HH:mm:ss")),
                      new SQLiteParameter("@endTime",endTime.ToString("yyyy-MM-dd HH:mm:ss"))
                });
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    journalList = new List<Journal>();
                    while (reader.Read())
                    {
                        journalList.Add(new Journal
                        {
                            JOURNAL_GUID = reader["JOURNAL_GUID"].ToString(),
                            ADDTIME = Convert.ToDateTime(reader["ADDTIME"]),
                            DESCRIPTION = reader["DESCRIPTION"].ToString(),
                            OPERATE = (JournalOperate)Enum.Parse(typeof(JournalOperate), reader["OPERATE"].ToString()),
                            USER_GUID = reader["USER_GUID"].ToString(),
                            USER_NAME = reader["USER_NAME"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("读取数据失败", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
            }
            return journalList;
        }
        //创建表
        public void createDataTable()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" CREATE TABLE TBL_JOURNAL  (");
            sql.Append("  JOURNAL_GUID varchar(64) PRIMARY KEY ,");
            sql.Append(" USER_GUID varchar(64) ,");
            sql.Append(" USER_NAME varchar(32) ,");
            sql.Append(" ADDTIME date ,");
            sql.Append(" DESCRIPTION varchar(512) ,");
            sql.Append(" OPERATE  varchar(121) ");
            sql.Append(" );");
            SQLiteConnection con = null;
            SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder
            {
                DataSource = "journal.db"
            };
            try
            {
                SQLiteConnection.CreateFile("journal.db");
                con = new SQLiteConnection(conString.ToString());
                SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("创建日志表失败", ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
        /// <summary>
        /// 获取日志的总数量
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public int GetPageCount(int pageSize, String start = null, String end = null)
        {
            int result = 0;
            String sql = "select count(*) from TBL_JOURNAL ";
            if (start != null && end != null)
            {
                sql += String.Format(" where ADDTIME >='{0}' and ADDTIME<='{1}'", start, end);
            }
            SQLiteConnection con = null;
            try
            {
                SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder
                {
                    DataSource = "journal.db"
                };
                con = new SQLiteConnection(conString.ToString());
                SQLiteCommand cmd = new SQLiteCommand(sql, con);
                con.Open();
                result = Convert.ToInt32(cmd.ExecuteScalar());
                return Convert.ToInt32(Math.Ceiling(result / (pageSize + 0.0)));
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("读取日志数量出错", ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }
    }
}
