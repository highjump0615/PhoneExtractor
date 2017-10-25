using Forensics.DAL;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.BLL
{
    public class CaseManager
    {
        CaseService caseService = new CaseService();
        DataService dataService = new DataService();
        JournalManager jm = new JournalManager();
        EvidenceService es = new EvidenceService();
        /// <summary>
        /// 创建案件(每个案件都会创建一个独立库)
        /// </summary>
        /// <param name="myCase"></param>
        /// <returns></returns>
        public bool AddCase(Case myCase)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "添加案件[" + myCase.CASE_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.添加案件,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            caseService.AddCase(myCase);
            return dataService.CreateDataBase(myCase.CASE_PATH, myCase);
        }
        public bool AddCaseEn(Case myCase)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "AddCase[" + myCase.CASE_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.AddCase,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            caseService.AddCase(myCase);
            return dataService.CreateDataBase(myCase.CASE_PATH, myCase);
        }

        public bool AddQuickCase(Case myCase)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "添加快速案件[" + myCase.CASE_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.添加案件,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            //caseService.AddCase(myCase);
            return dataService.CreateDataBase(myCase.CASE_PATH, myCase);
        }
        /// <summary>
        /// 导入案件
        /// </summary>
        /// <param name="myCase"></param>
        /// <returns></returns>
        public bool ImportCase(String casePath)
        {
            Case myCase = dataService.GetCase(casePath);
            if (myCase != null)
            {
                myCase.CASE_PATH = casePath;

                jm.AddJournal(new Journal
                {
                    ADDTIME = DateTime.Now,
                    DESCRIPTION = "导入案件[" + myCase.CASE_NAME + "]",
                    JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                    OPERATE = JournalOperate.导入案件,
                    USER_GUID = User.LoginUser.USER_GUID,
                    USER_NAME = User.LoginUser.USER_NAME
                });
                List<Evidence> evidenceList = dataService.GetAllEvidences(myCase.CASE_PATH);
                if (evidenceList == null)
                {
                    throw new Exception("数据库中没有物证信息，请检查确认。");
                }
                else
                {
                    foreach (Evidence evidence in evidenceList)
                    {
                        if (evidence.CASE_GUID == "") evidence.CASE_GUID = myCase.CASE_GUID;
                        es.DelEvidenceByIds(evidence.EVIDENCE_GUID);
                        es.AddEvidence(evidence);
                    }
                }
                myCase.USER_GUID = User.LoginUser.USER_GUID;
                myCase.USER_NAME = User.LoginUser.USER_NAME;
                return caseService.AddCase(myCase) > 0;
            }
            else
            {
                throw new Exception("数据库中没有案件信息");
            }

        }
        public bool ImportCaseEn(String casePath)
        {
            Case myCase = dataService.GetCase(casePath);
            if (myCase != null)
            {
                myCase.CASE_PATH = casePath;

                jm.AddJournal(new Journal
                {
                    ADDTIME = DateTime.Now,
                    DESCRIPTION = "ImportCase[" + myCase.CASE_NAME + "]",
                    JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                    OPERATE = JournalOperate.ImportCase,
                    USER_GUID = User.LoginUser.USER_GUID,
                    USER_NAME = User.LoginUser.USER_NAME
                });
                List<Evidence> evidenceList = dataService.GetAllEvidences(myCase.CASE_PATH);
                if (evidenceList == null)
                {
                    throw new Exception("Case DB without data please check.");
                }
                else
                {
                    foreach (Evidence evidence in evidenceList)
                    {
                        if (evidence.CASE_GUID == "") evidence.CASE_GUID = myCase.CASE_GUID;
                        es.DelEvidenceByIds(evidence.EVIDENCE_GUID);
                        es.AddEvidence(evidence);
                    }
                }
                myCase.USER_GUID = User.LoginUser.USER_GUID;
                myCase.USER_NAME = User.LoginUser.USER_NAME;
                return caseService.AddCase(myCase) > 0;
            }
            else
            {
                throw new Exception("Case DB is empty.");
            }

        }
        /// <summary>
        /// 获取所有案件信息
        /// </summary>
        /// <param name="where">all : 全部 week:本周 month:本月  year :本年 </param>
        /// <param name="orderColumn">排序的列名称</param>
        /// <param name="order">排序的方式</param>
        /// <returns></returns>
        public List<Case> GetCaseByWhere(String where, String orderColumn = "ADDTIME", String order = "asc", String case_name = "")
        {
            return caseService.GetCaseByWhere(where, orderColumn, order, case_name);
        }

        /// <summary>
        /// 根据GUID删除案件信息
        /// </summary>
        /// <param name="caseGuid"></param>
        /// <returns></returns>
        public bool DelCase(Case myCase)
        {

            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "删除案件[" + myCase.CASE_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.删除案件,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            return caseService.DelCase(myCase.CASE_GUID) > 0;
        }
        /// <summary>
        /// 根据GUID删除案件信息
        /// </summary>
        /// <param name="caseGuid"></param>
        /// <returns></returns>
        public bool DelCaseEn(Case myCase)
        {

            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "DeleteCase[" + myCase.CASE_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.DeleteCase,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            return caseService.DelCase(myCase.CASE_GUID) > 0;
        }
        /// <summary>
        /// 查询符合条件的案件数量
        /// </summary>
        /// <param name="where">week:本周 month:本月  year :本年</param>
        /// <returns></returns>
        public int GetListCountByWhere(String where)
        {
            return caseService.GetListCountByWhere(where);
        }
        /// <summary>
        /// 修改案件信息
        /// </summary>
        /// <param name="myCase"></param>
        /// <returns></returns>
        public void UpdateCase(Case myCase)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "修改案件[" + myCase.CASE_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.编辑案件,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            caseService.UpdateCase(myCase);
        }
        /// <summary>
        /// 修改案件信息
        /// </summary>
        /// <param name="myCase"></param>
        /// <returns></returns>
        public void UpdateCaseEn(Case myCase)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = "ModifyCase[" + myCase.CASE_NAME + "]",
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.EditCase,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            caseService.UpdateCase(myCase);
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
            return caseService.GetCaseByIds(list, orderColumn, order);
        }
        /// <summary>
        /// 根据ID 查询数量
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int GetCountByIds(List<String> list)
        {
            return caseService.GetCountByIds(list);
        }
        /// <summary>
        /// 根据案件名称获取案件信息
        /// </summary>
        /// <param name="caseName"></param>
        /// <returns></returns>
        public List<Case> GetCaseByName(String caseName)
        {
            return caseService.GetCaseByName(caseName);
        }
        /// <summary>
        /// 判断中央数据库中时候 有 caseId的案件
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        public bool HasCase(String caseId)
        {
            return caseService.HasCase(caseId);
        }
        /// <summary>
        /// 案件授权
        /// </summary>
        /// <param name="myCase"></param>
        public void CaseAuthorize(Case myCase)
        {
            jm.AddJournal(new Journal
            {
                ADDTIME = DateTime.Now,
                DESCRIPTION = User.LoginUser.NAME + "授权案件[" + myCase.CASE_NAME + "]给" + myCase.USER_NAME,
                JOURNAL_GUID = System.Guid.NewGuid().ToString(),
                OPERATE = JournalOperate.案件授权,
                USER_GUID = User.LoginUser.USER_GUID,
                USER_NAME = User.LoginUser.USER_NAME
            });
            caseService.AddCase(myCase);
        }

        /// <summary>
        /// 判断中央数据库中时候 有 casenumber的案件
        /// </summary>
        /// <param name="caseNumber"></param>
        /// <returns></returns>
        public bool HasCasenumber(String caseNumber)
        {
            return caseService.HasCaseNumber(caseNumber);
        }
    }
}
