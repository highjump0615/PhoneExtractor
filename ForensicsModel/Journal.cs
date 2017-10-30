using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model
{
    /// <summary>
    /// 日志类别
    /// </summary>
    public enum JournalOperate
    {
        添加用户, 添加案件, 删除案件, 上载证据信息, 导入案件, 导出案件报告, 添加物证, 编辑案件, 登录, 编辑用户, 添加工具, 删除物证, 合并物证, 案件授权,
        AddUser, AddCase, DeleteCase, UploadEvidence, ImportCase, ExportReport, AddEvidence, EditCase, Login, EditUser, AddTool, DeleteEvidence, CombineEvidence, CaseGrant
    }

    /// <summary>
    /// 日志
    /// </summary>
    public class Journal
    {
        private String _JOURNAL_GUID;

        public String JOURNAL_GUID
        {
            get { return _JOURNAL_GUID; }
            set { _JOURNAL_GUID = value; }
        }

        private String _USER_GUID;

        public String USER_GUID
        {
            get { return _USER_GUID; }
            set { _USER_GUID = value; }
        }
        private String _USER_NAME;

        public String USER_NAME
        {
            get { return _USER_NAME; }
            set { _USER_NAME = value; }
        }
        private DateTime _ADDTIME;

        public DateTime ADDTIME
        {
            get { return _ADDTIME; }
            set { _ADDTIME = value; }
        }
        private String _DESCRIPTION;

        public String DESCRIPTION
        {
            get { return _DESCRIPTION; }
            set { _DESCRIPTION = value; }
        }
        private JournalOperate _OPERATE;

        public JournalOperate OPERATE
        {
            get { return _OPERATE; }
            set { _OPERATE = value; }
        }
    }
}
