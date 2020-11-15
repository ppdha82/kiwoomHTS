using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public void onEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0)
            {
                string accountlist = axKHOpenAPI1.GetLoginInfo("ACCLIST");
                string[] account = accountlist.Split(';');
                for (int i = 0; i < account.Length; i++)
                {
                    accountComboBox.Items.Add(account[i]);
                }
                string userId = axKHOpenAPI1.GetLoginInfo("USER_ID");
                string userName = axKHOpenAPI1.GetLoginInfo("USER_NAME");
                string connectedServer = axKHOpenAPI1.GetLoginInfo("GetServerGubun");
                idLabel.Text = userId;
                nameLabel.Text = userName;
                serverLabel.Text = connectedServer;
            }
        }

        public Form1()
        {
            InitializeComponent();
            axKHOpenAPI1.CommConnect();
            axKHOpenAPI1.OnEventConnect += onEventConnect;
        }
    }
}
