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

        public void encryptPassword(object sender, EventArgs e)
        {
            if(sender.Equals(passwordTextBox))
            {
                passwordTextBox.PasswordChar = '*';
                passwordTextBox.MaxLength = 4;
            }
        }

        public void ButtonClicked(object sender, EventArgs e)
        {
            if(sender.Equals(balanceCheckButton))
            {
                if(accountComboBox.Text.Length > 0 && passwordTextBox.Text.Length > 0)
                {
                    string accountNumber = accountComboBox.Text;
                    string password = passwordTextBox.Text;

                    axKHOpenAPI1.SetInputValue("계좌번호", accountNumber);
                    axKHOpenAPI1.SetInputValue("비밀번호", password);
                    axKHOpenAPI1.SetInputValue("비밀번호 입력 매체 구분", "00");
                    axKHOpenAPI1.SetInputValue("조회구분", "1");

                    axKHOpenAPI1.CommRqData("계좌 평가 잔고 내역 요청", "opw00018", 0, "8100");
                }
            }
        }

        public void onReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            Console.WriteLine(e.sScrNo);
            Console.WriteLine(e.sRQName);
            Console.WriteLine(e.sTrCode);
            Console.WriteLine(e.sRecordName);
            Console.WriteLine(e.sPrevNext);
        }

        public Form1()
        {
            InitializeComponent();
            axKHOpenAPI1.CommConnect();
            axKHOpenAPI1.OnEventConnect += onEventConnect;
            passwordTextBox.TextChanged += encryptPassword;
            balanceCheckButton.Click += ButtonClicked;
            axKHOpenAPI1.OnReceiveTrData += onReceiveTrData;
        }
    }
}
