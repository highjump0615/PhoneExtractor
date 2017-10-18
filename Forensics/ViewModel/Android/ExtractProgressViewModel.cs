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

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                PropertyChanging("Title");
            }
        }
        public string Desc { get; set; }

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

                PropertyChanging("Percent");
                PropertyChanging("PercentWidth");
                PropertyChanging("PercentLeftWidth");
            }
        }

        public override Pages PageIndex => throw new NotImplementedException();

        public ExtractProgressViewModel()
        {
            Desc = "提取安卓设备";

            this.Percent = 0;
        }

        public void startExtract(ExtractType type)
        {
            if (type == ExtractType.Apple)
            {
                Desc = "提取苹果设备";
            }
        }
    }
}
