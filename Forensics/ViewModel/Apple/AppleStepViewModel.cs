using Forensics.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    public class AppleStepViewModel : BaseStepViewModel
    {
        public bool ShowSavePath { get; set; }

        // 默认是文件选择
        public int FileOpenType { get; set; } = 0;

        public AppleStepViewModel()
        {
            ImageSrc = "/Resources/Images/extract/ext_apple_img.png";
            ShowSavePath = false;
        }
    }
}
