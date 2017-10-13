using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Forensics.Model
{
    /// <summary>
    /// 相关工具类
    /// </summary>
    public class Tool
    {
        private String _TOOL_GUID;
        /// <summary>
        /// ID
        /// </summary>
        public String TOOL_GUID
        {
            get { return _TOOL_GUID; }
            set { _TOOL_GUID = value; }
        }
        private String _TOOL_NAME;
        /// <summary>
        /// 工具名称
        /// </summary>
        public String TOOL_NAME
        {
            get { return _TOOL_NAME; }
            set { _TOOL_NAME = value; }
        }

        private String _TOOL_IMG;
        /// <summary>
        /// 工具图标
        /// </summary>
        public String TOOL_IMG
        {
            get
            {
                return Directory.GetCurrentDirectory() + "\\Images\\" + _TOOL_IMG;
            }
            set { _TOOL_IMG = value; }
        }

        private String _TOOL_DESCRIPTIO;
        /// <summary>
        /// 工具描述
        /// </summary>
        public String TOOL_DESCRIPTION
        {
            get { return _TOOL_DESCRIPTIO; }
            set { _TOOL_DESCRIPTIO = value; }
        }

        private String _TOOL_METHOD;
        /// <summary>
        /// 工具的接入点
        /// </summary>
        public String TOOL_METHOD
        {
            get { return _TOOL_METHOD; }
            set { _TOOL_METHOD = value; }
        }

        private String _TOOL_NAME_EN;

        public String TOOL_NAME_EN
        {
            get { return _TOOL_NAME_EN; }
            set { _TOOL_NAME_EN = value; }
        }

        private String _TOOL_DESCRIPTION_EN;

        public String TOOL_DESCRIPTION_EN
        {
            get { return _TOOL_DESCRIPTION_EN; }
            set { _TOOL_DESCRIPTION_EN = value; }
        }
    }
}
