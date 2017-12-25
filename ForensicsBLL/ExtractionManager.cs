using Forensics.DAL;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.BLL
{
    public class ExtractionManager
    {
        ExtractionService es = new ExtractionService();
        /// <summary>
        /// 根据名称获取提取程序的路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public String GetPathByName(String name)
        {
            return es.GetPathByName(name);
        }
        /// <summary>
        /// 获取所有的提取信息
        /// </summary>
        /// <returns></returns>
        public List<Extraction> GetAll()
        {
            return es.GetAll();
        }

        /// <summary>
        /// 获取所有的模块hash信息
        /// </summary>
        /// <returns></returns>
        public List<Extraction_Hash> GetAll_Hash(string lsm, string lsp)
        {
            return es.GetAll_Hash(lsm, lsp);
        }
    }
}
