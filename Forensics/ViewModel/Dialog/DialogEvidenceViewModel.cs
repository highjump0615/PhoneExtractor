using Forensics.BLL;
using Forensics.Command;
using Forensics.Model;
using Forensics.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Forensics.ViewModel.Dialog
{
    public class DialogEvidenceViewModel : ViewModelBase
    {
        public enum GenderEnum
        {
            Male,
            Female
        }

        /// <summary>
        /// 获取性别文字
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public string getGenderString(GenderEnum gender)
        {
            if (gender == GenderEnum.Male)
            {
                return "男";
            }

            return "女";
        }

        public override Pages PageIndex => throw new NotImplementedException();

        String _clew = "操作提示";

        /// <summary>
        /// 保存命令
        /// </summary>
        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new DelegateCommand(DoSave)); }
        }

        /// <summary>
        /// 案件编号
        /// </summary>
        CaseManager caseManager = new CaseManager();
        EvidenceManager eviManager = new EvidenceManager();

        public ObservableCollection<Case> ListCase { get; set; }

        private string _caseSelected;
        public string CaseSelected
        {
            get { return _caseSelected; }
            set
            {
                _caseSelected = value;

                // 获取选中的案件
                var caseCurrent = this.ListCase.Where(x => x.CASE_NUMBER == _caseSelected).FirstOrDefault();
                if (caseCurrent == null)
                {
                    return;
                }

                //
                // 获取该案件的所有物证
                //
                List<Evidence> evidencelist = eviManager.GetAllEvidenceByCaseId(caseCurrent.CASE_GUID);
                
                // 重新加载物证列表
                for (var i = this.ListEvidence.Count() - 1; i > 0; i--)
                {
                    this.ListEvidence.RemoveAt(i);
                }
                if (evidencelist != null)
                {
                    foreach (Evidence evi in evidencelist)
                    {
                        this.ListEvidence.Add(evi);
                    }
                }
                this.EvidenceSelected = this.ListEvidence[0].EVIDENCE_NUMBER;

                this.CaseName = caseCurrent.CASE_NAME;
                PropertyChanging("CaseName");
            }
        }

        /// <summary>
        /// 物证编号
        /// </summary>
        private ObservableCollection<Evidence> _listEvidence = new ObservableCollection<Evidence>();
        public ObservableCollection<Evidence> ListEvidence
        {
            get
            {
                return _listEvidence;
            }
            set
            {
                _listEvidence = value;
                PropertyChanging("ListEvidence");
            }
        }

        private string _evidenceSelected;
        public string EvidenceSelected
        {
            get { return _evidenceSelected; }
            set
            {
                _evidenceSelected = value;

                // 获取选中的物证
                var evidenceCurrent = this.ListEvidence.Where(x => x.EVIDENCE_NUMBER == _evidenceSelected).FirstOrDefault();
                if (evidenceCurrent == null)
                {
                    return;
                }

                this.EvidenceName = evidenceCurrent.EVIDENCE_NAME;

                PropertyChanging("EvidenceSelected");
            }
        }

        /// <summary>
        /// 物证名称
        /// </summary>
        private string _evidenceName;
        public string EvidenceName
        {
            get
            {
                return _evidenceName;
            }
            set
            {
                _evidenceName = value;
                PropertyChanging("EvidenceName");
            }
        }
        /// <summary>
        /// 案件名称
        /// </summary>
        public string CaseName { get; set; }
        /// <summary>
        /// 手机号1
        /// </summary>
        public string Phone1 { get; set; }
        /// <summary>
        /// 手机号2
        /// </summary>
        public string Phone2 { get; set; }

        /// <summary>
        /// 涉案人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 涉案人性别
        /// </summary>
        public GenderEnum Gender { get; set; } = GenderEnum.Male;
        /// <summary>
        /// 涉案人民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 涉案人身份证
        /// </summary>
        public string IdNumber { get; set; }


        string gsid = "0";
        public string lgcase_file;
        private string mstrSavePath; 

        public DialogEvidenceViewModel(string savePath)
        {
            mstrSavePath = savePath;

            //
            // 获取已有的案件
            //
            try
            {
                this.ListCase = new ObservableCollection<Case>(caseManager.GetCaseByWhere("all"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, _clew);
            }

            //
            // 自动生成案件编号
            //
            gsid = CommonUtil.Rulename.GetCaseautoid().ToString();
            string caseno1 = CommonUtil.Rulename.GetCaseNoName();

            var strCasePath = ConfigurationManager.AppSettings["caseDefaultPath"].ToString(); ;

            int limax = 0;
            if (Directory.Exists(strCasePath))
            {
                DirectoryInfo cDIR = new DirectoryInfo(strCasePath);
                foreach (DirectoryInfo dir in cDIR.GetDirectories())
                {
                    if (dir.Name.IndexOf(caseno1) == 0 && dir.Name.IndexOf("_") > 0)
                    {
                        //found the old case folder double check the no
                        string lstmp = dir.Name.Substring(caseno1.Length, dir.Name.IndexOf("_") - caseno1.Length);
                        try
                        {
                            if (Convert.ToInt32(lstmp) > limax)
                                limax = Convert.ToInt32(lstmp);
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.ToString());
                        }
                    }
                }
            }
            if (limax > Convert.ToInt32(gsid))
            {
                gsid = (limax + 1).ToString();
            }

            //
            // 自动生成物证编号
            //
            string strNumberNew = CommonUtil.Rulename.GetEvidenceNo();

            PhoneDevice pd = CommonUtil.CurrentPD;
            if (pd.Phone_model != null)
            {
                this.EvidenceName = pd.Phone_model;
            }
            if (pd.Phone_number != null)
            {
                this.Phone1 = pd.Phone_number;
            }
            if (pd.Case_ai_file != null)
            {
                lgcase_file = pd.Case_ai_file;
                if (pd.Case_ai_file.IndexOf("Aimp_") > 0 && pd.Case_ai_file.IndexOf("Case.ai") > 0)
                {
                    string lstmp = pd.Case_ai_file.Substring(pd.Case_ai_file.IndexOf("Aimp_") + 5, pd.Case_ai_file.IndexOf("Case.ai") - pd.Case_ai_file.IndexOf("Aimp_") - 6);

                    if (strNumberNew.IndexOf("_") > 0)
                    {
                        strNumberNew = strNumberNew.Substring(0, strNumberNew.IndexOf("_")) + "_" + lstmp;
                    }
                    if (this.EvidenceName.IndexOf(":") > 0)
                    {
                        this.EvidenceName = this.EvidenceName.Substring(0, this.EvidenceName.IndexOf(":")) + ":" + lstmp;
                    }
                }
            }

            // 添加新的物证
            Evidence eviNew = new Evidence()
            {
                EVIDENCE_NUMBER = strNumberNew
            };
            this.ListEvidence.Insert(0, eviNew);
            this.EvidenceSelected = strNumberNew;

            // 添加新的案件
            this.CaseSelected = caseno1 + gsid;
            Case caseNew = new Case()
            {
                CASE_NUMBER = this.CaseSelected
            };
            this.ListCase.Insert(0, caseNew);            
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void DoSave()
        {
            if (!ValidateInput())
            {
                return;
            }

            // 获取选中的案件
            var caseCurrent = this.ListCase.Where(x => x.CASE_NUMBER == _caseSelected).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(caseCurrent.CASE_GUID))
            {
                caseCurrent.CASE_GUID = System.Guid.NewGuid().ToString();
                caseCurrent.CASE_NAME = this.CaseName;
                caseCurrent.ADDTIME = DateTime.Now;
                caseCurrent.USER_GUID = User.LoginUser.USER_GUID;
                caseCurrent.CASE_PATH = mstrSavePath + "\\" + caseCurrent.CASE_GUID + ".db";

                // 添加案件
                if (!caseManager.AddCase(caseCurrent))
                {
                    var strMsg = Application.Current.FindResource("msgAddCaseFail") as string;
                    MessageBox.Show(strMsg, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CommonUtil.Rulename.UpdateCaseautoid(gsid);
            }

            // 获取选中的物证
            var evidenceCurrent = this.ListEvidence.Where(x => x.EVIDENCE_NUMBER == this.EvidenceSelected).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(evidenceCurrent.EVIDENCE_GUID))
            {
                // 新的物证
                evidenceCurrent.ADDTIME = DateTime.Now;
                evidenceCurrent.EVIDENCE_GUID = System.Guid.NewGuid().ToString();
                evidenceCurrent.CASE_GUID = caseCurrent.CASE_GUID;
                evidenceCurrent.EVIDENCE_JYR = User.LoginUser.USER_NAME;
                evidenceCurrent.EVIDENCE_NUMBER = this.EvidenceSelected;
                evidenceCurrent.QUZHENG_DATE = DateTime.Now;
                evidenceCurrent.EVIDENCE_NAME = this.EvidenceName;

                evidenceCurrent.OWNER_NAME = this.Name;
                evidenceCurrent.OWNER_SEX = getGenderString(this.Gender);
                evidenceCurrent.OWNER_PHONENUMBER = this.Phone1;
                evidenceCurrent.OWNER_ID = this.IdNumber;

                evidenceCurrent.FILE_PATH = lgcase_file;

                // 添加
                eviManager.AddEvidence(caseCurrent, evidenceCurrent);
            }

            CommonUtil.currentEvidence = evidenceCurrent;

            ((Window)this.View).Close();
        }

        //非空验证
        private bool ValidateInput()
        {
            if (String.IsNullOrWhiteSpace(this.CaseName))
            {
                var strMsg = Application.Current.FindResource("msgEmptyCaseName") as string;
                MessageBox.Show(strMsg, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (String.IsNullOrWhiteSpace(this.EvidenceName))
            {
                var strMsg = Application.Current.FindResource("msgEmptyEvidName") as string;
                MessageBox.Show(strMsg, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
