using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pactera.Workflow
{
	public partial class EngineV2 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Write("123");
			Response.Write(Request.Form["HiddenIframeRequestDataJson"]);
		}
	}
}