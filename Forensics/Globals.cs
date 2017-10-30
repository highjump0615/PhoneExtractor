using Forensics.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics
{
    public class Globals
    {
        private static Globals _Instance;
        public static Globals Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Globals();

                return _Instance;
            }
        }

        private Globals()
        {
        }

        public MainViewModel MainVM { get; set; }
    }
}
