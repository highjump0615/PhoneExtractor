using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Extract
{
    public class ActExtractType
    {
        public string Desc { get; set; }
        public bool IsSelected { get; set; } = true;

        public ActExtractType(string Desc)
        {
            this.Desc = Desc;
        }
    }
}
