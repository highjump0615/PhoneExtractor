using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Device
{
    public class PhoneInfo
    {
        public string BrandModelID { get; set; }
        public string BrandName { get; set; }
        public string BrandNameEn { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        private string _imagePath;
        public string ImagePath
        {
            get
            {
                return Directory.GetCurrentDirectory() + "\\Images\\phone\\" + this.BrandNameEn + "\\" + _imagePath;
            }
            set
            {
                _imagePath = value;
            }
        }
        

        public string ModelName { get; set; }
    }
}
