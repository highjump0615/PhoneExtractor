using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Forensics.ViewModel.MainHomeViewModel;

namespace Forensics.ViewModel.Android
{
    class ExtractProgressViewModel : ViewModelBase
    {
        public GridLength PercentWidth { get; set; }
        public GridLength PercentLeftWidth { get; set; }

        private int _percent;
        public int Percent
        {
            get
            {
                return _percent;
            }
            set
            {
                PercentWidth = new GridLength(value, GridUnitType.Star);
                PercentLeftWidth = new GridLength(100 - value, GridUnitType.Star);

                _percent = value;
            }
        }

        public override Pages PageIndex => throw new NotImplementedException();

        public string Title { get; set; }
        public string Desc { get; set; }

        public ExtractProgressViewModel()
        {
            Title = "提取安卓设备";

            this.Percent = 0;
        }

        public void startExtract(ExtractType type)
        {
            if (type == ExtractType.Apple)
            {
                Title = "提取苹果设备";
            }
        }
    }
}
