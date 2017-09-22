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
        };

        private StatusEnum Status;

        public EnvironmentItem(string name, StatusEnum status)
        {
            Name = name;
            Status = status;
        }

        public String Name { get; set; }

        public String StatusDescription {
            get
            {
                if (Status == StatusEnum.INSTALLED)
                {
                    return "已安装";
                }
                else
                {
                    return "正在安装";
                }
            }

            set { }
        }
    }
}
