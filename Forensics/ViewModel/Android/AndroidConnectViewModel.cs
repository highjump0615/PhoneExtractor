using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel.Android
{
    public class AndroidConnectViewModel : StepViewModel
    {
        private int _version;
        public int Version {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                initInstructionData();
            }
        }

        public AndroidConnectViewModel()
        {
            _version = 20;
            initInstructionData();
        }

        /// <summary>
        /// 初始化提示内容
        /// </summary>
        private void initInstructionData()
        {
            ClearChild();
            AndroidStepViewModel stepVM;

            if (Version == 20)
            {
                // 第一步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤一： \n点击手机 Menu键(菜单键)，在弹出的菜单中选择设置(Setting)。应用程序中找到设置程序，点击进入。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_2_1.png";
                this.AddChild(stepVM);

                // 第二步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤二：\n进入设置界面的 应用程序即可打开USB调试模式。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_2_2.png";
                this.AddChild(stepVM);
            }
            else if (Version == 40)
            {
                // 第一步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤一：\n点击手机 Menu键(菜单键)，在弹出的菜单中选择 设置(Setting)";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_40_1.png";
                this.AddChild(stepVM);

                // 第二步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤二：\n或在应用程序中找到 设置进入设置界面的开发人员选项即可打开 USB调试模式。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_40_2.png";
                this.AddChild(stepVM);
            }
            else if (Version == 42)
            {
                // 第一步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤一：\n点击手机 Menu键(菜单键)，在弹出的菜单中选择 设置(Setting),或在应用程序中找到 设置点击 关于手机。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_42_1.png";
                this.AddChild(stepVM);

                // 第二步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤二：\n连续点击七次 版本号再返回设置菜单界面选择开发者者选项。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_42_2.png";
                this.AddChild(stepVM);

                // 第三步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤三：\n点击打开右上角的 开关,即可打开 USB调试模式。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_42_3.png";
                this.AddChild(stepVM);
            }
            else if (Version == 50)
            {
                // 第一步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤一：\n打开手机，找到设置，进入“关于手机”。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_50_1.png";
                this.AddChild(stepVM);

                // 第二步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤二：\n连续点击4次“版本号 LRX22C”会提示开启USB调试模式；返回上一次，此时可以看到在关于手机的上面会多出一个选项“开发者选项”。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_50_2.png";
                this.AddChild(stepVM);

                // 第三步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤三：\n进入开发者选项，选“USB调试”即可。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_50_3.png";
                this.AddChild(stepVM);
            }
            else if (Version == 60)
            {
                // 第一步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤一：\n进入“设置”，“关于手机”找到“版本号”或“内核版本”；";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_60_1.png";
                this.AddChild(stepVM);

                // 第二步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤二：\n连续快速点击“版本号”或“内核版本”多次，就可看见“开发者选项”；进入开发者选项，可找“usb调试”，开启即可。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_60_2.png";
                this.AddChild(stepVM);
            }
            else if (Version == 70)
            {
                // 第一步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤一：\n打开手机以后，从手机桌面上找到【设置】，点击打开。进入后，一直下拉找到【关于手机】选项，点击进入。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_70_1.png";
                this.AddChild(stepVM);

                // 第二步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤二：\n在【关于手机】菜单中，找到版本号，连续点击3下，就会提示进入【开发者选项】。返回【设置】菜单，找到【其他高级设置】点击进入。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_70_2.png";
                this.AddChild(stepVM);

                // 第三步
                stepVM = new AndroidStepViewModel();
                stepVM.Desc = "步骤三：\n在【其他高级设置】中下拉即可看到【开发者选项】。进入【开发者选项】后，找到【USB调试】，选择【确定】，就可以打开USB调试顺利连接电脑了。";
                stepVM.ImageSrc = "/Resources/Images/android/and_connect_auto_70_3.png";
                this.AddChild(stepVM);
            }

            // 初始化
            initPages();
        }
    }
}
