using Forensics.Command;
using Forensics.Model.Device;
using Forensics.Model.Extract;
using Forensics.Util;
using Forensics.ViewModel.Android;
using log4net;
using mbdbdump;
using RyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Plists;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using static Forensics.ViewModel.MainHomeViewModel;

namespace Forensics.ViewModel
{
    class MainExtractViewModel : ViewModelBase
    {
        private DeviceType Type { get; set; } = DeviceType.Apple;

        public ExtractProgressViewModel progressVM { get; set; }
        public ObservableCollection<SystemLog> LogList { get; set; } = new ObservableCollection<SystemLog>();

        private RyLib.AndroidExtractHelp AExtractHelp = null;

        /// 提取参数
        private string savePath = ConfigurationManager.AppSettings["caseDefaultPath"];
        private string tempDataPath = "";
        private string unbackPath = "";
        private List<mbdbdump.mbdb.MBFileRecord> files92;
        private string strUUID = "";
        private string gs_itunes_decpath;
        private string SerialNumber = "";
        private string timestamp = "";
        private iPhoneBackup backup = null;
        //////////////////////

        /// <summary>
        /// Timer
        /// </summary>
        private DispatcherTimer mTimer = new DispatcherTimer();
        private int mnProgress = 0;

        public override Pages PageIndex
        {
            get { return Pages.HomeExtract; }
        }

