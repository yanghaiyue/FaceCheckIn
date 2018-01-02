using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace OuShangAwards
{
    public partial class FaceRecognizeSignForm : Form
    { 
        private DeviceInfo deviceInfo = new DeviceInfo();
        private FaceRecognizeSignProcess process = null;
        public static bool isInit = false;
        CHCNetSDK.MSGCallBack_V31 m_falarmData;
        private DateTime dtShowTime;
        private DateTime dtCurrentTime = DateTime.Now;
        private System.Windows.Forms.Timer timer1;

        public FaceRecognizeSignForm()
        {
            InitializeComponent();
            try
            {

                process = new FaceRecognizeSignProcess();
                deviceInfo.deviceIp = ConfigurationManager.AppSettings["deviceIp"];
                deviceInfo.devicePort = int.Parse(ConfigurationManager.AppSettings["devicePort"]);
                deviceInfo.channel = int.Parse(ConfigurationManager.AppSettings["channel"]);
                deviceInfo.userName = ConfigurationManager.AppSettings["userName"];
                deviceInfo.userPwd = ConfigurationManager.AppSettings["userPwd"]; ;
                process.FaceCompareInfoLoad();
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void FaceRecognizeSignForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)027)
            {
                try
                {
                    process.StopPreview(deviceInfo);
                    process.LogOut(deviceInfo);
                    process.Cleanup();
                }
                catch (Exception ex)
                {
                    ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                }
                finally
                {
                    this.Close();
                    Application.Exit();
                }               
            }
        }

        //private void pictureBox1_Paint(object sender, PaintEventArgs e)
        //{
        //    try
        //    {
        //        GraphicsPath path = new GraphicsPath();
        //        path.AddLines(
        //            new Point[] {
        //            new Point(0, 36),
        //            new Point(36, 0),
        //            new Point(1397, 0),
        //            new Point(1433, 36),
        //            new Point(1433, 779),
        //            new Point(1397, 815),
        //            new Point(36, 815),
        //            new Point(0, 779),
        //            new Point(0, 36)
        //            }
        //      );
        //        this.pictureBox1.Region = new Region(path);
        //    }
        //    catch (Exception ex)
        //    {
        //        ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //    }
        //}

        private void FaceRecognizeSignForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.timer1 = new System.Windows.Forms.Timer();
                timer1.Interval = 1000;
                timer1.Tick += new EventHandler(Timer1_Tick);

                if (this.IsHandleCreated)
                {
                    m_falarmData = new CHCNetSDK.MSGCallBack_V31(process.MsgCallback_V31);
                    Action<DeviceInfo, CHCNetSDK.MSGCallBack_V31> setDVRMessageCallBackAction = process.SetDVRMessageCallBack;
                    setDVRMessageCallBackAction.BeginInvoke(deviceInfo, m_falarmData, null, null);

                    Action<DeviceInfo> setupAlarmAction = process.SetupAlarm;
                    setupAlarmAction.BeginInvoke(deviceInfo, null, null);
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void FaceRecognizeSignForm_Shown(object sender, EventArgs e)
        {
            try
            {
                process.onProcessEvent = new Func<string, string, string, bool>(ShowPicture);
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private bool ShowPicture(string fileName, string personName, string sex)
        {
            bool result = false;
            try
            {
                if (InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                            {
                                this.BackgroundImage = global::OuShangAwards.Properties.Resources.secondGround;
                                this.pictureBox2.Image = new Bitmap(fileName);
                                this.label1.Text = string.Format("    {0}    {1}", personName, sex);
                                this.timer1.Start();
                                dtShowTime = DateTime.Now;
                            }
                            ));
                }
                else
                {
                    this.BackgroundImage = global::OuShangAwards.Properties.Resources.secondGround;
                    this.pictureBox2.Image = new Bitmap(fileName);
                    this.label1.Text = string.Format("    {0}    {1}", personName, sex);
                    this.timer1.Start();
                    dtShowTime = DateTime.Now;
                }

                result = true;
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            FaceCompareInfoForm faceCompareInfoForm = new FaceCompareInfoForm();
            faceCompareInfoForm.ShowDialog();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if ((DateTime.Now - dtShowTime).TotalMilliseconds >= int.Parse(ConfigurationManager.AppSettings["disappeartime"]))
                {
                    this.BackgroundImage = global::OuShangAwards.Properties.Resources.firstBackGround;
                    this.pictureBox2.Image = null;
                    this.label1.Text = string.Empty;
                    timer1.Stop();
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}
