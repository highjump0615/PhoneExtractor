using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Device
{
    public class BrandInfo
    {
        public string Name { get; set; }
        public string NameEn { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath
        {
            get
            {
                return "/Resources/Images/brand/" + this.NameEn + ".png";
            }
        }
    }
}
