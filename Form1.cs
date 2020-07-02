using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WebScreenMain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        const int WM_COPYDATA = 0x004A;
        #region 接受另一个进程传过来的数据及相关操作
        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
                    WndProcAgreement comm = StrToObject(mystr.lpData);
                    if (comm.Identifier== WndProcAgreement_Identifiter.Activate)
                    {
                        Console.WriteLine(comm.Identifier);
                        this.Activate();
                        //this.Close();
                    }
                    else if (comm.Identifier== WndProcAgreement_Identifiter.Close)
                    {
                        this.Close();
                    }
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }
        //json串解析成对象
        public WndProcAgreement StrToObject(string s)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WndProcAgreement comm = serializer.Deserialize<WndProcAgreement>(s);
            return comm;
        }
        #endregion


        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
    }
}
