using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Setting
{
    public class EnvironmentItem
    {
        public enum StatusEnum
        {
            INSTALLING,
            INSTALLED,
            NOTINSTALLED
        };

        public StatusEnum Status { get; set; } = StatusEnum.NOTINSTALLED;

        public EnvironmentItem(string name)
        {
            Name = name;
        }

        public String Name { get; set; }

        public String StatusDescription {
            get
            {
                if (Status == StatusEnum.INSTALLED)
                {
                    return "已安装";
                }
                else if (Status == StatusEnum.INSTALLING)
                {
                    return "正在安装";
                }
                else
                {
                    return "未安装";
                }
            }

            set { }
        }
    }
}
