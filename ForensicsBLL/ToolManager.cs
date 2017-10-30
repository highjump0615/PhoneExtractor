using Forensics.DAL;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.BLL
{
    public class ToolManager
    {
        ToolService ts = new ToolService();
        JournalManager jm = new JournalManager();
        /// <summary>
        /// 获取所有的工具
        /// </summary>
        /// <returns></returns>
        public List<Tool> GetAllTools(int flag)
        {
            return ts.GetAllTools(flag);
        }

        /// <summary>
        /// 获取工具的数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return ts.GetCount();
        }

        /// <summary>
        /// 添加工具
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public int AddTool(Tool tool)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + User.LoginUser.USER_NAME + "]添加工具[" + tool.TOOL_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.添加工具,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            return ts.AddTool(tool);
        }
        public int AddToolEn(Tool tool)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + User.LoginUser.USER_NAME + "]AddTool[" + tool.TOOL_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.AddTool,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            return ts.AddTool(tool);
        }
        /// <summary>
        /// 更改工具启动程序的路径
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public int UpdateToolPath(Tool tool)
        {
            return ts.UpdateToolPath(tool);
        }
    }
}
