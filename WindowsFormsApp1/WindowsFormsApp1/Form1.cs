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
        class stockInfo
        {
            public string stockCode;
            public string stockName;
            public stockInfo(string code, string name)
            {
                this.stockCode = code;
                this.stockName = name;
            }
        }

        List<stockInfo> stockList;

        public void onEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0)
            {
                /* account list */
                string accountlist = axKHOpenAPI1.GetLoginInfo("ACCLIST");
                Console.WriteLine(accountlist);
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
                Console.WriteLine(userId);
                Console.WriteLine(userName);
                Console.WriteLine(connectedServer);

                /* stock code single */
                string stockCodeListSingle = axKHOpenAPI1.GetCodeListByMarket("0");
                string[] stockCodeSingle = stockCodeListSingle.Split(';');

                /* stock code list */
                stockList = new List<stockInfo>();

                AutoCompleteStringCollection stockcollection = new AutoCompleteStringCollection();
                string stockCode = axKHOpenAPI1.GetCodeListByMarket(null);
                string[] stockCodeArray = stockCode.Split(';');

                for(int i = 0;i < stockCodeArray.Length;i++)
                {
                    stockList.Add(new stockInfo(stockCodeArray[i], axKHOpenAPI1.GetMasterCodeName(stockCodeArray[i])));
                }

                for(int i = 0;i < stockList.Count;i++)
                {
                    stockcollection.Add(stockList[i].stockName);
                }
                stockTextBox.AutoCompleteCustomSource = stockcollection;
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

                    axKHOpenAPI1.CommRqData("계좌평가잔고내역요청", "opw00018", 0, "8100");
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
            if(e.sRQName == "계좌평가잔고내역요청")
            {
#if true
                Console.WriteLine(e.sRQName);
                Console.WriteLine(accountComboBox.SelectedItem.ToString());
#else
                long totalPurchase = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총매입금액"));
                long totalEstimate = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총평가금액"));
                long totalProfitLoss = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총평가손익금액"));
                double totalProfitRate = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "총수익률(%)"));

                totalProfitRateLabel.Text = String.Format("{0:#,###}", totalPurchase);
                totalEstimateLabel.Text = String.Format("{0:#,###}", totalEstimate);
                totalProfitLabel.Text = String.Format("{0:#,###}", totalProfitLoss);
                totalProfitRateLabel.Text = String.Format("{0:f2}", totalProfitRate);
#endif
            }
            else if(e.sRQName == "종목정보요청")
            {
                long stockPrice = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "현재가").Trim().Replace("-", ""));
                string stockName = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim();
                long upDown = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "전일대비").Trim());
                long volume = long.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "거래량").Trim());
                string upDownRate = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "등락율").Trim();

                stockPriceLabel.Text = String.Format("{0:#,###}", stockPrice);
                stockNameLabel.Text = stockName;
                stockUpDownLabel.Text = String.Format("{0:#,###}", upDown);
                stockVolumeLabel.Text = String.Format("{0:#,###}", volume);
                if (upDown == 0)
                {
                    stockUpDownLabel.Text = "0";
                }
                if (volume == 0)
                {
                    stockVolumeLabel.Text = "0";
                }
                stockUpDownRateLabel.Text = upDownRate + "%";
            }
        }

        public void onReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            Console.WriteLine(e.sRealType);

        }

        public void stockSearch(object sender, EventArgs e)
        {
            string stockName = stockTextBox.Text;
            int index = stockList.FindIndex(o => o.stockName == stockName);
            string stockCode = stockList[index].stockCode;
            axKHOpenAPI1.SetInputValue("종목코드", stockCode);
            axKHOpenAPI1.CommRqData("종목정보요청", "opt10001", 0, "5000");
        }

        public Form1()
        {
            InitializeComponent();
            axKHOpenAPI1.CommConnect();
            axKHOpenAPI1.OnEventConnect += onEventConnect;
            passwordTextBox.TextChanged += encryptPassword;
            balanceCheckButton.Click += ButtonClicked;
            axKHOpenAPI1.OnReceiveTrData += onReceiveTrData;
            stockSearchButton.Click += stockSearch;
        }
    }
}
