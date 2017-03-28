using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pactera.Common.DataExport
{
    public class ExportXml
    {
        /// <summary>
        /// 将DataTable转化为Xml
        /// <para>必须为DataTable设置一个主键列</para>
        /// <para>必须设置DataTable的TableName属性</para>
        /// <para>必须设置DataTable中每一列的Caption属性</para>
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableNameDesc"></param>
        /// <returns></returns>
        public static XmlDocument DataTableToXml(DataTable dataTable, string tableNameDesc)
        {
            var doc = new XmlDocument();

            //创建类型声明节点  
            var node = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.AppendChild(node);

            // 首先我们需要先创建一个根节点
            var root = doc.CreateElement("Table");
            root.SetAttribute("Name", dataTable.TableName);
            root.SetAttribute("Description", tableNameDesc);
            doc.AppendChild(root);

            // 先找到所有行，然后一行一行处理
            foreach (DataRow row in dataTable.Rows)
            {
                var rowNode = doc.CreateElement("Row");
                rowNode.SetAttribute("Key", dataTable.PrimaryKey[0].ColumnName);
                rowNode.SetAttribute("Value", row[dataTable.PrimaryKey[0]].ToString());
                root.AppendChild(rowNode);

                // 然后找到Table的所有列
                foreach (DataColumn column in dataTable.Columns)
                {
                    var cellNode = doc.CreateElement("Cell");
                    cellNode.SetAttribute("Name", column.ColumnName);
                    cellNode.SetAttribute("Description", column.Caption);
                    cellNode.InnerText = row[column].ToString();

                    rowNode.AppendChild(cellNode);
                    //headerRow.CreateCell(j).SetCellValue(dtSource.Columns[j].ColumnName);
                }
            }

            return doc;
        }

        /// <summary>
        /// 将DataTable转化为Xml
        /// <para>必须为DataTable设置一个主键列</para>
        /// <para>必须设置DataTable的TableName属性</para>
        /// <para>必须设置DataTable中每一列的Caption属性</para>
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableNameDesc"></param>
        /// <returns></returns>
        public static byte[] DataTableToXmlByte(DataTable dataTable, string tableNameDesc)
        {
            var doc = DataTableToXml(dataTable, tableNameDesc);

            var sw = new StringWriter();
            var xw = new XmlTextWriter(sw);

            // Save Xml Document to Text Writter.
            doc.WriteTo(xw);
            var encoding = new System.Text.UTF8Encoding();

            // Convert Xml Document To Byte Array.
            byte[] docBytes = encoding.GetBytes(sw.ToString());

            /*
            MemoryStream ms = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, doc);
            ms.Flush();
            ms.Position = 0;

            return ms.ToArray();*/
            return docBytes;
        }
    }
}
