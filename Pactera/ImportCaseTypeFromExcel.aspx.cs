using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Pactera.Data;

using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Pactera
{
    public partial class ImportCaseTypeFromExcel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BtnImportCaseType.ServerClick += BtnImportCaseType_ServerClick;
        }

        private void BtnImportCaseType_ServerClick(object sender, EventArgs e)
        {
            FileStream file = new FileStream("D:\\customer.xlsx", FileMode.Open, FileAccess.Read);
            IWorkbook workbook = new XSSFWorkbook(file);

            var sheet = workbook.GetSheetAt(0);

            var rows = sheet.GetRowEnumerator();

            while (rows.MoveNext())
            {
                IRow row = (XSSFRow)rows.Current;

                ICell username = row.GetCell(0);
                ICell usernumber = row.GetCell(1);
                ICell post = row.GetCell(2);
                ICell email = row.GetCell(3);
                ICell telphone = row.GetCell(4);
                ICell Mobile = row.GetCell(5);
                ICell WorkZone = row.GetCell(6);
                ICell Division = row.GetCell(7);
                ICell Floor = row.GetCell(8);
                ICell Vip = row.GetCell(9);
                ICell Office = row.GetCell(10);
                ICell Gallery = row.GetCell(11);
                ICell AssetType = row.GetCell(12);
                ICell AssetNumber = row.GetCell(13);

                // 过滤掉第一行（表头）
                if (username.ToString().Trim() == "员工姓名" && usernumber.ToString().Trim() == "员工编号") continue;

                var dbm = DataBaseFactory.Instance.Create();

                // 检查一级的是否存在
                Hashtable htCondition = new Hashtable();

                htCondition.Add("CustomerName", username.ToString());
                htCondition.Add("CustomerNumber", usernumber.ToString());

                if (post == null) { htCondition.Add("post", ""); }
                else { htCondition.Add("post", post.ToString()); }

                if (email==null) { htCondition.Add("Email", ""); }
                else { htCondition.Add("Email", email.ToString()); }

                if (telphone == null) { htCondition.Add("telphone", ""); }
                else { htCondition.Add("telphone", telphone.ToString()); }

                if (Mobile == null) { htCondition.Add("Mobile", ""); }
                else { htCondition.Add("Mobile", Mobile.ToString()); }

                if (WorkZone == null) { htCondition.Add("WorkZone", ""); }
                else { htCondition.Add("WorkZone", WorkZone.ToString()); }

                if (Division == null) { htCondition.Add("Division", ""); }
                else { htCondition.Add("Division", Division.ToString()); }

                if (Floor == null) { htCondition.Add("Floor", ""); }
                else { htCondition.Add("Floor", Floor.ToString()); }

                if (Vip == null) { htCondition.Add("Vip", ""); }
                else { htCondition.Add("Vip", Vip.ToString()); }

                if (Office == null) { htCondition.Add("Office", ""); }
                else { htCondition.Add("Office", Office.ToString()); }

                if (Gallery == null) { htCondition.Add("Gallery", ""); }
                else { htCondition.Add("Gallery", Gallery.ToString()); }

                if (AssetType == null) { htCondition.Add("AssetType", ""); }
                else { htCondition.Add("AssetType", AssetType.ToString()); }

                if (AssetNumber == null) { htCondition.Add("AssetNumber", ""); }
                else { htCondition.Add("AssetNumber", AssetNumber.ToString()); }


                dbm.Insert("Cus_Users", htCondition);

               
            }

        }
    }
}