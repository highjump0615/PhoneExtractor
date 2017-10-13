using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model
{
    public class User
    {
        public static User LoginUser { get; set; }
        public static string User_Authority { get; set; }
        private String _USER_GUID;

        public String USER_GUID
        {
            get { return _USER_GUID; }
            set { _USER_GUID = value; }
        }

        private String _USER_NAME;

        public String USER_NAME
        {
            get { return _USER_NAME; }
            set { _USER_NAME = value; }
        }

        private String _USER_PASSWORD;

        public String USER_PASSWORD
        {
            get { return _USER_PASSWORD; }
            set { _USER_PASSWORD = value; }
        }

        private String _USER_ROLE;

        public String USER_ROLE
        {
            get { return _USER_ROLE; }
            set { _USER_ROLE = value; }
        }

        private String _USER_SKI;

        public String USER_SKIN
        {
            get { return _USER_SKI; }
            set { _USER_SKI = value; }
        }

        private String _USER_LANGUAGE;

        public String USER_LANGUAGE
        {
            get { return _USER_LANGUAGE; }
            set { _USER_LANGUAGE = value; }
        }

        private String _IF_USER_IMPORT;

        public String IF_USER_IMPORT
        {
            get
            {
                if (LoginUser.USER_LANGUAGE.Equals("英文"))
                    return _IF_USER_IMPORT.Replace("Y", "Yes").Replace("N", "No");

                else
                    return _IF_USER_IMPORT.Replace("Y", "是").Replace("N", "否");
            }
            set { _IF_USER_IMPORT = value; }
        }

        private String _IF_USER_UPLOAD;

        public String IF_USER_UPLOAD
        {
            get
            {
                if (LoginUser.USER_LANGUAGE.Equals("英文"))
                    return _IF_USER_IMPORT.Replace("Y", "Yes").Replace("N", "No");
                else
                    return _IF_USER_UPLOAD.Replace("Y", "是").Replace("N", "否");
            }
            set { _IF_USER_UPLOAD = value; }
        }

        private String _NAME;

        public String NAME
        {
            get { return _NAME; }
            set { _NAME = value; }
        }

        private String _ID;

        public String ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
    }
}
