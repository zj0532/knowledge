using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pactera.UserRole
{
    public partial class User : Pactera.Web.UI.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DisableCurrentPageCache();
        }
    }
}