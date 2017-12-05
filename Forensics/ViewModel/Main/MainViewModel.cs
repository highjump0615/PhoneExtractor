using Forensics.BLL;
using Forensics.Command;
using Forensics.Model;
using Forensics.Model.Device;
using Forensics.View.Dialog;
using Forensics.ViewModel.Main;
using log4net;
using Manzana;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Management;
using System.Text.RegularExpressions;
using Forensics.Util;
using System.Threading;

namespace Forensics.ViewModel
{
    public enum Pages
    {
        Main,
        MainHome,
        HomeHome,
        HomeExtract,
        MainData,
        DataCase,
        DataEvidence,
        DataCaseDetail,
        MainTool,
        ToolList,
        ToolAndroid,
        ToolApple,
        ToolOther,
        Setting,
        SettingSetting,
        SettingEnv,
        SettingUpgrade,
        SettingFeedback,
        SettingAbout,

        // 提取方式
        ExtractTypeSecondary,

        Other,
    };

    public struct PnPEntityInfo
    {
        public String PNPDeviceID;      // 设备ID
        public String Name;             // 设备名称
        public String Description;      // 设备描述
        public String Service;          // 服务
        public String Status;           // 设备状态
        public UInt16 VendorID;         // 供应商标识
        public UInt16 ProductID;        // 产品编号 
        public Guid ClassGuid;          // 设备安装类GUID
    }

    public class MainViewModel : HostViewModel
    {
        public ConnectEventHandler i_Connect = null;
        public ConnectEventHandler i_DisConnect = null;
        public iPhone iPhoneInterface;

        /// <summary>
        /// 当前连接的设备
        /// </summary>
        public DeviceInfo CurrentDevice { get; set; }

        /// <summary>
        /// 首页命令
        /// </summary>
        private ICommand _goToHomeCommand;
        public ICommand GoToHomeCommand
        {
            get { return _goToHomeCommand ?? (_goToHomeCommand = new DelegateCommand(GoToHomePage)); }
        }

        /// <summary>
        /// 资料管理页面命令
        /// </summary>
        private ICommand _goToDataCommand;
        public ICommand GoToDataCommand
        {
            get { return _goToDataCommand ?? (_goToDataCommand = new DelegateCommand(GoToDataPage)); }
        }

        /// <summary>
        /// 工具管理页面命令
        /// </summary>
        private ICommand _goToToolCommand;
        public ICommand GoToToolCommand
        {
            get { return _goToToolCommand ?? (_goToToolCommand = new DelegateCommand(GoToToolPage)); }
        }

        /// <summary>
        /// 设置页面命令
        /// </summary>
        private ICommand _goToSettingCommand;
        public ICommand GoToSettingCommand
        {
            get { return _goToSettingCommand ?? (_goToSettingCommand = new DelegateCommand(GoToSettingPage)); }
        }

        /// <summary>
        /// 快速检测命令
        /// </summary>
        private ICommand _quickCheckCommand;
        public ICommand QuickCheckCommand
        {
            get { return _quickCheckCommand ?? (_quickCheckCommand = new DelegateCommand(DoQuickCheck)); }
        }

        /// <summary>
        /// 安卓自动连接命令
        /// </summary>
        private ICommand _androidAutoConnectCommand;
        public ICommand AndroidAutoConnectCommand
        {
            get { return _androidAutoConnectCommand ?? (_androidAutoConnectCommand = new DelegateCommand(ConnectAndroidAuto)); }
        }


        public override Pages PageIndex
        {
            get { return Pages.Main; }
        }

        public MainViewModel()
        {
            // 初始化
            this.CurrentDevice = null;

            this.RegisterChild<MainHomeViewModel>(() => new MainHomeViewModel());
            this.RegisterChild<MainDataViewModel>(() => new MainDataViewModel());
            this.RegisterChild<MainToolViewModel>(() => new MainToolViewModel());
            this.RegisterChild<MainSettingViewModel>(() => new MainSettingViewModel());

            this.SelectedChild = GetChild(typeof(MainHomeViewModel));

            // 苹果设备初始化
            try
            {
                i_Connect = new ConnectEventHandler(iPhoneConnected);
                i_DisConnect = new ConnectEventHandler(iPhoneDisconnected);
                iPhoneInterface = new iPhone(i_Connect, i_DisConnect);
            }
            catch (Exception ex)
            {
                saveErrorLog(ex.Message);
                saveErrorLog(ex.HelpLink);
                saveErrorLog(ex.StackTrace);
                saveErrorLog(ex.TargetSite.ToString());
            }

            //
            // 用户
            //
            string msg = "";
            UserManager um = new UserManager();
            User user = um.Login("admin", "ecryan", out msg);
            if (user == null)
            {
                MessageBox.Show(msg);
                return;
            }
            else
            {
                //保存登录人信息
                User.LoginUser = user;
                setLanguage();
            }
        }

