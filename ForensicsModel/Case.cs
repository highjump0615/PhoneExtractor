using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model
{
    /// <summary>
    /// 案件表
    /// </summary>
    public class Case
    {
        private String _CASE_GUID;
        public String CASE_GUID
        {
            get { return _CASE_GUID; }
            set { _CASE_GUID = value; }
        }

        private String _CASE_NUMBER;

        public String CASE_NUMBER
        {
            get { return _CASE_NUMBER; }
            set { _CASE_NUMBER = value; }
        }

        private String _CASE_NAME;

        public String CASE_NAME
        {
            get { return _CASE_NAME; }
            set { _CASE_NAME = value; }
        }
        private String _CASE_IMG;

        public String CASE_IMG
        {
            get { return _CASE_IMG; }
            set { _CASE_IMG = value; }
        }

        private String _USER_GUID;

        public String USER_GUID
        {
            get { return _USER_GUID; }
            set { _USER_GUID = value; }
        }

        private DateTime _ADDTIME;

        public DateTime ADDTIME
        {
            get { return _ADDTIME; }
            set { _ADDTIME = value; }
        }

        private String _CASE_DESCRIPTION;

        public String CASE_DESCRIPTION
        {
            get { return _CASE_DESCRIPTION; }
            set { _CASE_DESCRIPTION = value; }
        }
        private String _CASE_REMARK;
        public String CASE_REMARK
        {
            get { return _CASE_REMARK; }
            set { _CASE_REMARK = value; }
        }

        private String _CASE_PATH;

        public String CASE_PATH
        {
            get { return _CASE_PATH; }
            set { _CASE_PATH = value; }
        }

        private String _USER_NAME;
        /// <summary>
        /// 案件创建人
        /// </summary>
        public String USER_NAME
        {
            get { return _USER_NAME; }
            set { _USER_NAME = value; }
        }

        private int _EVIDENCECOUNT;
        /// <summary>
        /// 物证数量
        /// </summary>
        public int EVIDENCECOUNT
        {
            get { return _EVIDENCECOUNT; }
            set { _EVIDENCECOUNT = value; }
        }
    }
}
