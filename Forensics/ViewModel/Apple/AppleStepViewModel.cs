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

        public AppleStepViewModel()
        {
            ImageSrc = "/Resources/Images/extract/ext_apple_img.png";
            ShowSavePath = false;
        }
    }
}
