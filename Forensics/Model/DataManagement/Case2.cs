using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forensics.Model;
using System.Reflection;

namespace Forensics.Model.DataManagement
{
    public class Case2 : Case
    {
        public string DateText
        {
            get
            {
                return this.ADDTIME.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