        /// <summary>
        /// 是否在进行
        /// </summary>
        private bool _isInProgress = false;
        public bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                _isInProgress = value;
                PropertyChanging("IsInProgress");
            }
        }

        /// <summary>
        /// 是否能进入分析
        /// </summary>
        private bool _isAnalyseAvailable = false;
        public bool IsAnayseAvailable
        {
            get
            {
                return _isAnalyseAvailable && !_isInProgress;
            }
            set
            {
                _isAnalyseAvailable = value;
                PropertyChanging("IsAnayseAvailable");
            }
        }


        /// <summary>
        /// 分析命令
        /// </summary>
        private ICommand _analyseCommand;
        public ICommand AnalyseCommand
        {
            get { return _analyseCommand ?? (_analyseCommand = new DelegateCommand(DoAnalyse)); }
        }

        /// <summary>
        /// 停止命令
        /// </summary>
        private ICommand _pauseCommand;
        public ICommand PauseCommand
        {
            get { return _pauseCommand ?? (_pauseCommand = new DelegateCommand(PauseExtract)); }
        }

        /// <summary>
        /// 停止命令
        /// </summary>
        private ICommand _restartCommand;
        public ICommand RestartCommand
        {
            get { return _restartCommand ?? (_restartCommand = new DelegateCommand(RestartExtract)); }
        }


        public MainExtractViewModel()
        {
            this.progressVM = new ExtractProgressViewModel();

            // 初始化Timer
            mTimer.Tick += new EventHandler(dispatcherTimerProgressTick);
            mTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
        }

        private void dispatcherTimerProgressTick(object sender, EventArgs e)
        {
            this.progressVM.Percent++;

            // 达到100%, 停止
            if (this.progressVM.Percent >= 100)
            {
                mTimer.Stop();
            }

            // 达到已设置的
            if (this.progressVM.Percent >= mnProgress - 1)
            {
                mTimer.Stop();
            }
        }

        /// <summary>
        /// 安卓提取结束
        /// </summary>
        /// <param name="bError"></param>
        /// <param name="type"></param>
        private void AndroidExtractOK(bool bError, ExtractType type)
        {
            // 失败
            if (bError)
            {
                // 进度更新
                setProgress(null, null, 0);
                this.IsInProgress = false;
                return;
            }

            // 弹出添加物证
            doFinishExtract();
            this.IsInProgress = false;
        }

        private void AndroidExtractInfo(string s, string result)
        {
            addSystemLog(s, result);

            // 是否停止
            if (mnProgress < 0)
            {
                stopAndroidExtract();
            }
        }

        /// <summary>
        /// 开始提取
        /// </summary>
        /// <param name="type"></param>
        public void startExtract(DeviceType type, string saveExtractPath = null)
        {
            this.Type = type;
            mnProgress = 0;

            if (saveExtractPath != null)
            {
                savePath = saveExtractPath;
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            if (type == DeviceType.Apple || type == DeviceType.Android)
            {
                tempDataPath = savePath;

                // 更新进度条
                this.progressVM.startExtract(type);

                // 开始
                Thread threadMain = new Thread(new ThreadStart(backupThread));
                threadMain.Start();
            }
            // 测试
            else
            {
            }
        }

        /// <summary>
        /// 添加系统日志
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        private void addSystemLog(string item, string result)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.LogList.Add(new SystemLog() { Item = item, Result = result });
            }));
        }

        /// <summary>
        /// 进度更新准备
        /// </summary>
        /// <param name="percent"></param>
        /// <returns>false 触发停止</returns>
        private bool setPrgressPrepared(int percent)
        {
            if (mnProgress < 0)
            {
                return false;
            }

            mnProgress = percent;
            mTimer.Start();

            return true;
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        /// <param name="title"></param>
        private void setProgress(string title, string desc = null, int percent = -1)
        {
            if (title != null)
            {
                this.progressVM.Title = title;
            }

            if (desc != null)
            {
                this.progressVM.Desc = desc;
            }

            if (percent >= 0)
            {
                mTimer.Stop();
                this.progressVM.Percent = percent;
            }
        }


        /// <summary>
        /// 备份线程
        /// </summary>
        private void backupThread()
        {
            this.IsInProgress = true;
            DeviceInfo currentDevice = Globals.Instance.MainVM.CurrentDevice;
            // 设备中途已卸载
            if (currentDevice == null)
            {
                this.IsInProgress = false;
                return;
            }

            if (this.Type == DeviceType.Apple)
            {
                this.DoAppleExtract();
                this.IsInProgress = false;
            }
            else if (this.Type == DeviceType.Android)
            {
                this.DoAndroidExtract();
            }
        }

        /// <summary>
        /// 苹果提取
        /// </summary>
        private void DoAppleExtract()
        {
            DeviceInfo currentDevice = Globals.Instance.MainVM.CurrentDevice;
            DeviceProperty devProp = currentDevice.DeviceProperty;

            // 初始化
            SerialNumber = devProp.SerialNumber;

            addSystemLog("数据备份中，请勿中途卸载设备...", "开始");

            string backupEXE = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "Apple", "Mobile Device Support", "AppleMobileBackup.exe");
            if (!File.Exists(backupEXE)) backupEXE = Path.Combine(@"C:\Program Files\Common Files\Apple\Mobile Device Support", "AppleMobileBackup.exe");
            if (File.Exists(backupEXE))
            {
                if (devProp.UniqueDeviceID != "")
                {
                    setProgress("数据备份中，请勿中途卸载设备...", null);
                    if (setPrgressPrepared(50) == false)
                    {
                        return;
                    }

                    Process p = new Process();
                    try
                    {
                        tempDataPath = tempDataPath.TrimEnd('\\');
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.FileName = backupEXE;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.StartInfo.Arguments = "-b --target " + devProp.UniqueDeviceID + " --root \"" + tempDataPath + "\"";
                        p.Start();

                        string strst = p.StandardOutput.ReadToEnd();
                        strst += p.StandardError.ReadToEnd();

                        Console.WriteLine(strst);
                        if (strst.IndexOf("ERROR") > -1)
                        {
                            saveErrorLog(strst);
                            //saveErrorLog(backupEXE);
                            //saveErrorLog(p.StartInfo.Arguments);

                            showFailed();
                        }
                    }
                    catch (Exception ex)
                    {
                        saveErrorLog(ex.Message);
                        saveErrorLog(ex.HelpLink);
                        saveErrorLog(ex.StackTrace);
                        saveErrorLog(ex.TargetSite.ToString());
                        p.Close();

                        showFailed();
                    }
                    p.Close();
                    p.Dispose();


                    // 进度更新
                    setProgress(null, null, 50);
                    if (setPrgressPrepared(80) == false)
                    {
                        return;
                    }

                    backup = new iPhoneBackup();
                    backup.path = Path.Combine(tempDataPath, devProp.UniqueDeviceID);

                    timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    gs_itunes_decpath = timestamp;  //all evidence use same time to decode itunes file and import for evidence
                    savePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, devProp.SerialNumber + "_" + timestamp);
                    if (Directory.Exists(tempDataPath))
                        savePath = Path.Combine(tempDataPath, devProp.SerialNumber + "_" + timestamp);

                    string mbdbFile = Path.Combine(tempDataPath, devProp.UniqueDeviceID, "Manifest.mbdb");
                    string ios10dbFile = Path.Combine(backup.path, "Manifest.db");

                    if (File.Exists(mbdbFile))
                    {
                        files92 = mbdbdump.mbdb.ReadMBDB(backup.path);
                        BinaryPlistReader az = new BinaryPlistReader();
                        IDictionary er = az.ReadObject(Path.Combine(backup.path, "Manifest.plist"));
                        parseAll92(er);

                        string tmpdir = Path.Combine(savePath, "var", "mobile", "applications");
                        if (!Directory.Exists(tmpdir)) Directory.CreateDirectory(tmpdir);

                        //fnPushPro = new AppDoc.Form1.fnPushProgress(pushProg);
                        //fnPushWPro = new AppDoc.Form1.fnPushWholePro(pushWProg);
                        //fm.getAPPFiles(applist, tmpdir, fnPushPro, fnPushWPro, sender);

                        progressVM.Title = "备份完成";
                        addSystemLog("数据备份完成", "成功");
                        addSystemLog("数据目录建立", "开始");

                        // 进度更新
                        setProgress("正在建立数据目录树，请稍候...", null, 80);
                        if (setPrgressPrepared(100) == false)
                        {
                            return;
                        }

                        unbackPath = savePath;
                        //TreeNode rootNode = new TreeNode();
                        //if (Directory.Exists(unbackPath))
                        //{
                        //    rootNode = new TreeNode();
                        //    rootNode.Text = deviceName;
                        //    BuildTree(unbackPath, rootNode);
                        //}
                        //addTreeNode addNode = delegate ()
                        //{
                        //    if (Directory.Exists(unbackPath))
                        //    {
                        //        treeView1.Nodes.Add(rootNode);
                        //    }
                        //};
                        //treeView1.Invoke(addNode);
                        //conToolBtn.Enabled = true;
                        //conToolBtn2.Enabled = true;

                        // 
                        AnalysisProc();


                        // 提取成功
                        doFinishExtract();
                    }
                    else if (File.Exists(ios10dbFile))
                    {
                        string infoFile = Path.Combine(backup.path, "info.plist");
                        string manifestFile = Path.Combine(backup.path, "Manifest.plist");
                        if (File.Exists(infoFile))
                        {
                            xdict dd = xdict.open(infoFile);

                            if (dd != null)
                            {
                                foreach (xdictpair xp in dd)
                                {
                                    if (xp.item.GetType() == typeof(string))
                                    {
                                        switch (xp.key)
                                        {
                                            //case "Build Version":
                                            //    buildVersion = xp.item.ToString();
                                            //    break;
                                            //case "Device Name":
                                            //    deviceName = xp.item.ToString().Trim();
                                            //    Console.WriteLine(deviceName);
                                            //    break;
                                            //case "ICCID":
                                            //    iccid = xp.item.ToString();
                                            //    break;
                                            //case "IMEI":
                                            //    imei = xp.item.ToString();
                                            //    break;
                                            //case "Last Backup Date":
                                            //    //lbd = root.ChildNodes[i + 1].InnerText.Replace("T", " ").Replace("Z", "");
                                            //    DateTime tmpTime;
                                            //    DateTime.TryParse(xp.item.ToString(), out tmpTime);
                                            //    lbd = tmpTime.ToString("yyyy-MM-dd HH:mm:ss");
                                            //    break;
                                            //case "Phone Number":
                                            //    phoneNumber = xp.item.ToString();
                                            //    break;
                                            //case "Product Type":
                                            //    productType = xp.item.ToString();
                                            //    break;
                                            //case "Product Version":
                                            //    productVersion = xp.item.ToString();
                                            //    break;
                                            case "Serial Number":
                                                SerialNumber = xp.item.ToString();
                                                break;
                                        }
                                        Console.WriteLine(xp.key + " " + xp.item.ToString());
                                    }
                                }



                                //savePath = @"c:\temp\" + SerialNumber + "_" + timestamp;‘
                            }
                        }
                        if (File.Exists(manifestFile))
                        {
                            BinaryPlistReader az = new BinaryPlistReader();
                            IDictionary er = az.ReadObject(Path.Combine(backup.path, "Manifest.plist"));

                            if ((bool)er["IsEncrypted"])
                            {
                                addSystemLog("当前iTunes备份设置了备份密码，请尝试去除或破解后再进行导入分析......", "提示终止");

                                setProgress("提取失败", null, 0);
                                return;
                            }
                            var sd = er["Lockdown"] as Dictionary<object, object>;
                            if (sd != null)
                            {
                                foreach (var pp in sd)
                                {
                                    string pKey = pp.Key as string;
                                    string pValue = pp.Value as string;
                                    switch (pKey)
                                    {
                                        //case "BuildVersion":
                                        //    buildVersion = pValue;
                                        //    break;
                                        //case "DeviceName":
                                        //    deviceName = pValue;
                                        //    break;
                                        //case "ProductType":
                                        //    productType = pValue;
                                        //    break;
                                        //case "ProductVersion":
                                        //    productVersion = pValue;
                                        //    break;
                                        case "SerialNumber":
                                            SerialNumber = pValue;
                                            break;
                                        case "UniqueDeviceID":
                                            break;
                                    }
                                }
                            }
                        }
                        //labelDeviceName.Text = deviceName;
                        //labelICCID.Text = iccid;
                        //labelIMEI.Text = imei;
                        //labelPhone.Text = phoneNumber;
                        //labelSerial.Text = SerialNumber;
                        //switch (productType.ToLower())
                        //{
                        //    case "iphone1,1":
                        //        productType = "iPhone 2G";
                        //        break;
                        //    case "iphone1,2":
                        //        productType = "iPhone 3G";
                        //        break;
                        //    case "iphone2,1":
                        //        productType = "iPhone 3GS";
                        //        break;
                        //    case "iphone3,1":
                        //    case "iphone3,2":
                        //    case "iphone3,3":
                        //        productType = "iPhone 4";
                        //        break;
                        //    case "iphone4,1":
                        //        productType = "iPhone 4S";
                        //        break;
                        //    case "iphone5,1":
                        //    case "iphone5,2":

                        //        productType = "iPhone 5";
                        //        break;
                        //    case "ipod1,1":
                        //        productType = "iPod touch 1G";
                        //        break;
                        //    case "ipod2,1":
                        //        productType = "iPod touch 2G";
                        //        break;
                        //    case "ipod3,1":
                        //        productType = "iPod touch 3G";
                        //        break;
                        //    case "ipod4,1":
                        //        productType = "iPod touch 4G";
                        //        break;
                        //    case "ipod5,1":
                        //        productType = "iPod touch 5G";
                        //        break;
                        //    case "ipad1,1":
                        //        productType = "iPad 1G";
                        //        break;
                        //    case "ipad2,1":
                        //    case "ipad2,2":
                        //    case "ipad2,3":
                        //    case "ipad2,4":
                        //        productType = "iPad 2";
                        //        break;
                        //    case "ipad3,1":
                        //    case "ipad3,2":
                        //    case "ipad3,3":
                        //        productType = "iPad 3";
                        //        break;
                        //    case "ipad3,4":
                        //    case "ipad3,5":
                        //    case "ipad3,6":
                        //        productType = "iPad 4";
                        //        break;
                        //    case "ipad2,5":
                        //    case "ipad2,6":
                        //    case "ipad2,7":
                        //        productType = "iPad mini 1G";
                        //        break;
                        //    default:
                        //        break;
                        //}
                        //labelType.Text = productType;
                        //labelVersion.Text = productVersion + "(" + buildVersion + ")";
                        //labelBackTime.Text = lbd;

                        string dbsql = "select fileid,domain,relativepath,flags from files";
                        DataTable bakupfileDT = SQLiteCore.SQLiteHelper.ExecuteCleanQuery(ios10dbFile, dbsql);
                        string baktmppath = Path.Combine(tempDataPath, timestamp);//DateTime.Now.ToString("yyyyMMddHHmmss"));
                        if (!Directory.Exists(baktmppath)) Directory.CreateDirectory(baktmppath);
                        string basePath = Path.Combine(baktmppath, "var");
                        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                        foreach (DataRow dr in bakupfileDT.Rows)
                        {
                            int flags = int.Parse(dr["flags"].ToString());
                            string fileid = dr["fileid"].ToString();
                            string domain = dr["domain"].ToString();
                            string relativepath = dr["relativepath"].ToString();
                            string tmpPath = "";
                            try
                            {
                                switch (domain)
                                {
                                    case "ManagedPreferencesDomain":
                                    case "KeyboardDomain":
                                    case "DatabaseDomain":
                                        tmpPath = basePath;

                                        break;
                                    case "KeychainDomain":
                                        tmpPath = Path.Combine(basePath, "Keychains");

                                        break;
                                    case "HealthDomain":
                                        tmpPath = Path.Combine(basePath, "Mobile");

                                        break;
                                    case "HomeKitDomain":
                                    case "HomeDomain":
                                    case "CameraRollDomain":
                                    case "MediaDomain":
                                    case "MobileDeviceDomain":
                                        tmpPath = Path.Combine(basePath, "Mobile");

                                        break;
                                    case "RootDomain":
                                        tmpPath = Path.Combine(basePath, "root");

                                        break;
                                    case "SystemPreferencesDomain":
                                        tmpPath = Path.Combine(basePath, "Preferences");

                                        break;
                                    case "WirelessDomain":
                                        tmpPath = Path.Combine(basePath, "wireless");

                                        break;
                                    default:
                                        tmpPath = Path.Combine(basePath, "Mobile");
                                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                                        tmpPath = Path.Combine(tmpPath, "Applications");
                                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                                        if (domain.Contains("SysContainerDomain-"))
                                        {
                                            tmpPath = Path.Combine(tmpPath, domain.Substring("SysContainerDomain-".Length));
                                        }
                                        else if (domain.Contains("SysSharedContainerDomain-"))
                                        {
                                            tmpPath = Path.Combine(tmpPath, domain.Substring("SysSharedContainerDomain-".Length));
                                        }
                                        else if (domain.Contains("AppDomain-"))
                                        {
                                            tmpPath = Path.Combine(tmpPath, domain.Substring("AppDomain-".Length));
                                        }
                                        else if (domain.Contains("AppDomainGroup-"))
                                        {
                                            tmpPath = Path.Combine(tmpPath, domain.Substring("AppDomainGroup-".Length));
                                        }
                                        else if (domain.Contains("AppDomainPlugin-"))
                                        {
                                            tmpPath = Path.Combine(tmpPath, domain.Substring("AppDomainPlugin-".Length));
                                        }
                                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

                                        break;
                                }

                                if (flags == 2)
                                {
                                    if (relativepath.Contains("/"))
                                    {

                                        string[] s = relativepath.Split('/');
                                        for (int i = 0; i < s.Length; i++)
                                        {
                                            tmpPath = Path.Combine(tmpPath, s[i]);
                                            if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                                        }
                                    }
                                    else
                                    {
                                        tmpPath = Path.Combine(tmpPath, relativepath);
                                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

                                    }
                                }
                                else if (flags == 4 || flags == 1)
                                {
                                    string filePath = Path.Combine(backup.path, fileid.Substring(0, 2), fileid);
                                    if (File.Exists(filePath))
                                    {
                                        if (relativepath.Contains("/"))
                                        {
                                            string[] s = relativepath.Split('/');
                                            for (int i = 0; i < s.Length - 1; i++)
                                            {
                                                tmpPath = Path.Combine(tmpPath, s[i]);
                                                if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                                            }
                                            File.Copy(filePath, Path.Combine(tmpPath, s[s.Length - 1]), true);
                                        }
                                        else
                                        {
                                            tmpPath = Path.Combine(tmpPath, relativepath);
                                            File.Copy(filePath, tmpPath, true);
                                        }
                                    }
                                }


                            }
                            catch (Exception exx)
                            {
                                Console.WriteLine(exx.Message);
                                Console.WriteLine(relativepath);
                            }
                        }

                        setProgress(null, null, 50);

                        if (setPrgressPrepared(100) == false)
                        {
                            return;
                        }
                        setProgress("正在建立数据目录树，请稍候...");

                        //unbackPath = baktmppath;
                        //TreeNode rootNode = new TreeNode();
                        //if (Directory.Exists(unbackPath))
                        //{
                        //    rootNode = new TreeNode();
                        //    rootNode.Text = deviceName;
                        //    BuildTree(unbackPath, rootNode);
                        //}
                        //addTreeNode addNode = delegate ()
                        //{
                        //    if (Directory.Exists(unbackPath))
                        //    {
                        //        treeView1.Nodes.Add(rootNode);
                        //    }
                        //};
                        //treeView1.Invoke(addNode);
                        //conToolBtn.Enabled = true;

                        AnalysisProc();


                        // 提取成功
                        doFinishExtract();
                    }
                }
            }
        }

        /// <summary>
        /// 安卓提取
        /// </summary>
        private void DoAndroidExtract()
        {
            // 安卓提取初始化
            AExtractHelp = new RyLib.AndroidExtractHelp();

            AExtractHelp.FinishEvent += new RyLib.AndroidExtractHelp.FinishHandle(AndroidExtractOK);
            AExtractHelp.InfoEvent += new RyLib.AndroidExtractHelp.InfoHandle(AndroidExtractInfo);
            AExtractHelp.UpdateProgressBarEvent += (i) =>
            {
                // 进度更新
                setProgress(null, null, i);
                if (setPrgressPrepared(i + 10) == false)
                {
                    stopAndroidExtract();
                }
            };
            AExtractHelp.evidenceDataPath = CommonUtil.Rulename.GetEvRawFolder();
            AExtractHelp.evidenceXmlPath = CommonUtil.Rulename.GetEvDataFolder();

            ////AExtractHelp.appInfoPath = config.appInfoFilePath;
            AExtractHelp.rootPath = tempDataPath;
            AExtractHelp.evidenceDataPath = CommonUtil.Rulename.GetEvRawFolder();

            //
            // 逻辑提取
            //
            AExtractHelp.StartExtract(new string[] { "SMS", "CALLLOG", "CONTACT", "APP", "DEV" });

            if (setPrgressPrepared(5) == false)
            {
                stopAndroidExtract();
            }
        }

        /// <summary>
        /// 停止安卓提取
        /// </summary>
        private void stopAndroidExtract()
        {
            AExtractHelp.StopExtract();
        }

        /// <summary>
        /// 提取完成后处理
        /// </summary>
        private void doFinishExtract()
        {
            setProgress("提取完成", null, 100);

            if (this.Type == DeviceType.Apple)
            {
                addSystemLog("分析数据文件结束，提取全部完成。", "结束");
            }

            this.IsAnayseAvailable = true;
        }

        private void parseAll92(IDictionary mbdb)
        {
            var sd = mbdb["Applications"] as Dictionary<object, object>;
            if (sd == null)
                return;

            var filesByDomain = new Dictionary<string, appFiles>();

            for (int i = 0; i < files92.Count; ++i)
            {
                if ((files92[i].Mode & 0xF000) == 0x8000)
                {
                    string d = files92[i].Domain;
                    if (!filesByDomain.ContainsKey(d))
                        filesByDomain.Add(d, new appFiles());

                    filesByDomain[d].add(i, files92[i].FileLength);
                }
            }


            foreach (var p in sd)
            {
                iPhoneApp app = new iPhoneApp();

                app.Key = p.Key as string;

                var zz = p.Value as IDictionary;

                //app.DisplayName = zz["CFBundleDisplayName"] as string;
                //app.Name = zz["CFBundleName"] as string;
                app.Identifier = zz["CFBundleIdentifier"] as string;
                app.Container = zz["Path"] as string;

                // il y a des applis mal paramétrées...
                //if (app.Name == null) app.Name = app.Key;
                //if (app.DisplayName == null) app.DisplayName = app.Name;

                if (filesByDomain.ContainsKey("AppDomain-" + app.Key))
                {
                    app.Files = new List<String>();

                    foreach (int i in filesByDomain["AppDomain-" + app.Key].indexes)
                    {
                        app.Files.Add(i.ToString());
                    }
                    app.FilesLength = filesByDomain["AppDomain-" + app.Key].FilesLength;

                    filesByDomain.Remove("AppDomain-" + app.Key);
                }

                addApp(app);
            }


            {
                iPhoneApp system = new iPhoneApp();

                system.Key = "---";
                //system.Name = "System";
                //system.DisplayName = "---";
                system.Identifier = "---";
                system.Container = "---";
                system.Files = new List<String>();

                foreach (appFiles i in filesByDomain.Values)
                {
                    foreach (int j in i.indexes)
                    {
                        system.Files.Add(j.ToString());
                    }
                    system.FilesLength = i.FilesLength;
                }


                addApp(system);

                /*
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = system;
                lvi.Text = system.DisplayName;
                lvi.SubItems.Add(system.Name);
                lvi.SubItems.Add(system.Files != null ? system.Files.Count.ToString("N0") : "N/A");
                lvi.SubItems.Add(system.FilesLength.ToString("N0"));
                lvi.SubItems.Add("---");

                lvi.SubItems[2].Tag = (long)(system.Files != null ? system.Files.Count : 0);
                lvi.SubItems[3].Tag = (long)system.FilesLength;
                lvi.SubItems[4].Tag = (long)0;

                listView1.Items.Add(lvi);
                */
            }
        }

        private class appFiles
        {
            public List<int> indexes = new List<int>();
            public long FilesLength = 0;

            public void add(int index, long length)
            {
                indexes.Add(index);
                FilesLength += length;
            }
        }

        private iPhoneApps appsCatalog;

        private void addApp(iPhoneApp app)
        {
            int ncount = 0;
            iPhoneIPA ipa = null;


            if (appsCatalog != null)
            {
                appsCatalog.TryGetValue(app.Identifier, out ipa);
            }

            //ListViewItem lvi = new ListViewItem();
            //lvi.Tag = app;
            //lvi.Text = app.Key;
            //lvi.SubItems.Add(ipa != null ? ipa.itemName : app.Key);
            //lvi.SubItems.Add(app.Files != null ? app.Files.Count.ToString("N0") : "N/A");
            //lvi.SubItems.Add(app.FilesLength.ToString("N0"));
            //lvi.SubItems.Add(ipa != null ? ipa.totalSize.ToString("N0") : "");

            //lvi.SubItems[2].Tag = (long)(app.Files != null ? app.Files.Count : 0);
            //lvi.SubItems[3].Tag = (long)app.FilesLength;
            //lvi.SubItems[4].Tag = (long)(ipa != null ? ipa.totalSize : 0);

            //listView1.Items.Add(lvi);

            ////if (app.Files != null)
            ////{
            ////    Console.WriteLine(app.Key + " " + app.Files.Count.ToString("N0") + " " + app.FilesLength.ToString("N0"));
            ////}
            ////else
            ////{
            ////    Console.WriteLine(app.Key + " " + "N/A" + " " + app.FilesLength.ToString("N0"));
            ////}

            if (app.Files != null)
            {
                foreach (string f in app.Files)
                {
                    try
                    {
                        iPhoneFile ff;

                        ff = new iPhoneFile();

                        mbdb.MBFileRecord x = files92[Int32.Parse(f)];

                        ff.Key = x.key;
                        ff.Domain = x.Domain;
                        ff.Path = x.Path;
                        ff.ModificationTime = x.aTime;
                        ff.FileLength = x.FileLength;
                        ncount++;
                        //Console.WriteLine(ncount.ToString()+" "+ff.Path + " " + ff.FileLength.ToString("N0") + " " + ff.ModificationTime.ToString() + " " + ff.Domain + " " + ff.Key);
                        //if (SerialNumber != "")
                        //{
                        //string tmpPath = @"c:\temp";
                        string tmpPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "temp");
                        if (Directory.Exists(tempDataPath)) tmpPath = tempDataPath;
                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                        //tmpPath = Path.Combine(tmpPath, "AppleForensic");
                        //if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                        //tmpPath = Path.Combine(tmpPath, "_unback_");
                        //if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                        tmpPath = Path.Combine(tmpPath, SerialNumber + "_" + timestamp);
                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                        tmpPath = Path.Combine(tmpPath, "var");
                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                        switch (ff.Domain)
                        {
                            case "RootDomain":
                                tmpPath = Path.Combine(tmpPath, "root");
                                break;
                            case "CameraRollDomain":
                            case "HomeDomain":
                            case "MediaDomain":
                                tmpPath = Path.Combine(tmpPath, "mobile");
                                break;
                            case "KeychainDomain":
                                tmpPath = Path.Combine(tmpPath, "Keychains");
                                break;
                            case "SystemPreferencesDomain":
                                tmpPath = Path.Combine(tmpPath, "preferences");
                                break;
                            case "WirelessDomain":
                                tmpPath = Path.Combine(tmpPath, "wireless");
                                break;
                            default:
                                if (ff.Domain.Contains("AppDomain-"))
                                {
                                    tmpPath = Path.Combine(tmpPath, "mobile");
                                    if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

                                    tmpPath = Path.Combine(tmpPath, "Applications");
                                    if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

                                    string[] appPathTemp = ff.Domain.Split('-');
                                    if (appPathTemp.Length == 2) tmpPath = Path.Combine(tmpPath, appPathTemp[1]);
                                }
                                break;
                        }
                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                        string[] pathArray = ff.Path.Split('/');
                        for (int i = 0; i < pathArray.Length - 1; i++)
                        {
                            tmpPath = Path.Combine(tmpPath, pathArray[i]);
                            if (!Directory.Exists(tmpPath) && tmpPath.Length < 248) Directory.CreateDirectory(tmpPath);
                        }
                        string oriFile = Path.Combine(backup.path, ff.Key);
                        string targetFile = Path.Combine(tmpPath, pathArray[pathArray.Length - 1].Replace(":", " ").Replace("<", " ").Replace(">", " ").Replace("*", " ").Replace("?", " ").Replace("\"", " ").Replace("|", " ").Replace("/", " ").Replace("\\", " "));
                        if (targetFile.Length < 260)
                        {
                            if (!File.Exists(targetFile))
                            {
                                if (File.Exists(oriFile) && oriFile.Length < 260) File.Copy(oriFile, targetFile);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        saveErrorLog(ex.Message);
                        saveErrorLog(ex.HelpLink);
                        saveErrorLog(ex.StackTrace);
                        saveErrorLog(ex.TargetSite.ToString());
                        continue;
                    }
                }
            }
        }

        private void AnalysisProc()
        {
            try
            {
                //setProgress("正在分析数据，请稍候...");
                //addSystemLog("分析数据......", "开始");

                //string deviceSN = strUUID;
                ////deviceSN = "IOS";
                //string date = gs_itunes_decpath;//20160922 combine the logical evidence folder with decode ituntes folder // DateTime.Now.ToString("yyyyMMddHHmmss");
                ////string tempPath = Path.GetTempPath();
                //string tempPath = CommonUtil.CaseDataPath;//+ "Case";//Application.StartupPath + "\\Case";
                //if (Directory.Exists(caseSavePath)) tempPath = caseSavePath;
                //caseSavePath = tempPath;
                ////string tempPath = @"c:\temp";
                ////if (!Directory.Exists(tempPath))
                ////{
                ////    Directory.CreateDirectory(tempPath);
                ////}
                ////tempPath = Path.Combine(tempPath, "AICase");
                ////if (!Directory.Exists(tempPath))
                ////{
                ////    Directory.CreateDirectory(tempPath);
                ////}
                //string parentPath = string.Format("{0}_{1}", "IOS", date);
                //string deviceDir = Path.Combine(tempPath, parentPath);
                //string lan = "CHN";
                //if (isEnglish)
                //    lan = "ENG";

                ////------------------------20160304 add customer file list-------------------//
                //if (Directory.Exists(Path.Combine(Application.StartupPath, "Model", "Sql")))
                //{
                //    DirectoryInfo faDIR = new DirectoryInfo(Path.Combine(Application.StartupPath, "Model", "Sql"));
                //    foreach (FileInfo fi in faDIR.GetFiles())
                //    {
                //        if (fi.Name.ToLower().IndexOf(".sql") > 0)
                //        {
                //            string lstag = fi.FullName;
                //            string ls1 = OperatorIniFile.ReadIniData2("SQL", "APP", "", lstag);
                //            string ls2 = OperatorIniFile.ReadIniData2("SQL", "DB", "", lstag);
                //            string ls3 = OperatorIniFile.ReadIniData2("SQL", "APPEn", "", lstag);
                //            string ls4 = OperatorIniFile.ReadIniData2("SQL", "Path", "", lstag);
                //            if (OperatorIniFile.ReadIniData2("SQL", "OS", "", lstag) == "iOS")
                //            {
                //                if (extractFilepathCus.Contains(ls2.ToLower()))
                //                { }
                //                else
                //                {
                //                    extractFilepathCus.Add(ls2.ToLower(), ls1 + "|" + ls2 + "|" + ls3 + "|" + ls4);
                //                }
                //            }
                //        }
                //        if (fi.Name.ToLower().IndexOf(".txt") > 0)
                //        {
                //            string lstag = fi.FullName;
                //            string ls1 = OperatorIniFile.ReadIniData2("File", "APP", "", lstag);
                //            string ls2 = OperatorIniFile.ReadIniData2("File", "File", "", lstag);
                //            string ls3 = OperatorIniFile.ReadIniData2("File", "APPEn", "", lstag);
                //            string ls4 = OperatorIniFile.ReadIniData2("File", "Path", "", lstag);
                //            if (OperatorIniFile.ReadIniData2("File", "OS", "", lstag) == "iOS")
                //            {
                //                if (extractFilepathCus.Contains(ls2.ToLower()))
                //                { }
                //                else
                //                {
                //                    extractFilepathCus.Add(ls2.ToLower(), ls1 + "|" + ls2 + "|" + ls3 + "|" + ls4);
                //                }
                //            }
                //        }
                //    }
                //}
                ////------------------------20160304 add customer file list-------------------//

                //ExtractFileList files = new ExtractFileList(parentPath, deviceSN, date, "", labelIMEI.Text, "Apple", lan);
                //if (!Directory.Exists(deviceDir))
                //{
                //    Directory.CreateDirectory(deviceDir);
                //}

                ////toolStripProgressBar1.Maximum = extractDIRlist.Count + extractFilelist.Count;
                ////toolStripProgressBar1.Value = 0;
                ////toolStripProgressBar1.Step = 1;
                ////bool lbmedia = false;
                //toolStripProgressBar1.Maximum = extractDIRlist.Count;
                //toolStripProgressBar1.Value = 0;
                //toolStripProgressBar1.Step = 1;
                //foreach (DictionaryEntry de in extractDIRlist)
                //{
                //    toolStripProgressBar1.Value += toolStripProgressBar1.Step;
                //    if (de.Value != null)
                //    {
                //        string[] tmp = extractDIRpath[de.Key].ToString().Split('|');
                //        string localDir = Path.Combine(deviceDir, tmp[0]);
                //        if (!Directory.Exists(localDir))
                //        {
                //            Directory.CreateDirectory(localDir);

                //        }
                //        localDir = Path.Combine(localDir, tmp[1]);
                //        if (!Directory.Exists(localDir))
                //        {
                //            Directory.CreateDirectory(localDir);

                //        }
                //        localDir = Path.Combine(localDir, tmp[2]);
                //        if (!Directory.Exists(localDir))
                //        {
                //            Directory.CreateDirectory(localDir);

                //        }
                //        string dataPath = Path.Combine(unbackPath, de.Value.ToString());

                //        //-----------20160801 add for the import not from pack root path ,from the application path----------//
                //        if (!Directory.Exists(dataPath))
                //        {
                //            if (de.Value.ToString().ToLower().IndexOf("applications") > 0)
                //            {
                //                string lsapp = de.Value.ToString().ToLower().Substring(de.Value.ToString().ToLower().IndexOf("applications") + 13, de.Value.ToString().Length - de.Value.ToString().ToLower().IndexOf("applications") - 13);
                //                if (unbackPath.IndexOf(lsapp) > 0)
                //                {
                //                    dataPath = unbackPath;
                //                }
                //            }
                //        }
                //        //---------------------------------------------------------------------------------------------------//
                //        //saveErrorLog(dataPath);
                //        if (Directory.Exists(dataPath))
                //        {
                //            if (de.Key.ToString() == "Photoes")
                //            {
                //                DirectoryInfo di = new DirectoryInfo(dataPath);
                //                di.MoveTo(Path.Combine(localDir, "DCIM"));
                //                //lbmedia = true;
                //            }
                //            else if (de.Key.ToString() == "Video")
                //            {   //20160729 addd for video not copy any folder use the photoes folder
                //                //DirectoryInfo di = new DirectoryInfo(dataPath);
                //                //di.MoveTo(Path.Combine(localDir, "DCIM"));
                //                //lbmedia = true;
                //            }
                //            else
                //            {
                //                if (de.Key.ToString() == "com.tencent.xin")
                //                {
                //                    if (tencentxinPath.Count > 0)
                //                    {
                //                        foreach (DictionaryEntry xinde in tencentxinPath)
                //                        {
                //                            if (Directory.Exists(xinde.Key.ToString()))
                //                            {
                //                                CopyFolder(xinde.Key.ToString(), localDir);
                //                            }
                //                        }
                //                    }
                //                }
                //                else
                //                {
                //                    CopyFolder(dataPath, localDir);
                //                }
                //            }

                //            // CopyFolder(dataPath, localDir);
                //            files.Add(tmp[0], tmp[1], tmp[2], "Directory", tmp[0] + "\\" + tmp[1] + "\\" + tmp[2]);
                //            switch (de.Key.ToString())
                //            {
                //                case "SMS":
                //                    //eButtonSMS.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("SMS^SMS SMS^-");
                //                        PrintfLogInfo("Found SMS", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("SMS^短信 SMS^-");
                //                        PrintfLogInfo("分析数据 SMS", "完成");
                //                    }
                //                    break;
                //                case "com.tencent.mqq":
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("QQ^MobileQQ com.tencent.mqq^-");
                //                        PrintfLogInfo("Found MobileQQ", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("QQ^手机QQ com.tencent.mqq^-");
                //                        PrintfLogInfo("分析数据 手机QQ", "完成");
                //                    }
                //                    break;
                //                case "com.tencent.mipadqq":
                //                    //eButtonMQQ.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("QQ^MobileQQ com.tencent.mipadqq^-");
                //                        PrintfLogInfo("Found MobileQQ", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("QQ^手机QQ com.tencent.mipadqq^-");
                //                        PrintfLogInfo("分析数据 手机QQ", "完成");
                //                    }
                //                    break;
                //                case "com.sina.weibo":
                //                    //eButtonWeibo.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("Weibo^Weibo com.sina.weibo^-");
                //                        PrintfLogInfo("Found Weibo", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("Weibo^新浪微博 com.sina.weibo^-");
                //                        PrintfLogInfo("分析数据 新浪微博", "完成");
                //                    }
                //                    break;
                //                case "com.tencent.xin":
                //                    //eButtonMM.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("Wechat^Wechat com.tencent.xin^-");
                //                        PrintfLogInfo("Found Wechat", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("Wechat^腾讯微信 com.tencent.xin^-");
                //                        PrintfLogInfo("分析数据 腾讯微信", "完成");
                //                    }
                //                    break;
                //                case "com.skype.tomskype":
                //                    //eButtonSkype.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("Skype^Skype com.skype.tomskype^-");
                //                        PrintfLogInfo("Found Skype", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("Skype^Skype com.skype.tomskype^-");
                //                        PrintfLogInfo("分析数据 Skype", "完成");
                //                    }
                //                    break;
                //                case "AddressBook":
                //                    //eButtonPhoneBook.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("Contacts^Contacts AddressBook^-");
                //                        PrintfLogInfo("Found Contacts", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("Contacts^电话簿 AddressBook^-");
                //                        PrintfLogInfo("分析数据 电话簿", "完成");
                //                    }
                //                    break;
                //                case "CallHistory":
                //                    //eButtonCallLogs.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("Callog^Callogs CallHistory^-");
                //                        PrintfLogInfo("Found Callogs", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("Callog^通话记录 CallHistory^-");
                //                        PrintfLogInfo("分析数据 通话记录", "完成");
                //                    }
                //                    break;
                //                case "com.wemomo.momoappdemo1":
                //                    //eButtonimomo.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("imomo^imomo com.wemomo.momoappdemo1^-");
                //                        PrintfLogInfo("Found imomo", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("imomo^陌陌 com.wemomo.momoappdemo1^-");
                //                        PrintfLogInfo("分析数据 陌陌", "完成");
                //                    }
                //                    break;
                //                case "com.laiwang.phoneclient":
                //                    //eButtonLaiwang.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("alww^alww com.laiwang.phoneclient^-");
                //                        PrintfLogInfo("Found alww", "Success");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("alww^阿里旺旺 com.laiwang.phoneclient^-");
                //                        PrintfLogInfo("分析数据 阿里旺旺", "完成");
                //                    }
                //                    break;
                //            }
                //        }
                //        //else if(lbmedia && (de.Key.ToString() == "Video"))
                //        //{
                //        //    files.Add(tmp[0], tmp[1], tmp[2], "Directory", tmp[0] + "\\" + tmp[1] + "\\" + tmp[2]);
                //        //}
                //        //else if (lbmedia && (de.Key.ToString() == "Audio"))
                //        //{
                //        //    files.Add(tmp[0], tmp[1], tmp[2], "Directory", tmp[0] + "\\" + tmp[1] + "\\" + tmp[2]);
                //        //}
                //        if (de.Key.ToString().ToLower().Contains("skype"))
                //        {
                //            string skype2Path = Path.Combine(unbackPath, "var/mobile/Applications/com.skype.skype");
                //            if (Directory.Exists(skype2Path))
                //            {
                //                CopyFolder(skype2Path, localDir);
                //            }
                //        }
                //    }

                //}
                //toolStripProgressBar1.Maximum = extractFilelist.Count;
                //toolStripProgressBar1.Value = 0;
                //toolStripProgressBar1.Step = 1;
                //foreach (DictionaryEntry de in extractFilelist)
                //{
                //    toolStripProgressBar1.Value += toolStripProgressBar1.Step;
                //    if (de.Value != null)
                //    {
                //        string[] tmp = extractFilepath[de.Key].ToString().Split('|');
                //        string localDir = Path.Combine(deviceDir, tmp[0]);
                //        if (!Directory.Exists(localDir))
                //        {
                //            Directory.CreateDirectory(localDir);

                //        }
                //        localDir = Path.Combine(localDir, tmp[1]);
                //        if (!Directory.Exists(localDir))
                //        {
                //            Directory.CreateDirectory(localDir);

                //        }
                //        localDir = Path.Combine(localDir, tmp[2]);
                //        if (!Directory.Exists(localDir))
                //        {
                //            Directory.CreateDirectory(localDir);

                //        }
                //        string dataPath = Path.Combine(unbackPath, de.Value.ToString());
                //        //saveErrorLog(dataPath);
                //        string relationPath = Path.Combine(localDir, de.Key.ToString());
                //        if (File.Exists(dataPath))
                //        {
                //            File.Copy(dataPath, relationPath);
                //            files.Add(tmp[0], tmp[1], tmp[2], "File", tmp[0] + "\\" + tmp[1] + "\\" + tmp[2] + "\\" + de.Key.ToString());
                //            switch (de.Key.ToString())
                //            {
                //                case "call_history.db":
                //                    //eButtonCallLogs.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("Callog^Callog CallHistory^-");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("Callog^通话记录 CallHistory^-");
                //                    }
                //                    break;
                //                case "AddressBook.sqlitedb":
                //                    //eButtonPhoneBook.Enabled = true;
                //                    if (isEnglish)
                //                    {
                //                        InsertAPPView("Contacts^Contacts AddressBook^-");
                //                    }
                //                    else
                //                    {
                //                        InsertAPPView("Contacts^电话簿 AddressBook^-");
                //                    }
                //                    break;
                //            }
                //        }
                //    }

                //}
                //toolStripProgressBar1.Maximum = extractFilepathCus.Count;
                //toolStripProgressBar1.Value = 0;
                //toolStripProgressBar1.Step = 1;
                //foreach (DictionaryEntry de in extractFilepathCus) //201606  add for customer find files in accuate folder files
                //{
                //    toolStripProgressBar1.Value += toolStripProgressBar1.Step;
                //    string[] tmp = extractFilepathCus[de.Key].ToString().Split('|');



                //    string relationPath = tmp[3].ToString();
                //    if (relationPath.ToLower().IndexOf("var\\") == 0)
                //    { }
                //    else if (relationPath.ToLower().IndexOf("mobile\\") == 0)
                //    {
                //        relationPath = Path.Combine("var", relationPath);
                //    }
                //    else if (relationPath.ToLower().IndexOf("applications\\") == 0)
                //    {
                //        relationPath = Path.Combine("var", "mobile", relationPath);
                //    }
                //    else
                //    {
                //        relationPath = Path.Combine("var", "mobile", "applications", relationPath);
                //    }

                //    relationPath = Path.Combine(unbackPath, relationPath, de.Key.ToString());  //combine the path


                //    string packagePath = ""; //add for the application xxx.xxx. folder
                //    if (relationPath.ToLower().IndexOf("\\applications\\") > 0)
                //    {
                //        packagePath = relationPath.Substring(relationPath.ToLower().IndexOf("\\applications\\") + 14, relationPath.Length - relationPath.ToLower().IndexOf("\\applications\\") - 14);
                //        if (packagePath.IndexOf("\\") > 0)
                //            packagePath = packagePath.Substring(0, packagePath.IndexOf("\\"));
                //    }
                //    string dataPath = Path.Combine(deviceDir, "CusAPP", packagePath, de.Key.ToString());
                //    if (!Directory.Exists(Path.Combine(deviceDir, "CusAPP")))
                //    {
                //        Directory.CreateDirectory(Path.Combine(deviceDir, "CusAPP"));

                //    }
                //    if (!Directory.Exists(Path.Combine(deviceDir, "CusAPP", packagePath)))
                //    {
                //        Directory.CreateDirectory(Path.Combine(deviceDir, "CusAPP", packagePath));

                //    }
                //    //if (extractFilepathCus.Contains(fi.Name.ToString().ToLower()))
                //    if (File.Exists(relationPath))
                //    {

                //        File.Copy(relationPath, dataPath);
                //        if (File.Exists(Path.Combine(deviceDir, "CusAPP", packagePath, de.Key.ToString())))
                //        {
                //            if (!isEnglish)
                //            {
                //                //analysisFiles.Add(tmp[0], "all", tmp[3].ToLower(), "File", lslogxml + "\\" + "CusAPP" + "\\" + tmp[3] + "\\" + de.Key.ToString());  //app name chinese
                //                files.Add(tmp[0], "all", packagePath, "File", Path.Combine(packagePath, de.Key.ToString()));
                //            }
                //            else
                //            {
                //                //analysisFiles.Add(tmp[1], "all", tmp[3].ToLower(), "File", lslogxml + "\\" + "CusAPP" + "\\" + tmp[3] + "\\" + de.Key.ToString());  //app name english
                //                files.Add(tmp[1], "all", packagePath, "File", Path.Combine(packagePath, de.Key.ToString()));
                //            }

                //        }
                //        if (isEnglish)
                //        {
                //            PrintfLogInfo("CusAPP" + "\\" + tmp[2] + "\\" + de.Key.ToString(), "OK");
                //        }
                //        else
                //        {
                //            PrintfLogInfo("CusAPP" + "\\" + tmp[1] + "\\" + de.Key.ToString(), "OK");
                //        }
                //    }
                //}

                //files.CheckFileExist();
                //files.AddAppleInfo(deviceName, phoneNumber, imei, iccid, productType, productVersion + "(" + buildVersion + ")", SerialNumber, lbd, uniqueChipID, wifiAddress, blueAddress);
                ////Console.WriteLine();
                //files.Save();
                //applexmlPath = files.GetXMLPath();


            }
            catch (Exception ex)
            {
                saveErrorLog(ex.Message);
                saveErrorLog(ex.HelpLink);
                saveErrorLog(ex.StackTrace);
                saveErrorLog(ex.TargetSite.ToString());
            }

        }

        private void showFailed()
        {
            progressVM.Desc = "备份失败";
            progressVM.Percent = 0;

            addSystemLog("数据备份失败：请检查数据线和itunes是否能够正常联接。", "报错");
        }

        /// <summary>
        /// 跳转到分析
        /// </summary>
        private void DoAnalyse()
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                // 打开添加案件窗口
                MainWindow viewMain = (MainWindow)Globals.Instance.MainVM.View;
                viewMain.openAddEvidence(savePath);
            }));
        }

        /// <summary>
        /// 停止
        /// </summary>
        private void PauseExtract()
        {
            mnProgress = -1;
            addSystemLog("提交了停止要求。", "正在处理");
        }

        /// <summary>
        /// 重新提取
        /// </summary>
        private void RestartExtract()
        {
            if (this.Type == DeviceType.Android)
            {
                // 打开提取方式
                MainWindow viewMain = (MainWindow)Globals.Instance.MainVM.View;
                viewMain.openExtractType();
            }
            else if (this.Type == DeviceType.Apple)
            {
                startExtract(this.Type);
            }
        }
    }
}
