using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.DataManagement
{
    public class Case
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public DateTime Date { get; set; }

        public int Evidences { get; set; }

        public String Path { get; set; }

        public String Desc { get; set; }

        public Case()
        {
            Id = "Case01";
            Name = "余华东环路";
            Date = DateTime.Now;
            Evidences = 17;
            Path = "C:\\Desktop\\Chris\\app";
        }
    }
}