        /// <summary>
        /// 设置语言
        /// </summary>
        public void setLanguage()
        {
            //
            // 加载语言
            //
            ResourceDictionary langRd = null;
            try
            {
                langRd = Application.LoadComponent(
                        new Uri(@"Resources/Strings/String." + User.LoginUser.USER_LANGUAGE + ".xaml", 
                        UriKind.Relative)
                    ) as ResourceDictionary;
            }
            catch
            {
            }

            if (langRd != null)
            {
                // 删除已设置的语言
                if (Application.Current.Resources.MergedDictionaries.Count() > 2)
                {
                    Application.Current.Resources.MergedDictionaries.RemoveAt(2);
                }

                // 添加新的语言
                Application.Current.Resources.MergedDictionaries.Add(langRd);
            }
        }

        protected override void OnDispose()
        {
            if (this.SelectedChild != null)
                ((ViewModelBase)SelectedChild).Dispose();

            base.OnDispose();
        }

        /// <summary>
        /// 跳转到首页
        /// </summary>
        private void GoToHomePage()
        {
            this.SelectedChild = GetChild(typeof(MainHomeViewModel));
        }

        /// <summary>
        /// 跳转到资料管理页面
        /// </summary>
        private void GoToDataPage()
        {
            this.SelectedChild = GetChild(typeof(MainDataViewModel));
        }
        
        /// <summary>
        /// 跳转到工具管理页面
        /// </summary>
        private void GoToToolPage()
        {
            this.SelectedChild = GetChild(typeof(MainToolViewModel));
        }

        /// <summary>
        /// 跳转到设置页面
        /// </summary>
        private void GoToSettingPage(object param)
        {
            this.SelectedChild = GetChild(typeof(MainSettingViewModel));

            ((MainSettingViewModel)this.SelectedChild).SelectChildViewModel((Type)param);
        }

        /// <summary>
        /// 快速检测
        /// </summary>
        private void DoQuickCheck()
        {
            int status = checkDevicesStatus(); //detect the phone ios or android connect

            if (status == 1 || status == 2)
            {
                if (chcekOpencase()) { return; }

                CommonUtil.GB_flag_auto = true;
            }
            if (status == 1)   //ios
            {
                //if (this.frmiosext == null)
                //{
                //    frmiosext = new AI.AppleExtractor(0);                                   //苹果提取
                //    frmiosext.goCheckData += new AI.AppleExtractor.goCheck(ExchangeForm);
                //}
                //else
                //{
                //    if (Program.currentCase.CASE_GUID == Program.Logicios_caseid)                  //2015 add for the change case refresh logical ui
                //    { }
                //    else
                //    {
                //        frmiosext = new AI.AppleExtractor(0);                                   //苹果提取
                //        frmiosext.goCheckData += new AI.AppleExtractor.goCheck(ExchangeForm);
                //    }
                //}
                //FillMainPanel(this.frmiosext);
                ////this.frmiosext.refreshIphoneconnect();
                //ribbonControl1.SelectedRibbonTabItem = ribbonTabItem2;
            }
            else if (status == 2) //android
            {
                //if (this.logicalControl == null)
                //{
                //    logicalControl = new FORENSICS.AI.LogicalExtracter();                  //安卓提取
                //    logicalControl.Jumpto += new LogicalExtracter.gotoData(ToAnalyseForm);
                //}
                //else
                //{
                //    if (Program.currentCase.CASE_GUID == Program.Logic_caseid)                  //2015 add for the change case refresh logical ui
                //    { }
                //    else
                //    {
                //        logicalControl = new FORENSICS.AI.LogicalExtracter();                  //安卓提取
                //        logicalControl.Jumpto += new LogicalExtracter.gotoData(ToAnalyseForm);
                //    }
                //}
                //FillMainPanel(this.logicalControl);
                //ribbonControl1.SelectedRibbonTabItem = ribbonTabItem2;
            }
            else
            {
                var strMsg = Application.Current.FindResource("msgConnectNotFound") as string;
                MessageBox.Show(strMsg);
            }
        }

        private bool chcekOpencase()
        {
            bool lbcheck = false;
            foreach (System.Threading.Thread t in CommonUtil.GT_arr)
            {
                while (t.IsAlive)
                {
                    lbcheck = true;
                    break;
                    //System.Threading.Thread.Sleep(3000);
                }
            }
            if (lbcheck)
            {
                var strMsg = Application.Current.FindResource("msgAnalyseInProgress") as string;
                MessageBox.Show(strMsg);
                return true;
            }

            return false;
        }

