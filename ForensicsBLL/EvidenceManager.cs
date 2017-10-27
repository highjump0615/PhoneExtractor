using Forensics.DAL;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.BLL
{
    public class EvidenceManager
    {
        public bool isEnglish = User.LoginUser.USER_LANGUAGE.Equals("英文");
        EvidenceService es = new EvidenceService();
        DataService ds = new DataService();
        JournalService js = new JournalService();
        /// <summary>
        /// 添加物证 
        /// </summary>
        /// <param name="evidence"></param>
        /// <returns></returns>
        public bool AddEvidence(Case myCase, Evidence evidence)
        {
            js.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "[" + myCase.CASE_NAME + "]中添加了[" + evidence.EVIDENCE_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.添加物证,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            //向案件库中添加物证信息
            ds.AddEvidence(myCase.CASE_PATH, evidence);
            return es.AddEvidence(evidence) > 0;
        }
        /// <summary>
        /// 添加物证 
        /// </summary>
        /// <param name="evidence"></param>
        /// <returns></returns>
        public bool UpdateEvidence(Case myCase, Evidence evidence)
        {
            if (isEnglish)
            {
                js.AddJournal(new Journal
                {

                    ADDTIME = DateTime.Now,
                    DESCRIPTION = "[" + myCase.CASE_NAME + "]Updated[" + evidence.EVIDENCE_NAME + "]",
                    JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                    OPERATE = JournalOperate.AddEvidence,
                    USER_GUID = User.LoginUser.USER_GUID,
                    USER_NAME = User.LoginUser.USER_NAME
                });
            }
            else
            {
                js.AddJournal(new Journal
                {

                    ADDTIME = DateTime.Now,
                    DESCRIPTION = "[" + myCase.CASE_NAME + "]更新了[" + evidence.EVIDENCE_NAME + "]",
                    JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                    OPERATE = JournalOperate.添加物证,
                    USER_GUID = User.LoginUser.USER_GUID,
                    USER_NAME = User.LoginUser.USER_NAME
                });
            }
            //向案件库中添加物证信息
            ds.UpdateEvidence(myCase.CASE_PATH, evidence);
            return true;
        }
        /// <summary>
        /// 获取某一案件下所有的物证 
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        public List<Evidence> GetAllEvidenceByCaseId(String caseId)
        {
            return es.GetAllEvidenceByCaseId(caseId);
        }
        /// <summary>
        /// 根据ID 获取物证
        /// </summary>
        /// <param name="evidenceId"></param>
        /// <returns></returns>
        public Evidence GetEvidenceById(String evidenceId)
        {
            return es.GetEvidenceById(evidenceId);
        }
        /// <summary>
        /// 删除物证信息
        /// </summary>
        /// <param name="path">案件路径</param>
        /// <param name="evidenceIds">要删除的物证ID</param>
        public void DelEvidenceByIds(Case myCase, List<Evidence> evidenceList)
        {
            String evidenceIds = "";
            String evidenceInfo = "";
            foreach (Evidence evidence in evidenceList)
            {
                evidenceIds += "'" + evidence.EVIDENCE_GUID + "',";
                evidenceInfo += evidence.EVIDENCE_NAME + "、";
            }
            if (isEnglish)
            {
                js.AddJournal(new Journal
                {
                    ADDTIME = DateTime.Now,
                    DESCRIPTION = "[" + myCase.CASE_NAME + "]Deleted[" + evidenceInfo.TrimEnd('、') + "]",
                    JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                    OPERATE = JournalOperate.DeleteCase,
                    USER_GUID = User.LoginUser.USER_GUID,
                    USER_NAME = User.LoginUser.USER_NAME
                });
            }
            else
            {
                js.AddJournal(new Journal
                {
                    ADDTIME = DateTime.Now,
                    DESCRIPTION = "[" + myCase.CASE_NAME + "]删除了[" + evidenceInfo.TrimEnd('、') + "]",
                    JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                    OPERATE = JournalOperate.删除物证,
                    USER_GUID = User.LoginUser.USER_GUID,
                    USER_NAME = User.LoginUser.USER_NAME
                });
            }

            //删除中央数据库中的物证信息
            //es.DelEvidenceByIds(evidenceIds.TrimEnd(',')); 
            ds.DelByEvidenceId(myCase.CASE_PATH, evidenceIds.TrimEnd(','));

            //删除案件库下的物证信息(含数据)
            ds.DelDataByEvidenceId(myCase.CASE_PATH, evidenceIds.TrimEnd(','));
        }
        /// <summary>
        /// 删除前次提取信息
        /// </summary>
        /// <param name="casepath">案件路径</param>
        /// <param name="evidenceGuid">要删除的物证ID</param>
        public void DelEvidenceDataById(String casepath, String evidenceId)
        {
            String evidenceGuid = "'" + evidenceId + "'"; ;
            ds.DelDataByEvidenceId(casepath, evidenceGuid);
        }

        /// <summary>
        /// 合并物证信息
        /// </summary>
        /// <param name="path">案件库路径</param>
        /// <param name="evidenceList">合并的物证列表</param>
        /// <param name="newEvidence">新物证信息</param>
        public void MergeEvidence(Case myCase, List<Evidence> evidenceList, Evidence newEvidence, bool ibdelet)
        {
            String evidenceInfo = "";
            foreach (Evidence evidence in evidenceList)
            {
                evidenceInfo += evidence.EVIDENCE_NAME + "、";
            }
            if (isEnglish)
            {
                js.AddJournal(new Journal
                {
                    ADDTIME = DateTime.Now,
                    DESCRIPTION = "[" + myCase.CASE_NAME + "]Combine[" + evidenceInfo.TrimEnd('、') + "]To[" + newEvidence.EVIDENCE_NAME + "]",
                    JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                    OPERATE = JournalOperate.CombineEvidence,
                    USER_GUID = User.LoginUser.USER_GUID,
                    USER_NAME = User.LoginUser.USER_NAME
                });
            }
            else
            {
                js.AddJournal(new Journal
                {
                    ADDTIME = DateTime.Now,
                    DESCRIPTION = "[" + myCase.CASE_NAME + "]合并[" + evidenceInfo.TrimEnd('、') + "]为[" + newEvidence.EVIDENCE_NAME + "]",
                    JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                    OPERATE = JournalOperate.合并物证,
                    USER_GUID = User.LoginUser.USER_GUID,
                    USER_NAME = User.LoginUser.USER_NAME
                });
            }
            //es.MergeEvidence(evidenceList, newEvidence, ibdelet);
            ds.MergeEvidenceData(myCase.CASE_PATH, evidenceList, newEvidence, ibdelet);
        }

        /// <summary>
        /// 读取案件库中的case_ai信息
        /// </summary>
        /// <param name="casePath">案件所在路径</param>
        /// <param name="whereStr">sql 条件语句</param>
        /// <returns></returns>
        public String GetAiEvidendeData(String casePath, String whereStr)
        {
            return ds.GetAiEvidendeData(casePath, whereStr);
        }
    }
}
