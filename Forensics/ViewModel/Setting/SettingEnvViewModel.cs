using Forensics.Model.Setting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    class SettingEnvViewModel : ViewModelBase
    {
        public override Pages PageIndex
        {
            get { return Pages.SettingEnv; }
        }

        public List<EnvironmentItem> listEnvItem { get; set; } = new List<EnvironmentItem>();

        public SettingEnvViewModel()
        {
            this.listEnvItem.Add(new EnvironmentItem("adb.exe"));
            this.listEnvItem.Add(new EnvironmentItem("AdbHelper.dll"));
            this.listEnvItem.Add(new EnvironmentItem("AdbWinApi.dll"));
            this.listEnvItem.Add(new EnvironmentItem("busybox.exe"));
            this.listEnvItem.Add(new EnvironmentItem("service.apk"));
            this.listEnvItem.Add(new EnvironmentItem("mydos2unix.exe"));
            this.listEnvItem.Add(new EnvironmentItem("AdbWinUsbApi.dll"));
            this.listEnvItem.Add(new EnvironmentItem("libintl3.dll"));
            this.listEnvItem.Add(new EnvironmentItem("libiconv2.dll"));
            this.listEnvItem.Add(new EnvironmentItem("sms.tp"));
            this.listEnvItem.Add(new EnvironmentItem("contact.tp"));
            this.listEnvItem.Add(new EnvironmentItem("Devinfo.tp"));
            this.listEnvItem.Add(new EnvironmentItem("appInfo.tp"));
            this.listEnvItem.Add(new EnvironmentItem("calllog.tp"));
            this.listEnvItem.Add(new EnvironmentItem("Data.mdb"));
            this.listEnvItem.Add(new EnvironmentItem("Rar.exe"));

            // 检查环境
            foreach (EnvironmentItem ei in this.listEnvItem)
            {
                if (File.Exists(ei.Name))
                {
                    ei.Status = EnvironmentItem.StatusEnum.INSTALLED;
                }
            }
        }
    }
}