        private void ConnectAndroidThread()
        {
            Thread.Sleep(500);

            RyLib.AndroidExtractHelp AExtractHelp = new RyLib.AndroidExtractHelp();

            bool bRes = RyLib.ToolBox.MobileIsOnLine();
            if (bRes)
            {
                var strOSVer = RyLib.ToolBox.MobileOSVersion();

                AExtractHelp.GetDeviceBasicInfo();

                // 添加设备
                DeviceProperty devProperty = new DeviceProperty()
                {
                    OSVersion = strOSVer,
                    IMEI = AExtractHelp.deviceInfo.imei,
                    Brand = AExtractHelp.deviceInfo.brand,
                    ModelNumber = AExtractHelp.deviceInfo.model,
                    SerialNumber = AExtractHelp.deviceInfo.serialNo,
                    IsRooted = RyLib.ToolBox.MobileIsRoot()
                };

                this.CurrentDevice = new DeviceInfo(devProperty);
                showDeviceInfo();
            }

            MainWindow viewMain = (MainWindow)this.View;
            viewMain.Dispatcher.Invoke(new Action(() =>
            {
                closeWaiting();
                if (bRes)
                {
                    viewMain.openConnectSuccess(MainHomeViewModel.DeviceType.Android);
                }
                else
                {
                    // 连接失败, 弹出失败
                    viewMain.openConnectFail();
                }
            }));

        }

        /// <summary>
        /// 安卓自动连接
        /// </summary>
        public void ConnectAndroidAuto()
        {
            Thread thread = new Thread(new ThreadStart(ConnectAndroidThread));
            thread.Start();

            MainWindow viewMain = (MainWindow)this.View;
            viewMain.Dispatcher.Invoke(new Action(() => {
                viewMain.openConnectWaiting();
            }));
        }

        /// <summary>
        /// 跳转到提取页面
        /// </summary>
        public void GoToExtractPage(MainHomeViewModel.DeviceType type, string saveExtractPath = null)
        {
            this.SelectedChild = GetChild(typeof(MainHomeViewModel));

            MainHomeViewModel vm = (MainHomeViewModel)this.SelectedChild;
            vm.showExtractPage(type, saveExtractPath);
        }

        /// <summary>
        /// iOS设备连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void iPhoneConnected(object sender, ConnectEventArgs args)
        {
            Console.WriteLine(iPhoneInterface.DeviceName);

            //myTest = new afcTest(myAfcAct);
            //this.Invoke(myTest);

            DeviceProperty myIphoneProperty = new DeviceProperty()
            {
                UniqueChipID = iPhoneInterface.UniqueChipID,
                UniqueDeviceID = iPhoneInterface.DeviceId,
                Name = iPhoneInterface.DeviceName,
                SerialNumber = iPhoneInterface.SerialNumber,
                ProductType = iPhoneInterface.ProductType,
                ProductVersion = iPhoneInterface.ProductVersion,
                IMEI = iPhoneInterface.IMEI,
                ICCID = iPhoneInterface.ICCID,
                IMSI = iPhoneInterface.IMSI,
                ActivationState = iPhoneInterface.ActivationState,
                BasebandMasterKeyHash = iPhoneInterface.BasebandMasterKeyHash,
                BuildVersion = iPhoneInterface.BuildVersion,
                Class = iPhoneInterface.DeviceType,
                ModelNumber = iPhoneInterface.ModelNumber,
                PhoneNumber = iPhoneInterface.PhoneNumber,
                SIMStatus = iPhoneInterface.SIMStatus,
                BluetoothAddress = iPhoneInterface.BluetoothAddress,
                WiFiAddress = iPhoneInterface.WiFiAddress
            };

            this.CurrentDevice = new DeviceInfo(myIphoneProperty);

            //Program.CurrentPD.ICCID_string = iPhoneInterface.ICCID;
            //Program.CurrentPD.IMEI_string = iPhoneInterface.IMEI;
            //Program.CurrentPD.IMSI_string = iPhoneInterface.IMSI;
            //Program.CurrentPD.Phone_serial = iPhoneInterface.SerialNumber;
            //Program.CurrentPD.Phone_number = iPhoneInterface.PhoneNumber;

            //Program.CurrentPD.Phone_os = "iOS " + iPhoneInterface.BuildVersion;
            //Program.CurrentPD.Phone_model = iPhoneInterface.DeviceType;
            //Program.CurrentPD.Phone_brand = "Apple";


            // 显示设备信息
            showDeviceInfo();
            
            // 弹出连接成功对话框，如果已有弹出的窗口，则忽略            
            MainWindow viewMain = (MainWindow)this.View;
            viewMain.Dispatcher.Invoke(new Action(() => {
                if (App.Current.MainWindow.OwnedWindows.Count == 0)
                {
                    viewMain.openConnectSuccess(MainHomeViewModel.DeviceType.Apple);
                }
                else
                {
                    // 等待窗口，把它关闭进入提取阶段
                    if (closeWaiting())
                    {
                        GoToExtractPage(MainHomeViewModel.DeviceType.Apple, viewMain.ExtractPath);
                    }
                }
            }));
        }

