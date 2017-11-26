using Forensics.Command;
using Forensics.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    class SettingFeedbackViewModel : ViewModelBase
    {
        private String _clew = "操作提示";

        public override Pages PageIndex
        {
            get { return Pages.SettingFeedback; }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 提交命令
        /// </summary>
        private ICommand _submitCommand;
        public ICommand SubmitCommand
        {
            get { return _submitCommand ?? (_submitCommand = new DelegateCommand(DoSubmit)); }
        }

        /// <summary>
        /// 提交
        /// </summary>
        private void DoSubmit()
        {
            //
            // 检查数据
            //
            if (String.IsNullOrWhiteSpace(this.Title))
            {
                MessageBox.Show("标题不能为空！", _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (String.IsNullOrWhiteSpace(this.Content))
            {
                MessageBox.Show("内容不能为空！", _clew, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SendEmail se = new SendEmail();
            if (se.Send(null, this.Title, this.Content, false).Equals("OK"))
            {
                MessageBox.Show("已将您的反馈信息发送。", _clew);
            }
        }
    }
}
