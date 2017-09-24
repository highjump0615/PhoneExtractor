using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.DataManagement
{
    public class ItemsForCase : ObservableCollection<Case>
    {
        public ItemsForCase()
        {
            Add(new Case());
            Add(new Case());
            Add(new Case());
            Add(new Case());
            Add(new Case());
        }
    }
}
