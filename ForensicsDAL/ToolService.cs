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
    public class ToolService
    {
        private String _conString = ConfigurationManager.ConnectionStrings["sqliteCon_dynamic"].ToString();  //2015 move tool table to dynamic


        /// <summary>
        /// 获取所有的工具
        /// </summary>
        /// <returns></returns>
        public List<Tool> GetAllTools(int flag)
        {
            DBHelper._conString = _conString;
            List<Tool> toolList = null;
            String sql = " select TOOL_GUID,TOOL_NAME ,TOOL_IMG,TOOL_DESCRIPTION,TOOL_METHOD,TOOL_NAME_EN,TOOL_DESCRIPTION_EN from TBL_TOOL WHERE TOOL_GUID like '" + flag.ToString() + "%' order by TOOL_GUID Asc";
            DataTable dt = DBHelper.GetDataTable(sql, null);
            if (dt.Rows.Count != 0)
            {
                toolList = new List<Tool>();
                foreach (DataRow row in dt.Rows)
                {
                    toolList.Add(new Tool
                    {
                        TOOL_GUID = row["TOOL_GUID"].ToString(),
                        TOOL_DESCRIPTION = row["TOOL_DESCRIPTION"].ToString(),
                        TOOL_IMG = row["TOOL_IMG"].ToString(),
                        TOOL_METHOD = row["TOOL_METHOD"].ToString(),
                        TOOL_NAME = row["TOOL_NAME"].ToString(),
                        TOOL_NAME_EN = row["TOOL_NAME_EN"].ToString(),
                        TOOL_DESCRIPTION_EN = row["TOOL_DESCRIPTION_EN"].ToString(),
                    });
                }
            }
            return toolList;
        }
        /// <summary>
        /// 获取工具的数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            DBHelper._conString = _conString;
            String sql = "select count(*) from TBL_TOOL ";
            return Convert.ToInt32(DBHelper.ExecuteScalar(sql, null));
        }
        /// <summary>
        /// 添加工具
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public int AddTool(Tool tool)
        {
            DBHelper._conString = _conString;
            String sql = @"insert into TBL_TOOL (TOOL_GUID,TOOL_NAME ,TOOL_IMG,TOOL_DESCRIPTION,TOOL_METHOD,TOOL_NAME_EN,TOOL_DESCRIPTION_EN )
                        values(@TOOL_GUID,@TOOL_NAME ,@TOOL_IMG,@TOOL_DESCRIPTION,@TOOL_METHOD,@TOOL_NAME_EN,@TOOL_DESCRIPTION_EN )
                        ";
            return DBHelper.ExecuteCommand(sql, new SQLiteParameter[]
            {
                 new SQLiteParameter("@TOOL_GUID",tool.TOOL_GUID),
                   new SQLiteParameter("@TOOL_NAME",tool.TOOL_NAME),
                     new SQLiteParameter("@TOOL_IMG",tool.TOOL_IMG),
                       new SQLiteParameter("@TOOL_DESCRIPTION",tool.TOOL_DESCRIPTION),
                       new SQLiteParameter("@TOOL_METHOD",tool.TOOL_METHOD),
                       new SQLiteParameter("@TOOL_NAME_EN",tool.TOOL_NAME_EN),
                       new SQLiteParameter("@TOOL_DESCRIPTION_EN",tool.TOOL_DESCRIPTION_EN),
            });
        }
        /// <summary>
        /// 更改工具启动程序的路径
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public int UpdateToolPath(Tool tool)
        {
            DBHelper._conString = _conString;
            String sql = @"  update   TBL_TOOL set TOOL_METHOD=@TOOL_METHOD where  TOOL_GUID =@TOOL_GUID ;";
            return DBHelper.ExecuteCommand(sql, new SQLiteParameter[]
            {
                   new SQLiteParameter("@TOOL_METHOD",tool.TOOL_METHOD),
                 new SQLiteParameter("@TOOL_GUID",tool.TOOL_GUID),
            });
        }
    }
}