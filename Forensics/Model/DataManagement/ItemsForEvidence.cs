using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.DataManagement
{
    public class ItemsForEvidence : ObservableCollection<Evidence>
    {
        public ItemsForEvidence()
        {
            Add(new Evidence());
            Add(new Evidence());
        }
    }
}
