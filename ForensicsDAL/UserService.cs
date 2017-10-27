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
    public class UserService
    {
        private String _conString = ConfigurationManager.ConnectionStrings["sqliteCon"].ToString();


        /// <summary>
        /// 根据用户名称获取用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public User GetUserByName(String name)
        {
            DBHelper._conString = _conString;
            String sql = "select USER_GUID,USER_NAME,USER_PASSWORD,USER_ROLE,USER_SKIN,USER_LANGUAGE,IF_USER_IMPORT,IF_USER_UPLOAD,NAME,ID   from TBL_USER where USER_NAME=@USER_NAME ";
            DataTable dt = DBHelper.GetDataTable(sql, new SQLiteParameter("@USER_NAME", name));
            List<User> userList = getList(dt);
            return userList == null ? null : userList[0];
        }

        public List<User> GetAllUsers()
        {
            DBHelper._conString = _conString;
            String sql = "select USER_GUID,USER_NAME,USER_PASSWORD,USER_ROLE,USER_SKIN,USER_LANGUAGE,IF_USER_IMPORT,IF_USER_UPLOAD,NAME,ID  from TBL_USER ;";
            DataTable dt = DBHelper.GetDataTable(sql, null);
            return getList(dt);
        }
        public List<User> GetAllUsers(string where)
        {
            DBHelper._conString = _conString;
            String sql = "select USER_GUID,USER_NAME,USER_PASSWORD,USER_ROLE,USER_SKIN,USER_LANGUAGE,IF_USER_IMPORT,IF_USER_UPLOAD,NAME,ID  from TBL_USER where 1=1  " + where + ";";
            DataTable dt = DBHelper.GetDataTable(sql, null);
            return getList(dt);
        }

        private List<User> getList(DataTable dt)
        {
            List<User> userList = null;
            if (dt.Rows.Count > 0)
            {
                userList = new List<User>();
                foreach (DataRow row in dt.Rows)
                {
                    userList.Add(new User
                    {
                        USER_GUID = row["USER_GUID"].ToString(),
                        USER_NAME = row["USER_NAME"].ToString(),
                        USER_PASSWORD = row["USER_PASSWORD"].ToString(),
                        USER_ROLE = row["USER_ROLE"].ToString(),
                        USER_LANGUAGE = row["USER_LANGUAGE"].ToString(),
                        USER_SKIN = row["USER_SKIN"].ToString(),
                        IF_USER_IMPORT = row["IF_USER_IMPORT"].ToString(),
                        IF_USER_UPLOAD = row["IF_USER_UPLOAD"].ToString(),
                        NAME = row["NAME"].ToString(),
                        ID = row["ID"].ToString()
                    });
                }
            }
            return userList;
        }

        public int AddUser(User user)
        {
            DBHelper._conString = _conString;
            String sql = @"insert into  TBL_USER (USER_GUID,USER_NAME,USER_PASSWORD,USER_ROLE,USER_SKIN,USER_LANGUAGE,IF_USER_IMPORT,IF_USER_UPLOAD,NAME,ID)
                        values(@USER_GUID,@USER_NAME,@USER_PASSWORD,@USER_ROLE,@USER_SKIN,@USER_LANGUAGE,@IF_USER_IMPORT,@IF_USER_UPLOAD,@NAME,@ID)";
            return DBHelper.ExecuteCommand(sql, new SQLiteParameter[] {
                 new SQLiteParameter("@USER_GUID",user.USER_GUID),
                 new SQLiteParameter("@USER_NAME",user.USER_NAME),
                 new SQLiteParameter("@USER_PASSWORD",user.USER_PASSWORD),
                 new SQLiteParameter("@USER_ROLE",user.USER_ROLE),
                 new SQLiteParameter("@USER_SKIN",user.USER_SKIN),
                 new SQLiteParameter("@USER_LANGUAGE",user.USER_LANGUAGE),
                 new SQLiteParameter("@IF_USER_IMPORT",user.IF_USER_IMPORT.Replace("是","Y").Replace("否","N")),
                 new SQLiteParameter("@IF_USER_UPLOAD",user.IF_USER_UPLOAD.Replace("是","Y").Replace("否","N")),
                 new SQLiteParameter("@NAME",user.NAME),
                 new SQLiteParameter("@ID",user.ID),
            });
        }

        public int DelUser(String user_guid)
        {
            DBHelper._conString = _conString;
            String sql = "delete from TBL_USER where USER_GUID = @USER_GUID;";
            return DBHelper.ExecuteCommand(sql, new SQLiteParameter("@USER_GUID", user_guid));
        }

        public int UpdateUser(User user)
        {
            DBHelper._conString = _conString;
            String sql = "update  TBL_USER set USER_NAME=@USER_NAME ,  USER_PASSWORD=@USER_PASSWORD ,USER_ROLE=@USER_ROLE ,USER_SKIN=@USER_SKIN ,USER_LANGUAGE=@USER_LANGUAGE ,IF_USER_IMPORT=@IF_USER_IMPORT ,IF_USER_UPLOAD=@IF_USER_UPLOAD,NAME=@NAME ,ID=@ID  where   USER_GUID=@USER_GUID ;";
            return DBHelper.ExecuteCommand(sql, new SQLiteParameter[] {
                 new SQLiteParameter("@USER_NAME",user.USER_NAME),
                 new SQLiteParameter("@USER_PASSWORD",user.USER_PASSWORD),
                 new SQLiteParameter("@USER_ROLE",user.USER_ROLE),
                 new SQLiteParameter("@USER_SKIN",user.USER_SKIN),
                 new SQLiteParameter("@USER_LANGUAGE",user.USER_LANGUAGE),
                 new SQLiteParameter("@IF_USER_IMPORT",user.IF_USER_IMPORT.Replace("是","Y").Replace("否","N").Replace("Yes","Y").Replace("No","N")),
                 new SQLiteParameter("@IF_USER_UPLOAD",user.IF_USER_UPLOAD.Replace("是","Y").Replace("否","N").Replace("Yes","Y").Replace("No","N")),
                 new SQLiteParameter("@USER_GUID",user.USER_GUID),
                 new SQLiteParameter("@NAME",user.NAME),
                 new SQLiteParameter("@ID",user.ID),
            });
        }

        public List<User> GetUsersByWhere(String userRole = "", String userName = "")
        {
            DBHelper._conString = _conString;
            String sql = @"select USER_GUID,USER_NAME,USER_PASSWORD,USER_ROLE,USER_SKIN,USER_LANGUAGE,IF_USER_IMPORT,IF_USER_UPLOAD,NAME,ID  
                            from TBL_USER  where 1=1 ";
            if (userRole.Length > 0)
                sql += " and  USER_ROLE=@USER_ROLE ";
            if (userName.Length > 0)
                sql += " and USER_NAME like @USER_NAME ";
            DataTable dt = DBHelper.GetDataTable(sql,
                    new SQLiteParameter[]{
                        new SQLiteParameter("@USER_ROLE",userRole),
                         new SQLiteParameter("@USER_NAME","%"+userName+"%"),
                    });

            return getList(dt);

        }
        /// <summary>
        /// 根据用户名称判断是否有用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns></returns>
        public bool HasUser(String userName)
        {
            DBHelper._conString = _conString;
            String sql = "select count(*) from TBL_USER where USER_NAME=@USER_NAME";
            if (Convert.ToInt32(DBHelper.ExecuteScalar(sql, new SQLiteParameter("@USER_NAME", userName))) > 0)
                return true;
            else
                return false;
        }
    }
}
