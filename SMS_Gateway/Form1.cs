using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace SMS_Gateway
{
    public partial class frmSMS : Form
    {
        SqlConnection conn_SMS = new SqlConnection();
        string Msg;
        string PNo;
        Int32 Ref_ID;
        public frmSMS()
        {
            InitializeComponent();
            loadPorts();
        }

        private void loadPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbPort.Items.Add(port);
            }
        }

        private void frmSMS_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }


        public bool sendSMS(string cellNo, string sms)
        {
            string messages = null;
            messages = sms;
            if (this.serialPort.IsOpen == true)
            {
                try
                {
                    this.serialPort.WriteLine("AT" + (char)(13));
                    Thread.Sleep(4);
                    this.serialPort.WriteLine("AT+CMGF=1" + (char)(13));
                    Thread.Sleep(5);
                    this.serialPort.WriteLine("AT+CMGS=\"" + cellNo + "\"");
                    Thread.Sleep(10);
                    this.serialPort.WriteLine(">" + messages + (char)(26));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Source);
                }
                return true;
            }
            else
                return false;
        }

        private void cmbdb_Click(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Connections con = new Connections();
            conn_SMS = con.conn_DB();
            string SQLStr;
            SqlDataReader dr_SMS;
            SqlCommand com_SMS = new SqlCommand();
            SQLStr = "Select * from SMS_V_Open_Trans";
            com_SMS.Connection = conn_SMS;
            conn_SMS.Open();
            com_SMS.CommandText = SQLStr;
            dr_SMS = com_SMS.ExecuteReader();
            while (dr_SMS.Read())
            {
                Msg = "BLI SMS Gateway - "+dr_SMS["Message"].ToString().Trim();
                PNo = dr_SMS["Phone_No"].ToString().Trim();
                Ref_ID = Convert.ToInt32(dr_SMS["Ref_ID"]);
                SMS sm = new SMS("COM18");
                sm.Opens();
                sm.sendSMS(PNo, Msg);
                sm.Closes();
                do
                {                  
                } while (!SMS_Flag());

                Thread.Sleep(5000);
            }
            conn_SMS.Close();
        }

        private bool SMS_Flag()
        {
            SqlCommand cmd;
            SqlDataReader dr;

            //Change the connection string to use your SQL Server.
            SqlConnection conn_warehouse = new SqlConnection();
            Connections con = new Connections();
            

            try
            {
                conn_warehouse = con.conn_DB();
                conn_warehouse.Open();
                //Use ODBC call syntax.
                cmd = new SqlCommand("SMS_Flaging", conn_warehouse);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;

                //Passing Value to Parameters.
                cmd.Parameters.Add(new SqlParameter("@Ref_ID", Ref_ID ));
                dr = cmd.ExecuteReader();
            }
            catch (SqlException ex)
            {
                if (ex.Number == -2)
                {
                    Console.WriteLine("Timeout occurred");
                    return false;
                }
            }
            return true;
        }
    }
}
