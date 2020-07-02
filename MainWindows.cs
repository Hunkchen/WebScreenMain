using NetDimension.NanUI;
using NetDimension.NanUI.Browser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WebScreenMain
{
    class MainWindows : Formium
    {
        #region 引用
        [DllImport("user32", EntryPoint = "GetWindowThreadProcessId")]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);
        //用 PostThreadMessage 可以给无窗体的主线程发送消息
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
           int hWnd, // handle to destination window
           int Msg, // message
           int wParam, // first message parameter
           ref COPYDATASTRUCT lParam// second message parameter
           );
        #endregion

        #region 变量定义
        private const int WM_NCLBUTTONDOWN = 0XA1;//定义鼠标左键按下
        private const int WM_POSITION_CHANGED = 0x0047;
        private const int HTCAPTION = 2;
        const int WM_COPYDATA = 0x004A;
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        #endregion

        public string[]args = new string[3];
        public MainWindows(string[]arg)
        {
            args = arg;
        }
        public override string StartUrl => args[0];

        public override HostWindowType WindowType => HostWindowType.Standard;

        protected override Control LaunchScreen => null;

        protected override void OnRegisterGlobalObject(JSObject global)
        {
            var myObject = global.AddObject("my");
            var htmlCloseWindow = myObject.AddFunction("htmlCloseWindow");
            htmlCloseWindow.Execute += HtmlCloseWindow_Execute;
            var htmlTitleMouseDown = myObject.AddFunction("htmlTitleMouseDown");
            htmlTitleMouseDown.Execute += HtmlTitleMouseDown_Execute;
            var htmlTitleMouseUp = myObject.AddFunction("htmlTitleMouseUp");
            htmlTitleMouseUp.Execute += HtmlTitleMouseUp_Execute;
        }

        private void HtmlTitleMouseUp_Execute(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            ContainerForm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
        private void HtmlTitleMouseDown_Execute(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            //为当前应用程序释放鼠标捕获
            ReleaseCapture();
            SendMessage((IntPtr)ContainerForm.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            if (ContainerForm.WindowState != FormWindowState.Maximized)
            {
                WebBrowser.ExecuteJavascript("setFull()");
            }
        }
        private void HtmlCloseWindow_Execute(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            this.Close();
        }

        protected override void OnWindowReady(IWebBrowserHandler browserClient)
        {
            //在此处理CEF(浏览器)各项接口  
            //WebBrowser.ShowDevTools();\
            browserClient.LoadHandler.OnLoadStart += LoadHandler_OnLoadStart;
            browserClient.LoadHandler.OnLoadEnd += LoadHandler_OnLoadEnd;
        }

        #region web页面事件机制
        private void LoadHandler_OnLoadEnd(object sender, Chromium.Event.CfxOnLoadEndEventArgs e)
        {
            this.Title = "泉州时刻防盗电子有限责任公司";
            LoginByToken();
        }

        private void LoadHandler_OnLoadStart(object sender, Chromium.Event.CfxOnLoadStartEventArgs e)
        {
            this.ContainerForm.DefWndProcEvent += ContainerForm_DefWndProcEvent;
        }
        #endregion

        #region web窗口样式
        protected override void OnStandardFormStyle(IStandardHostWindowStyle style)
        {
            base.OnStandardFormStyle(style);
            style.FormBorderStyle = FormBorderStyle.None;
            style.WindowState = FormWindowState.Maximized;

        }
        #endregion

        #region 登录web界面
        //加密
        static public string SHA1(string str)
        {
            byte[] cleanBytes = Encoding.UTF8.GetBytes(str);
            byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }
        public void LoginByToken()
        {
            //string a = WebService.SHA1("123");
            Random rand = new Random();
            string r = "" + rand.Next(100000, 999999);
            string queryString = "branchCenterId=" + args[1]
                + "&username=" + args[2]
                + "&r=" + r;
            string md = SHA1(queryString + "&masterKey=701CAD396352B8C8EF1B1C758F337E344D535E03");
            JObject jo = new JObject();
            jo["md"] = md;
            jo["branchCenterId"] = 0;
            jo["username"] = "wzbzy";
            jo["r"] = r;
            var jsText = "tlogin(" + jo.ToString() + ")";
            //var jsText = "tlogin(" + jo.ToString() + ")";
            WebBrowser.ExecuteJavascript(jsText);
        }
        #endregion

        #region 接受另一个程序传过来的数据以及相应操作
        public WndProcAgreement StrToObject(string s)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WndProcAgreement wpa = serializer.Deserialize<WndProcAgreement>(s);
            return wpa;
        }
        private void ContainerForm_DefWndProcEvent(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_COPYDATA:
                    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (COPYDATASTRUCT)msg.GetLParam(mytype);
                    WndProcAgreement comm = StrToObject(mystr.lpData);
                    if (comm.Identifier == WndProcAgreement_Identifiter.Activate)
                    {
                        this.ContainerForm.Activate();
                    }
                    else if (comm.Identifier == WndProcAgreement_Identifiter.Close)
                    {
                        this.Close();
                    }
                    else if (comm.Identifier == WndProcAgreement_Identifiter.FlipScreens)
                    {

                    }
                    break;

            }
        }
        #endregion

        #region 将数据传递给另一个数据
        private void SendMsgToProcess(int iflag, string msg = "")
        {
            //利用发送消息方式，给进程 DCZKStart.exe 发送心跳 
            try
            {
                int ihwd = GetDCZKStartProcessHandle();
                byte[] sarr = System.Text.Encoding.Default.GetBytes(msg);
                int len = sarr.Length;
                COPYDATASTRUCT cds = new COPYDATASTRUCT();

                cds.dwData = (IntPtr)Convert.ToInt16(iflag.ToString());//可以是任意值 
                cds.cbData = len + 1;//指定lpData内存区域的字节数 
                cds.lpData = msg;//发送给目标窗口所在进程的数据

                SendMessage(ihwd, WM_COPYDATA, 0, ref cds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private int GetDCZKStartProcessHandle() //获取进程 NanUIDemo.exe 的进程句柄
        {
            int ihWnd = -1;

            try
            {
                Process p = Process.GetProcessesByName("NanUIDemo")[0];
                //Process p = Process.GetProcessesByName("NanUI Host Window")[0];
                if (p != null)
                {
                    IntPtr hwnd = p.MainWindowHandle;
                    //如果窗体最小化，这根据窗体名称来查找
                    if (hwnd == IntPtr.Zero)
                    {
                        hwnd = (IntPtr)FindWindow(null, @"shike");
                        if (hwnd != IntPtr.Zero)
                        {
                            ihWnd = (int)hwnd;
                        }
                    }
                    else
                    {
                        ihWnd = (int)hwnd;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ihWnd;
        }
        #endregion

    }
}
