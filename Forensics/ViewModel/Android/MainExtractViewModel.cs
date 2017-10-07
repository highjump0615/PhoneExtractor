using Forensics.Model.Extract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    class MainExtractViewModel : ViewModelBase
    {
        public ObservableCollection<SystemLog> LogList { get; set; }

        public override Pages PageIndex
        {
            get { return Pages.HomeExtract; }
        }

        public MainExtractViewModel()
        {
            this.LogList = new ObservableCollection<SystemLog>();

            this.LogList.Add(new SystemLog() { Date = DateTime.Now, Item = "正在检测连接状态", Result = "成功" });
            this.LogList.Add(new SystemLog() { Date = DateTime.Now, Item = "正在检测连接状态", Result = "成功" });
            this.LogList.Add(new SystemLog() { Date = DateTime.Now, Item = "正在检测连接状态", Result = "成功" });
            this.LogList.Add(new SystemLog() { Date = DateTime.Now, Item = "正在检测连接状态", Result = "成功" });
            this.LogList.Add(new SystemLog() { Date = DateTime.Now, Item = "正在检测连接状态", Result = "成功" });
            this.LogList.Add(new SystemLog() { Date = DateTime.Now, Item = "正在检测连接状态", Result = "成功" });
            this.LogList.Add(new SystemLog() { Date = DateTime.Now, Item = "正在检测连接状态", Result = "成功" });
        }
    }
}