        /// <summary>
        /// 关闭正在连接
        /// </summary>
        /// <returns></returns>
        private bool closeWaiting()
        {
            bool bFound = false;
            foreach (Window wnd in App.Current.MainWindow.OwnedWindows)
            {
                // 等待窗口，把它关闭进入提取阶段
                if (wnd is DialogConnectWaiting)
                {
                    bFound = true;
                    wnd.Close();
                }
            }

            return bFound;
        }

        /// <summary>
        /// iOS设备断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void iPhoneDisconnected(object sender, ConnectEventArgs args)
        {
            Console.WriteLine("disconnected");

            this.CurrentDevice = null;
            showDeviceInfo();

            // 弹出连接断开对话框，如果已有弹出的窗口，则忽略
            MainWindow viewMain = (MainWindow)this.View;
            viewMain.Dispatcher.Invoke(new Action(() => {
                if (App.Current.MainWindow.OwnedWindows.Count == 0)
                {
                    viewMain.openConnectDisconnect();
                }
            }));

            ////myDisconnect = new afcTest(myAFCDisconnect);
            ////this.Invoke(myDisconnect);
            //cleariPhoneInfo();
            //disconButton1.Enabled = true;
            //conToolBtn.Enabled = false;
            ////conToolBtn2.Enabled = false;
            //AfcExtractor.Enabled = false;
            ////iPhoneInterface = new iPhone(null,null); 
        }

        private void showDeviceInfo()
        {
            // 显示设备信息
            if (this.SelectedChild is MainHomeViewModel)
            {
                ((MainHomeViewModel)this.SelectedChild).showDeviceInfo();
            }
        }

        #region check
        //检查安卓或者苹果手机连接状况
        //1:苹果手机连接
        //2:安卓手机连接
        //0:未检到连接信息
        public int checkDevicesStatus()
        {
            String QueryString;
            QueryString = "SELECT * FROM Win32_PnPEntity"; // WHERE PNPDeviceID LIKE '%VID[_]____&PID[_]____%'
            string ls = "";
            try
            {
                ManagementObjectCollection PnPEntityCollection = new ManagementObjectSearcher(QueryString).Get();

                if (PnPEntityCollection != null)
                {
                    //MessageBox.Show("c");
                    foreach (ManagementObject Entity in PnPEntityCollection)
                    {

                        String PNPDeviceID = Entity["PNPDeviceID"] as String;

                        Match match = Regex.Match(PNPDeviceID, "VID_[0-9|A-F]{4}&PID_[0-9|A-F]{4}");

                        if (match.Success)
                        {
                            PnPEntityInfo Element;

                            Element.PNPDeviceID = PNPDeviceID;                      // 设备ID
                            if (Entity["Name"] == null)
                            { Element.Name = "none"; }
                            else
                            {
                                Element.Name = Entity["Name"] as String;                // 设备名称
                            }
                            //Element.Description = Entity["Description"] as String;  // 设备描述
                            //Element.Service = Entity["Service"] as String;          // 服务
                            //Element.Status = Entity["Status"] as String;            // 设备状态
                            // Element.VendorID = Convert.ToUInt16(match.Value.Substring(4, 4), 16);   // 供应商标识
                            //Element.ProductID = Convert.ToUInt16(match.Value.Substring(13, 4), 16); // 产品编号
                            //Element.ClassGuid = new Guid(Entity["ClassGuid"] as String);            // 设备安装类GUID

                            if ((Element.Name.ToLower().IndexOf("android") > -1) || (Element.Name.ToLower().IndexOf("adb interface") > -1))
                            {
                                //MessageBox.Show("nb");
                                return 2;
                            }
                            if (Element.Name.ToLower().IndexOf("apple mobile") > -1)
                            {
                                //MessageBox.Show("nb2");
                                //break;
                                return 1;
                            }
                            ls = ls + Element.Name + "\r\n";
                            //MessageBox.Show(ls);
                        }
                    }
                    //MessageBox.Show(ls);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ls);
                return 0;
            }
            return 0;
        }
        #endregion
    }
}
