using Forensics.BLL;
using Forensics.Command;
using Forensics.Model;
using Forensics.Model.DataManagement;
using Forensics.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    class DataCaseViewModel : ViewModelBase
    {
        private String _clew = "操作提示";
        private CaseManager caseManager = new CaseManager();
        private DataManager dataManager = new DataManager();

        public ObservableCollection<Case2> ListCase { get; set; } = new ObservableCollection<Case2>();

        public override Pages PageIndex
        {
            get { return Pages.DataCase; }
        }

        /// <summary>
        /// 详情命令
        /// </summary>
        private ICommand _goToDetailCommand;
        public ICommand GoToDetailCommand
        {
            get { return _goToDetailCommand ?? (_goToDetailCommand = new DelegateCommand(GoToDetailPage)); }
        }

        /// <summary>
        /// 案件导入命令
        /// </summary>
        private ICommand _importCommand;
        public ICommand ImportCommand
        {
            get { return _importCommand ?? (_importCommand = new DelegateCommand(ImportCase)); }
        }

        public DataCaseViewModel(ViewModelBase vmParent)
        {
            this.ViewModelParent = vmParent;

            // 加载案件数据
            InitialCaseInfo();
        }

        /// <summary>
        /// 加载案件
        /// </summary>
        /// <param name="caseList"></param>
        private void InitialCaseInfo()
        {
            this.ListCase.Clear();

            // 获取案件列表
            List<Case> caseList = caseManager.GetCaseByWhere("all");

            foreach (Case c in caseList)
            {
                Case2 c2 = CommonUtil.ToDerived<Case, Case2>(c);
                this.ListCase.Add(c2);
            }
        }

        /// <summary>
        /// 跳转到详情页
        /// </summary>
        private void GoToDetailPage(object param)
        {
            Case2 caseInfo = (Case2)param;
            MainDataViewModel parentVM = (MainDataViewModel)this.ViewModelParent;
            parentVM.GoToCaseDetailPage(caseInfo);
        }

        /// <summary>
        /// 案件导入
        /// </summary>
        private void ImportCase()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "文件格式|*.db";

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                // 点击了取消，直接退出
                return;
            }

            try
            {
                int lireturn = dataManager.GetAllData2(dialog.FileName);
                if (lireturn == 0)
                {
                    MessageBox.Show("库中没有数据", _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 正在导入
                Case myCase = dataManager.GetCase(dialog.FileName);
                if (myCase != null)
                {
                    if (caseManager.HasCase(myCase.CASE_GUID))
                    {
                        MessageBox.Show("数据库中已经存在此案件，不能重复导入", _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 设置鼠标
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                    });

                    bool lb = false;
                    lb = caseManager.ImportCase(dialog.FileName);

                    if (lb)
                    {
                        //----------------20150525 add the import after regonize the data again ---------//
                        //List<Data> dataList = dm.GetAllData(myCase.CASE_PATH, myCase.CASE_GUID, evidence.EVIDENCE_GUID);
                        //if (dataList == null || dataList.Count == 0)
                        //    _em.DelEvidenceByIds(myCase, evidenceList);

                        //if (control.Tag.ToString().Equals("1") || control.Tag.ToString().Equals("2") || control.Tag.ToString().Equals("15") || control.Tag.ToString().Equals("21") || control.Tag.ToString().Equals("22"))
                        dataManager.ReorganizeAllData(System.AppDomain.CurrentDomain.BaseDirectory + "\\data.mdb", dialog.FileName, myCase.CASE_GUID, myCase.CASE_NAME, false);

                        // 重新加载案件数据
                        InitialCaseInfo();

                        MessageBox.Show("导入成功 ！！！", _clew);
                    }
                }
                else
                {
                    MessageBox.Show("目录下没有此案件，不能导入", _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "导入失败", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // 恢复鼠标
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }
    }
}
