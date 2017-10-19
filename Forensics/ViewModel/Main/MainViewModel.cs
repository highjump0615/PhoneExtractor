using Forensics.Command;
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

        Other,
    };

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
        /// 跳转到提取页面
        /// </summary>
        public void GoToExtractPage(MainHomeViewModel.ExtractType type, string saveExtractPath = null)
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

            DeviceProperty myIphoneProperty;
            myIphoneProperty.UniqueChipID = iPhoneInterface.UniqueChipID;
            myIphoneProperty.UniqueDeviceID = iPhoneInterface.DeviceId;
            myIphoneProperty.Name = iPhoneInterface.DeviceName;
            myIphoneProperty.SerialNumber = iPhoneInterface.SerialNumber;
            myIphoneProperty.ProductType = iPhoneInterface.ProductType;
            myIphoneProperty.ProductVersion = iPhoneInterface.ProductVersion;
            myIphoneProperty.IMEI = iPhoneInterface.IMEI;
            myIphoneProperty.ICCID = iPhoneInterface.ICCID;
            myIphoneProperty.IMSI = iPhoneInterface.IMSI;
            myIphoneProperty.ActivationState = iPhoneInterface.ActivationState;
            myIphoneProperty.BasebandMasterKeyHash = iPhoneInterface.BasebandMasterKeyHash;
            myIphoneProperty.BuildVersion = iPhoneInterface.BuildVersion;
            myIphoneProperty.Class = iPhoneInterface.DeviceType;
            myIphoneProperty.ModelNumber = iPhoneInterface.ModelNumber;
            myIphoneProperty.PhoneNumber = iPhoneInterface.PhoneNumber;
            myIphoneProperty.SIMStatus = iPhoneInterface.SIMStatus;
            myIphoneProperty.BluetoothAddress = iPhoneInterface.BluetoothAddress;
            myIphoneProperty.WiFiAddress = iPhoneInterface.WiFiAddress;

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
                    viewMain.openConnectSuccess();
                }
                else
                {
                    foreach (Window wnd in App.Current.MainWindow.OwnedWindows)
                    {
                        // 等待窗口，把它关闭进入提取阶段
                        if (wnd is DialogConnectWaiting)
                        {
                            wnd.Close();
                            GoToExtractPage(MainHomeViewModel.ExtractType.Apple, viewMain.ExtractPath);
                        }
                    }
                }
            }));
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
    }
}
