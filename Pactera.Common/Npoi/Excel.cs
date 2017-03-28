using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
 
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.HSSF.Util;
using System.Text.RegularExpressions;
using System.IO;
 
namespace Pactera.Common.Npoi
{
    public class Excel
    {
        private HSSFWorkbook _hssfworkbook;

        public byte[] TableToExcel(DataTable dtSource, string sheetName)
        {
            InitializeWorkbook();
            HSSFSheet sheet1 = (HSSFSheet)_hssfworkbook.CreateSheet(sheetName);

            string rowContent = string.Empty;
            //MatchCollection rowCollection = Regex.Matches(tableHtml, @"<tr[^>]*>[\s\S]*?<\/tr>", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture); //对tr进行筛选

            // 加粗
            //NPOI.SS.UserModel.IFont fontSubTitle = _hssfworkbook.CreateFont();
            //fontSubTitle.Boldweight = 800;

            // 加粗
            //NPOI.SS.UserModel.IFont fontBody = _hssfworkbook.CreateFont();
            //fontBody.Boldweight = 500;

            HSSFRow headerRow = (HSSFRow)sheet1.CreateRow(0);

            // 先写入标题行
            for (int j = 0; j < dtSource.Columns.Count; j++)
            {
                headerRow.CreateCell(j).SetCellValue(dtSource.Columns[j].ColumnName);
            }

            // 写入内容
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                HSSFRow excRow = (HSSFRow)sheet1.CreateRow(i + 1);

                for (int k = 0; k < dtSource.Columns.Count; k++)
                {
                    string value = dtSource.Rows[i][k].ToString();
                    excRow.CreateCell(k).SetCellValue(value);
                }
            }

            MemoryStream ms = new MemoryStream();
            //FileStream stream = new FileStream("D:\\12345.xls", FileMode.Create);
            _hssfworkbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            return ms.ToArray();
        }

        private void WriteToFile()
        {
            string year = DateTime.Now.Year.ToString();
            //string path = HttpContext.Current.Server.MapPath(DateTime.Now.ToString("yyyyMMddmmss") + ".xls");
            string path = "d:\\a.xls";
            FileStream file = new FileStream(path, FileMode.Create);
            _hssfworkbook.Write(file);
            file.Close();

            _hssfworkbook.Clear();
        }

        public void InitializeWorkbook()
        {
            _hssfworkbook = new HSSFWorkbook();
            // create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "company";
            _hssfworkbook.DocumentSummaryInformation = dsi;
            // create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "xxx";
            _hssfworkbook.SummaryInformation = si;
        }
        /*
    public void downFile(string ppath)
    {
        if (File.Exists(ppath))
        {
            Response.ClearHeaders();
            Response.Clear();
            Response.Expires = 0;
            Response.Buffer = true;
            Response.AddHeader("Accept-Language", "zh-cn");
            string name = System.IO.Path.GetFileName(ppath);
            System.IO.FileStream files = new FileStream(ppath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] byteFile = null;
            if (files.Length == 0)
            {
                byteFile = new byte[1];
            }
            else
            {
                byteFile = new byte[files.Length];
            }
            files.Read(byteFile, 0, (int)byteFile.Length);
            files.Close();
            File.Delete(files.Name);
            Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8));
            Response.ContentType = "application/octet-stream;charset=gbk";
            Response.BinaryWrite(byteFile);
            Response.End();
        }
    }

    private string GetValue(string name)
    {
        string result = ConvertData.ConvertToString(Request.QueryString[name], "");
        if (string.IsNullOrEmpty(result))
        {
            result = ConvertData.ConvertToString(Request.Form[name], "");
        }
        return StrTools.SafeSqlstr(result);
    }*/


    }
}
