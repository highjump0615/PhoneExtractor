using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model
{
    /// <summary>
    /// 应用来源
    /// </summary>
    public class Source
    {
        private String _SOURCE_GUID;

        public String SOURCE_GUID
        {
            get { return _SOURCE_GUID; }
            set { _SOURCE_GUID = value; }
        }

        private String _SOURCE_NUMBER;

        public String SOURCE_NUMBER
        {
            get { return _SOURCE_NUMBER; }
            set { _SOURCE_NUMBER = value; }
        }
        private String _SOURCE_NAME;

        public String SOURCE_NAME
        {
            get { return _SOURCE_NAME; }
            set { _SOURCE_NAME = value; }
        }
        private String _SOURCE_PARENT;

        public String SOURCE_PARENT
        {
            get { return _SOURCE_PARENT; }
            set { _SOURCE_PARENT = value; }
        }
        private String _SOURCE_TAG;

        public String SOURCE_TAG
        {
            get { return _SOURCE_TAG; }
            set { _SOURCE_TAG = value; }
        }

        private String _SOURCE_NAME_EN;

        public String SOURCE_NAME_EN
        {
            get { return _SOURCE_NAME_EN; }
            set { _SOURCE_NAME_EN = value; }
        }

        private String _SOURCE_IMGPATH;

        public String SOURCE_IMGPATH
        {
            get { return _SOURCE_IMGPATH; }
            set { _SOURCE_IMGPATH = value; }
        }
    }
}
