using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel.Base
{
    public class BaseStepViewModel : ViewModelBase
    {
        public override Pages PageIndex
        {
            get { return Pages.Other; }
        }

        public String Desc { get; set; }
        public String ImageSrc { get; set; }

        public BaseStepViewModel()
        {
        }
    }
}
