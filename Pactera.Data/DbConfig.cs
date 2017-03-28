using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Pactera.Data
{
    public class DbConfig
    {
        public static string CONN_STRING = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
    }
}
