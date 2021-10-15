using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SMS_Gateway
{
    class Connections
    {
        public SqlConnection conn_DB()
        {
            string conn_string;
            SqlConnection con_SMS;
            con_SMS = new SqlConnection();

            conn_string = "Data Source=10.227.241.27;Initial Catalog=SMS_Gateway;Persist Security Info=True;User ID=SMS_User;pwd=welcome@123";
            con_SMS.ConnectionString = conn_string;
            try
            {
                //con_SMS.Open();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return (con_SMS);
        }
    }
}
