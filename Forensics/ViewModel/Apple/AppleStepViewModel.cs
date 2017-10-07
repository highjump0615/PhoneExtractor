using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    public class AppleStepViewModel : ViewModelBase
    {
        public override Pages PageIndex
        {
            get { return Pages.Other; }
        }

        public String Desc { get; set; }
        public String ImageSrc { get; set; }
        public bool ShowSavePath { get; set; }

        public AppleStepViewModel()
        {
            ImageSrc = "/Resources/Images/extract/ext_apple_img.png";
            ShowSavePath = false;
        }
    }
}
