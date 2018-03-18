using Forensics.BLL;
using Forensics.Command;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    public class SettingSettingViewModel : ViewModelBase
    {
        public enum NamingRule
        {
            Default,
            Custom
        }

        private String _clew = "操作提示";

        /// <summary>
        /// 案件目录命名规则
        /// </summary>
        private NamingRule _namingRuleOption = NamingRule.Default;
        public NamingRule NamingRuleOption
        {
            get { return _namingRuleOption; }
            set { _namingRuleOption = value; PropertyChanging("NamingOption"); }
        }

        /// <summary>
        /// 案件目录
        /// </summary>
        public string RuleCase { get; set; } = "[CaseNo]_时间戳";
        /// <summary>
        /// 物证目录
        /// </summary>
        public string RuleEvidence { get; set; } = "[CellBrand]_序列号";
        /// <summary>
        /// 物证原始目录
        /// </summary>
        public string RuleRaw { get; set; } = "Data";
        /// <summary>
        /// 物证解析目录
        /// </summary>
        public string RuleData { get; set; } = "Xml";
        /// <summary>
        /// 物证报告目录
        /// </summary>
        public string RuleReport { get; set; } = "Report";
        /// <summary>
        /// 案件编号
        /// </summary>
        public string RuleCaseNo { get; set; } = "[Case]+序列号";
        /// <summary>
        /// 物证编号
        /// </summary>
        public string RuleEviNo { get; set; } = "[Evidence]+时间戳";

        /// <summary>
        /// 检验人
        /// </summary>
        public string Examiner { get; set; }

        /// <summary>
        /// 系统时区
        /// </summary>
        public ObservableCollection<TimeZoneInfo> ListTimeZone { get; set; } = new ObservableCollection<TimeZoneInfo>();
        public string TimeZoneSelected { get; set; }
        public string Language { get; set; }

        public override Pages PageIndex
        {
            get { return Pages.SettingSetting; }
        }

        /// <summary>
        /// 默认路径
        /// </summary>
        public string DefaultPathCase { get; set; }
        public string DefaultPathWc { get; set; }
        public string DefaultPathMap { get; set; }

        /// <summary>
        /// 照片数据分页上限
        /// </summary>
        public string PhotoLimit { get; set; }

        /// <summary>
        /// 保存命令
        /// </summary>
        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new DelegateCommand(SaveSetting)); }
        }

        public SettingSettingViewModel()
        {
            // 案件目录
            this.DefaultPathCase = ConfigurationManager.AppSettings["caseDefaultPath"];

            // 彩虹表目录
            this.DefaultPathWc = ConfigurationManager.AppSettings["wcDefaultPath"];
            if (this.DefaultPathWc == null)
            {
                this.DefaultPathWc = "D:\\Ryan\\EDEC\\tool\\mmdb";
            }

            // 离线地图目录
            this.DefaultPathMap = ConfigurationManager.AppSettings["mapDefaultPath"];
            if (this.DefaultPathMap == null)
            {
                this.DefaultPathMap = "D:\\Ryan\\E5\\maps";
            }

            // 照片数据分页上限
            this.PhotoLimit = ConfigurationManager.AppSettings["PhotolimitsNumber"];
            if (this.PhotoLimit == null)
            {
                this.PhotoLimit = "0";
            }

            // 命名规则
            string ics = ConfigurationManager.AppSettings["RuleCustomer"];
            if (ics == "Yes")
            {
                this.NamingRuleOption = NamingRule.Custom;
            }

            // 系统时区
            foreach (TimeZoneInfo tzi in TimeZoneInfo.GetSystemTimeZones())
            {
                this.ListTimeZone.Add(tzi);
            }
            this.TimeZoneSelected = ConfigurationManager.AppSettings["TimeZoneName"];

            // 语言
            this.Language = User.LoginUser.USER_LANGUAGE;
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void SaveSetting()
        {
            #region 目录设置
            if (!Directory.Exists(this.DefaultPathCase))
            {
                Directory.CreateDirectory(this.DefaultPathCase);
            }

            UpdateAppConfig("caseDefaultPath", this.DefaultPathCase);
            UpdateAppConfig("wcDefaultPath", this.DefaultPathWc);
            UpdateAppConfig("mapDefaultPath", this.DefaultPathMap);

            if (String.IsNullOrWhiteSpace(this.PhotoLimit))
            {
                var strMsg = Application.Current.FindResource("msgSettingPageSizeLimit") as string;
                MessageBox.Show(strMsg, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            UpdateAppConfig("PhotolimitsNumber", this.PhotoLimit);
            #endregion

            #region 物证设置
            if (this.NamingRuleOption == NamingRule.Default)
            {
                UpdateAppConfig("RuleCustomer", "No");  //use the default path
            }
            else
            {
                if (String.IsNullOrWhiteSpace(this.RuleCase) || 
                    String.IsNullOrWhiteSpace(this.RuleEvidence) || 
                    String.IsNullOrWhiteSpace(this.RuleRaw) || 
                    String.IsNullOrWhiteSpace(this.RuleData) || 
                    String.IsNullOrWhiteSpace(this.RuleReport))
                {
                    var strMsg = Application.Current.FindResource("msgSettingCaseNaming") as string;
                    MessageBox.Show(strMsg, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateAppConfig("RuleCustomer", "Yes");  //set the customer path
                UpdateAppConfig("RuleCase", this.RuleCase);
                UpdateAppConfig("RuleEvidence", this.RuleEvidence);
                UpdateAppConfig("RuleRaw", this.RuleRaw);
                UpdateAppConfig("RuleData", this.RuleData);
                UpdateAppConfig("RuleReport", this.RuleReport);
                UpdateAppConfig("RuleCaseNo", this.RuleCaseNo);
                UpdateAppConfig("RuleEviNo", this.RuleEviNo);
            }

            UpdateAppConfig("Examiner", this.Examiner);
            #endregion

            #region  语言设置
            UserManager um = new UserManager();
            User.LoginUser.USER_LANGUAGE = this.Language;
            Globals.Instance.MainVM.setLanguage();

            // 更新用户信息
            try
            {
                um.UpdateUser(User.LoginUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            #endregion 

            #region  时区设置
            int hours = 0;
            if (this.TimeZoneSelected != null)
            {
                foreach (TimeZoneInfo tzi in this.ListTimeZone)
                {
                    if (tzi.Id == this.TimeZoneSelected)
                    {
                        hours = tzi.BaseUtcOffset.Hours;
                        break;
                    }
                }
            }
            else
            {
                var strMsg = Application.Current.FindResource("msgSettingTimeZone") as string;
                MessageBox.Show(strMsg, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                UpdateAppConfig("TimeZoneName", this.TimeZoneSelected);
                UpdateAppConfig("TimeZoneNumber", hours.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            #endregion
        }

        private void UpdateAppConfig(string newKey, string newValue)
        {
            bool isModified = false;
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == newKey)
                {
                    isModified = true;
                }
            }
            // Open App.Config of executable
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // You need to remove the old settings object before you can replace it
            if (isModified)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            // Add an Application Setting.
            config.AppSettings.Settings.Add(newKey, newValue);
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
