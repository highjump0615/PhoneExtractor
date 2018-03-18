using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.DataManagement
{
    public class Evidence2 : Evidence
    {
        public bool IsSelected { get; set; }

        public string DateText
        {
            get
            {
                return this.ADDTIME.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 所属案件
        /// </summary>
        public Case CaseBelonged { get; set; }
    }
}
