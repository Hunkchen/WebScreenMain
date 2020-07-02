using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NetDimension.Formium.Common.Window
{
    public class FormTest:Form
    {

        public delegate void DefWndProcDelegate(ref System.Windows.Forms.Message m);

        public event DefWndProcDelegate DefWndProcEvent;
        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            if (DefWndProcEvent != null)
            {
                DefWndProcEvent(ref m);
            }
            base.DefWndProc(ref m);
            //switch (m.Msg)
            //{
            //case WM_COPYDATA:
            //    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
            //    Type mytype = mystr.GetType();
            //    mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
            //    this.textBox1.Text = mystr.lpData;
            //    break;
            //default:
            //    base.DefWndProc(ref m);
            //    break;
            //}
        }
    }
}
