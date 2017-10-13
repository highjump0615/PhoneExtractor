using Forensics.DAL;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.BLL
{
    public class JournalManager
    {
        JournalService js = new JournalService();
        /// <summary>
        /// 添加日志
        /// </summary> 
        /// <param name="journal">journal 添加的日志信息</param>
        /// <returns></returns>
        public int AddJournal(Journal journal)
        {
            //js.createDataTable();

            return js.AddJournal(journal);
        }
        /// <summary>
        /// 分页获取日志信息
        /// </summary> 
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <returns></returns>
        public List<Journal> GetAllJournal(int pageIndex, int pageSize)
        {
            return js.GetAllJournal(pageIndex, pageSize);
        }
        /// <summary>
        /// 批量删除
        /// </summary> 
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DelJournal(String ids)
        {
            return js.DelJournal(ids);
        }
        /// <summary>
        /// 获取指定时间段的日志信息
        /// </summary>
        /// <param name="beginTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">当前第N 页</param>
        /// <param name="pageSize">每页N 条</param>
        /// <returns></returns>
        public List<Journal> GetJournalByTimeSpan(DateTime beginTime, DateTime endTime, int pageIndex, int pageSize)
        {
            return js.GetJournalByTimeSpan(beginTime, endTime, pageIndex, pageSize);
        }
        /// <summary>
        /// 获取日志的总数量
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public int GetPageCount(int pageSize, String start = null, String end = null)
        {
            return js.GetPageCount(pageSize, start, end);
        }
    }
}
