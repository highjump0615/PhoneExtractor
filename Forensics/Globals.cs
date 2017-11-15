using Forensics.Model.Extract;
using Forensics.Util;
using Forensics.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            // 添加两个提取方式组
            this.MainActGroup.Add(new ActGroup());
            this.MainActGroup.Add(new ActGroup());

            //
            // 加载所有提取方式
            //
            string strConnection = ConfigurationManager.ConnectionStrings["mdb_phone"].ToString();
            string strQuery = "select * from edec_act_type";

            DataTable dt = DatabaseUtil.Query(strQuery, strConnection);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow tmpdr in dt.Rows)
                {
                    Act newAct = new Act()
                    {
                        Id = int.Parse(tmpdr["ACT_ID"].ToString()),
                        Name = tmpdr["ACT_Name"].ToString()
                    };

                    //
                    // 添加提取方式
                    //

                    // 逻辑提取
                    if (newAct.Id == 101)
                    {
                        newAct.listExtractType.Add(new ExtractType("机身系统信息"));
                        newAct.listExtractType.Add(new ExtractType("机身电话簿"));
                        newAct.listExtractType.Add(new ExtractType("机身通话记录"));
                        newAct.listExtractType.Add(new ExtractType("机身短信息"));
                        newAct.listExtractType.Add(new ExtractType("机身app列表"));
                    }
                    // 文件提取
                    else if (newAct.Id == 102)
                    {
                        newAct.listExtractType.Add(new ExtractType("图片照片"));
                        newAct.listExtractType.Add(new ExtractType("音频文件"));
                        newAct.listExtractType.Add(new ExtractType("视频文件"));
                        newAct.listExtractType.Add(new ExtractType("全部文件"));
                    }
                    // ADB备份
                    else if (newAct.Id == 103)
                    {
                        newAct.listExtractType.Add(new ExtractType("备份全部"));
                        newAct.listExtractType.Add(new ExtractType("微信降级"));
                        newAct.listExtractType.Add(new ExtractType("QQ降级"));
                    }
                    // 手机系统备份
                    else if (newAct.Id == 104)
                    {
                        newAct.listExtractType.Add(new ExtractType("华为"));
                        newAct.listExtractType.Add(new ExtractType("小米"));
                        newAct.listExtractType.Add(new ExtractType("oppo"));
                        newAct.listExtractType.Add(new ExtractType("vivo"));
                        newAct.listExtractType.Add(new ExtractType("联想"));
                    }
                    // Root提取, Recovery提取 
                    else if (newAct.Id == 105 || newAct.Id == 106)
                    {
                        newAct.listExtractType.Add(new ExtractType("镜像提取"));
                        newAct.listExtractType.Add(new ExtractType("微信及其数据目录"));
                        newAct.listExtractType.Add(new ExtractType("QQ及其数据目录"));
                        newAct.listExtractType.Add(new ExtractType("选中目录"));
                    }

                    // 添加到提取方式组
                    if (newAct.Id < 200)
                    {
                        this.MainActGroup[0].Acts.Add(newAct);
                    }
                    else
                    {
                        this.MainActGroup[1].Acts.Add(newAct);
                    }
                }
            }
        }

        public MainViewModel MainVM { get; set; }

        public List<ActGroup> MainActGroup { get; set; } = new List<ActGroup>();
    }
}
