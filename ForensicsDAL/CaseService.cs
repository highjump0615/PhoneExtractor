using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.DAL
{
    public class CaseService
    {
        String _conString = ConfigurationManager.ConnectionStrings["sqliteCon"].ToString();


        /// <summary>
        /// 获取所有案件信息
        /// </summary>
        /// <param name="where">all : 全部 week:本周 month:本月  year :本年   </param>
        /// <param name="orderColumn">排序的列名称</param>
        /// <param name="order">排序的方式</param>
        /// <returns></returns>
        public List<Case> GetCaseByWhere(String where, String orderColumn = "ADDTIME", String order = "asc", String case_name = "")
        {
            DBHelper._conString = _conString;// "Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + "center.db;version=3 ;";
            String sql = @"select  CASE_GUID,CASE_NUMBER,CASE_NAME,CASE_IMG,USER_GUID,ADDTIME,CASE_DESCRIPTION,CASE_REMARK,CASE_PATH ,
		                     (select count(*) from  TBL_EVIDENCE where CASE_GUID =TBL_CASE.CASE_GUID)EVIDENCECOUNT,
		                       (select USER_NAME  from TBL_USER where USER_GUID=TBL_CASE.USER_GUID)USER_NAME 
                              from TBL_CASE
                            where 1==1  
                            and USER_GUID=@USER_GUID 
                            ";
            if (case_name.Length > 0)
            {
                sql += " and CASE_NAME  like '%" + case_name + "%'  ";
            }
            switch (where.Trim().ToLower())
            {
                case "week":
                    //sql += " and TBL_CASE.ADDTIME>=datetime('now','start of day','-7 day','weekday 1') and TBL_CASE.ADDTIME<=datetime('now','start of day','+0 day','weekday 1') ";
                    sql += " and strftime('%Y',date('now'))-strftime('%Y',TBL_CASE.ADDTIME) ==0 ";
                    sql += " and   strftime('%m',date('now'))-strftime('%m',TBL_CASE.ADDTIME) ==0 ";
                    sql += "   and  strftime('%W',date('now'))-strftime('%W',TBL_CASE.ADDTIME) == 0 ";
                    break;
                case "month":
                    sql += " and strftime('%Y',date('now'))-strftime('%Y',TBL_CASE.ADDTIME) ==0 ";
                    sql += " and   strftime('%m',date('now'))-strftime('%m',TBL_CASE.ADDTIME) ==0 ";
                    break;
                case "year":
                    sql += " and strftime('%Y',date('now'))-strftime('%Y',TBL_CASE.ADDTIME) ==0 ";
                    break;
                case "today":
                    sql += " and strftime('%Y',date('now'))-strftime('%Y',TBL_CASE.ADDTIME) ==0 ";
                    sql += " and   strftime('%m',date('now'))-strftime('%m',TBL_CASE.ADDTIME) ==0 ";
                    sql += "   and  strftime('%d',date('now'))-strftime('%d',TBL_CASE.ADDTIME) == 0 ";
                    break;
                default:
                    break;
            }
            sql += " order by " + orderColumn + "  " + order;
            DataTable dt = DBHelper.GetDataTable(sql, new SQLiteParameter("@USER_GUID", User.LoginUser.USER_GUID));
            return getList(dt);
        }

        private List<Case> getList(DataTable dt)
        {
            List<Case> caseList = null;
            if (dt.Rows.Count > 0)
            {
                caseList = new List<Case>();
                foreach (DataRow row in dt.Rows)
                {
                    caseList.Add(new Case
                    {
                        CASE_GUID = row["CASE_GUID"].ToString(),
                        ADDTIME = Convert.ToDateTime(row["ADDTIME"].ToString()),
                        CASE_IMG = row["CASE_IMG"].ToString(),
                        CASE_NAME = row["CASE_NAME"].ToString(),
                        CASE_NUMBER = row["CASE_NUMBER"].ToString(),
                        CASE_PATH = row["CASE_PATH"].ToString(),
                        CASE_REMARK = row["CASE_REMARK"].ToString(),
                        USER_GUID = row["USER_GUID"].ToString(),
                        CASE_DESCRIPTION = row["CASE_DESCRIPTION"].ToString(),
                        USER_NAME = row["USER_NAME"].ToString(),
                        EVIDENCECOUNT = Convert.ToInt32(row["EVIDENCECOUNT"].ToString())
                    });
                }
            }

            return caseList;
        }

        /// <summary>
        /// 添加案件
        /// </summary>
        /// <param name="myCase"></param>
        /// <returns></returns>
        public int AddCase(Case myCase)
        {
            DBHelper._conString = _conString;
            String sql = "insert into  TBL_CASE (CASE_GUID,CASE_NUMBER,CASE_NAME,CASE_IMG,USER_GUID,ADDTIME,CASE_DESCRIPTION,CASE_REMARK,CASE_PATH) values (@CASE_GUID,@CASE_NUMBER,@CASE_NAME,@CASE_IMG,@USER_GUID,@ADDTIME,@CASE_DESCRIPTION,@CASE_REMARK,@CASE_PATH)";
            return DBHelper.ExecuteCommand(sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@CASE_GUID",myCase.CASE_GUID),
                    new SQLiteParameter("@CASE_NUMBER",myCase.CASE_NUMBER),
                    new SQLiteParameter("@CASE_NAME",myCase.CASE_NAME),
                    new SQLiteParameter("@CASE_IMG",myCase.CASE_IMG),
                    new SQLiteParameter("@USER_GUID",myCase.USER_GUID),
                    new SQLiteParameter("@ADDTIME",myCase.ADDTIME),
                    new SQLiteParameter("@CASE_DESCRIPTION",myCase.CASE_DESCRIPTION),
                    new SQLiteParameter("@CASE_REMARK",myCase.CASE_REMARK),
                    new SQLiteParameter("@CASE_PATH",myCase.CASE_PATH),
                });
        }
        /// <summary>
        /// 查询符合条件的案件数量
        /// </summary>
        /// <param name="where">week:本周 month:本月  year :本年</param>
        /// <returns></returns>
        public int GetListCountByWhere(String where)
        {
            DBHelper._conString = _conString;
            String sql = "select count(*) from TBL_CASE where   USER_GUID=@USER_GUID ";
            switch (where.Trim().ToLower())
            {
                case "week":
                    //sql += " and TBL_CASE.ADDTIME>=datetime('now','start of day','-7 day','weekday 1') and TBL_CASE.ADDTIME<=datetime('now','start of day','+0 day','weekday 1') ";
                    sql += " and strftime('%Y',date('now'))-strftime('%Y',TBL_CASE.ADDTIME) ==0 ";
                    sql += " and   strftime('%m',date('now'))-strftime('%m',TBL_CASE.ADDTIME) ==0 ";
                    sql += "   and  strftime('%W',date('now'))-strftime('%W',TBL_CASE.ADDTIME) ==0";
                    break;
                case "month":
                    sql += " and strftime('%Y',date('now'))-strftime('%Y',TBL_CASE.ADDTIME) ==0 ";
                    sql += " and   strftime('%m',date('now'))-strftime('%m',TBL_CASE.ADDTIME) ==0 ";
                    break;
                case "year":
                    sql += " and strftime('%Y',date('now'))-strftime('%Y',TBL_CASE.ADDTIME) ==0 ";
                    break;
                case "today":
                    sql += " and strftime('%Y',date('now'))-strftime('%Y',TBL_CASE.ADDTIME) ==0 ";
                    sql += " and   strftime('%m',date('now'))-strftime('%m',TBL_CASE.ADDTIME) ==0 ";
                    sql += "   and  strftime('%d',date('now'))-strftime('%d',TBL_CASE.ADDTIME) ==0";
                    break;
                default:
                    break;
            }
            return Convert.ToInt32(DBHelper.ExecuteScalar(sql, new SQLiteParameter("@USER_GUID", User.LoginUser.USER_GUID)));
        }

        /// <summary>
        /// 根据GUID删除案件信息
        /// </summary>
        /// <param name="caseGuid"></param>
        /// <returns></returns>
        public int DelCase(String caseGuid)
        {
            DBHelper._conString = _conString;
            int result = 0;
            SQLiteCommand cmd = null;
            try
            {
                cmd = DBHelper.GetCommand();
                String sql = "delete from TBL_EVIDENCE  where CASE_GUID = @CASE_GUID  ";
                DBHelper.ExecuteCommand(cmd, sql, new SQLiteParameter("@CASE_GUID", caseGuid));
                sql = " delete from TBL_CASE where CASE_GUID = @CASE_GUID ";
                result = DBHelper.ExecuteCommand(cmd, sql, new SQLiteParameter("@CASE_GUID", caseGuid));
                DBHelper.Commit(cmd);
                return result;
            }
            catch (Exception ex)
            {
                DBHelper.Rollback(cmd);
                throw ex;
            }

        }
        /// <summary>
        /// 修改案件信息
        /// </summary>
        /// <param name="myCase"></param>
        /// <returns></returns>
        public int UpdateCase(Case myCase)
        {
            DBHelper._conString = _conString;
            String sql = "update TBL_CASE set CASE_NUMBER=@CASE_NUMBER, CASE_NAME=@CASE_NAME,CASE_IMG =@CASE_IMG,CASE_DESCRIPTION=@CASE_DESCRIPTION,CASE_REMARK=@CASE_REMARK,CASE_PATH=@CASE_PATH where CASE_GUID=@CASE_GUID";
            return DBHelper.ExecuteCommand(sql, new SQLiteParameter[]
            {
                    new SQLiteParameter("@CASE_NUMBER",myCase.CASE_NUMBER),
                    new SQLiteParameter("@CASE_NAME",myCase.CASE_NAME),
                    new SQLiteParameter("@CASE_IMG",myCase.CASE_IMG),
                    new SQLiteParameter("@CASE_DESCRIPTION",myCase.CASE_DESCRIPTION),
                    new SQLiteParameter("@CASE_REMARK",myCase.CASE_REMARK),
                    new SQLiteParameter("@CASE_PATH",myCase.CASE_PATH),
                    new SQLiteParameter("@CASE_GUID",myCase.CASE_GUID),
            });
        }

        /// <summary>
        /// 根据ID 获取案件信息
        /// </summary>
        /// <param name="list"></param>
        /// <param name="orderColumn">排序的列</param>
        /// <param name="order">排序的方式 (asc or desc)</param>
        /// <returns></returns>
        public List<Case> GetCaseByIds(List<String> list, String orderColumn = "CASE_NUMBER", String order = "asc")
        {
            if (list.Count == 0)
            {
                return null;
            }
            DBHelper._conString = _conString;
            String sql = @"select  CASE_GUID,CASE_NUMBER,CASE_NAME,CASE_IMG,USER_GUID,ADDTIME,CASE_DESCRIPTION,CASE_REMARK,CASE_PATH ,
		                     (select count(*) from  TBL_EVIDENCE where CASE_GUID =TBL_CASE.CASE_GUID)EVIDENCECOUNT,
		                       (select USER_NAME  from TBL_USER where USER_GUID=TBL_CASE.USER_GUID)USER_NAME
                              from TBL_CASE 
                              where    USER_GUID=@USER_GUID
                            ";
            String where = "";
            foreach (String item in list)
            {
                where += "'" + item + "'" + ",";
            }
            if (where.Length > 0)
            {
                sql += " and CASE_GUID in (" + where.TrimEnd(',') + ")";
            }
            sql += " order by " + orderColumn + "  " + order;
            DataTable dt = DBHelper.GetDataTable(sql, new SQLiteParameter("@USER_GUID", User.LoginUser.USER_GUID));
            return getList(dt);
        }
        /// <summary>
        /// 根据ID 查询数量
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int GetCountByIds(List<String> list)
        {
            if (list.Count == 0)
                return 0;
            DBHelper._conString = _conString;
            String sql = "select count(*) from TBL_CASE where 1==1 and USER_GUID=@USER_GUID ";
            String where = "";
            foreach (String item in list)
            {
                where += "'" + item + "'" + ",";
            }
            if (where.Length > 0)
            {
                sql += " and CASE_GUID in (" + where.TrimEnd(',') + ")";
            }
            return Convert.ToInt32(DBHelper.ExecuteScalar(sql, new SQLiteParameter("@USER_GUID", User.LoginUser.USER_GUID)));
        }
        /// <summary>
        /// 根据案件名称获取案件信息
        /// </summary>
        /// <param name="caseName"></param>
        /// <returns></returns>
        public List<Case> GetCaseByName(String caseName)
        {
            DBHelper._conString = _conString;
            String sql = @"  select  CASE_GUID,CASE_NUMBER,CASE_NAME,CASE_IMG,USER_GUID,ADDTIME,CASE_DESCRIPTION,CASE_REMARK,CASE_PATH ,
		                     (select count(*) from  TBL_EVIDENCE where CASE_GUID =TBL_CASE.CASE_GUID)EVIDENCECOUNT,
		                       (select USER_NAME  from TBL_USER where USER_GUID=TBL_CASE.USER_GUID)USER_NAME
                              from TBL_CASE 
                              where  USER_GUID=@USER_GUID  and  CASE_NAME  like @CASE_NAME ";
            DataTable dt = DBHelper.GetDataTable(sql, new SQLiteParameter[] { new SQLiteParameter("@USER_GUID", User.LoginUser.USER_GUID), new SQLiteParameter("@CASE_NAME", "%" + caseName + "%") });
            return getList(dt);
        }
        /// <summary>
        /// 判断中央数据库中时候 有 caseId的案件
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        public bool HasCase(String caseId)
        {
            DBHelper._conString = _conString;
            String sql = "  select count(*) from TBL_CASE where CASE_GUID=@CASE_GUID ";
            return Convert.ToInt32(DBHelper.ExecuteScalar(sql, new SQLiteParameter("@CASE_GUID", caseId))) > 0;
        }

        /// <summary>
        /// 判断中央数据库中时候 有 caseNumber的案件
        /// </summary>
        /// <param name="caseNumber"></param>
        /// <returns></returns>
        public bool HasCaseNumber(String caseNumber)
        {
            DBHelper._conString = _conString;
            String sql = "  select count(*) from TBL_CASE where CASE_NUMBER=@CASE_GUID ";
            return Convert.ToInt32(DBHelper.ExecuteScalar(sql, new SQLiteParameter("@CASE_GUID", caseNumber))) > 0;
        }
    }
}
