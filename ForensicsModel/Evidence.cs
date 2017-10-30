using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model
{
    /// <summary>
    /// 物证
    /// </summary>
    public class Evidence
    {
        private String _EVIDENCE_GUID;

        public String EVIDENCE_GUID
        {
            get { return _EVIDENCE_GUID; }
            set { _EVIDENCE_GUID = value; }
        }
        private String _EVIDENCE_NUMBER;

        public String EVIDENCE_NUMBER
        {
            get { return _EVIDENCE_NUMBER; }
            set { _EVIDENCE_NUMBER = value; }
        }

        private String _EVIDENCE_SENDER;


        public String EVIDENCE_SENDER
        {
            get { return _EVIDENCE_SENDER; }
            set { _EVIDENCE_SENDER = value; }
        }
        private String _EVIDENCE_SENDERUNIT;

        public String EVIDENCE_SENDERUNIT
        {
            get { return _EVIDENCE_SENDERUNIT; }
            set { _EVIDENCE_SENDERUNIT = value; }
        }

        private DateTime _QUZHENG_DATE;

        public DateTime QUZHENG_DATE
        {
            get { return _QUZHENG_DATE; }
            set { _QUZHENG_DATE = value; }
        }
        private DateTime _ADDTIME;

        public DateTime ADDTIME
        {
            get { return _ADDTIME; }
            set { _ADDTIME = value; }
        }

        private String _EVIDENCE_REMARK;

        public String EVIDENCE_REMARK
        {
            get { return _EVIDENCE_REMARK; }
            set { _EVIDENCE_REMARK = value; }
        }

        private String _CASE_GUID;

        public String CASE_GUID
        {
            get { return _CASE_GUID; }
            set { _CASE_GUID = value; }
        }

        private String _EVIDENCE_JYR;

        public String EVIDENCE_JYR
        {
            get { return _EVIDENCE_JYR; }
            set { _EVIDENCE_JYR = value; }
        }

        private String _EVIDENCE_NAME;

        public String EVIDENCE_NAME
        {
            get { return _EVIDENCE_NAME; }
            set { _EVIDENCE_NAME = value; }
        }

        private String _FILE_PATH;
        /// <summary>
        /// 物证目录路径
        /// </summary>
        public String FILE_PATH
        {
            get { return _FILE_PATH; }
            set { _FILE_PATH = value; }
        }


        /// <summary>
        /// 被采集人姓名
        /// </summary>
        /// 
        private String _OWNER_NAME;
        public String OWNER_NAME
        {
            get { return _OWNER_NAME; }
            set { _OWNER_NAME = value; }
        }

        /// <summary>
        /// 被采集人性别
        /// </summary>
        /// 
        private String _OWNER_SEX;
        public String OWNER_SEX
        {
            get { return _OWNER_SEX; }
            set { _OWNER_SEX = value; }
        }

        /// <summary>
        /// 被采集人身份证号码
        /// </summary>
        /// 
        private String _OWNER_ID;
        public String OWNER_ID
        {
            get { return _OWNER_ID; }
            set { _OWNER_ID = value; }
        }

        /// <summary>
        /// 被采集人电话号码
        /// </summary>
        /// 
        private String _OWNER_PHONENUMBER;
        public String OWNER_PHONENUMBER
        {
            get { return _OWNER_PHONENUMBER; }
            set { _OWNER_PHONENUMBER = value; }
        }

        /// <summary>
        /// 采集单位组织机构代码
        /// </summary>
        /// 
        private String _COLLECTIONUNIT_CODE;
        public String COLLECTIONUNIT_CODE
        {
            get { return _COLLECTIONUNIT_CODE; }
            set { _COLLECTIONUNIT_CODE = value; }
        }

        /// <summary>
        /// 采集单位名称
        /// </summary>
        /// 
        private String _COLLECTIONUNIT_NAME;
        public String COLLECTIONUNIT_NAME
        {
            get { return _COLLECTIONUNIT_NAME; }
            set { _COLLECTIONUNIT_NAME = value; }
        }

        /// <summary>
        /// 采集单位联系电话
        /// </summary>
        /// 
        private String _COLLECTIONUNIT_PHONENUMBER;
        public String COLLECTIONUNIT_PHONENUMBER
        {
            get { return _COLLECTIONUNIT_PHONENUMBER; }
            set { _COLLECTIONUNIT_PHONENUMBER = value; }
        }

        /// <summary>
        /// 采集人姓名
        /// </summary>
        /// 
        private String _COLLECTIONPEOPLE_NAME;
        public String COLLECTIONPEOPLE_NAME
        {
            get { return _COLLECTIONPEOPLE_NAME; }
            set { _COLLECTIONPEOPLE_NAME = value; }
        }

        /// <summary>
        /// 采集人警号
        /// </summary>
        /// 
        private String _COLLECTIONPEOPLE_ID;
        public String COLLECTIONPEOPLE_ID
        {
            get { return _COLLECTIONPEOPLE_ID; }
            set { _COLLECTIONPEOPLE_ID = value; }
        }
    }
}
