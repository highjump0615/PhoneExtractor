using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Extract
{
    class SystemLog
    {
        // 时间
        public DateTime Date { get; set; }

        // 项目
        public String Item { get; set; }

        // 结果
        public String Result { get; set; }

        public SystemLog()
        {
            Date = DateTime.Now;
            Item = "正在检测连接状态";
            Result = "成功";
        }
    }
}
