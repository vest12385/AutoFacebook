﻿using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WatiN.Core;
using System.IO;
using System.Data.SQLite;

namespace AutoFacebook
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        #region IE
        private int i = 0;
        private List<string> Users = new List<string>();
        private List<List<string>> UsersFriend = new List<List<string>>();
        private List<string> Post = new List<string>();
        private List<string> UsersFinish = new List<string>();
        private List<string> First = new List<string>();
        private SQLiteConnection m_dbConnection;
        private SQLiteCommand command;
        private string CurrentPath = System.Environment.CurrentDirectory;
        private string winDir = System.Environment.GetEnvironmentVariable("windir");
        private bool haveFriend = false;
        private bool FirstPost = false;
        
        private Process _browserProcess;
        private IE _browser;
        private IE Browser
        {
            get
            {
                //瀏覽器執行個體還在
                if (_browser != null && !_browserProcess.HasExited)
                {
                    try
                    {
                        //測試Browser是否可使用
                        IntPtr tmp = _browser.hWnd;
                    }
                    catch (Exception ex)
                    {
                        _browser = null;
                        _browserProcess = null;
                    }
                }
                //如果執行個體不在
                if (_browser == null || _browserProcess == null || _browserProcess.HasExited)
                {
                    var FindIE = Find.ByUrl(new Regex(@"http:\/\/tw\S*\.yahoo\.com"));
                    //開一個新的IE
                    if (IE.Exists<IE>(FindIE))
                        _browser = IE.AttachTo<IE>(FindIE);
                    else
                        _browser = new IE();
                    _browserProcess = Process.GetProcessById(_browser.ProcessID);
                }
                return _browser;
            }
        }
        #endregion

        #region 建構函式
        public Form1()
        {
            InitializeComponent();
            DirectoryInfo DB1 = new DirectoryInfo(CurrentPath + @"\FB\friend\");
            DirectoryInfo DB2 = new DirectoryInfo(CurrentPath + @"\FB\friendFriend\");
            DirectoryInfo DB3 = new DirectoryInfo(CurrentPath + @"\FB\post\");
            FileInfo FI1 = new FileInfo(CurrentPath + @"\FB\Finish.txt");
            FileInfo FI2 = new FileInfo(CurrentPath + @"\FB\friend\Backup.txt");
            FileInfo FI3 = new FileInfo(CurrentPath + @"\FB\Finish_Backup.txt");
            if (!DB1.Exists || !DB2.Exists || !DB3.Exists || !FI1.Exists || !FI2.Exists || !FI3.Exists)
            {
                DB1.Create();
                DB2.Create();
                DB3.Create();
                FileStream FS1 = FI1.Create();
                FileStream FS2 = FI2.Create();
                FileStream FS3 = FI3.Create();
                FS1.Close();
                FS2.Close();
                FS3.Close();
            }
            //設定等待網頁時間不超過10秒
            Settings.WaitForCompleteTimeOut = 10;
            Settings.WaitUntilExistsTimeOut = 10;
            //初始化控制項
            //cboSearchType.SelectedIndex = 0;
            //lblCurrentColor.ForeColor = Color.FromName(Settings.HighLightColor);
            string DataBasePath = CurrentPath + @"\FB\FaceBookDatabase.sqlite";
            if ( !SQLiteExist() )
            {
                SQLiteConnection.CreateFile(DataBasePath);
                m_dbConnection = new SQLiteConnection("Data Source= " + DataBasePath +"; Version = 3;");
                m_dbConnection.Open();
                command = m_dbConnection.CreateCommand();
                command.CommandText = "CREATE TABLE post (name TEXT, time TEXT, content TEXT)";
                command.ExecuteNonQuery();
            }
            else
            {
                m_dbConnection = new SQLiteConnection ("Data Source= " + DataBasePath + "; Version = 3;");
                m_dbConnection.Open ();
            }
            #region 產生Conosle
            AllocConsole();
            Console.CancelKeyPress += new
                ConsoleCancelEventHandler(Console_CancelKeyPress);
            Console.Beep();
            ConsoleColor oriColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                "* Don't close this console window or the application will also close.");
            Console.WriteLine();
            Console.ForegroundColor = oriColor;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr1 = new StreamReader(CurrentPath + @"\FB\friend\Backup.txt");
            StreamReader sr = new StreamReader(CurrentPath + @"\FB\Finish_Backup.txt");
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string OneUser = "";
            string OneFriend = "";
            while ( ( OneUser = sr.ReadLine() ) != null)
            {
                UsersFinish.Add(OneUser);
            }
            while ((OneFriend = sr1.ReadLine()) != null)
            {
                Users.Add(OneFriend);
                haveFriend = true;
            }
            sr.Close();
            sr1.Close();
        }
        static void Console_CancelKeyPress(object sender,
        ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

            #endregion
        #endregion

        #region 1.網頁自動登入登出
        private bool CheckIdAndPasswordNotEmpty()
        {
            if (string.IsNullOrWhiteSpace(email.Text) || string.IsNullOrWhiteSpace(password.Text))
            {
                MessageBox.Show("請輸入帳密碼已進行測試");
                return false;
            }
            else
                return true;
        }
        private void login_Click(object sender, EventArgs e)
        {
            if (CheckIdAndPasswordNotEmpty())
            {
                try
                {
                    Browser.GoTo("https://www.facebook.com/");
                    if (Browser.TextField(Find.ById("email")).Text == null || Browser.TextField(Find.ById("pass")).Text == null)
                    {
                        Browser.TextField(Find.ById("email")).TypeText(email.Text);
                        Browser.TextField(Find.ById("pass")).TypeText(password.Text);
                    }
                    Browser.Button(Find.ById("u_0_l")).Click();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void logout_Click(object sender, EventArgs e)
        {

            try
            {
                Link linkWebLogout = Browser.Link(Find.ByText("帳號設定"));
                linkWebLogout.Click();
                Browser.Button(Find.ByClass("uiLinkButtonInput")).Click();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion

        #region 2.好友取得
        private void GetFriend()
        {
            try
            {
                state.Text += " Fetch Friend Start \r\n";
                var PostName = Browser.Div(Find.ByClass("_6-e")).Element(Find.ByClass("_6-f"));
                StreamWriter sw1 = new StreamWriter(CurrentPath + @"\FB\friend\" + PostName + ".txt");
                StreamWriter sw2 = new StreamWriter(CurrentPath + @"\FB\friend\Backup.txt");
                Browser.GoTo("https://www.facebook.com/" + PostName + "/friends");

                scrollWindows();
                try
                {
                    var liList = Browser.ListItems.Filter(Find.ByClass("_698"));
                    i = 0;
                    foreach (var li in liList)
                    {
                        var Div = li.Div(Find.ByClass("fsl fwb fcb"));
                        sw1.WriteLine(i + ".Name:" + Div.Text + " \tLink:" + Div.Link(Find.Any).Url);
                        sw2.WriteLine(Div.Link(Find.Any).Url);
                        Console.WriteLine(i + "." + Div.Link(Find.Any).Url);
                        Users.Add(Div.Link(Find.Any).Url);
                        i++;
                    }
                }
                catch
                {
                    Browser.GoTo("https://www.facebook.com/" + PostName + "/friends");
                }
                sw1.Close();
                sw2.Close();
                state.Text += " Fetch Friend Success \r\n";
                state.Text += " ---------------------------------------- \r\n";
            }
            catch (WatiN.Core.Exceptions.RunScriptException ex)
            {
                state.Text += "Some Script Wrong, sleep 5 second and Reload website\r\n";
                state.Text += " ---------------------------------------- \r\n";
                Console.WriteLine("Some Script Wrong, sleep 5 second and Reload website");
                textBoxScroll();
                Thread.Sleep(5000);
                GetFriend();
            }
            catch (WatiN.Core.Exceptions.TimeoutException ex)
            {
                state.Text += "Timeout, sleep 5 second and Try Again\r\n";
                state.Text += " ---------------------------------------- \r\n";
                Console.WriteLine("Timeout, sleep 5 second and Try Again");
                textBoxScroll();
                Thread.Sleep(5000);
                GetFriend();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 3.優先取得

        private void FirstName_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains(@"https://www.facebook.com/") || textBox1.Text.Equals(null))
            {
                FirstPost = true;
                First.Add(textBox1.Text);
                listBox1.Items.Clear();
                foreach (string show in First)
                {
                    listBox1.Items.Add(show);
                }
            }
            else
            {
                MessageBox.Show("格式錯誤，請輸入包含\"https://www.facebook.com/\"的網址");
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Delete)
                {
                    First.Remove(listBox1.SelectedItem.ToString());
                    listBox1.Items.Remove(listBox1.SelectedItem.ToString());
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 4.貼文取得
        private void GetPost(  string Url)
        {
            try
            {
                Browser.GoTo(Url);
                var PostName = Browser.Div(Find.ByClass("_6-e")).Element(Find.ByClass("_6-f"));
                if (!PostName.Exists)
                {
                    PostName = Browser.Div(Find.ByClass("_58gh _2pii")).Element(Find.ByClass("_58gi"));
                }
                if (!PostName.Exists)
                {
                    PostName = Browser.Div(Find.ByClass("_58gh _2pii")).Element(Find.ByClass("_58gi _5rqs"));
                }
                string PostNameString = "";
                state.Text += "Now is " + PostName + "'s Post\r\n";
                textBoxScroll();
                StreamWriter sw1 = new StreamWriter(CurrentPath +@"\FB\post\" + PostName + ".txt");
                StreamWriter sw2 = new StreamWriter(CurrentPath + @"\FB\Finish.txt");
                Console.WriteLine(PostName + "'s Post" + "\r\n" + "\r\n");
                sw1.WriteLine(PostName + "'s Post" + "\r\n" + "\r\n");
                scrollWindows();
                if (Browser.Div(Find.ByClass("uiMorePager fbTimelineSectionExpandPager fbTimelineShowOlderSections")).Element(Find.ByClass("uiMorePagerPrimary")).Exists)
                {
                    Browser.Div(Find.ByClass("uiMorePager fbTimelineSectionExpandPager fbTimelineShowOlderSections")).Element(Find.ByClass("uiMorePagerPrimary")).Click();
                    scrollWindows();
                }
                var liList = Browser.ListItems.Filter(Find.ByClass("fbTimelineUnit fbTimelineTwoColumn clearfix"));
                var liDiv = Browser.Divs.Filter(Find.ByClass("_2d10"));
                i = 0;
                if (liList.Count == 0)
                {
                    foreach (var li in liDiv)
                    {
                        PostNameString = PostName.Text;
                        var content = li.Span(Find.ByClass("userContent"));
                        var time = li.Div(Find.ByClass("_1_n fsm fwn fcg"));
                        var name = li.Div(Find.ByClass("_3dp _29k")).Element(Find.ByClass("_1_s"));
                        var ps = li.Span(Find.ByClass("userContentSecondary fcg"));
                        if (name.Exists && content.Exists && time.Exists && content.ToString() != null)
                        {
                            if (name.Text.TrimEnd().Equals(PostNameString) || (name.Text.Contains(PostName.Text + "分享")))
                            {
                                Console.WriteLine(name + "\r\n");
                                sw1.WriteLine(name + "\r\n");
                                Console.WriteLine(time + "\r\n");
                                sw1.WriteLine(time + "\r\n");
                                Console.WriteLine(content);
                                sw1.WriteLine(content);
                                if (ps.Exists)
                                {
                                    Console.WriteLine(ps);
                                    sw1.WriteLine(ps);
                                }
                                else
                                {
                                    ps = null;
                                }
                                Console.WriteLine("-thepageisoverpleasechangenextpagetodownload-");
                                sw1.WriteLine("-thepageisoverpleasechangenextpagetodownload-");
                                //Users.Add(new List<string>() { Div.Text, Div.Link(Find.Any).Url });
                                command = m_dbConnection.CreateCommand();
                                command.CommandText = "INSERT INTO post ( name, time, content ) VALUES ( @name ,@time, @content)";
                                command.Parameters.Add(new SQLiteParameter("@name", name.Text));
                                command.Parameters.Add(new SQLiteParameter("@time", time.Text));
                                command.Parameters.Add(new SQLiteParameter("@content", content.Text + ps));
                                command.ExecuteNonQuery();
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var li in liList)
                    {
                        if (PostName.ToString().Contains("("))
                        {
                            int start = PostName.ToString().IndexOf("(");
                            int end = PostName.ToString().IndexOf(")");
                            PostNameString = PostName.ToString().Remove(start);
                        }
                        else
                        {
                            PostNameString = PostName.Text;
                        }
                        var content = li.Span(Find.ByClass("userContent"));
                        var time = li.Div(Find.ByClass("_1_n fsm fwn fcg"));
                        var name = li.Div(Find.ByClass("_3dp _29k")).Element(Find.ByClass("_1_s"));
                        var ps = li.Span(Find.ByClass("userContentSecondary fcg"));
                        if (name.Exists && content.Exists && time.Exists && content.ToString() != null)
                        {
                            if (name.Text.TrimEnd().Equals(PostNameString.TrimEnd()) || (name.Text.Contains(PostName.Text + "分享")))
                            {
                                if (content.ToString().Contains("神魔之塔"))
                                {
                                    Console.WriteLine(name + "\r\n");
                                    Console.WriteLine(time + "\r\n");
                                    Console.WriteLine(content);
                                    Console.WriteLine("It Post Contains \"神魔之塔\", Now remove it");
                                }
                                else
                                {
                                    Console.WriteLine(name + "\r\n");
                                    sw1.WriteLine(name + "\r\n");
                                    Console.WriteLine(time + "\r\n");
                                    sw1.WriteLine(time + "\r\n");
                                    Console.WriteLine(content);
                                    sw1.WriteLine(content);
                                    if (ps.Exists)
                                    {
                                        Console.WriteLine(ps);
                                        sw1.WriteLine(ps);
                                    }
                                    else
                                    {
                                        ps = null;
                                    }
                                    Console.WriteLine("-thepageisoverpleasechangenextpagetodownload-");
                                    sw1.WriteLine("-thepageisoverpleasechangenextpagetodownload-");
                                    //Users.Add(new List<string>() { Div.Text, Div.Link(Find.Any).Url });
                                    command = m_dbConnection.CreateCommand();
                                    command.CommandText = "INSERT INTO post ( name, time, content ) VALUES ( @name ,@time, @content)";
                                    command.Parameters.Add(new SQLiteParameter("@name", name.Text));
                                    command.Parameters.Add(new SQLiteParameter("@time", time.Text));
                                    command.Parameters.Add(new SQLiteParameter("@content", content.Text + ps));
                                    command.ExecuteNonQuery();
                                    i++;
                                }
                            }
                        }
                    }
                }
                foreach (string show in UsersFinish)
                {
                    sw2.WriteLine( show );
                }
                sw2.WriteLine(Url);
                sw2.Close();
                sw1.Close();
                File.Copy(CurrentPath + @"\FB\Finish.txt", CurrentPath + @"\FB\Finish_Backup.txt", true);
                UsersFinish.Add(Url);
                if (i < 10)
                {
                    state.Text += "He/She have less than ten Post \r\n";
                    state.Text += "delete \r\n";
                    state.Text += " ---------------------------------------- \r\n";
                    //File.Delete(CurrentPath +@"\FB\post\" + PostName + ".txt");
                }  
                else
                {
                    state.Text += "He/She have" + i + "Post \r\n";
                    state.Text += "Done\r\n";
                    state.Text += " ---------------------------------------- \r\n";
                }
            }
            catch (WatiN.Core.Exceptions.TimeoutException ex)
            {
                state.Text += "Timeout, sleep 5 second and Try Again\r\n";
                state.Text += " ---------------------------------------- \r\n";
                textBoxScroll();
                Console.WriteLine("Timeout, sleep 5 second and Try Again");
                Thread.Sleep(5000);
                GetPost(Url);
            }
            catch (WatiN.Core.Exceptions.RunScriptException ex)
            {
                state.Text += "Some Script Wrong,  sleep 5 second and Reload website\r\n";
                state.Text += " ---------------------------------------- \r\n";
                textBoxScroll();
                Console.WriteLine("Some Script Wrong, sleep 5 second and Reload website");
                Thread.Sleep(5000);
                GetPost(Url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void scrollWindows()
        {
            for (int i = 0; i < 30; i++)
            {
                Browser.Eval("window.scrollBy(0, 10000)");
                Thread.Sleep(2000);
            }
        }

        private bool SQLiteExist()
        {
            string DataBasePath = CurrentPath + @"\FB\FaceBookDatabase.sqlite";
            bool exist = false;
            if (File.Exists(DataBasePath))
            {
                state.Text += "SQLite exist\r\n";
                state.Text += " ---------------------------------------- \r\n";
                exist = true;
            }
            else
            {
                exist = false;
                state.Text += "SQLite not exist, Will create a new one \r\n";
                state.Text += " ---------------------------------------- \r\n";
            }
            return exist;
        }

        private void start_Click(object sender, EventArgs e)
        {
            bool CanRead = true;
            try
            {
                if (!haveFriend)
                {
                    GetFriend();
                }
                else
                {
                    state.Text += "Fetch Friend Done \r\n";
                    state.Text += " ---------------------------------------- \r\n";
                }
                foreach (string eachFriend in Users) 
                {
                    foreach (string show in UsersFinish)
                    {
                        if (!eachFriend.Equals(show))
                        {
                            CanRead = true;
                        }
                        else
                        {
                            CanRead = false;
                            state.Text += "Have Repeat\r\n";
                            break;
                        }
                        textBoxScroll();
                    }

                    if (CanRead)
                    {
                        if (FirstPost)
                        {
                            foreach (string show in First)
                            {
                                GetPost(show);
                            }
                            GetPost(eachFriend);
                            FirstPost = false;
                        }
                        else
                        {
                            GetPost(eachFriend);
                        }
                    }
                }
                state.Text += "Fetch Post Done \r\n";
                m_dbConnection.Close();
            }
            catch
            {
                Console.WriteLine( "Something wrong" );
            }
        }

        private void textBoxScroll()
        {
            state.SelectionStart = state.Text.Length;
            state.ScrollToCaret();
        }


    }
}
