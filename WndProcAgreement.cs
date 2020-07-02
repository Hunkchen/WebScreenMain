using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScreenMain
{
    public class WndProcAgreement
    {
        /// <summary>
        /// 协议版本
        /// </summary>
        public string Version
        {
            get;
            set;
        } = "1.0";

        /// <summary>
        /// 命令标识符
        /// </summary>
        public WndProcAgreement_Identifiter Identifier
        {
            get;
            set;
        }

        /// <summary>
        /// 数据内容
        /// </summary>
        public string Data
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 通讯协议命令标识
    /// </summary>
    public enum WndProcAgreement_Identifiter
    {
        /// <summary>
        /// 程序激活(即置顶)
        /// </summary>
        Activate = 1024,

        /// <summary>
        /// 程序关闭
        /// </summary>
        Close,

        /// <summary>
        /// 屏幕切换   
        /// Data 为屏幕索引
        /// </summary>
        FlipScreens,
    }
}
