using NetDimension.NanUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebScreenMain
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[]args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            Bootstrap.Initialize().WithChromiumCommandLineArguments((process, cmd) =>
            {
                //设置chromium初始化命令行参数
            }).WithChromiumSettings(settings =>
            {
                //处理chromium相关设置参数
                //禁用日志
                settings.LogSeverity = Chromium.CfxLogSeverity.Disable;
                //指定中文为当前CEF环境的默认语言
                settings.AcceptLanguageList = "zh-CN";
                settings.Locale = "zh-CN";
            })
           .Run(() => new MainWindows(args));
        }
    }
}
