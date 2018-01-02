using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OuShangAwards
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                System.Diagnostics.Process[] myprocesses = System.Diagnostics.Process.GetProcessesByName("OuShangAwards");
                if (myprocesses.Length > 1)
                {
                    MessageBox.Show("程序已启动！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    foreach (System.Diagnostics.Process myprocess in myprocesses)
                    {
                        if (myprocess.ProcessName == "OuShangAwards")
                        {
                            Application.Exit();
                        }
                    }
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FaceRecognizeSignForm());
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog("Program", "Main", ex.Message, ex.StackTrace);
            }          
        }
    }
}
