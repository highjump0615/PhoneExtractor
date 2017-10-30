using Forensics.DAL;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.BLL
{
    public class UserManager
    {
        UserService us = new UserService();
        JournalManager jm = new JournalManager();
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="logmsg"></param>
        /// <returns></returns>

        public User Login(String name, String pwd, out String logmsg)
        {
            User user = us.GetUserByName(name);
            if (user != null)
            {
                if (user.USER_PASSWORD.Equals(pwd))
                {
                    logmsg = "登录成功";
                    return user;
                }
                else
                {
                    logmsg = "账号或密码不匹配";
                    return null;
                }
            }
            else
            {
                logmsg = "此用户不存在";
                return null;
            }


        }
        public List<User> GetAllUsers(String whereStr = "")
        {
            return us.GetAllUsers(whereStr);
        }

        public bool AddUser(User user)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + User.LoginUser.USER_NAME + "]在" + DateTime.Now + "创建用户[" + user.USER_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.添加用户,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            return us.AddUser(user) > 0;
        }
        public bool AddUserEn(User user)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + User.LoginUser.USER_NAME + "]at" + DateTime.Now + "CreateUser[" + user.USER_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.AddUser,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            return us.AddUser(user) > 0;
        }
        public bool DelUser(String user_guid)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + User.LoginUser.USER_NAME + "]在" + DateTime.Now + "删除用户",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.编辑用户,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            return us.DelUser(user_guid) > 0;
        }
        public bool DelUserEn(String user_guid)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + User.LoginUser.USER_NAME + "]at" + DateTime.Now + "Delete User",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.EditUser,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            return us.DelUser(user_guid) > 0;
        }
        public void UpdateUser(User user)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + User.LoginUser.USER_NAME + "]在" + DateTime.Now + "修改用户[" + user.USER_NAME + "]的信息",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.编辑用户,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            us.UpdateUser(user);
        }
        public void UpdateUserEn(User user)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + User.LoginUser.USER_NAME + "]at" + DateTime.Now + "EditUser[" + user.USER_NAME + "]info",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.EditUser,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            us.UpdateUser(user);
        }
        public List<User> GetUsersByWhere(String userRole = "", String userName = "")
        {
            return us.GetUsersByWhere(userRole: userRole, userName: userName);
        }
        /// <summary>
        /// 根据用户名称判断是否有用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns></returns>
        public bool HasUser(String userName)
        {
            return us.HasUser(userName);
        }
    }
}
