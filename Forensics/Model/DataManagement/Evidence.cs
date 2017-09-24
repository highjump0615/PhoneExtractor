using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.DataManagement
{
    public class Evidence
    {
        public String Id { get; set; }

        public String Phone { get; set; }
        public String Name { get; set; }
        public String Gender { get; set; }
        public String Nation { get; set; }
        public String IdNumber { get; set; }

        public Evidence()
        {
            Id = "Case01";
            Phone = "15050505050";
            Name = "明天";
            Gender = "男";
            Nation = "汉";
            IdNumber = "1010108185746131";
        }
    }
}
