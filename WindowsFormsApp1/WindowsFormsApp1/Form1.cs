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
        /* chap 6.1 */
        List<stockInfo> stockList6_1;

        /* chap 4.2 & chap 5.1 */
        public string currentStockCode = "";

        /* chap 4.1 & chap 4.2 & chap 5.1 & chap 6.1 */
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

                /* chap 4.1 & chap 4.2 & chap 5.1 */
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

                /* chap 6.1 */
                orderComboBox6_1.Items.Add("00:지정가".ToString());
                orderComboBox6_1.Items.Add("03:시장가".ToString());
                stockList6_1 = new List<stockInfo>();
                string accountList6_1 = axKHOpenAPI1.GetLoginInfo("ACCLIST");
                string[] accountArray6_1 = accountList6_1.Split(';');
                for (int i = 0; i < accountArray6_1.Length; i++)
                {
                    accountComboBox6_1.Items.Add(accountArray6_1[i]);
                }
                string stocklist6_1 = axKHOpenAPI1.GetCodeListByMarket(null);
                string[] stockArray6_1 = stocklist6_1.Split(';');
                AutoCompleteStringCollection stockCollection6_1 = new AutoCompleteStringCollection();
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

        /* chap 4.1 */
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
            /* chap 6.1 */
            else if (e.sRQName == "종목정보요청")
            {
                string stockPrice = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "현재가");
                priceNumericUpDown6_1.Value = long.Parse(stockPrice.Replace("-", ""));
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
        /* chap 4.1 & chap 4.2 & chap 5.1 & chap 6.1 */
        public void stockSearch(object sender, EventArgs e)
        {
            string stockName = stockTextBox.Text;
            int index = stockList.FindIndex(a => a.stockName == stockName);
            currentStockCode = stockList[index].stockCode;
            if(currentStockCode.Length > 0)
            {
                axKHOpenAPI1.SetInputValue("종목코드", currentStockCode);
                axKHOpenAPI1.CommRqData("종목정보요청", "opt10001", 0, "5000");

                axKHOpenAPI1.SetInputValue("종목코드", currentStockCode);
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

        /* chap 6.1 */
        public void orderButtonClicked(object sender, EventArgs e)
        {
            if (sender.Equals(buyButton6_1))
            {
                if (stockTextBox.Text.Length > 0 && accountComboBox6_1.Text.Length > 0)
                {
                    string stockText = stockTextBox.Text;
                    int index = stockList6_1.FindIndex(o => o.stockName == stockText);

                    string orderCode = stockList6_1[index].stockCode;
                    string accountCode = accountComboBox6_1.Text;
                    int stockQty = int.Parse(numberNumericUpDown6_1.Value.ToString());
                    int stockPrice = int.Parse(priceNumericUpDown6_1.Value.ToString());
                    string[] orderCombo = orderComboBox6_1.Text.Split(':');

                    axKHOpenAPI1.SendOrder("종목신규매수", "8249", accountCode, 1, orderCode, stockQty, stockPrice, orderCombo[0], "");
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            axKHOpenAPI1.CommConnect();
            /* chap 4.1 & chap 4.2 & chap 5.1 & chap 6.1 */
            axKHOpenAPI1.OnEventConnect += onEventConnect;
            passwordTextBox.TextChanged += encryptPassword;
            balanceCheckButton.Click += ButtonClicked;
            axKHOpenAPI1.OnReceiveRealData += onReceiveRealData;
            axKHOpenAPI1.OnReceiveTrData += onReceiveTrData;
            /* chap 4.1 & chap 4.2 */
            stockSearchButton.Click += stockSearch;
            /* chap 5.1 */
            priceListBox.DrawItem += ListBox_DrawItem;
            priceListBox.DrawMode = DrawMode.OwnerDrawVariable;

            priceListBox.ItemHeight = priceListBox.Height / 20;

            volumeListBox.DrawItem += ListBox_DrawItem;
            volumeListBox.DrawMode = DrawMode.OwnerDrawVariable;
            volumeListBox.ItemHeight = volumeListBox.Height / 20;

            /* chap 6.1 */
            buyButton6_1.Click += orderButtonClicked;
        }
    }
}
