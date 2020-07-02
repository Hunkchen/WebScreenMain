using NetDimension.NanUI.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NetDimension.Formium.Common.Window
{
    public class FormNew : Form
    {
        public delegate void WndProcDelegate(ref Message msg);
        public event WndProcDelegate DefWndProcEvent;
        protected override void DefWndProc(ref Message msg)
        {
            if(DefWndProcEvent!=null)
            {
                DefWndProcEvent(ref msg);
            }
            base.DefWndProc(ref msg);
        }
    }
}
