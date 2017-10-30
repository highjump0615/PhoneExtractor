using Forensics.DAL;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.BLL
{
    public class DataManager
    {
        DataService ds = new DataService();
        /// <summary>
        /// 读取指定目录案件库中所有数据
        /// </summary>
        /// <param name="casePath">路径</param>
        /// <param name="caseId">案件Id</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="searchText">要检索的关键字</param>
        /// <returns></returns>
        public List<Data> GetAllData(String casePath, String caseId = "", String evidenceId = "", String sourceId = "", String searchText = "")
        {
            return ds.GetAllData(casePath, caseId, evidenceId, sourceId, searchText);
        }

        /// <summary>
        /// 读取指定目录案件库中所有数据
        /// </summary>
        /// <param name="casePath">路径</param>
        /// <param name="caseId">案件Id</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="searchText">要检索的关键字</param>
        /// <returns></returns>
        public int GetAllData2(String casePath, String caseId = "", String evidenceId = "", String sourceId = "", String searchText = "")
        {
            return ds.GetAllData2(casePath, caseId, evidenceId, sourceId, searchText);
        }

        /// <summary>
        /// 获取指定SOURCEID 下的数据个数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="evidenceId"></param>
        /// <param name="sourceId"></param>
        /// <param name="searchText">要检索的关键字</param>
        /// <returns></returns>
        public int GetCountBySourceID(String path, String evidenceId, String sourceId, String searchText = "")
        {
            return ds.GetCountBySourceID(path, evidenceId, sourceId, searchText);
        }
        /// <summary>
        /// 获取指定SOURCEID 下指定用户的数据个数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="evidenceId"></param>
        /// <param name="sourceId"></param>
        /// <param name="aVal"></param>
        /// <param name="searchText">要检索的关键字</param>
        /// <returns></returns>
        public int GetCountBySourceID_User(String path, String evidenceId, String sourceId, String aVal, String searchText = "")
        {
            return ds.GetCountBySourceID(path, evidenceId, sourceId, aVal, searchText);
        }
        /// <summary>
        /// 获取案件下某一个物证下涉及到的用户
        /// </summary>
        /// <param name="path">案件库的路径</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="columnName">QQ号码列名称(A or B ...)</param>
        /// <returns></returns>
        public List<String> GetDataQQID(String path, String evidenceId, String sourceId, String columnName)
        {
            return ds.GetDataQQID(path, evidenceId, sourceId, columnName);
        }
        /// <summary>
        /// 获取数据信息
        /// </summary>
        /// <param name="path">库路径</param>
        /// <param name="evidenceId">物证ID </param>
        /// <param name="sourceId">来源ID </param> 
        /// <param name="a">用户(QQID、 微信用户)</param>
        /// <param name="searchText">要检索的关键字</param>
        /// /// <returns></returns>
        public List<Data> GetDataBySourceID_EvidenceId_A(String path, String evidenceId, String sourceId, String a, String searchText = "")
        {
            return ds.GetDataBySourceID_EvidenceId_A(path, evidenceId, sourceId, a, searchText);
        }
        /// <summary>
        /// 获取分组列的值
        /// </summary>
        /// <param name="path">案件所在路径</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="columnName">字段</param>
        /// <returns></returns>
        public List<String> GetColumValueGroup(String path, String evidenceId, String sourceId, String columnName)
        {
            return ds.GetColumValueGroup(path, evidenceId, sourceId, columnName);
        }
        /// <summary>
        /// 获取某一列中的数据
        /// </summary>
        /// <param name="path">案件所在路径</param>
        /// <param name="evidenceId">物证</param>
        /// <param name="sourceId">ID</param>
        /// <param name="columnName">列名称</param>
        /// <param name="columnValue">列值</param>
        /// <returns></returns>
        public List<Data> GetDataByGroupValue(String path, String evidenceId, String sourceId, String columnName, String columnValue)
        {
            return ds.GetDataByGroupValue(path, evidenceId, sourceId, columnName, columnValue);
        }

        /// <summary>
        /// 获取某一案件库下所有的物信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Evidence> GetAllEvidence(String path)
        {
            return ds.GetAllEvidence(path);
        }
        /// <summary>
        /// 获取某一物证下的来源
        /// </summary>
        /// <param name="casePath"></param>
        /// <param name="evidenceID"></param>
        /// <returns></returns>
        public List<Source> GetAllSourceByEvidence(String path, String evidenceID)
        {
            return ds.GetAllSourceByEvidence(path, evidenceID);
        }
        /// <summary>
        /// 获取案件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Case GetCase(String path)
        {
            return ds.GetCase(path);
        }
        /// <summary>
        /// 判断某个物证下的来源是否有数据
        /// </summary>
        /// <param name="path">案件路径</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceId">来源ID</param>
        /// <returns>true: 有  false ： 无</returns>
        public bool HasData(String path, String evidenceId, String sourceId)
        {
            return ds.HasData(path, evidenceId, sourceId);
        }
        /// <summary>
        /// (多ID)判断某个物证下的来源是否有数据
        /// </summary>
        /// <param name="path">案件路径</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceId">来源ID</param>
        /// <returns>true: 有  false ： 无</returns>
        public bool HasData1(String path, String evidenceId, String sourceIds)
        {
            return ds.HasData1(path, evidenceId, sourceIds);
        }


        /// <summary>
        /// 根据ID 修改数据的选中状态
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <param name="isSelect"></param>
        public void UpdateIsSelectById(String path, String id, String isSelect)
        {
            ds.UpdateIsSelectById(path, id, isSelect);
        }
        /// <summary>
        /// 获取某个物证下的指定的多个SOURCE_GUID 的数据
        /// </summary>
        /// <param name="path">案件路径</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceIds">多个SOURCE_GUID 格式为：('2','3',)</param>
        public List<Data> GetDataBySourceIds(String path, String evidenceId, String sourceIds)
        {
            return ds.GetDataBySourceIds(path, evidenceId, sourceIds);
        }

        /// <summary>
        /// 获取此案件下所有的物证信息(用于案件导入使用)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Evidence> GetAllEvidences(String path)
        {
            return ds.GetAllEvidences(path);
        }


        /// <summary>
        /// 根据物证ID 删除数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="evidenceIds">物证ID </param> 
        public void DelDataByEvidenceId(String path, String evidenceIds)
        {
            ds.DelDataByEvidenceId(path, evidenceIds);
        }
        /// <summary>
        /// 合并物证信息
        /// </summary>
        /// <param name="path">案件库路径</param>
        /// <param name="evidenceList">合并的物证列表</param>
        /// <param name="newEvidence">新物证信息</param>
        public void MergeEvidenceData(String path, List<Evidence> evidenceList, Evidence newEvidence, bool ibdelete)
        {
            ds.MergeEvidenceData(path, evidenceList, newEvidence, ibdelete);
        }

        /// <summary>
        /// 检索功能
        /// </summary>
        /// <param name="whereText"></param>
        /// <returns></returns>
        public List<Data> GetDataByWhere(String casePath, String whereText)
        {
            return ds.GetDataByWhere(casePath, whereText);
        }

        /// <summary>
        /// 数据整理
        /// </summary>
        /// <param name="whereText"></param>
        /// <returns></returns>
        public void ReorganizeData(String locationdb, String path, String caseId, String caseName, String evidenceId, bool isEnglish)
        {
            ds.ReorganizeData(locationdb, path, caseId, caseName, evidenceId, isEnglish);
        }

        /// <summary>
        /// 数据整理
        /// </summary>
        /// <param name="whereText"></param>
        /// <returns></returns>
        public void ReorganizeAllData(String locationdb, String path, String caseId, String caseName, bool isEnglish)
        {
            ds.ReorganizeAllData(locationdb, path, caseId, caseName, isEnglish);
        }
        /// <summary>
        /// 创建导出使用的数据库
        /// </summary>
        /// <param name="casePath"></param> 
        /// <returns></returns>
        public bool CreateExportDataBase(String casePath)
        {
            return ds.CreateExportDataBase(casePath);
        }
        /// <summary>
        /// 导出数据到数据库中
        /// </summary>
        /// <param name="casePath"></param>
        /// <param name="dataList"></param>
        public void ExportDataToBase(String casePath, List<Data> dataList)
        {
            ds.AddDatas(casePath, dataList);
        }

        /// <summary>
        /// 读取案件库中的信息
        /// </summary>
        /// <param name="casePath">案件所在路径</param>
        /// <param name="whereStr">sql 条件语句</param>
        /// <returns></returns>
        public List<Data> GetAllData(String casePath, String whereStr)
        {
            return ds.GetAllData(casePath, whereStr);
        }

        /// <summary>
        /// 读取案件库中的case_ai信息
        /// </summary>
        /// <param name="casePath">案件所在路径</param>
        /// <param name="whereStr">sql 条件语句</param>
        /// <returns></returns>
        public String GetAiEvidendeData(String casePath, String whereStr)
        {
            return ds.GetAiEvidendeData(casePath, whereStr);
        }
    }
}
