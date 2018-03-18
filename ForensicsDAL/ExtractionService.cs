using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.DAL
{
    public class ExtractionService
    {
        private String _conString = ConfigurationManager.ConnectionStrings["sqliteCon_dynamic"].ToString();


        /// <summary>
        /// 根据名称获取提取程序的路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public String GetPathByName(String name)
        {
            DBHelper._conString = _conString;
            String sql = " select EXTRACTION_PATH  from TBL_EXTRACTION where EXTRACTION_NAME=@EXTRACTION_NAME   ";
            return DBHelper.ExecuteScalar(sql, new SQLiteParameter("@EXTRACTION_NAME", name)).ToString();
        }
        /// <summary>
        /// 获取所有的提取信息
        /// </summary>
        /// <returns></returns>
        public List<Extraction> GetAll()
        {
            DBHelper._conString = _conString;
            List<Extraction> list = null;
            String sql = " select EXTRACTION_PATH ,EXTRACTION_NAME,EXTRACTION_VERSION from TBL_EXTRACTION where USE_FLAG='Y'  ";
            DataTable dt = DBHelper.GetDataTable(sql, null);
            if (dt.Rows.Count != null)
            {
                list = new List<Extraction>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Extraction
                    {
                        EXTRACTION_VERSION = row["EXTRACTION_VERSION"].ToString(),
                        ExtractionName = row["EXTRACTION_NAME"].ToString(),
                        ExtractionPath = row["EXTRACTION_PATH"].ToString(),
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有的模块Hash信息
        /// </summary>
        /// <returns></returns>
        public List<Extraction_Hash> GetAll_Hash(string lsmodule, string lspath)
        {
            DBHelper._conString = _conString;
            List<Extraction_Hash> list = null;
            String sql = " select EXTRACTION_FILES ,EXTRACTION_PATH,EXTRACTION_HASH from TBL_EXTRACTION_FILES where EXTRACTION_NAME ='" + lsmodule + "' and EXTRACTION_PATH='" + lspath + "' and USE_FLAG ='YES'  ";
            DataTable dt = DBHelper.GetDataTable(sql, null);
            if (dt.Rows.Count != null)
            {
                list = new List<Extraction_Hash>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Extraction_Hash
                    {
                        ExtractionFileName = row["EXTRACTION_FILES"].ToString(),
                        EXTRACTION_HASH = row["EXTRACTION_HASH"].ToString(),
                        ExtractionPath = row["EXTRACTION_PATH"].ToString(),
                    });
                }
            }
            return list;
        }
    }
}
