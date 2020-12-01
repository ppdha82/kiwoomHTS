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

        /* chap 4.1 */
        List<stockInfo> stockList;
        /* chap 5.1 */
        List<stockInfo> stockList5_1;

        /* chap 4.2 */
        public string currentStockCode = "";
        /* chap 5.1 */
        public string currentstockCode5_1 = "";

        /* chap 4.1 */
        /* chap 4.2 */
        /* chap 5.1 */
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

                /* chap 4.1 */
                /* chap 4.2 */
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

                /* chap 5.1 */
                stockList5_1 = new List<stockInfo>();

                AutoCompleteStringCollection stockcollection5_1 = new AutoCompleteStringCollection();
                string stockCodeList5_1 = axKHOpenAPI1.GetCodeListByMarket(null);
                string[] stockCodeArray5_1 = stockCodeList5_1.Split(';');
                for (int i = 0; i < stockCodeArray5_1.Length; i++)
                {
                    stockList5_1.Add(new stockInfo(stockCodeArray5_1[i], axKHOpenAPI1.GetMasterCodeName(stockCodeArray5_1[i])));
                    stockcollection5_1.Add(stockList5_1[i].stockName);
                }
                stockTextBox5_1.AutoCompleteCustomSource = stockcollection5_1;
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

        /* chap 4.1 */
        /* chap 4.2 */
        public void onReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            Console.WriteLine(e.sScrNo);
            Console.WriteLine(e.sRQName);
            Console.WriteLine(e.sTrCode);
            Console.WriteLine(e.sRecordName);
            Console.WriteLine(e.sPrevNext);
            if (e.sRQName == "계좌평가잔고내역요청")
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
            /* chap 4.1 */
            /* chap 4.2 */
            else if (e.sRQName == "종목정보요청")
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
            /* chap 5.1 */
            else if (e.sRQName == "주식호가")
            {
                priceListBox.Items.Clear();
                volumeListBox.Items.Clear();
                for (int i = 0; i < 9; i++)
                {
                    int 차선호가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매도" + (10 - i) + "차선호가"));
                    priceListBox.Items.Add(차선호가 + " / " + (10 - i) + "차선");

                    if (i == 4)
                    {
                        int 차선잔량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매도" + (10 - i) + "우선잔량"));
                        volumeListBox.Items.Add(차선잔량);
                    }
                    else
                    {
                        int 차선잔량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매도" + (10 - i) + "차선잔량"));
                        volumeListBox.Items.Add(차선잔량);
                    }

                }
                int 매도최우선호가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매도최우선호가"));
                priceListBox.Items.Add(매도최우선호가 + " / 매도최우선호가");
                int 매도최우선잔량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매도최우선잔량"));
                volumeListBox.Items.Add(매도최우선잔량);

                int 매수최우선호가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매수최우선호가"));
                priceListBox.Items.Add(매수최우선호가 + " / 매수최우선호가");
                int 매수최우선잔량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매수최우선잔량"));
                volumeListBox.Items.Add(매수최우선잔량);

                for (int i = 0; i < 9; i++)
                {
                    if (i == 4)
                    {
                        int 호가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매수" + (2 + i) + "우선호가"));
                        priceListBox.Items.Add(호가 + " / " + (2 + i) + "차선");

                        int 잔량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매수" + (2 + i) + "우선잔량"));
                        volumeListBox.Items.Add(잔량);
                    }
                    else
                    {
                        int 호가 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매수" + (2 + i) + "차선호가"));
                        priceListBox.Items.Add(호가 + " / " + (2 + i) + "차선");
                        int 잔량 = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "매수" + (2 + i) + "차선잔량"));
                        volumeListBox.Items.Add(잔량);
                    }
                }

                MessageBox.Show("[매도최우선호가 = " + 매도최우선호가 + "][매수최우선호가 = " + 매수최우선호가 + "]");
            }
        }

        /* chap 4.2 */
        /* chap 5.1 */
        public void onReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            Console.WriteLine(e.sRealType);
            /* chap 4.2 */
            if (e.sRealData == "주식체결")
            {
                if(e.sRealKey == currentStockCode)
                {
                    long stockPrice = long.Parse(axKHOpenAPI1.GetCommRealData(currentStockCode, 10));
                    long upDown = long.Parse(axKHOpenAPI1.GetCommRealData(currentStockCode, 11));
                    string upDownRate = axKHOpenAPI1.GetCommRealData(currentStockCode, 12);
                    long volume = long.Parse(axKHOpenAPI1.GetCommRealData(currentStockCode, 15));

                    stockPriceLabel.Text = String.Format("{0:#,###}", stockPrice);
                    stockUpDownLabel.Text = String.Format("{0:#,###}", upDown);
                    stockVolumeLabel.Text = String.Format("{0:#,###}", volume);
                    if(upDown == 0)
                    {
                        stockUpDownLabel.Text = "0";
                    }

                    if(volume == 0)
                    {
                        stockVolumeLabel.Text = "0";
                    }
                    stockUpDownRateLabel.Text = upDownRate + "%";
                }
            }
            /* chap 5.1 */
            if (e.sRealType == "주식호가잔량")
            {
                Console.WriteLine(currentStockCode + "------------------------------------------");
                if (e.sRealKey.Equals(currentStockCode))
                {
                    try
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int 매도호가 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 50 - i));
                            int 매도잔량 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 70 - i));

                            priceListBox.Items[i] = 매도호가 + " / " + (10 - i) + "차선";
                            volumeListBox.Items[i] = 매도잔량;

                            if (i == 9)
                            {
                                priceListBox.Items[i] = 매도호가 + " / 매도최우선호가";
                            }


                        }
                        for (int i = 0; i < 10; i++)
                        {
                            int 매수호가 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 51 + i));
                            int 매수잔량 = int.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 71 + i));

                            priceListBox.Items[10 + i] = 매수호가 + " / " + (i + 1) + "차선";
                            volumeListBox.Items[10 + i] = 매수잔량;

                            if (i == 0)
                            {
                                priceListBox.Items[10 + i] = 매수호가 + " / 매수최우선호가";
                            }

                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message.ToString());

                    }

                }
            }
            else if (e.sRealType == "주식체결")
            {
                if (e.sRealKey.Equals(currentStockCode))
                {
                    long stockPrice = long.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 10).Replace("-", ""));
                    priceLabel.Text = String.Format("{0:#,###}", stockPrice);

                    upDownRateLabel.Text = double.Parse(axKHOpenAPI1.GetCommRealData(e.sRealType, 12)) + "%";
                }
            }
        }

        /* Search Button Click Event */
        /* chap 4.1 */
        /* chap 4.2 */
        public void stockSearch(object sender, EventArgs e)
        {
            /* chap 4.1 & chap 4.2 */
            string stockName = stockTextBox.Text;
            int index = stockList.FindIndex(a => a.stockName == stockName);
            string stockCode = stockList[index].stockCode;
            axKHOpenAPI1.SetInputValue("종목코드", stockCode);
            axKHOpenAPI1.CommRqData("종목정보요청", "opt10001", 0, "5000");
        }

        /* chap 5.1 */
        public void stockSearch5_1(object sender, EventArgs e)
        {
            /* chap 5.1 */
            string stockName5_1 = stockTextBox5_1.Text;
            int index5_1 = stockList5_1.FindIndex(a => a.stockName == stockName5_1);
            currentstockCode5_1 = stockList5_1[index5_1].stockCode;

            if (currentstockCode5_1.Length > 0)
            {
                Console.WriteLine("run chap5.1");
                axKHOpenAPI1.SetInputValue("종목코드", currentstockCode5_1);
                axKHOpenAPI1.CommRqData("종목정보요청", "opt10001", 0, "5000");

                axKHOpenAPI1.SetInputValue("종목코드", currentstockCode5_1);
                axKHOpenAPI1.CommRqData("주식호가", "opt10004", 0, "5001");
            }
        }

        /* chap 5.1 */
        public void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if(sender.Equals(priceListBox))
            {
                try
                {
                    if(e.Index < 10)
                    {
                        e.Graphics.FillRectangle(Brushes.LightSteelBlue, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.LightPink, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                    }
                    String value = priceListBox.Items[e.Index].ToString();
                    Brush brush;

                    if (value[0] == '-')
                    {
                        brush = Brushes.Blue;
                    }
                    else
                    {
                        brush = Brushes.Red;
                    }
                    int x = e.Bounds.X + e.Font.Height / 2;
                    int y = e.Bounds.Y + e.Font.Height / 2;

                    e.Graphics.DrawString(value.Replace("-", ""), e.Font, brush, x, y, StringFormat.GenericDefault);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString());
                }
            }
            else if (sender.Equals(volumeListBox))
            {
                try
                {
                    string value = volumeListBox.Items[e.Index].ToString();

                    int x = e.Bounds.X + e.Font.Height / 2 + e.Bounds.Width / 2;
                    int y = e.Bounds.Y + e.Font.Height / 2;

                    e.Graphics.DrawString(value, e.Font, Brushes.Black, x, y, StringFormat.GenericDefault);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message.ToString());

                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            axKHOpenAPI1.CommConnect();
            axKHOpenAPI1.OnEventConnect += onEventConnect;
            passwordTextBox.TextChanged += encryptPassword;
            balanceCheckButton.Click += ButtonClicked;
            axKHOpenAPI1.OnReceiveRealData += onReceiveRealData;
            axKHOpenAPI1.OnReceiveTrData += onReceiveTrData;
            stockSearchButton.Click += stockSearch;
            /* chap 5.1 */
            stockSearchButton5_1.Click += stockSearch5_1;
            priceListBox.DrawItem += ListBox_DrawItem;
            priceListBox.DrawMode = DrawMode.OwnerDrawVariable;

            priceListBox.ItemHeight = priceListBox.Height / 20;

            volumeListBox.DrawItem += ListBox_DrawItem;
            volumeListBox.DrawMode = DrawMode.OwnerDrawVariable;
            volumeListBox.ItemHeight = volumeListBox.Height / 20;
        }
    }
}
