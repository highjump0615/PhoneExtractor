using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model
{
    /// <summary>
    /// 案件信息表
    /// </summary>
    public class Data
    {
        private String _DATA_GUID;

        public String DATA_GUID
        {
            get { return _DATA_GUID; }
            set { _DATA_GUID = value; }
        }

        private String _CASE_GUID;

        public String CASE_GUID
        {
            get { return _CASE_GUID; }
            set { _CASE_GUID = value; }
        }
        private String _EVIDENCE_GUID;

        public String EVIDENCE_GUID
        {
            get { return _EVIDENCE_GUID; }
            set { _EVIDENCE_GUID = value; }
        }

        private String _SOURCE_GUID;

        public String SOURCE_GUID
        {
            get { return _SOURCE_GUID; }
            set { _SOURCE_GUID = value; }
        }

        private String _CASE_NAME;

        public String CASE_NAME
        {
            get { return _CASE_NAME; }
            set { _CASE_NAME = value; }
        }

        private String _SOURCE_NAME;

        public String SOURCE_NAME
        {
            get { return _SOURCE_NAME; }
            set { _SOURCE_NAME = value; }
        }

        private String _IF_DATA_ISSELECT;

        public String IF_DATA_ISSELECT
        {
            get { return _IF_DATA_ISSELECT; }
            set { _IF_DATA_ISSELECT = value; }
        }
        public String this[String index]
        {

            get
            {
                String str = "";
                switch (index.ToUpper())
                {
                    case "A":
                        str = A;
                        break;
                    case "B":
                        str = B;
                        break;
                    case "C":
                        str = C;
                        break;
                    case "D":
                        str = D;
                        break;
                    case "E":
                        str = E;
                        break;
                    case "F":
                        str = F;
                        break;
                    case "G":
                        str = G;
                        break;
                    case "H":
                        str = H;
                        break;
                    case "I":
                        str = I;
                        break;
                    case "J":
                        str = J;
                        break;
                    case "K":
                        str = K;
                        break;
                    case "L":
                        str = L;
                        break;
                    case "M":
                        str = M;
                        break;
                    case "N":
                        str = N;
                        break;
                    case "O":
                        str = O;
                        break;
                    case "P":
                        str = P;
                        break;
                    case "Q":
                        str = Q;
                        break;
                    case "R":
                        str = R;
                        break;
                    case "S":
                        str = S;
                        break;
                    case "T":
                        str = T;
                        break;
                    case "U":
                        str = U;
                        break;
                    case "V":
                        str = V;
                        break;
                    case "W":
                        str = W;
                        break;
                    case "X":
                        str = X;
                        break;
                    case "Y":
                        str = Y;
                        break;
                    case "Z":
                        str = Z;
                        break;
                    default:
                        break;
                }
                return str;
            }
        }
        private String _A;

        public String A
        {
            get { return _A; }
            set { _A = value; }
        }
        private String _B;

        public String B
        {
            get { return _B; }
            set { _B = value; }
        }
        private String _C;

        public String C
        {
            get { return _C; }
            set { _C = value; }
        }

        private String _D;

        public String D
        {
            get { return _D; }
            set { _D = value; }
        }
        private String _E;

        public String E
        {
            get { return _E; }
            set { _E = value; }
        }

        private String _F;

        public String F
        {
            get { return _F; }
            set { _F = value; }
        }
        private String _G;

        public String G
        {
            get { return _G; }
            set { _G = value; }
        }
        private String _H;

        public String H
        {
            get { return _H; }
            set { _H = value; }
        }
        private String _I;

        public String I
        {
            get { return _I; }
            set { _I = value; }
        }

        private String _J;

        public String J
        {
            get { return _J; }
            set { _J = value; }
        }
        private String _K;

        public String K
        {
            get { return _K; }
            set { _K = value; }
        }

        private String _L;

        public String L
        {
            get { return _L; }
            set { _L = value; }
        }

        private String _M;

        public String M
        {
            get { return _M; }
            set { _M = value; }
        }

        private String _N;

        public String N
        {
            get { return _N; }
            set { _N = value; }
        }

        private String _O;

        public String O
        {
            get { return _O; }
            set { _O = value; }
        }

        private String _P;

        public String P
        {
            get { return _P; }
            set { _P = value; }
        }

        private String _Q;

        public String Q
        {
            get { return _Q; }
            set { _Q = value; }
        }

        private String _R;

        public String R
        {
            get { return _R; }
            set { _R = value; }
        }
        private String _S;

        public String S
        {
            get { return _S; }
            set { _S = value; }
        }

        private String _T;

        public String T
        {
            get { return _T; }
            set { _T = value; }
        }

        private String _U;

        public String U
        {
            get { return _U; }
            set { _U = value; }
        }

        private String _V;

        public String V
        {
            get { return _V; }
            set { _V = value; }
        }

        private String _W;

        public String W
        {
            get { return _W; }
            set { _W = value; }
        }
        private String _X;

        public String X
        {
            get { return _X; }
            set { _X = value; }
        }
        private String _Y;

        public String Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        private String _Z;

        public String Z
        {
            get { return _Z; }
            set { _Z = value; }
        }


        /// <summary>
        /// 导出时，是否需要过滤用户取消选中的数据 
        /// true: 过滤
        /// false: 不过滤
        /// </summary>
        public static bool IsFilter = false;

        private static List<String> _strList = new List<string>();
        /// <summary>
        /// 获取要过滤的数据ID
        /// </summary>
        public static String FilterDataID
        {
            get
            {
                if (_strList.Count == 0)
                    return null;
                StringBuilder sb = new StringBuilder();
                foreach (String str in _strList)
                {
                    sb.Append("'" + str + "',");
                }
                return sb.ToString().TrimEnd(',');
            }
        }
        /// <summary>
        /// 添加过滤数据ID
        /// </summary>
        /// <param name="id"></param>
        public static void AddDataId(String id)
        {
            if (!_strList.Contains(id))
            {
                _strList.Add(id);
            }
        }
        /// <summary>
        /// 移除过滤数据ID
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveDataId(String id)
        {
            if (_strList.Contains(id))
            {
                _strList.Remove(id);
            }
        }
        public static void RemoveAll()
        {
            foreach (String item in _strList)
            {
                _strList.Remove(item);
            }
        }
    }
}
