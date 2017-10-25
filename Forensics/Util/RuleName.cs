using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Util
{
    public class RuleName
    {
        private void UpdateAppConfig(string newKey, string newValue)
        {
            bool isModified = false;
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == newKey)
                {
                    isModified = true;
                }
            }

            // Open App.Config of executable
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // You need to remove the old settings object before you can replace it
            if (isModified)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            // Add an Application Setting.
            config.AppSettings.Settings.Add(newKey, newValue);
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
        }
        private bool CheckDefault() //检查系统设置
        {
            string name = ConfigurationManager.AppSettings["RuleCustomer"];
            if (String.IsNullOrWhiteSpace(name))
            {
                return true;
            }
            else
            {
                if (name == "Yes")
                {
                    return false;
                }
                else if (name == "No")
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }
        public string GetCaseFolder(string lscasenumber)   //案件目录
        {
            if (CheckDefault())
            {

                return lscasenumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else
            {
                string name = ConfigurationManager.AppSettings["RuleCase"];
                string lsc = "";
                string lsconnection = "_";
                if (name.IndexOf("[") > -1 && name.IndexOf("]") > -1)
                {
                    lsc = name.Substring(name.IndexOf("[") + 1, name.IndexOf("]") - name.IndexOf("[") - 1);
                }
                if (name.IndexOf("(") > -1 && name.IndexOf(")") > -1)
                {
                    lsconnection = name.Substring(name.IndexOf("(") + 1, name.IndexOf(")") - name.IndexOf("(") - 1);
                }
                bool timeflag = true;
                string lstime = DateTime.Now.ToString("yyyyMMddHHmmss");

                if (name.IndexOf("时间戳") > -1)
                {
                    timeflag = true;
                }
                else
                {
                    timeflag = false;
                }
                if (lsc.ToLower().Trim() == "caseno")
                {
                    if (timeflag)
                    {
                        return lscasenumber + lsconnection + lstime;
                    }
                    else
                    {
                        return lscasenumber;
                    }
                }
                else
                {
                    if (timeflag)
                    {
                        return lsc + lsconnection + lstime;
                    }
                    else
                    {
                        return lsc;
                    }

                }
            }
        }
        public string GetEvidenceFolder()
        {
            if (CheckDefault())
            {

                return "default";
            }
            else
            {
                return "default";
                //string name = ConfigurationManager.AppSettings["RuleCase"];
                //string lsc = "";
                //string lsconnection = "_";
                //if (name.IndexOf("[") > -1 && name.IndexOf("]") > -1)
                //{
                //    lsc = name.Substring(name.IndexOf("[") + 1, name.IndexOf("]") - name.IndexOf("[") - 1);
                //}
                //if (name.IndexOf("(") > -1 && name.IndexOf(")") > -1)
                //{
                //    lsconnection = name.Substring(name.IndexOf("(") + 1, name.IndexOf(")") - name.IndexOf("(") - 1);
                //}
                //bool timeflag = true;
                //string lstime = DateTime.Now.ToString("yyyyMMddHHmmss");

                //if (name.IndexOf("时间戳") > -1)
                //{
                //    timeflag = true;
                //}
                //else
                //{
                //    timeflag = false;
                //}
                //if (lsc.ToLower().Trim() == "caseno")
                //{
                //    if (timeflag)
                //    {
                //        return lscasenumber + lsconnection + lstime;
                //    }
                //    else
                //    {
                //        return lscasenumber;
                //    }
                //}
                //else
                //{
                //    if (timeflag)
                //    {
                //        return lsc + lsconnection + lstime;
                //    }
                //    else
                //    {
                //        return lsc;
                //    }

                //}
            }

        }     //物证目录

        public string GetCaseNoName()
        {
            if (CheckDefault())
            {
                //int lic = GetCaseautoid();
                return "Case";/// lic.ToString();
            }
            else
            {
                //int lic = GetCaseautoid();
                string name = ConfigurationManager.AppSettings["RuleCaseNo"];
                string lsc = "";
                if (name.IndexOf("[") > -1 && name.IndexOf("]") > -1)
                {
                    lsc = name.Substring(name.IndexOf("[") + 1, name.IndexOf("]") - name.IndexOf("[") - 1);
                }
                return lsc;//+ lic.ToString();
            }

            //return "";
        }        //case no

        public int GetCaseautoid()
        {
            int caseid = 0;
            try
            {
                string lsa = ConfigurationManager.AppSettings["AutoCaseID"].ToString();
                caseid = Convert.ToInt32(lsa) + 1;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                UpdateAppConfig("AutoCaseID", "0");
                caseid = 1;
            }

            return caseid;
        }
        public bool UpdateCaseautoid(string id)
        {
            try
            {
                //int caseid = GetCaseautoid();
                //UpdateAppConfig("AutoCaseID", caseid.ToString());
                UpdateAppConfig("AutoCaseID", id);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                UpdateAppConfig("AutoCaseID", "0");
                return true;
            }

            return true;
        }
        public string GetEvidenceNo()
        {

            if (CheckDefault())
            {
                String ls = DateTime.Now.ToString("yyyyMMddHHmmss");
                return "Evidence_" + ls;
            }
            else
            {
                String ls = DateTime.Now.ToString("yyyyMMddHHmmss");
                string name = ConfigurationManager.AppSettings["RuleEviNo"];
                string lsc = "";
                if (name.IndexOf("[") > -1 && name.IndexOf("]") > -1)
                {
                    lsc = name.Substring(name.IndexOf("[") + 1, name.IndexOf("]") - name.IndexOf("[") - 1);
                }
                return lsc + "_" + ls;
                // return "Evidence_" + ls; 
            }
        }

        public string GetEvRawFolder()
        {
            if (CheckDefault())
            { return "Data"; }
            else
            {
                string name = ConfigurationManager.AppSettings["RuleRaw"];

                if (String.IsNullOrWhiteSpace(name))
                {
                    return "";
                }
                else
                { return name; }
            }
        }
        public string GetEvDataFolder()
        {
            if (CheckDefault())
            { return "XML"; }
            else
            {
                string name = ConfigurationManager.AppSettings["RuleData"];
                if (String.IsNullOrWhiteSpace(name))
                {
                    return "XML";
                }
                else
                {
                    if (CommonUtil.currentEvidence == null)
                    {
                        return name;
                    }
                    else
                    {
                        if (CommonUtil.currentEvidence.FILE_PATH == null)
                        {
                            return name;
                        }
                        else
                        {
                            if (Directory.Exists(Path.Combine(Path.GetDirectoryName(CommonUtil.currentEvidence.FILE_PATH), name)))
                            {
                                return name;
                            }
                            else if (Directory.Exists(Path.Combine(Path.GetDirectoryName(CommonUtil.currentEvidence.FILE_PATH), "XML")))
                            {
                                return "XML";
                            }
                            else
                            {
                                return "XML";
                            }
                        }
                    }
                }
            }
        }
        public string GetEvRepFolder()
        {
            if (CheckDefault())
            { return "Report"; }
            else
            {
                string name = ConfigurationManager.AppSettings["RuleReport"];
                if (String.IsNullOrWhiteSpace(name))
                {
                    return "";
                }
                else
                { return name; }
            }
        }
    }
}
