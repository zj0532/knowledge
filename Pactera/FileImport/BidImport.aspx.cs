using Pactera.Common.Npoi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pactera.FileImport
{
    public partial class BidImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var table = ExcelEdit.ImportExcelToDataTable("sheet", "filepath");
        }
    }
}