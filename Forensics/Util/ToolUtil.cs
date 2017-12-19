using Forensics.BLL;
using Forensics.Model;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forensics.Util
{
    /// <summary>
    /// 工具相关
    /// </summary>
    public class ToolUtil
    {
        private static ILog log = LogManager.GetLogger(typeof(ToolUtil));

        private static String _clew = "操作提示";

        /// <summary>
        /// 打开工具
        /// </summary>
        /// <param name="param"></param>
        public static void OpenTool(object param)
        {
            Tool tl = (Tool)param;

            try
            {
                if (File.Exists(tl.TOOL_METHOD))
                {
                    startApplication(tl.TOOL_METHOD);
                }
                else if (File.Exists(tl.TOOL_METHOD.ToLower().Replace("d:\\", "c:\\")))
                {
                    startApplication(tl.TOOL_METHOD.ToLower().Replace("d:\\", "c:\\"));
                }
                else
                {
                    ToolManager toolManager = new ToolManager();

                    // MessageBox.Show("找不到程序",_clew,MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    var strMsg = Application.Current.FindResource("msgToolPath") as string;
                    if (MessageBoxResult.OK.Equals(MessageBox.Show(strMsg, _clew, MessageBoxButton.OKCancel, MessageBoxImage.Question)))
                    {
                        //创建一个对话框对象
                        OpenFileDialog ofd = new OpenFileDialog();

                        //为对话框设置标题
                        ofd.Title = "请选择导入的案件";
                        //设置筛选的图片格式
                        ofd.Filter = "文件格式|*.exe";

                        //设置是否允许多选
                        ofd.Multiselect = false;
                        //如果你点了“确定”按钮
                        if (ofd.ShowDialog() == true)
                        {
                            tl.TOOL_METHOD = ofd.FileName;
                            toolManager.UpdateToolPath(tl);

                            MessageBox.Show("设定程序目录成功", _clew);
                        }
                    }
                    else
                    {
                        //取消 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现错误：" + ex.Message, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static void startApplication(String path)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.WorkingDirectory = path.Substring(0, path.LastIndexOf("\\"));
            try
            {
                p.Start();
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);

                var strMsg = Application.Current.FindResource("msgToolStartError") as string;
                MessageBox.Show(strMsg, _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
