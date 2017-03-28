using System;
using System.Data.OleDb;
using System.IO;
using DataTable = System.Data.DataTable;
using System.Data;

namespace Pactera.Common.Npoi
{
    public class ExcelEdit
    {

        /// <summary>
        /// 将excel数据读入到datatable中
        /// </summary>
        /// <param name="_improtFilePath">excel文件路径</param>
        /// <param name="_improtSheet">excel文件sheet名</param>
        public static DataTable ImportExcelToDataTable(string _improtSheet, string _improtFilePath)
        {
            string strConn;
            string sheetstr = _improtSheet + "$";
            switch (Path.GetExtension(_improtFilePath))
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + _improtFilePath + "';Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.Ace.OleDb.12.0;Data Source='" + _improtFilePath + "';Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";
                    break;
                default:
                    throw new NotSupportedException("The file is not a valid excel file");
            }
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
             
           string strselect = "select * from ["+sheetstr+"]";
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strselect, conn);
            DataTable dt = new DataTable();
            try
            {
                myCommand.Fill(dt);
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        /// <summary>
        /// 将excel数据读入到datatable中
        /// </summary>
        /// <param name="FilePath">excel文件路径</param>
        /// <param name="sheet">excel文件sheet名</param>
        /// <param name="selectstr">select查询语句</param>
        public static   DataTable ImportExcelToDataTable(string FilePath, string sheet,string selectstr)
        {
            string strConn;
            string sheetstr = sheet + "$";
            switch (Path.GetExtension(FilePath))
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + FilePath + "';Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.Ace.OleDb.12.0;Data Source='" + FilePath + "';Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";
                    break;
                default:
                    throw new NotSupportedException("The file is not a valid excel file");
            }
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();

            selectstr = "select * from [" + sheetstr + "]";
            OleDbDataAdapter myCommand = new OleDbDataAdapter(selectstr, conn);
            DataTable dt = new DataTable();
            try
            {
                myCommand.Fill(dt);
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        /// <summary>
        /// 根据设定条件将DataTable中的数据更新到Excel中，形如"update [" + expotrSheet$ + "] set "+setName+"='" + dt.Rows[i][setValue].ToString() + "' where "+whereName+"='" + dt.Rows[i][whereValue].ToString() + "'"
        /// </summary>
        /// <param name="_expotrSheet">sheet</param>
        /// <param name="_exportFilePath">文件</param>
        /// <param name="setName">需要更新的字段名</param>
        /// <param name="setValue">需要更新的数据</param>
        /// <param name="whereName">条件字段名</param>
        /// <param name="whereValue">条件数据</param>
        /// <returns></returns>
        public static Boolean UpdateDataTableToExcel(string _expotrSheet, string _exportFilePath, string setName, string setValue, string whereName, string whereValue)
        {
            string strConn;
            Boolean b = false;
            string sheetstr = _expotrSheet + "$";
            switch (Path.GetExtension(_exportFilePath))
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + _exportFilePath + "';Extended Properties='Excel 8.0;HDR=YES;IMEX=2'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.Ace.OleDb.12.0;Data Source='" + _exportFilePath + "';Extended Properties='Excel 12.0;HDR=YES;IMEX=2'";
                    break;
                default:
                    throw new NotSupportedException("The file is not a valid excel file");
            }
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbCommand cmd = new OleDbCommand();
            String cmdStr = "";
            try
            {
                conn.Open();
                cmd.Connection = conn;
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{//cmd.CommandText"UPDATE [Sheet1$] SET values = '01AI0325' where Tags = '01AI0324'";//
                string StrWhere = "";
                //if (whereName.Trim() != "")
                StrWhere = " WHERE A1='xxx设备类'";
                cmdStr = "UPDATE [" + sheetstr + "] SET A1='" + setValue + "'" + StrWhere;
                    cmd = new OleDbCommand(cmdStr,conn);
                    cmd.ExecuteNonQuery();
                    b = true;
                //}
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.ToString());
                b = false;
            }
            finally
            {
                conn.Close();
            }
            return b;
        }

        ///
        /// <summary>
        /// 根据设定条件将DataTable中的数据插入到Excel中
        /// 孙伟
        /// 2014.2.12
        /// </summary>
        /// <param name="_expotrSheet">sheet</param>
        /// <param name="_exportFilePath">文件</param>
        /// <param name="dt">数据来源DataTable</param>
        /// <param name="StartCol">插入到excel表开始列</param>
        /// <param name="EndCol">插入到excel表终点列</param>
        /// <returns></returns>
        public static Boolean AddDataTableToExcel(string _expotrSheet, string _exportFilePath, DataTable dt, int StartCol, int EndCol)
        {
            string strConn;
            Boolean b = false;
            string sheetstr = _expotrSheet + "$";
            switch (Path.GetExtension(_exportFilePath))
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + _exportFilePath + "';Extended Properties='Excel 8.0;HDR=YES;IMEX=2'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.Ace.OleDb.12.0;Data Source='" + _exportFilePath + "';Extended Properties='Excel 12.0;HDR=YES;IMEX=2'";
                    break;
                default:
                    throw new NotSupportedException("The file is not a valid excel file");
            }
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbCommand cmd = new OleDbCommand();
            String cmdStr = "";
            try
            {
                conn.Open();
                cmd.Connection = conn;
                //
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Excel第一列的“A”
                    int ColI = 65;//放到webconfig
                    string SqlSele = "";
                    string SqlWhere = "";
                    //
                    for (int j = 0; j < EndCol - StartCol + 1; j++)
                    {
                        //
                        ColI = ColI + StartCol + j;
                        if (j == 0)
                        {
                            SqlSele = ((char)ColI).ToString();
                            SqlWhere = dt.Rows[i][j].ToString();
                            continue;
                        }
                        SqlSele += "," + ((char)ColI).ToString();
                        //事物
                        SqlWhere += "," + dt.Rows[i][j].ToString();//先做一个类
                    }
                    cmdStr = "INSERT INTO [" + sheetstr + "]  (" + SqlSele + ")  VALUES (" + SqlWhere + ")";
                    cmd = new OleDbCommand(cmdStr, conn);
                    cmd.ExecuteNonQuery();
                    b = true;
                }
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.ToString());
                //错误抛出
                b = false;
            }
            finally
            {
                conn.Close();
            }
            return b;
        }

        /// <summary>
        /// 获取指定文件的sheet名称
        /// </summary>
        /// <param name="path">指定的excel路径</param>
        /// <returns></returns>
        public static string [] GetTableName(string path)
        {
            string strConn;
            switch (Path.GetExtension(path))
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + path + "';Extended Properties='Excel 8.0;HDR=YES'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.Ace.OleDb.12.0;Data Source='" + path + "';Extended Properties='Excel 12.0;HDR=YES'";
                    break;
                default:
                    throw new NotSupportedException("The file is not a valid excel file");
            }
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataTable dataTable = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
            
            string[] result;
            string error;
            DatatableToArray(dataTable, "Table_Name", out result, out error);
            conn.Close();
            return result;
        }
        /// <summary>
        /// 将datatable中指定列的数据转化为数组类型
        /// </summary>
        /// <param name="dt">需要转化的datatable</param>
        /// <param name="col">列名</param>
        /// <returns>转化后的数组</returns>
        public static  bool DatatableToArray(DataTable dt, string col, out string[] result, out string error)
        {
            bool b = true;
            error = "";
            result = new string[dt.Rows.Count];
            try
            {
                int i = 0;
                foreach (DataRow dataRow in dt.Rows)
                {
                    result[i] = dataRow[col].ToString();
                    i++;
                }
            }
            catch (Exception exception)
            {
                b = false;
                error = exception.ToString();
            }
            return b;
        }
        /// <summary>
        /// 判断文件类型
        /// </summary>
        /// <param name="filename">文件路径及名（带后缀)</param>
        /// <param name="type">文件类型后缀(.xls;.doc;.xlsx......)</param>
        /// <returns>true/false</returns>       

        public static bool CheckFileType(string filename, string type)
        {
            bool result = false;
            string filetype = Path.GetExtension(filename); //截取文件后缀
            if (filetype == type)
            {
                result = true;
            }
            return result;
        }
  
    }
 }
