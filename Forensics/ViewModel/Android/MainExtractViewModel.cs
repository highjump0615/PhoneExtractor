using Forensics.Model.Extract;
using Forensics.ViewModel.Android;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Forensics.ViewModel.MainHomeViewModel;

namespace Forensics.ViewModel
{
    class MainExtractViewModel : ViewModelBase
    {
        private ExtractType Type = ExtractType.Apple;
        public ExtractProgressViewModel progressVM { get; set; }
        public ObservableCollection<SystemLog> LogList { get; set; }

        private string uniqueChipID = "";

        public override Pages PageIndex
        {
            get { return Pages.HomeExtract; }
        }

        public MainExtractViewModel()
        {
            this.LogList = new ObservableCollection<SystemLog>();
            this.progressVM = new ExtractProgressViewModel();
        }

        /// <summary>
        /// 开始提取
        /// </summary>
        /// <param name="type"></param>
        public void startExtract(ExtractType type)
        {
            this.Type = type;

            var strSavePath = "f:\\temp\\Data";
            if (!Directory.Exists(strSavePath))
            {
                Directory.CreateDirectory(strSavePath);
            }

            // 更新进度条
            this.progressVM.startExtract(type);

            // 开始
            Thread threadMain = new Thread(new ThreadStart(backupThread));
            threadMain.Start();
        }

        /// <summary>
        /// 添加系统日志
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        private void addSystemLog(string item, string result)
        {
            this.LogList.Add(new SystemLog() { Date = DateTime.Now, Item = item, Result = result });
        }

        /// <summary>
        /// 备份线程
        /// </summary>
        private void backupThread()
        {
            if (Type == ExtractType.Apple)
            {
                addSystemLog("数据备份中，请勿中途卸载设备...", "开始");
            }

            string backupEXE = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "Apple", "Mobile Device Support", "AppleMobileBackup.exe");
            if (!File.Exists(backupEXE)) backupEXE = Path.Combine(@"C:\Program Files\Common Files\Apple\Mobile Device Support", "AppleMobileBackup.exe");
            if (File.Exists(backupEXE))
            {
                if (uniqueChipID != "")
                {
                }
            }
        }
    }
}
