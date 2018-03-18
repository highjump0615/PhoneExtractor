using Forensics.BLL;
using Forensics.Command;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    public class VersionInfo
    {
        public string Name { get; set; }

        public string Tag { get; set; }

        public VersionInfo(string name, string tag)
        {
            this.Name = name;
            this.Tag = tag;
        }
    }

    public class SettingUpgradeViewModel : ViewModelBase
    {
        private ExtractionManager _em = new ExtractionManager();
        private List<VersionInfo> mListVersion = new List<VersionInfo>();

        public string Description { get; set; }

        public override Pages PageIndex
        {
            get { return Pages.SettingUpgrade; }
        }

        /// <summary>
        /// 升级命令
        /// </summary>
        private ICommand _upgradeCommand;
        public ICommand UpgradeCommand
        {
            get { return _upgradeCommand ?? (_upgradeCommand = new DelegateCommand(DoUpgrade)); }
        }

        public SettingUpgradeViewModel()
        {
            // 初始化
            mListVersion.Add(new VersionInfo("安卓一键提取", "plYJTQ"));
            mListVersion.Add(new VersionInfo("苹果一键提取", "plPGYJTQ"));
            mListVersion.Add(new VersionInfo("安卓逻辑", "picAndroid_LJ"));
            mListVersion.Add(new VersionInfo("安卓物理", "picAndroid_WL"));
            mListVersion.Add(new VersionInfo("安卓密码", "picAndroid_MM"));
            mListVersion.Add(new VersionInfo("安卓导入", "picAndroidDR"));
            mListVersion.Add(new VersionInfo("苹果逻辑", "picIphoneLJ"));
            mListVersion.Add(new VersionInfo("苹果导入", "picIphoneDR"));
            mListVersion.Add(new VersionInfo("功能机提取", "picGNTQ"));
            mListVersion.Add(new VersionInfo("SIM卡提取", "picSim"));
            mListVersion.Add(new VersionInfo("蓝牙提取", "picLYTQ"));
            mListVersion.Add(new VersionInfo("通用分析", "picJtag"));
            mListVersion.Add(new VersionInfo("DB查看", "picXPTQ"));
            mListVersion.Add(new VersionInfo("密码搜索", "picTYFX"));
            mListVersion.Add(new VersionInfo("高级密码", "picPWD"));
            mListVersion.Add(new VersionInfo("离线微信", "picJXFX"));

            try
            {
                List<Extraction> list = _em.GetAll();
                foreach (VersionInfo vi in mListVersion)
                {
                    foreach (Extraction el in list)
                    {
                        if (el.ExtractionName.Equals(vi.Tag))
                        {
                            this.Description += vi.Name + "\t\t\t" + el.EXTRACTION_VERSION + "\n";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 升级
        /// </summary>
        private void DoUpgrade()
        {
            System.Diagnostics.Process.Start("http://www.ecryan.com.cn/downloadfac.aspx?m=145");
        }
    }
}
