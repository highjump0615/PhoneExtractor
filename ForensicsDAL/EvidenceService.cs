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
    public class EvidenceService
    {
        private String _conString = ConfigurationManager.ConnectionStrings["sqliteCon"].ToString();

        /// <summary>
        /// 添加物证 
        /// </summary>
        /// <param name="evidence"></param>
        /// <returns></returns>
        public int AddEvidence(Evidence evidence)
        {
            DBHelper._conString = _conString;
            String sql = @"insert into   TBL_EVIDENCE (EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK,CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,
                                                    OWNER_NAME,OWNER_SEX,OWNER_ID,OWNER_PHONENUMBER,COLLECTIONUNIT_CODE,COLLECTIONUNIT_NAME,COLLECTIONUNIT_PHONENUMBER,COLLECTIONPEOPLE_NAME,COLLECTIONPEOPLE_ID)  
                        values(@EVIDENCE_GUID,@EVIDENCE_NUMBER,@EVIDENCE_SENDER,@EVIDENCE_SENDERUNIT,@ADDTIME,@EVIDENCE_REMARK,@CASE_GUID,@QUZHENG_DATE,@EVIDENCE_JYR,@EVIDENCE_NAME,
                               @OWNER_NAME,@OWNER_SEX,@OWNER_ID,@OWNER_PHONENUMBER,@COLLECTIONUNIT_CODE,@COLLECTIONUNIT_NAME,@COLLECTIONUNIT_PHONENUMBER,@COLLECTIONPEOPLE_NAME,@COLLECTIONPEOPLE_ID)";

            return DBHelper.ExecuteCommand(sql, new SQLiteParameter[]
           {
                   new SQLiteParameter ("@EVIDENCE_GUID",evidence.EVIDENCE_GUID),
                   new SQLiteParameter ("@EVIDENCE_NUMBER",evidence.EVIDENCE_NUMBER),
                   new SQLiteParameter ("@EVIDENCE_SENDER",evidence.EVIDENCE_SENDER),
                   new SQLiteParameter ("@EVIDENCE_SENDERUNIT",evidence.EVIDENCE_SENDERUNIT),
                   new SQLiteParameter ("@ADDTIME",evidence.ADDTIME),
                   new SQLiteParameter ("@EVIDENCE_REMARK",evidence.EVIDENCE_REMARK),
                   new SQLiteParameter ("@CASE_GUID",evidence.CASE_GUID),
                   new SQLiteParameter ("@QUZHENG_DATE",evidence.QUZHENG_DATE),
                   new SQLiteParameter ("@EVIDENCE_JYR",evidence.EVIDENCE_JYR),
                   new SQLiteParameter("@EVIDENCE_NAME",evidence.EVIDENCE_NAME),
                   new SQLiteParameter("@OWNER_NAME",evidence.OWNER_NAME),
                   new SQLiteParameter("@OWNER_SEX",evidence.OWNER_SEX),
                   new SQLiteParameter("@OWNER_ID",evidence.OWNER_ID),
                   new SQLiteParameter("@OWNER_PHONENUMBER",evidence.OWNER_PHONENUMBER),
                   new SQLiteParameter("@COLLECTIONUNIT_CODE",evidence.COLLECTIONUNIT_CODE),
                   new SQLiteParameter("@COLLECTIONUNIT_NAME",evidence.COLLECTIONUNIT_NAME),
                   new SQLiteParameter("@COLLECTIONUNIT_PHONENUMBER",evidence.COLLECTIONUNIT_PHONENUMBER),
                   new SQLiteParameter("@COLLECTIONPEOPLE_NAME",evidence.COLLECTIONPEOPLE_NAME),
                   new SQLiteParameter("@COLLECTIONPEOPLE_ID",evidence.COLLECTIONPEOPLE_ID),
           });
        }
        /// <summary>
        /// 获取某一案件下所有的物证 
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        public List<Evidence> GetAllEvidenceByCaseId(String caseId)
        {
            DBHelper._conString = _conString;
            String sql = @"select   EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK,CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,
                           OWNER_NAME,OWNER_SEX,OWNER_ID,OWNER_PHONENUMBER,COLLECTIONUNIT_CODE,COLLECTIONUNIT_NAME,COLLECTIONUNIT_PHONENUMBER,COLLECTIONPEOPLE_NAME,COLLECTIONPEOPLE_ID
                           from  TBL_EVIDENCE
                        where TBL_EVIDENCE.CASE_GUID = @CASE_GUID
                        order by rowid desc";
            DataTable dt = DBHelper.GetDataTable(sql, new SQLiteParameter("@CASE_GUID", caseId));
            return getList(dt);
        }

        /// <summary>
        /// 获取所有物证信息
        /// </summary>
        /// <returns></returns>
        public List<Evidence> GetAllEvidences()
        {
            DBHelper._conString = _conString;
            String sql = @"select   EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK,CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,
                           OWNER_NAME,OWNER_SEX,OWNER_ID,OWNER_PHONENUMBER,COLLECTIONUNIT_CODE,COLLECTIONUNIT_NAME,COLLECTIONUNIT_PHONENUMBER,COLLECTIONPEOPLE_NAME,COLLECTIONPEOPLE_ID
                           from  TBL_EVIDENCE
                        order by rowid desc";

            DataTable dt = DBHelper.GetDataTable(sql);
            return getList(dt);
        }

        private List<Evidence> getList(DataTable dt)
        {
            List<Evidence> list = null;
            if (dt.Rows.Count > 0)
            {
                list = new List<Evidence>();
                foreach (DataRow row in dt.Rows)
                {
                    DateTime datetime = DateTime.Now;
                    if (row["QUZHENG_DATE"].ToString() != "")
                        datetime = Convert.ToDateTime(row["QUZHENG_DATE"].ToString());
                    list.Add(new Evidence
                    {
                        CASE_GUID = row["CASE_GUID"].ToString(),
                        ADDTIME = Convert.ToDateTime(row["ADDTIME"]),
                        EVIDENCE_GUID = row["EVIDENCE_GUID"].ToString(),
                        EVIDENCE_JYR = row["EVIDENCE_JYR"].ToString(),
                        EVIDENCE_NUMBER = row["EVIDENCE_NUMBER"].ToString(),
                        EVIDENCE_REMARK = row["EVIDENCE_REMARK"].ToString(),
                        EVIDENCE_SENDER = row["EVIDENCE_SENDER"].ToString(),
                        EVIDENCE_SENDERUNIT = row["EVIDENCE_SENDERUNIT"].ToString(),
                        QUZHENG_DATE = datetime,
                        EVIDENCE_NAME = row["EVIDENCE_NAME"].ToString(),
                        OWNER_NAME = row["OWNER_NAME"].ToString(),
                        OWNER_SEX = row["OWNER_SEX"].ToString(),
                        OWNER_ID = row["OWNER_ID"].ToString(),
                        OWNER_PHONENUMBER = row["OWNER_PHONENUMBER"].ToString(),
                        COLLECTIONUNIT_CODE = row["COLLECTIONUNIT_CODE"].ToString(),
                        COLLECTIONUNIT_NAME = row["COLLECTIONUNIT_NAME"].ToString(),
                        COLLECTIONUNIT_PHONENUMBER = row["COLLECTIONUNIT_PHONENUMBER"].ToString(),
                        COLLECTIONPEOPLE_NAME = row["COLLECTIONPEOPLE_NAME"].ToString(),
                        COLLECTIONPEOPLE_ID = row["COLLECTIONPEOPLE_ID"].ToString(),
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// 根据ID 获取物证
        /// </summary>
        /// <param name="evidenceId"></param>
        /// <returns></returns>
        public Evidence GetEvidenceById(String evidenceId)
        {
            DBHelper._conString = _conString;
            String sql = @"select   EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK, CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,
                           OWNER_NAME,OWNER_SEX,OWNER_ID,OWNER_PHONENUMBER,COLLECTIONUNIT_CODE,COLLECTIONUNIT_NAME,COLLECTIONUNIT_PHONENUMBER,COLLECTIONPEOPLE_NAME,COLLECTIONPEOPLE_ID
                           from  TBL_EVIDENCE
                            where EVIDENCE_GUID = @EVIDENCE_GUID
                            ";
            DataTable dt = DBHelper.GetDataTable(sql, new SQLiteParameter("@EVIDENCE_GUID", evidenceId));
            List<Evidence> list = getList(dt);
            return list == null ? null : list[0];
        }
        /// <summary>
        /// 删除物证信息
        /// </summary>
        /// <param name="evidenceIds"></param> 
        public void DelEvidenceByIds(String evidenceIds)
        {
            DBHelper._conString = _conString;
            if (evidenceIds.IndexOf("'") > -1)
            { }
            else
            {
                evidenceIds = "'" + evidenceIds.TrimEnd(',') + "'";
            }
            String sql = " delete from TBL_EVIDENCE where EVIDENCE_GUID in (" + evidenceIds + ") ";
            DBHelper.ExecuteCommand(sql);
        }
        /// <summary>
        /// 合并物证信息
        /// </summary>
        /// <param name="evidenceList"></param>
        /// <param name="newEvidence"></param>
        public void MergeEvidence(List<Evidence> evidenceList, Evidence newEvidence, Boolean ibdelet)
        {
            DBHelper._conString = _conString;
            SQLiteCommand cmd = null;
            try
            {
                String delId = "";
                String sql = "";
                if (ibdelet)
                {
                    for (int i = 0; i < evidenceList.Count; i++)
                    {
                        delId += "'" + evidenceList[i].EVIDENCE_GUID + "',";
                    }
                    sql = "  delete from TBL_EVIDENCE where EVIDENCE_GUID in (" + delId.TrimEnd(',') + ")";
                    cmd = DBHelper.GetCommand();
                    DBHelper.ExecuteCommand(cmd, sql);
                }
                sql = @"insert into   TBL_EVIDENCE (EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK,CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,
                                                    OWNER_NAME,OWNER_SEX,OWNER_ID,OWNER_PHONENUMBER,COLLECTIONUNIT_CODE,COLLECTIONUNIT_NAME,COLLECTIONUNIT_PHONENUMBER,COLLECTIONPEOPLE_NAME,COLLECTIONPEOPLE_ID)  
                        values(@EVIDENCE_GUID,@EVIDENCE_NUMBER,@EVIDENCE_SENDER,@EVIDENCE_SENDERUNIT,@ADDTIME,@EVIDENCE_REMARK,@CASE_GUID,@QUZHENG_DATE,@EVIDENCE_JYR,@EVIDENCE_NAME,
                               @OWNER_NAME,@OWNER_SEX,@OWNER_ID,@OWNER_PHONENUMBER,@COLLECTIONUNIT_CODE,@COLLECTIONUNIT_NAME,@COLLECTIONUNIT_PHONENUMBER,@COLLECTIONPEOPLE_NAME,@COLLECTIONPEOPLE_ID)";
                List<SQLiteParameter> list = new List<SQLiteParameter>()
           {
                   new SQLiteParameter ("@EVIDENCE_GUID",newEvidence.EVIDENCE_GUID),
                   new SQLiteParameter ("@EVIDENCE_NUMBER",newEvidence.EVIDENCE_NUMBER),
                   new SQLiteParameter ("@EVIDENCE_SENDER",newEvidence.EVIDENCE_SENDER),
                   new SQLiteParameter ("@EVIDENCE_SENDERUNIT",newEvidence.EVIDENCE_SENDERUNIT),
                   new SQLiteParameter ("@ADDTIME",newEvidence.ADDTIME),
                   new SQLiteParameter ("@EVIDENCE_REMARK",newEvidence.EVIDENCE_REMARK),
                   new SQLiteParameter ("@CASE_GUID",newEvidence.CASE_GUID),
                   new SQLiteParameter ("@QUZHENG_DATE",newEvidence.QUZHENG_DATE),
                   new SQLiteParameter ("@EVIDENCE_JYR",newEvidence.EVIDENCE_JYR),
                   new SQLiteParameter("@EVIDENCE_NAME",newEvidence.EVIDENCE_NAME),
                   new SQLiteParameter("@OWNER_NAME",newEvidence.OWNER_NAME),
                   new SQLiteParameter("@OWNER_SEX",newEvidence.OWNER_SEX),
                   new SQLiteParameter("@OWNER_ID",newEvidence.OWNER_ID),
                   new SQLiteParameter("@OWNER_PHONENUMBER",newEvidence.OWNER_PHONENUMBER),
                   new SQLiteParameter("@COLLECTIONUNIT_CODE",newEvidence.COLLECTIONUNIT_CODE),
                   new SQLiteParameter("@COLLECTIONUNIT_NAME",newEvidence.COLLECTIONUNIT_NAME),
                   new SQLiteParameter("@COLLECTIONUNIT_PHONENUMBER",newEvidence.COLLECTIONUNIT_PHONENUMBER),
                   new SQLiteParameter("@COLLECTIONPEOPLE_NAME",newEvidence.COLLECTIONPEOPLE_NAME),
                   new SQLiteParameter("@COLLECTIONPEOPLE_ID",newEvidence.COLLECTIONPEOPLE_ID),

           };
                DBHelper.ExecuteCommand(cmd, sql, list.ToArray());
                DBHelper.Commit(cmd);
            }
            catch (Exception ex)
            {
                if (cmd != null)
                    DBHelper.Rollback(cmd);
                throw ex;
            }
        }
    }
}
