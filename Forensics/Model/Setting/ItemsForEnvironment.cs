using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Setting
{
    public class ItemsForEnvironment : ObservableCollection<EnvironmentItem>
    {
        public ItemsForEnvironment()
        {
            Add((new EnvironmentItem("Microsoft.NET Framework 4", EnvironmentItem.StatusEnum.INSTALLED)));
            Add((new EnvironmentItem("Microsoft Visual C++ 2010 x86 Redistributable", EnvironmentItem.StatusEnum.INSTALLED)));
            Add((new EnvironmentItem("EDEC 狼蛛安卓密码工具", EnvironmentItem.StatusEnum.INSTALLING)));
        }
    }
}
