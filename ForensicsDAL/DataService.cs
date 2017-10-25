using Forensics.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.DAL
{
    public class DataService
    {
        ILog log = LogManager.GetLogger(typeof(DataService));
        public bool CreateDataBase(String casePath, Case myCase)
        {
            SQLiteConnection con = null;
            try
            {
                String dir = casePath.Substring(0, casePath.LastIndexOf('\\'));
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                SQLiteConnection.CreateFile(casePath);
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = casePath,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = "CREATE TABLE [TBL_DATA] (";
                sql += "  [DATA_GUID] VARCHAR(64)  PRIMARY KEY NULL, ";
                sql += "  [CASE_GUID] VARCHAR(64)  NULL,";
                sql += "  [EVIDENCE_GUID] VARCHAR(512)  NULL,";
                sql += "  [SOURCE_GUID] VARCHAR(512)  NULL,";
                sql += "  [EVIDENCETYPE_GUID] VARCHAR(64)  NULL,";
                sql += "  [CASE_NAME] VARCHAR(64)  NULL,";
                sql += "  [EVIDENCE_NUMBER] VARCHAR(1024)  NULL,";
                sql += "  [SOURCE_NAME] VARCHAR(1024)  NULL,";
                sql += "   [IF_DATA_ISSELECT] VARCHAR(32) NULL, ";
                for (char i = 'A'; i <= 'Z'; i++)
                {
                    sql += " [" + i + "] VARCHAR(1024)  NULL,";
                }
                sql = sql.TrimEnd(',');
                sql += " ) ;";

                sql += @"CREATE TABLE [TBL_TMP] (";
                for (char i = 'A'; i <= 'Z'; i++)
                {
                    sql += " [" + i + "] VARCHAR(1024)  NULL,";
                }
                sql = sql.TrimEnd(',');
                sql += " ) ;";

                sql += @" CREATE TABLE [TBL_CASE] (
                    [CASE_GUID] VARCHAR(64)  PRIMARY KEY NULL,
                    [CASE_NUMBER] VARCHAR(64)  NULL,
                    [CASE_NAME] VARCHAR(512)  NULL,
                    [CASE_IMG] VARCHAR(512)  NULL,
                    [USER_GUID] VARCHAR(64)  NULL,
                    [ADDTIME] DATE  NULL,
                    [CASE_DESCRIPTION] VARCHAR(1024)  NULL,
                    [CASE_REMARK] VARCHAR(1024)  NULL,
                    [CASE_PATH] VARCHAR(1024)  NULL
                    ) ;";

                sql += @"CREATE TABLE [TBL_EVIDENCE] (
                        [EVIDENCE_GUID] VARCHAR(64)  PRIMARY KEY NULL,
                        [EVIDENCE_NUMBER] VARCHAR(64)  NULL,
                        [EVIDENCE_SENDER] VARCHAR(64)  NULL,
                        [EVIDENCE_SENDERUNIT] VARCHAR(128)  NULL,
                        [ADDTIME] DATE  NULL,
                        [EVIDENCE_REMARK] VARCHAR(1024)  NULL,
                        [EVIDENCE_NAME] VARCHAR(64) NULL,
                        [CASE_GUID] VARCHAR(64)  NULL,
                        [QUZHENG_DATE] DATE  NULL,
                        [EVIDENCE_JYR] VARCHAR(32)  NULL,
                        [EVIDENCE_FILE] VARCHAR(1024) NULL,
                        [OWNER_NAME] VARCHAR(1024) NULL,
                        [OWNER_SEX] VARCHAR(1024) NULL,
                        [OWNER_ID] VARCHAR(1024) NULL,
                        [OWNER_PHONENUMBER] VARCHAR(1024) NULL,
                        [COLLECTIONUNIT_CODE] VARCHAR(1024) NULL,
                        [COLLECTIONUNIT_NAME] VARCHAR(1024) NULL,
                        [COLLECTIONUNIT_PHONENUMBER] VARCHAR(1024) NULL,
                        [COLLECTIONPEOPLE_NAME] VARCHAR(1024) NULL,
                        [COLLECTIONPEOPLE_ID] VARCHAR(1024) NULL
                        );";
                sql += @"insert into TBL_CASE 
                        (CASE_GUID,CASE_NUMBER,CASE_NAME,CASE_IMG,USER_GUID,ADDTIME,CASE_DESCRIPTION,CASE_REMARK,CASE_PATH) 
                        values (@CASE_GUID,@CASE_NUMBER,@CASE_NAME,@CASE_IMG,@USER_GUID,@ADDTIME,@CASE_DESCRIPTION,@CASE_REMARK,@CASE_PATH);
";

                con = new SQLiteConnection(connectionString.ToString());
                SQLiteCommand cmd = new SQLiteCommand(con);
                cmd.Parameters.AddRange(new SQLiteParameter[]
                {
                     new SQLiteParameter("@CASE_GUID",myCase.CASE_GUID),
                     new SQLiteParameter("@CASE_NUMBER",myCase.CASE_NUMBER),
                     new SQLiteParameter("@CASE_NAME",myCase.CASE_NAME),
                     new SQLiteParameter("@CASE_IMG",myCase.CASE_IMG),
                     new SQLiteParameter("@USER_GUID",myCase.USER_GUID),
                     new SQLiteParameter("@ADDTIME",myCase.ADDTIME),
                     new SQLiteParameter("@CASE_DESCRIPTION",myCase.CASE_DESCRIPTION),
                     new SQLiteParameter("@CASE_REMARK",myCase.CASE_REMARK),
                     new SQLiteParameter("@CASE_PATH",myCase.CASE_PATH),
                });
                cmd.CommandText = sql;
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("创建数据库文件失败！");
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// 读取指定目录案件库中所有数据
        /// </summary>
        /// <param name="casePath">路径</param>
        /// <returns></returns>
        public List<Data> GetAllData(String casePath)
        {
            List<Data> dataList = null;
            if (File.Exists(casePath))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = casePath,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(casePath.ToString());
                    String sql = " select  * from TBL_DATA   where 1==1";
                    if (Data.IsFilter)
                    {
                        sql += " and DATA_GUID not in (" + Data.FilterDataID + ") ";
                    }
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dataList = new List<Data>();
                        while (reader.Read())
                        {
                            dataList.Add(new Data
                            {
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                                CASE_NAME = reader["CASE_NAME"].ToString(),
                                DATA_GUID = reader["DATA_GUID"].ToString(),
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                SOURCE_GUID = reader["SOURCE_GUID"].ToString(),
                                IF_DATA_ISSELECT = reader["IF_DATA_ISSELECT"].ToString(),
                                SOURCE_NAME = reader["SOURCE_NAME"].ToString(),
                                A = reader["A"].ToString(),
                                B = reader["B"].ToString(),
                                C = reader["C"].ToString(),
                                D = reader["D"].ToString(),
                                E = reader["E"].ToString(),
                                F = reader["F"].ToString(),
                                G = reader["G"].ToString(),
                                H = reader["H"].ToString(),
                                I = reader["I"].ToString(),
                                J = reader["J"].ToString(),
                                K = reader["K"].ToString(),
                                L = reader["L"].ToString(),
                                M = reader["M"].ToString(),
                                N = reader["N"].ToString(),
                                O = reader["O"].ToString(),
                                P = reader["P"].ToString(),
                                Q = reader["Q"].ToString(),
                                R = reader["R"].ToString(),
                                S = reader["S"].ToString(),
                                T = reader["T"].ToString(),
                                U = reader["U"].ToString(),
                                V = reader["V"].ToString(),
                                W = reader["W"].ToString(),
                                X = reader["X"].ToString(),
                                Y = reader["Y"].ToString(),
                                Z = reader["Z"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    reader.Close();
                    con.Close();
                }
            }
            return dataList;
        }
        /// <summary>
        /// 读取指定目录案件库中所有数据
        /// </summary>
        /// <param name="casePath">路径</param>
        /// <param name="caseId">案件Id</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="searchText">检索的关键字</param>
        /// <returns></returns>
        public List<Data> GetAllData(String casePath, String caseId = "", String evidenceId = "", String sourceId = "", String searchText = "")
        {
            List<Data> dataList = null;
            if (File.Exists(casePath))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = casePath,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    String sql = " select  * from TBL_DATA  where 1==1   ";

                    if (caseId != "")
                    {
                        sql += " and CASE_GUID='" + caseId + "' ";
                    }
                    if (evidenceId != "")
                    {
                        sql += " and  EVIDENCE_GUID='" + evidenceId + "'  ";
                    }
                    if (sourceId != "")
                    {
                        sql += " and SOURCE_GUID='" + sourceId + "' ";
                    }
                    if (Data.IsFilter)
                    {
                        sql += " and DATA_GUID  in (" + Data.FilterDataID + ") ";
                    }
                    if (searchText.Length > 0)
                    {
                        sql += String.Format(" and  (A like '%{0}%' or B like '%{0}%' or C like '%{0}%' or D like '%{0}%' or E like '%{0}%' or F like '%{0}%' or G like '%{0}%' or H like '%{0}%'  or I like '%{0}%' or J like '%{0}%' or K like '%{0}%' or L like '%{0}%'  or M like '%{0}%' or N like '%{0}%' or O like '%{0}%'  or P like '%{0}%'  or Q like '%{0}%' or R like '%{0}%' or S like '%{0}%' or T like '%{0}%'  or U like '%{0}%' or V like '%{0}%'  or W like '%{0}%'  or X like '%{0}%' or Y like '%{0}%' or Z like '%{0}%' ) ", searchText);
                    }
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dataList = new List<Data>();
                        while (reader.Read())
                        {
                            dataList.Add(new Data
                            {
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                                CASE_NAME = reader["CASE_NAME"].ToString(),
                                DATA_GUID = reader["DATA_GUID"].ToString(),
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                SOURCE_GUID = reader["SOURCE_GUID"].ToString(),
                                IF_DATA_ISSELECT = reader["IF_DATA_ISSELECT"].ToString(),
                                SOURCE_NAME = reader["SOURCE_NAME"].ToString(),
                                A = reader["A"].ToString(),
                                B = reader["B"].ToString(),
                                C = reader["C"].ToString(),
                                D = reader["D"].ToString(),
                                E = reader["E"].ToString(),
                                F = reader["F"].ToString(),
                                G = reader["G"].ToString(),
                                H = reader["H"].ToString(),
                                I = reader["I"].ToString(),
                                J = reader["J"].ToString(),
                                K = reader["K"].ToString(),
                                L = reader["L"].ToString(),
                                M = reader["M"].ToString(),
                                N = reader["N"].ToString(),
                                O = reader["O"].ToString(),
                                P = reader["P"].ToString(),
                                Q = reader["Q"].ToString(),
                                R = reader["R"].ToString(),
                                S = reader["S"].ToString(),
                                T = reader["T"].ToString(),
                                U = reader["U"].ToString(),
                                V = reader["V"].ToString(),
                                W = reader["W"].ToString(),
                                X = reader["X"].ToString(),
                                Y = reader["Y"].ToString(),
                                Z = reader["Z"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return dataList;
        }

        /// <summary>
        /// 读取指定目录案件库中所有数据   //201507 add for evidence new logic 
        /// </summary>
        /// <param name="casePath">路径</param>
        /// <param name="caseId">案件Id</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="searchText">检索的关键字</param>
        /// <returns></returns>
        public int GetAllData2(String casePath, String caseId = "", String evidenceId = "", String sourceId = "", String searchText = "")
        {
            int reader = 0;
            List<Data> dataList = null;
            if (File.Exists(casePath))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = casePath,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;

                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    String sql = " select count(*) from TBL_EVIDENCE  where 1==1   ";

                    if (caseId != "")
                    {
                        sql += " and CASE_GUID='" + caseId + "' ";
                    }
                    if (evidenceId != "")
                    {
                        sql += " and  EVIDENCE_GUID='" + evidenceId + "'  ";
                    }
                    if (sourceId != "")
                    {
                        sql += " and SOURCE_GUID='" + sourceId + "' ";
                    }
                    if (Data.IsFilter)
                    {
                        sql += " and DATA_GUID  in (" + Data.FilterDataID + ") ";
                    }
                    if (searchText.Length > 0)
                    {
                        sql += String.Format(" and  (A like '%{0}%' or B like '%{0}%' or C like '%{0}%' or D like '%{0}%' or E like '%{0}%' or F like '%{0}%' or G like '%{0}%' or H like '%{0}%'  or I like '%{0}%' or J like '%{0}%' or K like '%{0}%' or L like '%{0}%'  or M like '%{0}%' or N like '%{0}%' or O like '%{0}%'  or P like '%{0}%'  or Q like '%{0}%' or R like '%{0}%' or S like '%{0}%' or T like '%{0}%'  or U like '%{0}%' or V like '%{0}%'  or W like '%{0}%'  or X like '%{0}%' or Y like '%{0}%' or Z like '%{0}%' ) ", searchText);
                    }
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();

                    reader = Convert.ToInt16(cmd.ExecuteScalar().ToString());

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {

                }
            }
            return reader;
        }
        /// <summary>
        /// 执行批插入
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public int AddDatas(String path, List<Data> dataList)
        {
            int result = 0;
            if (File.Exists(path))
            {
                StringBuilder sql = new StringBuilder();
                List<SQLiteParameter> paraList = new List<SQLiteParameter>();
                int i = 0;
                foreach (Data data in dataList)
                {
                    sql.Append(@"insert into TBL_DATA (DATA_GUID,CASE_GUID,EVIDENCE_GUID,SOURCE_GUID,CASE_NAME,SOURCE_NAME,IF_DATA_ISSELECT,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z) values(@DATA_GUID" + i + ",@CASE_GUID" + i + ",@EVIDENCE_GUID" + i + ",@SOURCE_GUID" + i + ",@CASE_NAME" + i + ",@SOURCE_NAME" + i + ",@IF_DATA_ISSELECT" + i + ",@A" + i + ",@B" + i + ",@C" + i + ",@D" + i + ",@E" + i + ",@F" + i + ",@G" + i + ",@H" + i + ",@I" + i + ",@J" + i + ",@K" + i + ",@L" + i + ",@M" + i + ",@N" + i + ",@O" + i + ",@P" + i + ",@Q" + i + ",@R" + i + ",@S" + i + ",@T" + i + ",@U" + i + ",@V" + i + ",@W" + i + ",@X" + i + ",@Y" + i + ",@Z" + i + ");"
                     );
                    paraList.Add(new SQLiteParameter("@DATA_GUID" + i, data.DATA_GUID));
                    paraList.Add(new SQLiteParameter("@CASE_GUID" + i, data.CASE_GUID));
                    paraList.Add(new SQLiteParameter("@EVIDENCE_GUID" + i, data.EVIDENCE_GUID));
                    paraList.Add(new SQLiteParameter("@SOURCE_GUID" + i, data.SOURCE_GUID));

                    paraList.Add(new SQLiteParameter("@CASE_NAME" + i, data.CASE_NAME));

                    paraList.Add(new SQLiteParameter("@SOURCE_NAME" + i, data.SOURCE_NAME));
                    paraList.Add(new SQLiteParameter("@IF_DATA_ISSELECT" + i, data.IF_DATA_ISSELECT));
                    paraList.Add(new SQLiteParameter("@A" + i, data.A));
                    paraList.Add(new SQLiteParameter("@B" + i, data.B));
                    paraList.Add(new SQLiteParameter("@C" + i, data.C));
                    paraList.Add(new SQLiteParameter("@D" + i, data.D));

                    paraList.Add(new SQLiteParameter("@E" + i, data.E));
                    paraList.Add(new SQLiteParameter("@F" + i, data.F));
                    paraList.Add(new SQLiteParameter("@G" + i, data.G));
                    paraList.Add(new SQLiteParameter("@H" + i, data.H));
                    paraList.Add(new SQLiteParameter("@I" + i, data.I));
                    paraList.Add(new SQLiteParameter("@J" + i, data.J));
                    paraList.Add(new SQLiteParameter("@K" + i, data.K));
                    paraList.Add(new SQLiteParameter("@L" + i, data.L));
                    paraList.Add(new SQLiteParameter("@M" + i, data.M));
                    paraList.Add(new SQLiteParameter("@N" + i, data.N));
                    paraList.Add(new SQLiteParameter("@O" + i, data.O));
                    paraList.Add(new SQLiteParameter("@P" + i, data.P));

                    paraList.Add(new SQLiteParameter("@Q" + i, data.K));
                    paraList.Add(new SQLiteParameter("@R" + i, data.R));
                    paraList.Add(new SQLiteParameter("@S" + i, data.S));
                    paraList.Add(new SQLiteParameter("@T" + i, data.T));
                    paraList.Add(new SQLiteParameter("@U" + i, data.U));
                    paraList.Add(new SQLiteParameter("@V" + i, data.V));

                    paraList.Add(new SQLiteParameter("@W" + i, data.W));
                    paraList.Add(new SQLiteParameter("@X" + i, data.X));
                    paraList.Add(new SQLiteParameter("@Y" + i, data.Y));
                    paraList.Add(new SQLiteParameter("@Z" + i, data.Z));
                    i++;
                }
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteTransaction tran = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    con.Open();
                    tran = con.BeginTransaction();
                    cmd = new SQLiteCommand(sql.ToString(), con);

                    if (paraList.Count != 0)
                    {
                        cmd.Parameters.AddRange(paraList.ToArray());
                    }
                    cmd.Transaction = tran;
                    result = cmd.ExecuteNonQuery();
                    tran.Commit();

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    tran.Rollback();
                    throw new Exception("添加数据失败");
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
            #region 注释部分
            //int result = 0;
            //if (File.Exists(path))
            //{
            //    StringBuilder sql = new StringBuilder();
            //    List<SQLiteParameter> paraList = new List<SQLiteParameter>();
            //    int i = 0;
            //    sql.Append("insert into TBL_DATA (");
            //    sql.Append("DATA_GUID");
            //    sql.Append(",CASE_GUID");
            //    sql.Append(",EVIDENCE_GUID");
            //    sql.Append(",SOURCE_GUID");
            //    sql.Append(",CASE_NAME");
            //    sql.Append(",SOURCE_NAME");
            //    sql.Append(",IF_DATA_ISSELECT");
            //    sql.Append(",A");
            //    sql.Append(",B");
            //    sql.Append(",C");
            //    sql.Append(",D");
            //    sql.Append(",E");
            //    sql.Append(",F");
            //    sql.Append(",G");
            //    sql.Append(",H");


            //    sql.Append(",I");
            //    sql.Append(",J");
            //    sql.Append(",K");

            //    sql.Append(",L");
            //    sql.Append(",M");
            //    sql.Append(",N");

            //    sql.Append(",O");
            //    //sql.Append(",P");
            //    //sql.Append(",Q");

            //    //sql.Append(",R");
            //    //sql.Append(",S");
            //    //sql.Append(",T"); 
            //    //sql.Append(",U");
            //    //sql.Append(",V");
            //    //sql.Append(",W");
            //    //sql.Append(",X");
            //    //sql.Append(",Y");
            //    //sql.Append(",Z");
            //    sql.Append(")");
            //    foreach (Data data in dataList)
            //    {
            //        sql.Append(" select  @DATA_GUID" + i + ",@CASE_GUID" + i + ",@EVIDENCE_GUID" + i + ",@SOURCE_GUID" + i + ",@CASE_NAME" + i + ",@SOURCE_NAME" + i + ",@IF_DATA_ISSELECT" + i + ",@A" + i + ",@B" + i + ",@C" + i + ",@D" + i + ",@E" + i + ",@F" + i + ",@G" + i + ",@H" + i + ",@I" + i + ",@J" + i + ",@K" + i + ",@L" + i + ",@M" + i + ",@N" + i + ",@O" + i/* + ",@P" + i + ",@Q" + i + ",@R" + i + ",@S" + i + ",@T" + i + ",@U" + i + ",@V" + i + ",@W" + i + ",@X" + i + ",@Y" + i + ",@Z" + i */);
            //        if (i != dataList.Count - 1)
            //        {
            //            sql.Append(" union all ");
            //        }
            //        //sql.Append(@"insert into TBL_DATA (DATA_GUID,CASE_GUID,EVIDENCE_GUID,SOURCE_GUID,CASE_NAME,SOURCE_NAME,IF_DATA_ISSELECT,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z) values(@DATA_GUID" + i + ",@CASE_GUID" + i + ",@EVIDENCE_GUID" + i + ",@SOURCE_GUID" + i + ",@CASE_NAME" + i + ",@SOURCE_NAME" + i + ",@IF_DATA_ISSELECT" + i + ",@A" + i + ",@B" + i + ",@C" + i + ",@D" + i + ",@E" + i + ",@F" + i + ",@G" + i + ",@H" + i + ",@I" + i + ",@J" + i + ",@K" + i + ",@L" + i + ",@M" + i + ",@N" + i + ",@O" + i + ",@P" + i + ",@Q" + i + ",@R" + i + ",@S" + i + ",@T" + i + ",@U" + i + ",@V" + i + ",@W" + i + ",@X" + i + ",@Y" + i + ",@Z" + i + ");"  );
            //        paraList.Add(new SQLiteParameter("@DATA_GUID" + i, data.DATA_GUID));
            //        paraList.Add(new SQLiteParameter("@CASE_GUID" + i, data.CASE_GUID));
            //        paraList.Add(new SQLiteParameter("@EVIDENCE_GUID" + i, data.EVIDENCE_GUID));
            //        paraList.Add(new SQLiteParameter("@SOURCE_GUID" + i, data.SOURCE_GUID));

            //        paraList.Add(new SQLiteParameter("@CASE_NAME" + i, data.CASE_NAME));

            //        paraList.Add(new SQLiteParameter("@SOURCE_NAME" + i, data.SOURCE_NAME));
            //        paraList.Add(new SQLiteParameter("@IF_DATA_ISSELECT" + i, data.IF_DATA_ISSELECT));
            //        paraList.Add(new SQLiteParameter("@A" + i, data.A));
            //        paraList.Add(new SQLiteParameter("@B" + i, data.B));
            //        paraList.Add(new SQLiteParameter("@C" + i, data.C));
            //        paraList.Add(new SQLiteParameter("@D" + i, data.D));

            //        paraList.Add(new SQLiteParameter("@E" + i, data.E));
            //        paraList.Add(new SQLiteParameter("@F" + i, data.F));
            //        paraList.Add(new SQLiteParameter("@G" + i, data.G));
            //        paraList.Add(new SQLiteParameter("@H" + i, data.H));

            //        paraList.Add(new SQLiteParameter("@I" + i, data.I));
            //        paraList.Add(new SQLiteParameter("@J" + i, data.J));
            //        paraList.Add(new SQLiteParameter("@K" + i, data.K));

            //        paraList.Add(new SQLiteParameter("@L" + i, data.L));
            //        paraList.Add(new SQLiteParameter("@M" + i, data.M));
            //        paraList.Add(new SQLiteParameter("@N" + i, data.N));

            //        paraList.Add(new SQLiteParameter("@O" + i, data.O));
            //        //paraList.Add(new SQLiteParameter("@P" + i, data.P));
            //        //paraList.Add(new SQLiteParameter("@Q" + i, data.Q));

            //        //paraList.Add(new SQLiteParameter("@R" + i, data.R));
            //        //paraList.Add(new SQLiteParameter("@S" + i, data.S));
            //        //paraList.Add(new SQLiteParameter("@T" + i, data.T));
            //        //paraList.Add(new SQLiteParameter("@U" + i, data.U));
            //        //paraList.Add(new SQLiteParameter("@V" + i, data.V));

            //        //paraList.Add(new SQLiteParameter("@W" + i, data.W));
            //        //paraList.Add(new SQLiteParameter("@X" + i, data.X));
            //        //paraList.Add(new SQLiteParameter("@Y" + i, data.Y));
            //        //paraList.Add(new SQLiteParameter("@Z" + i, data.Z));
            //        i++;
            //    }
            //    SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
            //    {
            //        DataSource = path,
            //        //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
            //    };
            //    SQLiteConnection con = null;
            //    SQLiteCommand cmd = null;
            //    SQLiteTransaction tran = null;
            //    try
            //    {
            //        con = new SQLiteConnection(connectionString.ToString());
            //        con.Open();
            //        tran = con.BeginTransaction();
            //        cmd = new SQLiteCommand(sql.ToString(), con);

            //        if (paraList.Count != 0)
            //        {
            //            cmd.Parameters.AddRange(paraList.ToArray());
            //        }
            //        cmd.Transaction = tran;
            //        result = cmd.ExecuteNonQuery();
            //        tran.Commit();

            //    }
            //    catch (Exception ex)
            //    {
            //        log.Info(ex.Message, ex);
            //        tran.Rollback();
            //        throw new Exception("添加数据失败");
            //    }
            //    finally
            //    {
            //        con.Close();
            //    }
            //}
            //return result; 
            #endregion
        }
        /// <summary>
        /// 获取指定SOURCEID 下的数据个数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="evidenceId"></param>
        /// <param name="sourceId"></param>
        /// <param name="searchText">要检索的关键字</param>
        /// <returns></returns>
        public int GetCountBySourceID(String path, String evidenceId, String sourceId, String searchText)
        {
            int result = 0;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    String sql = " select count(*) from TBL_DATA where SOURCE_GUID in (" + sourceId.TrimEnd(',') + ")  and EVIDENCE_GUID=@EVIDENCE_GUID ";
                    if (searchText.Length != 0)
                    {
                        sql += String.Format(" and  (A like '%{0}%' or B like '%{0}%' or C like '%{0}%' or D like '%{0}%' or E like '%{0}%' or F like '%{0}%' or G like '%{0}%' or H like '%{0}%'  or I like '%{0}%' or J like '%{0}%' or K like '%{0}%' or L like '%{0}%'  or M like '%{0}%' or N like '%{0}%' or O like '%{0}%'  or P like '%{0}%'  or Q like '%{0}%' or R like '%{0}%' or S like '%{0}%' or T like '%{0}%'  or U like '%{0}%' or V like '%{0}%'  or W like '%{0}%'  or X like '%{0}%' or Y like '%{0}%' or Z like '%{0}%' ) ", searchText);
                    }
                    cmd = new SQLiteCommand(sql, con);

                    cmd.Parameters.AddRange(new SQLiteParameter[] { new SQLiteParameter("@EVIDENCE_GUID", evidenceId) });
                    con.Open();
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }
        /// <summary>
        /// 获取指定SOURCEID 下指定用户的数据个数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="evidenceId"></param>
        /// <param name="sourceId"></param>
        /// <param name="searchText">要检索的关键字</param>
        /// <returns></returns>
        public int GetCountBySourceID(String path, String evidenceId, String sourceId, String aVal, String searchText)
        {
            int result = 0;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    String sql = " select count(*) from TBL_DATA where SOURCE_GUID in (" + sourceId.TrimEnd(',') + ")  and EVIDENCE_GUID=@EVIDENCE_GUID and A ='" + aVal + "'";
                    if (searchText.Length != 0)
                    {
                        sql += String.Format(" and  (A like '%{0}%' or B like '%{0}%' or C like '%{0}%' or D like '%{0}%' or E like '%{0}%' or F like '%{0}%' or G like '%{0}%' or H like '%{0}%'  or I like '%{0}%' or J like '%{0}%' or K like '%{0}%' or L like '%{0}%'  or M like '%{0}%' or N like '%{0}%' or O like '%{0}%'  or P like '%{0}%'  or Q like '%{0}%' or R like '%{0}%' or S like '%{0}%' or T like '%{0}%'  or U like '%{0}%' or V like '%{0}%'  or W like '%{0}%'  or X like '%{0}%' or Y like '%{0}%' or Z like '%{0}%' ) ", searchText);
                    }
                    cmd = new SQLiteCommand(sql, con);

                    cmd.Parameters.AddRange(new SQLiteParameter[] { new SQLiteParameter("@EVIDENCE_GUID", evidenceId) });
                    con.Open();
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
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
            List<String> qqList = null;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select {0} from TBL_DATA where EVIDENCE_GUID ='{1}' and SOURCE_GUID in ({2})  group by {0}", columnName, evidenceId, sourceId.TrimEnd(','));
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        qqList = new List<string>();
                        while (reader.Read())
                        {
                            qqList.Add(reader[0].ToString());
                        }
                    }

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return qqList;
        }
        /// <summary>
        /// 获取数据信息
        /// </summary>
        /// <param name="path">库路径</param>
        /// <param name="evidenceId">物证ID </param>
        /// <param name="sourceId">来源ID </param>
        /// <param name="a">用户(QQID、 微信用户)</param>
        /// <param name="searchText">检索的关键字</param>
        /// <returns></returns>
        public List<Data> GetDataBySourceID_EvidenceId_A(String path, String evidenceId, String sourceId, String a, String searchText)
        {
            List<Data> dataList = null;
            if (File.Exists(path))
            {

                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select * from TBL_DATA where EVIDENCE_GUID ='{0}'  and SOURCE_GUID='{1}' and A='{2}'", evidenceId, sourceId, a);
                if (Data.IsFilter)
                {
                    sql += " and DATA_GUID  in (" + Data.FilterDataID + ") ";
                }
                if (searchText.Length != 0)
                {
                    sql += String.Format(" and  (A like '%{0}%' or B like '%{0}%' or C like '%{0}%' or D like '%{0}%' or E like '%{0}%' or F like '%{0}%' or G like '%{0}%' or H like '%{0}%'  or I like '%{0}%' or J like '%{0}%' or K like '%{0}%' or L like '%{0}%'  or M like '%{0}%' or N like '%{0}%' or O like '%{0}%'  or P like '%{0}%'  or Q like '%{0}%' or R like '%{0}%' or S like '%{0}%' or T like '%{0}%'  or U like '%{0}%' or V like '%{0}%'  or W like '%{0}%'  or X like '%{0}%' or Y like '%{0}%' or Z like '%{0}%' ) ", searchText);
                }
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dataList = new List<Data>();
                        while (reader.Read())
                        {
                            dataList.Add(new Data
                            {
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                                CASE_NAME = reader["CASE_NAME"].ToString(),
                                DATA_GUID = reader["DATA_GUID"].ToString(),
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                SOURCE_GUID = reader["SOURCE_GUID"].ToString(),
                                IF_DATA_ISSELECT = reader["IF_DATA_ISSELECT"].ToString(),
                                SOURCE_NAME = reader["SOURCE_NAME"].ToString(),
                                A = reader["A"].ToString(),
                                B = reader["B"].ToString(),
                                C = reader["C"].ToString(),
                                D = reader["D"].ToString(),
                                E = reader["E"].ToString(),
                                F = reader["F"].ToString(),
                                G = reader["G"].ToString(),
                                H = reader["H"].ToString(),
                                I = reader["I"].ToString(),
                                J = reader["J"].ToString(),
                                K = reader["K"].ToString(),
                                L = reader["L"].ToString(),
                                M = reader["M"].ToString(),
                                N = reader["N"].ToString(),
                                O = reader["O"].ToString(),
                                P = reader["P"].ToString(),
                                Q = reader["Q"].ToString(),
                                R = reader["R"].ToString(),
                                S = reader["S"].ToString(),
                                T = reader["T"].ToString(),
                                U = reader["U"].ToString(),
                                V = reader["V"].ToString(),
                                W = reader["W"].ToString(),
                                X = reader["X"].ToString(),
                                Y = reader["Y"].ToString(),
                                Z = reader["Z"].ToString(),
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return dataList;
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
            List<String> qqList = null;
            if (File.Exists(path))
            {

                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select {0} from TBL_DATA where EVIDENCE_GUID ='{1}' and SOURCE_GUID='{2}'  group by {0}", columnName, evidenceId, sourceId);
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        qqList = new List<string>();
                        while (reader.Read())
                        {
                            qqList.Add(reader[0].ToString());
                        }
                    }

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return qqList;
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
            List<Data> dataList = null;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select *  from TBL_DATA where EVIDENCE_GUID ='{0}' and SOURCE_GUID='{1}' and {2}='{3}' ", evidenceId, sourceId, columnName, columnValue);
                if (Data.IsFilter)
                {
                    sql += " and DATA_GUID  in (" + Data.FilterDataID + ") ";
                }
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dataList = new List<Data>();
                        while (reader.Read())
                        {
                            dataList.Add(new Data
                            {
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                                CASE_NAME = reader["CASE_NAME"].ToString(),
                                DATA_GUID = reader["DATA_GUID"].ToString(),
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                SOURCE_GUID = reader["SOURCE_GUID"].ToString(),
                                IF_DATA_ISSELECT = reader["IF_DATA_ISSELECT"].ToString(),
                                SOURCE_NAME = reader["SOURCE_NAME"].ToString(),
                                A = reader["A"].ToString(),
                                B = reader["B"].ToString(),
                                C = reader["C"].ToString(),
                                D = reader["D"].ToString(),
                                E = reader["E"].ToString(),
                                F = reader["F"].ToString(),
                                G = reader["G"].ToString(),
                                H = reader["H"].ToString(),
                                I = reader["I"].ToString(),
                                J = reader["J"].ToString(),
                                K = reader["K"].ToString(),
                                L = reader["L"].ToString(),
                                M = reader["M"].ToString(),
                                N = reader["N"].ToString(),
                                O = reader["O"].ToString(),
                                P = reader["P"].ToString(),
                                Q = reader["Q"].ToString(),
                                R = reader["R"].ToString(),
                                S = reader["S"].ToString(),
                                T = reader["T"].ToString(),
                                U = reader["U"].ToString(),
                                V = reader["V"].ToString(),
                                W = reader["W"].ToString(),
                                X = reader["X"].ToString(),
                                Y = reader["Y"].ToString(),
                                Z = reader["Z"].ToString(),
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return dataList;
        }
        /// <summary>
        /// 获取某一案件库下所有的物信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Evidence> GetAllEvidence(String path)
        {
            List<Evidence> evidenceList = null;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select * from TBL_DATA group by EVIDENCE_GUID");
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        evidenceList = new List<Evidence>();
                        while (reader.Read())
                        {
                            evidenceList.Add(new Evidence
                            {
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return evidenceList;
        }
        /// <summary>
        /// 获取某一物证下的来源
        /// </summary>
        /// <param name="casePath"></param>
        /// <param name="evidenceID"></param>
        /// <returns></returns>
        public List<Source> GetAllSourceByEvidence(String path, String evidenceID)
        {
            List<Source> sourceList = null;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select SOURCE_GUID,SOURCE_NAME from TBL_DATA  where EVIDENCE_GUID='{0}' group by SOURCE_GUID", evidenceID);
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        sourceList = new List<Source>();
                        while (reader.Read())
                        {
                            sourceList.Add(new Source
                            {
                                SOURCE_GUID = reader["SOURCE_GUID"].ToString(),
                                SOURCE_NAME = reader["SOURCE_NAME"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return sourceList;
        }
        /// <summary>
        /// 向物证表中添加物证信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="evidence"></param>
        public void AddEvidence(String path, Evidence evidence)
        {
            if (File.Exists(path))
            {

                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = @"insert into   TBL_EVIDENCE (EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK,CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,
                                                    OWNER_NAME,OWNER_SEX,OWNER_ID,OWNER_PHONENUMBER,COLLECTIONUNIT_CODE,COLLECTIONUNIT_NAME,COLLECTIONUNIT_PHONENUMBER,COLLECTIONPEOPLE_NAME,COLLECTIONPEOPLE_ID,EVIDENCE_FILE)  
                                values(@EVIDENCE_GUID,@EVIDENCE_NUMBER,@EVIDENCE_SENDER,@EVIDENCE_SENDERUNIT,@ADDTIME,@EVIDENCE_REMARK,@CASE_GUID,@QUZHENG_DATE,@EVIDENCE_JYR,@EVIDENCE_NAME,
                                       @OWNER_NAME,@OWNER_SEX,@OWNER_ID,@OWNER_PHONENUMBER,@COLLECTIONUNIT_CODE,@COLLECTIONUNIT_NAME,@COLLECTIONUNIT_PHONENUMBER,@COLLECTIONPEOPLE_NAME,@COLLECTIONPEOPLE_ID,@EVIDENCE_FILE)";
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;

                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();
                    cmd.Parameters.AddRange(new SQLiteParameter[]
                    {
                         new SQLiteParameter ("@EVIDENCE_GUID",evidence.EVIDENCE_GUID),
                           new SQLiteParameter ("@EVIDENCE_NUMBER",evidence.EVIDENCE_NUMBER),
                           new SQLiteParameter ("@EVIDENCE_SENDER",evidence.EVIDENCE_SENDER),
                           new SQLiteParameter ("@EVIDENCE_SENDERUNIT",evidence.EVIDENCE_SENDERUNIT),
                           new SQLiteParameter ("@ADDTIME",evidence.ADDTIME),
                           new SQLiteParameter ("@EVIDENCE_REMARK",evidence.EVIDENCE_REMARK),
                           new SQLiteParameter ("@CASE_GUID",evidence.CASE_GUID),
                           new SQLiteParameter ("@QUZHENG_DATE",evidence.QUZHENG_DATE),
                           new SQLiteParameter ("@EVIDENCE_JYR",evidence.EVIDENCE_JYR),
                           new SQLiteParameter("@EVIDENCE_NAME",evidence.EVIDENCE_NAME),
                           new SQLiteParameter("@OWNER_NAME",evidence.OWNER_NAME),
                           new SQLiteParameter("@OWNER_SEX",evidence.OWNER_SEX),
                           new SQLiteParameter("@OWNER_ID",evidence.OWNER_ID),
                           new SQLiteParameter("@OWNER_PHONENUMBER",evidence.OWNER_PHONENUMBER),
                           new SQLiteParameter("@COLLECTIONUNIT_CODE",evidence.COLLECTIONUNIT_CODE),
                           new SQLiteParameter("@COLLECTIONUNIT_NAME",evidence.COLLECTIONUNIT_NAME),
                           new SQLiteParameter("@COLLECTIONUNIT_PHONENUMBER",evidence.COLLECTIONUNIT_PHONENUMBER),
                           new SQLiteParameter("@COLLECTIONPEOPLE_NAME",evidence.COLLECTIONPEOPLE_NAME),
                           new SQLiteParameter("@COLLECTIONPEOPLE_ID",evidence.COLLECTIONPEOPLE_ID),
                           new SQLiteParameter("@EVIDENCE_FILE",evidence.FILE_PATH),
                    });
                    ;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("添加物证信息失败");
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }

        /// <summary>
        /// 向物证表中添加物证信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="evidence"></param>
        public void UpdateEvidence(String path, Evidence evidence)
        {
            if (File.Exists(path))
            {

                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = @"update  TBL_EVIDENCE  set EVIDENCE_FILE =@EVIDENCE_FILE where EVIDENCE_GUID=@EVIDENCE_GUID";
                //                                values(@EVIDENCE_GUID,@EVIDENCE_NUMBER,@EVIDENCE_SENDER,@EVIDENCE_SENDERUNIT,@ADDTIME,@EVIDENCE_REMARK,@CASE_GUID,@QUZHENG_DATE,@EVIDENCE_JYR,@EVIDENCE_NAME,
                //                                       @OWNER_NAME,@OWNER_SEX,@OWNER_ID,@OWNER_PHONENUMBER,@COLLECTIONUNIT_CODE,@COLLECTIONUNIT_NAME,@COLLECTIONUNIT_PHONENUMBER,@COLLECTIONPEOPLE_NAME,@COLLECTIONPEOPLE_ID,@EVIDENCE_FILE)";
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;

                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();
                    cmd.Parameters.AddRange(new SQLiteParameter[]
                    {
                         new SQLiteParameter ("@EVIDENCE_GUID",evidence.EVIDENCE_GUID),
                         //  new SQLiteParameter ("@EVIDENCE_NUMBER",evidence.EVIDENCE_NUMBER),
                         //  new SQLiteParameter ("@EVIDENCE_SENDER",evidence.EVIDENCE_SENDER),
                         //  new SQLiteParameter ("@EVIDENCE_SENDERUNIT",evidence.EVIDENCE_SENDERUNIT), 
                         //  new SQLiteParameter ("@ADDTIME",evidence.ADDTIME),
                         //  new SQLiteParameter ("@EVIDENCE_REMARK",evidence.EVIDENCE_REMARK), 
                         //  new SQLiteParameter ("@CASE_GUID",evidence.CASE_GUID),
                         //  new SQLiteParameter ("@QUZHENG_DATE",evidence.QUZHENG_DATE),
                         //  new SQLiteParameter ("@EVIDENCE_JYR",evidence.EVIDENCE_JYR),
                         //  new SQLiteParameter("@EVIDENCE_NAME",evidence.EVIDENCE_NAME),
                         //  new SQLiteParameter("@OWNER_NAME",evidence.OWNER_NAME),  
                         //  new SQLiteParameter("@OWNER_SEX",evidence.OWNER_SEX),  
                         //  new SQLiteParameter("@OWNER_ID",evidence.OWNER_ID),  
                         //  new SQLiteParameter("@OWNER_PHONENUMBER",evidence.OWNER_PHONENUMBER),  
                         //  new SQLiteParameter("@COLLECTIONUNIT_CODE",evidence.COLLECTIONUNIT_CODE),  
                         //  new SQLiteParameter("@COLLECTIONUNIT_NAME",evidence.COLLECTIONUNIT_NAME),  
                         //  new SQLiteParameter("@COLLECTIONUNIT_PHONENUMBER",evidence.COLLECTIONUNIT_PHONENUMBER),  
                         //  new SQLiteParameter("@COLLECTIONPEOPLE_NAME",evidence.COLLECTIONPEOPLE_NAME),  
                         //  new SQLiteParameter("@COLLECTIONPEOPLE_ID",evidence.COLLECTIONPEOPLE_ID),  
                           new SQLiteParameter("@EVIDENCE_FILE",evidence.FILE_PATH),
                    });
                    ;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("更新物证信息失败");
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }
        /// <summary>
        /// 获取此案件下所有的物证信息(用于案件导入使用)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Evidence> GetAllEvidences(String path)
        {
            List<Evidence> evidenceList = null;
            if (File.Exists(path))
            {

                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                //String sql = String.Format(" select   EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK,CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,EVIDENCE_FILE  from  TBL_EVIDENCE");
                String sql = String.Format(" select   EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK,CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,EVIDENCE_FILE,OWNER_NAME,OWNER_SEX,OWNER_ID,OWNER_PHONENUMBER,COLLECTIONUNIT_CODE,COLLECTIONUNIT_NAME,COLLECTIONUNIT_PHONENUMBER,COLLECTIONPEOPLE_NAME,COLLECTIONPEOPLE_ID  from  TBL_EVIDENCE");
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        evidenceList = new List<Evidence>();
                        while (reader.Read())
                        {
                            DateTime datetime = DateTime.Now;
                            if (reader["QUZHENG_DATE"].ToString() != "")
                                datetime = Convert.ToDateTime(reader["QUZHENG_DATE"].ToString());
                            evidenceList.Add(new Evidence
                            {
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                                ADDTIME = Convert.ToDateTime(reader["ADDTIME"]),
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                EVIDENCE_JYR = reader["EVIDENCE_JYR"].ToString(),
                                EVIDENCE_NUMBER = reader["EVIDENCE_NUMBER"].ToString(),
                                EVIDENCE_REMARK = reader["EVIDENCE_REMARK"].ToString(),
                                EVIDENCE_SENDER = reader["EVIDENCE_SENDER"].ToString(),
                                EVIDENCE_SENDERUNIT = reader["EVIDENCE_SENDERUNIT"].ToString(),
                                QUZHENG_DATE = datetime,
                                EVIDENCE_NAME = reader["EVIDENCE_NAME"].ToString(),
                                FILE_PATH = reader["EVIDENCE_FILE"].ToString(),
                                OWNER_NAME = reader["OWNER_NAME"].ToString(),
                                OWNER_SEX = reader["OWNER_SEX"].ToString(),
                                OWNER_ID = reader["OWNER_ID"].ToString(),
                                OWNER_PHONENUMBER = reader["OWNER_PHONENUMBER"].ToString(),
                                COLLECTIONUNIT_CODE = reader["COLLECTIONUNIT_CODE"].ToString(),
                                COLLECTIONUNIT_NAME = reader["COLLECTIONUNIT_NAME"].ToString(),
                                COLLECTIONUNIT_PHONENUMBER = reader["COLLECTIONUNIT_PHONENUMBER"].ToString(),
                                COLLECTIONPEOPLE_NAME = reader["COLLECTIONPEOPLE_NAME"].ToString(),
                                COLLECTIONPEOPLE_ID = reader["COLLECTIONPEOPLE_ID"].ToString(),
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return evidenceList;
        }



        /// <summary>
        /// 获取案件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Case GetCase(String path)
        {
            Case myCase = null;
            if (File.Exists(path))
            {

                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select  CASE_GUID,CASE_NUMBER,CASE_NAME,CASE_IMG,USER_GUID,ADDTIME,CASE_DESCRIPTION,CASE_REMARK,CASE_PATH  from TBL_CASE ");
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        myCase = new Case()
                        {
                            CASE_GUID = reader["CASE_GUID"].ToString(),
                            ADDTIME = Convert.ToDateTime(reader["ADDTIME"].ToString()),
                            CASE_IMG = reader["CASE_IMG"].ToString(),
                            CASE_NAME = reader["CASE_NAME"].ToString(),
                            CASE_NUMBER = reader["CASE_NUMBER"].ToString(),
                            CASE_PATH = reader["CASE_PATH"].ToString(),
                            CASE_REMARK = reader["CASE_REMARK"].ToString(),
                            CASE_DESCRIPTION = reader["CASE_DESCRIPTION"].ToString(),
                        };

                    }

                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return myCase;
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
            bool flag = false;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select count(*) from TBL_DATA where EVIDENCE_GUID='" + evidenceId + "' and  SOURCE_GUID='" + sourceId + "' ");
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    {
                        flag = true;
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
            return flag;
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
            bool flag = false;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select count(*) from TBL_DATA where EVIDENCE_GUID='" + evidenceId + "' and  SOURCE_GUID in (" + sourceIds + ")");
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    {
                        flag = true;
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
            return flag;
        }

        /// <summary>
        /// 根据ID 修改数据的选中状态
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <param name="isSelect"></param>
        public void UpdateIsSelectById(String path, String id, String isSelect)
        {
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = " update TBL_DATA set IF_DATA_ISSELECT='" + isSelect + "' where DATA_GUID='" + id + "' ";
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }
        /// <summary>
        /// 获取某个物证下的指定的多个SOURCE_GUID 的数据
        /// </summary>
        /// <param name="path">案件路径</param>
        /// <param name="evidenceId">物证ID</param>
        /// <param name="sourceIds">多个SOURCE_GUID 格式为：('2','3',)</param>
        public List<Data> GetDataBySourceIds(String path, String evidenceId, String sourceIds)
        {
            List<Data> dataList = null;
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = String.Format(" select * from TBL_DATA where EVIDENCE_GUID='{0}' and SOURCE_GUID in ({1}) ", evidenceId, sourceIds);
                if (Data.IsFilter)
                {
                    sql += " and DATA_GUID  in (" + Data.FilterDataID + ") ";
                }
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dataList = new List<Data>();
                        while (reader.Read())
                        {
                            dataList.Add(new Data
                            {
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                                CASE_NAME = reader["CASE_NAME"].ToString(),
                                DATA_GUID = reader["DATA_GUID"].ToString(),
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                SOURCE_GUID = reader["SOURCE_GUID"].ToString(),
                                IF_DATA_ISSELECT = reader["IF_DATA_ISSELECT"].ToString(),
                                SOURCE_NAME = reader["SOURCE_NAME"].ToString(),
                                A = reader["A"].ToString(),
                                B = reader["B"].ToString(),
                                C = reader["C"].ToString(),
                                D = reader["D"].ToString(),
                                E = reader["E"].ToString(),
                                F = reader["F"].ToString(),
                                G = reader["G"].ToString(),
                                H = reader["H"].ToString(),
                                I = reader["I"].ToString(),
                                J = reader["J"].ToString(),
                                K = reader["K"].ToString(),
                                L = reader["L"].ToString(),
                                M = reader["M"].ToString(),
                                N = reader["N"].ToString(),
                                O = reader["O"].ToString(),
                                P = reader["P"].ToString(),
                                Q = reader["Q"].ToString(),
                                R = reader["R"].ToString(),
                                S = reader["S"].ToString(),
                                T = reader["T"].ToString(),
                                U = reader["U"].ToString(),
                                V = reader["V"].ToString(),
                                W = reader["W"].ToString(),
                                X = reader["X"].ToString(),
                                Y = reader["Y"].ToString(),
                                Z = reader["Z"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return dataList;
        }
        /// <summary>
        /// 根据物证ID 删除数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="evidenceIds">物证ID </param> 
        public void DelDataByEvidenceId(String path, String evidenceIds)
        {
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };

                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteTransaction tran = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    con.Open();
                    tran = con.BeginTransaction();
                    cmd.Transaction = tran;

                    String sql = String.Format(" delete from TBL_DATA where EVIDENCE_GUID in ({0}) ;", evidenceIds.TrimEnd(','));
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //sql = " delete from TBL_EVIDENCE where EVIDENCE_GUID in (" + evidenceIds.TrimEnd(',') + ") ; ";
                    //cmd.CommandText = sql;
                    //cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();
                    log.Info(ex.Message, ex);
                    throw new Exception("删除数据失败");
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }

        public void DelByEvidenceId(String path, String evidenceIds)
        {
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };

                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteTransaction tran = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    con.Open();
                    tran = con.BeginTransaction();
                    cmd.Transaction = tran;

                    String sql = String.Format(" delete from TBL_EVIDENCE where EVIDENCE_GUID in ({0}) ;", evidenceIds.TrimEnd(','));
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //sql = " delete from TBL_EVIDENCE where EVIDENCE_GUID in (" + evidenceIds.TrimEnd(',') + ") ; ";
                    //cmd.CommandText = sql;
                    //cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();
                    log.Info(ex.Message, ex);
                    throw new Exception("删除数据失败");
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }
        /// <summary>
        /// 合并物证信息
        /// </summary>
        /// <param name="path">案件库路径</param>
        /// <param name="evidenceList">合并的物证列表</param>
        /// <param name="newEvidence">新物证信息</param>
        public void MergeEvidenceData(String path, List<Evidence> evidenceList, Evidence newEvidence, bool ibdelete)
        {
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String delId = "";
                String filePath = "";
                //for (int i = 0; i < evidenceList.Count; i++)
                //{
                //    delId += "'" + evidenceList[i].EVIDENCE_GUID + "',";
                //    filePath += evidenceList[i].FILE_PATH + "^";
                //}
                String sql = @" insert into   TBL_EVIDENCE (EVIDENCE_GUID,EVIDENCE_NUMBER,EVIDENCE_SENDER,EVIDENCE_SENDERUNIT,ADDTIME,EVIDENCE_REMARK,CASE_GUID,QUZHENG_DATE,EVIDENCE_JYR,EVIDENCE_NAME,EVIDENCE_FILE) values(@EVIDENCE_GUID,@EVIDENCE_NUMBER,@EVIDENCE_SENDER,@EVIDENCE_SENDERUNIT,@ADDTIME,@EVIDENCE_REMARK,@CASE_GUID,@QUZHENG_DATE,@EVIDENCE_JYR,@EVIDENCE_NAME,@EVIDENCE_FILE) ";
                //newEvidence.FILE_PATH = filePath.TrimEnd('^');
                //删除合并物证信息，重新录入新的物证信息 

                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteTransaction tran = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();
                    tran = con.BeginTransaction();
                    cmd.Transaction = tran;
                    cmd.Parameters.AddRange(new SQLiteParameter[]
                    {
                         new SQLiteParameter ("@EVIDENCE_GUID",newEvidence.EVIDENCE_GUID),
                           new SQLiteParameter ("@EVIDENCE_NUMBER",newEvidence.EVIDENCE_NUMBER),
                           new SQLiteParameter ("@EVIDENCE_SENDER",newEvidence.EVIDENCE_SENDER),
                           new SQLiteParameter ("@EVIDENCE_SENDERUNIT",newEvidence.EVIDENCE_SENDERUNIT),
                           new SQLiteParameter ("@ADDTIME",newEvidence.ADDTIME),
                           new SQLiteParameter ("@EVIDENCE_REMARK",newEvidence.EVIDENCE_REMARK),
                           new SQLiteParameter ("@CASE_GUID",newEvidence.CASE_GUID),
                           new SQLiteParameter ("@QUZHENG_DATE",newEvidence.QUZHENG_DATE),
                           new SQLiteParameter ("@EVIDENCE_JYR",newEvidence.EVIDENCE_JYR),
                           new SQLiteParameter("@EVIDENCE_NAME",newEvidence.EVIDENCE_NAME),
                           new SQLiteParameter("@EVIDENCE_FILE",newEvidence.FILE_PATH),
                    });
                    cmd.ExecuteNonQuery();
                    if (ibdelete)
                    {
                        sql = " delete from TBL_EVIDENCE where EVIDENCE_GUID in (" + delId.TrimEnd(',') + ")";
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                    //修改合并物证信息下的数据 
                    //sql = " update TBL_DATA set EVIDENCE_GUID ='" + newEvidence.EVIDENCE_GUID + "' where DATA_GUID in (select DATA_GUID from TBL_DATA where EVIDENCE_GUID in (" + delId.TrimEnd(',') + ") )";
                    //cmd.CommandText = sql;
                    //cmd.ExecuteNonQuery();

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();
                    log.Info(ex.Message, ex);
                    throw new Exception("合并物证信息失败");
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }

        /// <summary>
        /// 检索功能
        /// </summary>
        /// <param name="whereText"></param>
        /// <returns></returns>
        public List<Data> GetDataByWhere(String casePath, String whereText)
        {
            List<Data> dataList = null;
            if (File.Exists(casePath))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = casePath,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    String sql = " select  * from TBL_DATA    ";
                    sql += String.Format(" where A like '%{0}%' or B like '%{0}%' or C like '%{0}%' or D like '%{0}%' or E like '%{0}%' or F like '%{0}%' or G like '%{0}%' or H like '%{0}%'  or I like '%{0}%' or J like '%{0}%' or K like '%{0}%' or L like '%{0}%'  or M like '%{0}%' or N like '%{0}%' or O like '%{0}%'  or P like '%{0}%'  or Q like '%{0}%' or R like '%{0}%' or S like '%{0}%' or T like '%{0}%'  or U like '%{0}%' or V like '%{0}%'  or W like '%{0}%'  or X like '%{0}%' or Y like '%{0}%' or Z like '%{0}%'  ", whereText);
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dataList = new List<Data>();
                        while (reader.Read())
                        {
                            dataList.Add(new Data
                            {
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                                CASE_NAME = reader["CASE_NAME"].ToString(),
                                DATA_GUID = reader["DATA_GUID"].ToString(),
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                SOURCE_GUID = reader["SOURCE_GUID"].ToString(),
                                IF_DATA_ISSELECT = reader["IF_DATA_ISSELECT"].ToString(),
                                SOURCE_NAME = reader["SOURCE_NAME"].ToString(),
                                A = reader["A"].ToString(),
                                B = reader["B"].ToString(),
                                C = reader["C"].ToString(),
                                D = reader["D"].ToString(),
                                E = reader["E"].ToString(),
                                F = reader["F"].ToString(),
                                G = reader["G"].ToString(),
                                H = reader["H"].ToString(),
                                I = reader["I"].ToString(),
                                J = reader["J"].ToString(),
                                K = reader["K"].ToString(),
                                L = reader["L"].ToString(),
                                M = reader["M"].ToString(),
                                N = reader["N"].ToString(),
                                O = reader["O"].ToString(),
                                P = reader["P"].ToString(),
                                Q = reader["Q"].ToString(),
                                R = reader["R"].ToString(),
                                S = reader["S"].ToString(),
                                T = reader["T"].ToString(),
                                U = reader["U"].ToString(),
                                V = reader["V"].ToString(),
                                W = reader["W"].ToString(),
                                X = reader["X"].ToString(),
                                Y = reader["Y"].ToString(),
                                Z = reader["Z"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    reader.Close();
                    con.Close();
                }
            }
            return dataList;
        }

        private byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        private string GetTextFromHexString(string hexString)
        {
            try
            {
                UnicodeEncoding unicodeGo = new UnicodeEncoding();
                Byte[] encodedBytes;
                encodedBytes = strToToHexByte(hexString);
                string text = unicodeGo.GetString(encodedBytes);
                //if (text.Length >= 20)
                //{
                //    text = text.Substring(0, 20);
                //}

                return text;
            }
            catch (Exception ex)
            {
                return hexString;
            }
        }
        private string BytesToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 数据整理
        /// </summary>
        /// <param name="whereText"></param>
        /// <returns></returns>
        /// 
        public void ReorganizeData(String locationdb, String path, String caseId, String caseName, String evidenceId, bool isEnglish)
        {
            String sql = null;
            SQLiteConnection con = null;
            SQLiteCommand cmd = null;
            SQLiteCommand cmd2 = null;
            SQLiteCommand cmd3 = null;
            SQLiteDataReader reader = null;
            SQLiteDataReader reader2 = null;
            DbTransaction trans = null;

            string lg_source_guid = "";
            string lg_case_guid = caseId;
            string lg_evidence_guid = evidenceId;
            string lg_case_name = caseName;
            bool lg_isEnglish = isEnglish;

            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };

                #region 关联归属地
                //===========prepare the address db ==============/
                string ls = locationdb;
                string C_conn = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + ls + ";Jet OLEDB:Database Password=ecryan2015;";
                OleDbConnection conn_ole = new OleDbConnection(C_conn);
                conn_ole.Open();
                //-----------------get the dictroiony------------------------//

                IDictionary<string, string> di = new Dictionary<string, string>();
                string sqladdressall = "select num,des from mobile_region";
                OleDbDataAdapter mydball = new OleDbDataAdapter(sqladdressall, C_conn);
                DataTable dtall = new DataTable();
                mydball.Fill(dtall);
                for (int l = 0; l < dtall.Rows.Count; l++)
                {
                    di.Add(dtall.Rows[l][0].ToString(), dtall.Rows[l][1].ToString());
                }
                conn_ole.Close();
                conn_ole.Dispose();

                #region 电话本归属地
                #region 电话簿（逻辑）
                //lg_source_name = "电话簿";
                lg_source_guid = "9";
                sql = String.Format(" select  DATA_GUID,E  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["E"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))
                            {

                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();

                                lg_source_guid = "9";

                                string strSQL2 = @"update TBL_DATA set F='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Contact(Logic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：电话薄（逻辑）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 电话簿（物理）
                //lg_source_name = "电话簿";
                lg_source_guid = "31";
                sql = String.Format(" select  DATA_GUID,B  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["B"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {

                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "电话簿";
                                lg_source_guid = "31";

                                string strSQL2 = @"update TBL_DATA set H='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Contact(Physic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：电话薄（物理）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 电话簿（删除）
                //lg_source_name = "电话簿";
                lg_source_guid = "35";
                sql = String.Format(" select  DATA_GUID,B  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["B"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);
                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "电话簿";
                                lg_source_guid = "35";

                                string strSQL2 = @"update TBL_DATA set H='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Contact(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：电话薄（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion
                #endregion

                #region 通话记录归属地
                #region 通话记录(逻辑)
                //lg_source_name = "通话记录";
                lg_source_guid = "8";
                sql = String.Format(" select  DATA_GUID,A  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["A"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "8";

                                string strSQL2 = @"update TBL_DATA set G='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log(Logic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录（逻辑）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录(物理)
                //lg_source_name = "通话记录";
                lg_source_guid = "32";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {

                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "32";

                                string strSQL2 = @"update TBL_DATA set H='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log(Physic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录（物理）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录(删除)
                //lg_source_name = "通话记录";
                lg_source_guid = "36";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "36";

                                string strSQL2 = @"update TBL_DATA set H='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录日志
                //lg_source_name = "通话记录";
                lg_source_guid = "38";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {

                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "38";

                                string strSQL2 = @"update TBL_DATA set G='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log log");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录日志");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录日志（删除）
                //lg_source_name = "通话记录";
                lg_source_guid = "40";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "40";

                                string strSQL2 = @"update TBL_DATA set G='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log log(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录日志（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion
                #endregion

                #region 短信归属地
                #region 短信（逻辑）
                //lg_source_name = "短信";
                lg_source_guid = "7";
                sql = String.Format(" select  DATA_GUID,A  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["A"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set I='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS(Logic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信（逻辑）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信（物理）
                //lg_source_name = "短信";
                lg_source_guid = "30";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set I='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS(Physic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信（物理）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信（删除）
                //lg_source_name = "短信";
                lg_source_guid = "34";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set I='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信日志
                //lg_source_name = "短信";
                lg_source_guid = "37";
                sql = String.Format(" select  DATA_GUID,B  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();

                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["B"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {

                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set F='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS log");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信日志");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信日志（删除）
                //lg_source_name = "短信";
                lg_source_guid = "39";
                sql = String.Format(" select  DATA_GUID,B  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["B"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set F='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS log(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信日志（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion
                #endregion
                #endregion

                #region 关联微信好友电话号码
                lg_source_guid = "12";
                sql = String.Format(" select DATA_GUID,C  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' and c <>''");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = reader["C"].ToString().Trim();

                            lg_source_guid = "9";
                            string strSQL = @"select E from TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' and D = '" + lsnum + "%'";

                            cmd2 = con.CreateCommand();
                            cmd2.CommandText = strSQL;
                            reader2 = cmd2.ExecuteReader();

                            if (reader2.HasRows)
                            {
                                reader2.Read();
                                string lstmp = reader2["E"].ToString().Trim();

                                string strSQL2 = @"update TBL_DATA set O='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";

                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ToString(count));
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate WeChat Friend phone number");
                    //else
                    //    throw new Exception("关联微信好友列表电话号码失败");
                }
                if (reader != null)
                    reader.Close();
                if (reader2 != null)
                    reader2.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信合并
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    con.Open();
                    trans = con.BeginTransaction();

                    // 插入逻辑提取数据插入到临时表中
                    lg_source_guid = "7";
                    sql = String.Format("insert into tbl_tmp (A,B,C,D,F,G,H) select F,C,A,H,B,E,I from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' ");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    //将逻辑未提取到的物理数据插入到临时表中                   
                    lg_source_guid = "30";
                    sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F,G,H) select B,C,D,E,F,G,H,I from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.G not in (select F from tbl_tmp)");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    //将删除未提取到的日志数据插入到临时表中    
                    lg_source_guid = "37";
                    sql = String.Format("insert into tbl_tmp (C,G,F,A,H) select B,C,D,E,G  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.D not in (select F from tbl_tmp)");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    //将合并后的数据重新写入tbl_data    
                    lg_source_guid = "41";
                    sql = String.Format("select A,B,C,D,E,F,G,H,I from tbl_tmp");
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (lg_isEnglish)
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                    Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','SMS','" + reader["A"] +
                                                    "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "','" + reader["I"] + "')");
                            }
                            else
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                        Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','短信息','" + reader["A"] +
                                                        "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "','" + reader["I"] + "')");
                            }
                            cmd2 = con.CreateCommand();
                            cmd2.CommandText = sql;
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();

                    //清空临时表    
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();

                    //trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();
                    //trans.Commit();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to Combin SMS");
                    //else
                    //    throw new Exception("合并短信失败");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信(删除)合并
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    con.Open();
                    trans = con.BeginTransaction();

                    //删除数据插入到临时表中    
                    lg_source_guid = "34";
                    sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F,G,H) select B,C,D,E,F,G,H,I from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "'");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();


                    //将日志未提取到的日志删除数据插入到临时表中    
                    lg_source_guid = "39";
                    sql = String.Format("insert into tbl_tmp (C,G,F,A,H) select B,C,D,E,G  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.D not in (select F from tbl_tmp)");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //trans.Commit();

                    //将合并后的数据重新写入tbl_data    
                    lg_source_guid = "81";
                    sql = String.Format("select A,B,C,D,E,F,G,H,I from tbl_tmp");
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (lg_isEnglish)
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                    Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','SMS(Deleted)','" + reader["A"] +
                                                    "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "','" + reader["I"] + "')");
                            }
                            else
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                        Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','短信息(删除)','" + reader["A"] +
                                                        "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "','" + reader["I"] + "')");
                            }
                            cmd2 = con.CreateCommand();
                            cmd2.CommandText = sql;
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();

                    //清空临时表    
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();

                    //trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 电话簿合并
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    con.Open();
                    trans = con.BeginTransaction();

                    // 插入逻辑提取数据插入到临时表中
                    lg_source_guid = "9";
                    sql = String.Format("insert into tbl_tmp (A,B,C,G) select D,E,A,F from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' ");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //trans.Commit();

                    //将逻辑未提取到的物理数据插入到临时表中                   
                    lg_source_guid = "31";
                    sql = String.Format("insert into tbl_tmp (A,B,D,E,F,G) select C,B,F,D,E,H  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.a not in (select tbl_data.a from tbl_data,tbl_tmp where tbl_data.c = tbl_tmp.a and tbl_data.b = tbl_tmp.b)");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    //将合并后的数据重新写入tbl_data    
                    lg_source_guid = "42";
                    sql = String.Format("select A,B,C,D,E,F,G,H  from tbl_tmp");
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (lg_isEnglish)
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','Contact','" + reader["A"] +
                                                "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                            }
                            else
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                     Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','电话簿','" + reader["A"] +
                                                     "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                            }
                            cmd2 = con.CreateCommand();
                            cmd2.CommandText = sql;
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();

                    //清空临时表    
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();

                    //trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();
                    //trans.Commit();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to Combin Contact");
                    //else
                    //    throw new Exception("合并电话簿失败");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 电话簿（删除）合并
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    con.Open();
                    trans = con.BeginTransaction();

                    //将物理未提取到的删除数据插入到临时表中    
                    lg_source_guid = "35";
                    sql = String.Format("insert into tbl_tmp (A,B,D,E,F,G) select C,B,F,D,E,H from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "'");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //trans.Commit();

                    //将合并后的数据重新写入tbl_data    
                    lg_source_guid = "82";
                    sql = String.Format("select A,B,C,D,E,F,G,H  from tbl_tmp");
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (lg_isEnglish)
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','Contact(Deleted)','" + reader["A"] +
                                                "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                            }
                            else
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                     Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','电话簿(删除)','" + reader["A"] +
                                                     "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                            }
                            cmd2 = con.CreateCommand();
                            cmd2.CommandText = sql;
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();

                    //清空临时表    
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();

                    //trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录合并
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    con.Open();
                    trans = con.BeginTransaction();
                    // 插入逻辑提取数据插入到临时表中
                    lg_source_guid = "8";
                    sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,A,E,C,D,G from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' ");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //trans.Commit();

                    //将逻辑未提取到的物理数据插入到临时表中                   
                    lg_source_guid = "32";
                    sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,D,G,F,C,H from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.F not in (select D from tbl_tmp)");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //trans.Commit();

                    //将删除未提取到的日志数据插入到临时表中    
                    lg_source_guid = "38";
                    sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,D,F,E,C,G  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.E not in (select D from tbl_tmp)");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    //将合并后的数据重新写入tbl_data    
                    lg_source_guid = "43";
                    sql = String.Format("select A,B,C,D,E,F,G  from tbl_tmp");
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (lg_isEnglish)
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','Call log','" + reader["A"] +
                                                "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                            }
                            else
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                     Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','通话记录','" + reader["A"] +
                                                     "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                            }
                            cmd2 = con.CreateCommand();
                            cmd2.CommandText = sql;
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    //清空临时表    
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();
                    //trans.Commit();

                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();
                    //trans.Commit();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to Combin Call log");
                    //else
                    //    throw new Exception("合并通话记录");
                }
                #endregion

                #region 通话记录(删除)合并
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    con.Open();
                    trans = con.BeginTransaction();

                    //将物理未提取到的删除数据插入到临时表中    
                    lg_source_guid = "36";
                    sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,D,G,F,C,H from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "'");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //trans.Commit();

                    //将日志未提取到的日志删除数据插入到临时表中    
                    lg_source_guid = "40";
                    sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,D,F,E,C,G  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.E not in (select D from tbl_tmp)");

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    //trans.Commit();

                    //将合并后的数据重新写入tbl_data    
                    lg_source_guid = "83";
                    sql = String.Format("select A,B,C,D,E,F,G  from tbl_tmp");
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (lg_isEnglish)
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','Call log','" + reader["A"] +
                                                "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                            }
                            else
                            {
                                sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                     Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','通话记录','" + reader["A"] +
                                                     "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                            }
                            cmd2 = con.CreateCommand();
                            cmd2.CommandText = sql;
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    //清空临时表    
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();
                    //trans.Commit();

                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    sql = String.Format("delete from tbl_tmp");
                    cmd3 = con.CreateCommand();
                    cmd3.CommandText = sql;
                    cmd3.ExecuteNonQuery();
                    //trans.Commit();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to Combin Call log");
                    //else
                    //    throw new Exception("合并通话记录");
                }
                #endregion
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (reader2 != null)
                        reader2.Close();
                    if (con != null)
                        con.Close();
                }
            }
        }


        /// <summary>
        /// 数据整理
        /// </summary>
        /// <param name="whereText"></param>
        /// <returns></returns>
        /// 
        public void ReorganizeAllData(String locationdb, String path, String caseId, String caseName, bool isEnglish)
        {
            String sql = null;
            SQLiteConnection con = null;
            SQLiteCommand cmd = null;
            SQLiteCommand cmd2 = null;
            SQLiteCommand cmd3 = null;
            SQLiteDataReader reader = null;
            SQLiteDataReader reader2 = null;
            DbTransaction trans = null;

            string lg_source_guid = "";
            List<Evidence> evidenceList;
            string lg_case_guid = caseId;
            string lg_evidence_guid = "";
            string lg_case_name = caseName;
            bool lg_isEnglish = isEnglish;
            string ls_hex = "";
            string ls_flag = "none";  //201505 add for al logic inport
            if (File.Exists(path))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = path,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };

                #region 关联归属地
                //===========prepare the address db ==============/
                string ls = locationdb;
                string C_conn = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + ls + ";Jet OLEDB:Database Password=;";
                OleDbConnection conn_ole = new OleDbConnection(C_conn);
                conn_ole.Open();
                //-----------------get the dictroiony------------------------//

                IDictionary<string, string> di = new Dictionary<string, string>();
                string sqladdressall = "select MobileNumber,MobileArea&MobileType from Dm_Mobile";//201508 change to data.mdb "select num,des from mobile_region";
                OleDbDataAdapter mydball = new OleDbDataAdapter(sqladdressall, C_conn);
                DataTable dtall = new DataTable();
                mydball.Fill(dtall);
                for (int l = 0; l < dtall.Rows.Count; l++)
                {
                    di.Add(dtall.Rows[l][0].ToString(), dtall.Rows[l][1].ToString());
                }
                conn_ole.Close();
                conn_ole.Dispose();

                #region 电话本归属地
                #region 电话簿（逻辑）
                //lg_source_name = "电话簿";
                lg_source_guid = "9";
                sql = String.Format(" select  DATA_GUID,E  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = ""; //201505 add for accept the logical from al project su
                            if (ls_flag == "AL" || reader["E"].ToString().IndexOf("电话 1：") == 0)
                            {
                                ls_flag = "AL";
                                lsnum = reader["E"].ToString().Trim();
                                lsnum = lsnum.Substring(5, lsnum.IndexOf(",") - 5);
                                // ls_hex = BytesToHexString(System.Text.Encoding.Unicode.GetBytes(lsnum));
                            }
                            else
                            {
                                lsnum = GetTextFromHexString(reader["E"].ToString().Trim());
                            }
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))
                            {

                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();

                                lg_source_guid = "9";

                                string strSQL2 = @"update TBL_DATA set F='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Contact(Logic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：电话薄（逻辑）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 电话簿（物理）
                //lg_source_name = "电话簿";
                lg_source_guid = "31";
                sql = String.Format(" select  DATA_GUID,B  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["B"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {

                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "电话簿";
                                lg_source_guid = "31";

                                string strSQL2 = @"update TBL_DATA set H='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Contact(Physic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：电话薄（物理）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 电话簿（删除）
                //lg_source_name = "电话簿";
                lg_source_guid = "35";
                sql = String.Format(" select  DATA_GUID,B  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["B"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);
                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "电话簿";
                                lg_source_guid = "35";

                                string strSQL2 = @"update TBL_DATA set H='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Contact(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：电话薄（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion
                #endregion

                #region 通话记录归属地
                #region 通话记录(逻辑)
                //lg_source_name = "通话记录";
                lg_source_guid = "8";
                sql = String.Format(" select  DATA_GUID,A  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {

                            string lsnum = reader["A"].ToString().Trim();  //201505 add for accept the logical from al project su
                            if (ls_flag == "AL" || lsnum.Substring(0, 1) == "3")   //3031 start with 3 then hex data
                            {
                                //ls_hex = lsnum;
                                lsnum = GetTextFromHexString(reader["A"].ToString().Trim());
                            }
                            //else
                            //{

                            //    //ls_hex = BytesToHexString(System.Text.Encoding.Unicode.GetBytes(lsnum));
                            //}
                            //GetTextFromHexString(reader["A"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "8";

                                string strSQL2 = @"update TBL_DATA set G='" + lstmp + "', where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log(Logic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录（逻辑）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录(物理)
                //lg_source_name = "通话记录";
                lg_source_guid = "32";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {

                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "32";

                                string strSQL2 = @"update TBL_DATA set H='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log(Physic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录（物理）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录(删除)
                //lg_source_name = "通话记录";
                lg_source_guid = "36";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "36";

                                string strSQL2 = @"update TBL_DATA set H='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录日志
                //lg_source_name = "通话记录";
                lg_source_guid = "38";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {

                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "38";

                                string strSQL2 = @"update TBL_DATA set G='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log log");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录日志");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 通话记录日志（删除）
                //lg_source_name = "通话记录";
                lg_source_guid = "40";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();//dt.Rows[0][0].ToString();
                                //lg_source_name = "通话记录";
                                lg_source_guid = "40";

                                string strSQL2 = @"update TBL_DATA set G='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:Call log log(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：通话记录日志（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion
                #endregion

                #region 短信归属地
                #region 短信（逻辑）
                //lg_source_name = "短信";
                lg_source_guid = "7";
                sql = String.Format(" select  DATA_GUID,A  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = reader["A"].ToString().Trim();  //201505 add for accept the logical from al project su
                            if (ls_flag == "AL" || lsnum.Substring(0, 1) == "3")   //3031 start with 3 then hex data
                            {
                                //ls_hex = lsnum;
                                lsnum = GetTextFromHexString(reader["A"].ToString().Trim());
                            }
                            //else
                            //{
                            //    ls_hex = BytesToHexString(System.Text.Encoding.Unicode.GetBytes(lsnum));
                            //}
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set I='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS(Logic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信（逻辑）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信（物理）
                //lg_source_name = "短信";
                lg_source_guid = "30";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set I='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS(Physic)");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信（物理）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信（删除）
                //lg_source_name = "短信";
                lg_source_guid = "34";
                sql = String.Format(" select  DATA_GUID,D  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["D"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set I='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信日志
                //lg_source_name = "短信";
                lg_source_guid = "37";
                sql = String.Format(" select  DATA_GUID,B  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();

                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["B"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {

                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set F='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS log");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信日志");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion

                #region 短信日志（删除）
                //lg_source_name = "短信";
                lg_source_guid = "39";
                sql = String.Format(" select  DATA_GUID,B  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' ");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = GetTextFromHexString(reader["B"].ToString().Trim());
                            //lsnum.Replace("+", "");
                            if (lsnum.IndexOf("+86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(lsnum.IndexOf("+86") + 3, lsnum.Length - lsnum.IndexOf("+86") - 3);
                            if (lsnum.IndexOf("86") == 0 && lsnum.Length > 12)
                                lsnum = lsnum.Substring(2, lsnum.Length - 2);
                            if (lsnum.Length > 7)
                                lsnum = lsnum.Substring(0, 7);

                            if (di.ContainsKey(lsnum))//dt.Rows.Count > 0)
                            {
                                string lstmp = di[lsnum].ToString();
                                string strSQL2 = @"update TBL_DATA set F='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";
                                //SQLiteCore.SQLiteHelper.ExecuteNonQuery(lg_case_dbfile, strSQL2);
                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate location:SMS log(Deleted)");
                    //else
                    //    throw new Exception("关联归属地信息失败：短信日志（删除）");
                }
                if (reader != null)
                    reader.Close();
                if (con != null)
                    con.Close();
                #endregion
                #endregion
                #endregion

                #region 关联微信好友电话号码
                lg_source_guid = "12";
                sql = String.Format(" select DATA_GUID,C  from  TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' and c <>''");
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    con.Open();

                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        trans = con.BeginTransaction();
                        while (reader.Read())
                        {
                            string lsnum = reader["C"].ToString().Trim();

                            lg_source_guid = "9";
                            string strSQL = @"select E from TBL_DATA where SOURCE_GUID='" + lg_source_guid + "' and D = '" + lsnum + "%'";

                            cmd2 = con.CreateCommand();
                            cmd2.CommandText = strSQL;
                            reader2 = cmd2.ExecuteReader();

                            if (reader2.HasRows)
                            {
                                reader2.Read();
                                string lstmp = reader2["E"].ToString().Trim();

                                string strSQL2 = @"update TBL_DATA set O='" + lstmp + "' where DATA_GUID='" + reader["DATA_GUID"].ToString() + "'";

                                cmd3 = con.CreateCommand();
                                cmd3.CommandText = strSQL2;
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    //MessageBox.Show(ToString(count));
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show(sql);
                    //if (lg_isEnglish)
                    //    throw new Exception("Failed to relate WeChat Friend phone number");
                    //else
                    //    throw new Exception("关联微信好友列表电话号码失败");
                }
                if (reader != null)
                    reader.Close();
                if (reader2 != null)
                    reader2.Close();
                if (con != null)
                    con.Close();
                #endregion

                evidenceList = GetAllEvidences(path);
                if (evidenceList != null)
                {
                    foreach (Evidence evidence in evidenceList)
                    {
                        lg_evidence_guid = evidence.EVIDENCE_GUID;

                        #region 短信合并
                        try
                        {
                            con = new SQLiteConnection(connectionString.ToString());
                            cmd = con.CreateCommand();
                            con.Open();
                            trans = con.BeginTransaction();

                            // 插入逻辑提取数据插入到临时表中
                            lg_source_guid = "7";
                            sql = String.Format("insert into tbl_tmp (A,B,C,D,F,G,H) select F,C,A,H,B,E,I from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' ");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();

                            //将逻辑未提取到的物理数据插入到临时表中                   
                            lg_source_guid = "30";
                            sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F,G,H) select B,C,D,E,F,G,H,I from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.G not in (select F from tbl_tmp)");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();

                            //将删除未提取到的日志数据插入到临时表中    
                            lg_source_guid = "37";
                            sql = String.Format("insert into tbl_tmp (C,G,F,A,H) select B,C,D,E,G  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.D not in (select F from tbl_tmp)");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //清理以往整理过的数据  201505add su
                            sql = String.Format("delete from TBL_DATA where EVIDENCE_GUID ='" + lg_evidence_guid + "' and source_guid='41'  ");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //将合并后的数据重新写入tbl_data    
                            lg_source_guid = "41";
                            sql = String.Format("select A,B,C,D,E,F,G,H,I from tbl_tmp");
                            cmd.CommandText = sql;
                            reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (lg_isEnglish)
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                            Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','SMS','" + reader["A"] +
                                                            "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "','" + reader["I"] + "')");
                                    }
                                    else
                                    {
                                        if (ls_flag == "AL")
                                        {
                                            sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                                Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','短信息','" + reader["A"] +
                                                                "','" + reader["B"] + "','" + BytesToHexString(System.Text.Encoding.Unicode.GetBytes(reader["C"].ToString().Trim())) + "','" + BytesToHexString(System.Text.Encoding.Unicode.GetBytes(reader["D"].ToString().Trim())) + "','" + reader["E"] + "','" + reader["F"] + "','" + BytesToHexString(System.Text.Encoding.Unicode.GetBytes(reader["G"].ToString().Trim())) + "','" + reader["H"] + "','" + reader["I"] + "')");

                                        }
                                        else
                                        {
                                            sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                                    Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','短信息','" + reader["A"] +
                                                                    "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "','" + reader["I"] + "')");
                                        }
                                    }
                                    cmd2 = con.CreateCommand();
                                    cmd2.CommandText = sql;
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();

                            //清空临时表    
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();

                            //trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();
                            //trans.Commit();
                            //MessageBox.Show(ex.Message);
                            //MessageBox.Show(sql);
                            //if (lg_isEnglish)
                            //    throw new Exception("Failed to Combin SMS");
                            //else
                            //    throw new Exception("合并短信失败");
                        }
                        if (reader != null)
                            reader.Close();
                        if (con != null)
                            con.Close();
                        #endregion

                        #region 短信(删除)合并
                        try
                        {
                            con = new SQLiteConnection(connectionString.ToString());
                            cmd = con.CreateCommand();
                            con.Open();
                            trans = con.BeginTransaction();

                            //删除数据插入到临时表中    
                            lg_source_guid = "34";
                            sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F,G,H) select B,C,D,E,F,G,H,I from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "'");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();


                            //将日志未提取到的日志删除数据插入到临时表中    
                            lg_source_guid = "39";
                            sql = String.Format("insert into tbl_tmp (C,G,F,A,H) select B,C,D,E,G  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.D not in (select F from tbl_tmp)");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //trans.Commit();

                            //将合并后的数据重新写入tbl_data    
                            lg_source_guid = "81";
                            sql = String.Format("select A,B,C,D,E,F,G,H,I from tbl_tmp");
                            cmd.CommandText = sql;
                            reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (lg_isEnglish)
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                            Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','SMS(Deleted)','" + reader["A"] +
                                                            "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "','" + reader["I"] + "')");
                                    }
                                    else
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,C,D,B,A,G,E,F,H,I) values( '" +
                                                                Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','短信息(删除)','" + reader["A"] +
                                                                "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "','" + reader["I"] + "')");
                                    }
                                    cmd2 = con.CreateCommand();
                                    cmd2.CommandText = sql;
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();

                            //清空临时表    
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();

                            //trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();
                        }
                        if (reader != null)
                            reader.Close();
                        if (con != null)
                            con.Close();
                        #endregion

                        #region 电话簿合并
                        try
                        {
                            con = new SQLiteConnection(connectionString.ToString());
                            cmd = con.CreateCommand();
                            con.Open();
                            trans = con.BeginTransaction();

                            // 插入逻辑提取数据插入到临时表中
                            lg_source_guid = "9";
                            sql = String.Format("insert into tbl_tmp (A,B,C,G) select D,E,A,F from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' ");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //trans.Commit();

                            //将逻辑未提取到的物理数据插入到临时表中                   
                            lg_source_guid = "31";
                            sql = String.Format("insert into tbl_tmp (A,B,D,E,F,G) select C,B,F,D,E,H  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.a not in (select tbl_data.a from tbl_data,tbl_tmp where tbl_data.c = tbl_tmp.a and tbl_data.b = tbl_tmp.b)");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();

                            //清理以往整理过的数据  201505add su
                            sql = String.Format("delete from TBL_DATA where EVIDENCE_GUID ='" + lg_evidence_guid + "' and source_guid='42'  ");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //将合并后的数据重新写入tbl_data    
                            lg_source_guid = "42";
                            sql = String.Format("select A,B,C,D,E,F,G,H  from tbl_tmp");
                            cmd.CommandText = sql;
                            reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (lg_isEnglish)
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                        Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','Contact','" + reader["A"] +
                                                        "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                                    }
                                    else
                                    {
                                        if (ls_flag == "AL")
                                        {
                                            sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                                  Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','电话簿','" + BytesToHexString(System.Text.Encoding.Unicode.GetBytes(reader["A"].ToString().Trim())) +
                                                                  "','" + BytesToHexString(System.Text.Encoding.Unicode.GetBytes(reader["B"].ToString().Trim())) + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                                        }
                                        else
                                        {
                                            sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                                 Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','电话簿','" + reader["A"] +
                                                                 "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                                        }
                                    }
                                    cmd2 = con.CreateCommand();
                                    cmd2.CommandText = sql;
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();

                            //清空临时表    
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();

                            //trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();
                            //trans.Commit();
                            //MessageBox.Show(ex.Message);
                            //MessageBox.Show(sql);
                            //if (lg_isEnglish)
                            //    throw new Exception("Failed to Combin Contact");
                            //else
                            //    throw new Exception("合并电话簿失败");
                        }
                        if (reader != null)
                            reader.Close();
                        if (con != null)
                            con.Close();
                        #endregion

                        #region 电话簿（删除）合并
                        try
                        {
                            con = new SQLiteConnection(connectionString.ToString());
                            cmd = con.CreateCommand();
                            con.Open();
                            trans = con.BeginTransaction();

                            //将物理未提取到的删除数据插入到临时表中    
                            lg_source_guid = "35";
                            sql = String.Format("insert into tbl_tmp (A,B,D,E,F,G) select C,B,F,D,E,H from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "'");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //trans.Commit();

                            //将合并后的数据重新写入tbl_data    
                            lg_source_guid = "82";
                            sql = String.Format("select A,B,C,D,E,F,G,H  from tbl_tmp");
                            cmd.CommandText = sql;
                            reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (lg_isEnglish)
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                        Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','Contact(Deleted)','" + reader["A"] +
                                                        "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                                    }
                                    else
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G,H) values( '" +
                                                             Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','电话簿(删除)','" + reader["A"] +
                                                             "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "','" + reader["H"] + "')");
                                    }
                                    cmd2 = con.CreateCommand();
                                    cmd2.CommandText = sql;
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();

                            //清空临时表    
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();

                            //trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();
                        }
                        if (reader != null)
                            reader.Close();
                        if (con != null)
                            con.Close();
                        #endregion

                        #region 通话记录合并
                        try
                        {
                            con = new SQLiteConnection(connectionString.ToString());
                            cmd = con.CreateCommand();
                            con.Open();
                            trans = con.BeginTransaction();
                            // 插入逻辑提取数据插入到临时表中
                            lg_source_guid = "8";
                            sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,A,E,C,D,G from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' ");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //trans.Commit();

                            //将逻辑未提取到的物理数据插入到临时表中                   
                            lg_source_guid = "32";
                            sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,D,G,F,C,H from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.F not in (select D from tbl_tmp)");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //trans.Commit();

                            //将删除未提取到的日志数据插入到临时表中    
                            lg_source_guid = "38";
                            sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,D,F,E,C,G  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.E not in (select D from tbl_tmp)");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //清理以往整理过的数据  201505add su
                            sql = String.Format("delete from TBL_DATA where EVIDENCE_GUID ='" + lg_evidence_guid + "' and source_guid='43'  ");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //将合并后的数据重新写入tbl_data    
                            lg_source_guid = "43";
                            sql = String.Format("select A,B,C,D,E,F,G  from tbl_tmp");
                            cmd.CommandText = sql;
                            reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (lg_isEnglish)
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                        Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','Call log','" + reader["A"] +
                                                        "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                                    }
                                    else
                                    {
                                        if (ls_flag == "AL")
                                        {
                                            sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                                 Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','通话记录','" + BytesToHexString(System.Text.Encoding.Unicode.GetBytes(reader["A"].ToString().Trim())) +
                                                                 "','" + BytesToHexString(System.Text.Encoding.Unicode.GetBytes(reader["B"].ToString().Trim())) + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                                        }
                                        else
                                        {
                                            sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                                 Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','通话记录','" + reader["A"] +
                                                                 "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                                        }
                                    }
                                    cmd2 = con.CreateCommand();
                                    cmd2.CommandText = sql;
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();
                            //清空临时表    
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();
                            //trans.Commit();

                            if (reader != null)
                                reader.Close();
                            if (con != null)
                                con.Close();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();
                            //trans.Commit();
                            //MessageBox.Show(ex.Message);
                            //MessageBox.Show(sql);
                            //if (lg_isEnglish)
                            //    throw new Exception("Failed to Combin Call log");
                            //else
                            //    throw new Exception("合并通话记录");
                        }
                        #endregion

                        #region 通话记录(删除)合并
                        try
                        {
                            con = new SQLiteConnection(connectionString.ToString());
                            cmd = con.CreateCommand();
                            con.Open();
                            trans = con.BeginTransaction();

                            //将物理未提取到的删除数据插入到临时表中    
                            lg_source_guid = "36";
                            sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,D,G,F,C,H from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "'");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //trans.Commit();

                            //将日志未提取到的日志删除数据插入到临时表中    
                            lg_source_guid = "40";
                            sql = String.Format("insert into tbl_tmp (A,B,C,D,E,F) select B,D,F,E,C,G  from tbl_data where source_guid ='" + lg_source_guid + "' and case_guid = '" + lg_case_guid + "' and  evidence_guid = '" + lg_evidence_guid + "' and tbl_data.E not in (select D from tbl_tmp)");

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            //trans.Commit();

                            //将合并后的数据重新写入tbl_data    
                            lg_source_guid = "83";
                            sql = String.Format("select A,B,C,D,E,F,G  from tbl_tmp");
                            cmd.CommandText = sql;
                            reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (lg_isEnglish)
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                        Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','Call log','" + reader["A"] +
                                                        "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                                    }
                                    else
                                    {
                                        sql = String.Format("insert into tbl_data (data_guid,case_guid,evidence_guid,source_guid,case_name,source_name,A,B,C,D,E,F,G) values( '" +
                                                             Guid.NewGuid().ToString() + "','" + lg_case_guid + "','" + lg_evidence_guid + "','" + lg_source_guid + "','" + lg_case_name + "','通话记录','" + reader["A"] +
                                                             "','" + reader["B"] + "','" + reader["C"] + "','" + reader["D"] + "','" + reader["E"] + "','" + reader["F"] + "','" + reader["G"] + "')");
                                    }
                                    cmd2 = con.CreateCommand();
                                    cmd2.CommandText = sql;
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();
                            //清空临时表    
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();
                            //trans.Commit();

                            if (reader != null)
                                reader.Close();
                            if (con != null)
                                con.Close();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            sql = String.Format("delete from tbl_tmp");
                            cmd3 = con.CreateCommand();
                            cmd3.CommandText = sql;
                            cmd3.ExecuteNonQuery();
                            //trans.Commit();
                            //MessageBox.Show(ex.Message);
                            //MessageBox.Show(sql);
                            //if (lg_isEnglish)
                            //    throw new Exception("Failed to Combin Call log");
                            //else
                            //    throw new Exception("合并通话记录");
                        }

                        finally
                        {

                        }
                        #endregion
                    }
                }
                if (reader != null)
                    reader.Close();
                if (reader2 != null)
                    reader2.Close();
                if (con != null)
                    con.Close();
            }
        }


        /// <summary>
        /// 创建导出使用的数据库
        /// </summary>
        /// <param name="casePath"></param>
        /// <returns></returns>
        public bool CreateExportDataBase(String casePath)
        {
            SQLiteConnection con = null;
            try
            {
                String dir = casePath.Substring(0, casePath.LastIndexOf('\\'));
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                SQLiteConnection.CreateFile(casePath);
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = casePath,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                String sql = "CREATE TABLE [TBL_DATA] (";
                sql += "  [DATA_GUID] VARCHAR(64)  PRIMARY KEY NULL, ";
                sql += "  [CASE_GUID] VARCHAR(64)  NULL,";
                sql += "  [EVIDENCE_GUID] VARCHAR(512)  NULL,";
                sql += "  [SOURCE_GUID] VARCHAR(512)  NULL,";
                sql += "  [EVIDENCETYPE_GUID] VARCHAR(64)  NULL,";
                sql += "  [CASE_NAME] VARCHAR(64)  NULL,";
                sql += "  [EVIDENCE_NUMBER] VARCHAR(1024)  NULL,";
                sql += "  [SOURCE_NAME] VARCHAR(1024)  NULL,";
                sql += "   [IF_DATA_ISSELECT] VARCHAR(32) NULL, ";
                for (char i = 'A'; i <= 'Z'; i++)
                {
                    sql += " [" + i + "] VARCHAR(1024)  NULL,";
                }
                sql = sql.TrimEnd(',');
                sql += " ) ;";

                con = new SQLiteConnection(connectionString.ToString());
                SQLiteCommand cmd = new SQLiteCommand(con);
                cmd.CommandText = sql;
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                throw new Exception("创建数据库文件失败！");
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// 读取案件库中的信息
        /// </summary>
        /// <param name="casePath">案件所在路径</param>
        /// <param name="whereStr">sql 条件语句</param>
        /// <returns></returns>
        public List<Data> GetAllData(String casePath, String whereStr)
        {
            List<Data> dataList = null;
            if (File.Exists(casePath))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = casePath,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    String sql = " select  * from TBL_DATA  where 1=1   ";
                    if (whereStr.Trim().Length > 0)
                    {
                        sql += whereStr;
                    }
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        dataList = new List<Data>();
                        while (reader.Read())
                        {
                            dataList.Add(new Data
                            {
                                CASE_GUID = reader["CASE_GUID"].ToString(),
                                CASE_NAME = reader["CASE_NAME"].ToString(),
                                DATA_GUID = reader["DATA_GUID"].ToString(),
                                EVIDENCE_GUID = reader["EVIDENCE_GUID"].ToString(),
                                SOURCE_GUID = reader["SOURCE_GUID"].ToString(),
                                IF_DATA_ISSELECT = reader["IF_DATA_ISSELECT"].ToString(),
                                SOURCE_NAME = reader["SOURCE_NAME"].ToString(),
                                A = reader["A"].ToString(),
                                B = reader["B"].ToString(),
                                C = reader["C"].ToString(),
                                D = reader["D"].ToString(),
                                E = reader["E"].ToString(),
                                F = reader["F"].ToString(),
                                G = reader["G"].ToString(),
                                H = reader["H"].ToString(),
                                I = reader["I"].ToString(),
                                J = reader["J"].ToString(),
                                K = reader["K"].ToString(),
                                L = reader["L"].ToString(),
                                M = reader["M"].ToString(),
                                N = reader["N"].ToString(),
                                O = reader["O"].ToString(),
                                P = reader["P"].ToString(),
                                Q = reader["Q"].ToString(),
                                R = reader["R"].ToString(),
                                S = reader["S"].ToString(),
                                T = reader["T"].ToString(),
                                U = reader["U"].ToString(),
                                V = reader["V"].ToString(),
                                W = reader["W"].ToString(),
                                X = reader["X"].ToString(),
                                Y = reader["Y"].ToString(),
                                Z = reader["Z"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return dataList;
        }

        /// <summary>
        /// 读取案件库中的信息
        /// </summary>
        /// <param name="casePath">案件所在路径</param>
        /// <param name="whereStr">sql 条件语句</param>
        /// <returns></returns>
        public String GetAiEvidendeData(String casePath, String whereStr)
        {
            string lsai = "";
            if (File.Exists(casePath))
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = casePath,
                    //Password = "xxxxxx" // 此处假设数据库密码为: xxxxxx。
                };
                SQLiteConnection con = null;
                SQLiteCommand cmd = null;
                SQLiteDataReader reader = null;
                try
                {
                    con = new SQLiteConnection(connectionString.ToString());
                    String sql = " select  EVIDENCE_FILE from TBL_EVIDENCE  where 1=1   ";
                    if (whereStr.Trim().Length > 0)
                    {
                        sql += whereStr;
                    }
                    cmd = new SQLiteCommand(sql, con);
                    con.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {

                        while (reader.Read())
                        {
                            if (reader["EVIDENCE_FILE"].ToString().Length > 0)
                            { lsai = reader["EVIDENCE_FILE"].ToString(); }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message, ex);
                    //throw new Exception("读取案件信息失败");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (con != null)
                        con.Close();
                }
            }
            return lsai;
        }
    }
}
