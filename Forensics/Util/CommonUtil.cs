using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Util
{
    public struct PhoneDevice               //current phone device info global 201506 
    {
        public string IMEI_string;
        public string ICCID_string;
        public string IMSI_string;
        public string Case_ai_file;
        public string Phone_number;
        public string Phone_os;
        public string Phone_brand;
        public string Phone_model;
        public string Phone_serial;
        public string btmac;//蓝牙mac
        public string wfmac;//wifi mac
    }

    public static class CommonUtil
    {
        public static Evidence currentEvidence;//current evidence add 201506
        public static PhoneDevice CurrentPD;   //current device info add 201506

        public static RuleName Rulename = new RuleName();  //20160323 add for name logic function su

        #region Upcasting
        public static TDerived ToDerived<TBase, TDerived>(TBase tBase) where TDerived : TBase, new()
        {
            TDerived tDerived = new TDerived();
            foreach (PropertyInfo propBase in typeof(TBase).GetProperties())
            {
                PropertyInfo propDerived = typeof(TDerived).GetProperty(propBase.Name);
                propDerived.SetValue(tDerived, propBase.GetValue(tBase, null), null);
            }
            return tDerived;
        }
        #endregion
    }
}
