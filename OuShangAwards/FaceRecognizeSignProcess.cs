using DataHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Xml;

namespace OuShangAwards
{
    public class FaceRecognizeSignProcess
    {
        private List<FaceCompareInfo> faceCompareInfoList = new List<FaceCompareInfo>();
        public Func<string, string, string, bool> onProcessEvent;
        private Timer timer1;
        private DateTime dtShowTime;
        private bool isTimer1Enable = false;
        private Action<object, ElapsedEventArgs> timerCallBackAction;

        public FaceRecognizeSignProcess()
        {
            timer1 = new Timer();
            timer1.Interval = Double.Parse(ConfigurationManager.AppSettings["staytime"].ToString());
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(Timer1CallBack);
            timerCallBackAction = new Action<object, ElapsedEventArgs>(Timer1CallBack);
        }

        /// <summary>
        /// 加载xml数据
        /// </summary>
        public void FaceCompareInfoLoad()
        {
            try
            {
                string filePathStr = string.Format(@"{0}\FaceCompareXmlFile", Environment.CurrentDirectory);
                string fileNameStr = string.Format(@"{0}\faceCompare_compareSuccess.xml", filePathStr);

                if (File.Exists(fileNameStr))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(fileNameStr);
                    XmlNodeList userList = xmlDoc.SelectNodes("Users/User");

                    if (userList != null && userList.Count > 0)
                    {
                        FaceCompareInfo faceCompareInfoItem = null;

                        foreach (XmlNode item in userList)
                        {
                            faceCompareInfoItem = new FaceCompareInfo();
                            faceCompareInfoItem.CaputerDeviceIp = item.SelectSingleNode("caputerDeviceIp").InnerText;
                            faceCompareInfoItem.Channel = item.SelectSingleNode("channel").InnerText;
                            faceCompareInfoItem.CaptureTime = item.SelectSingleNode("captureTime").InnerText;
                            faceCompareInfoItem.SimilarityDegree = item.SelectSingleNode("similarityDegree").InnerText;
                            faceCompareInfoItem.PersonName = item.SelectSingleNode("personName").InnerText;
                            faceCompareInfoItem.AlarmTime = item.SelectSingleNode("alarmTime").InnerText;
                            faceCompareInfoItem.AlarmDeviceIp = item.SelectSingleNode("alarmDeviceIp").InnerText;
                            faceCompareInfoItem.Sex = item.SelectSingleNode("sex").InnerText;
                            faceCompareInfoItem.Glasses = item.SelectSingleNode("glasses").InnerText;
                            faceCompareInfoItem.FaceScore = item.SelectSingleNode("faceScore").InnerText;
                            faceCompareInfoItem.BigImage = item.SelectSingleNode("bigImage").InnerText;
                            faceCompareInfoItem.FaceImage = item.SelectSingleNode("faceImage").InnerText;
                            faceCompareInfoItem.CompareImage = item.SelectSingleNode("compareImage").InnerText;
                            faceCompareInfoItem.TelePhone = item.SelectSingleNode("telePhone").InnerText;
                            faceCompareInfoItem.isShow = true;
                            faceCompareInfoList.Add(faceCompareInfoItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 设置报警上传回调
        /// </summary>
        /// <param name="deviceInfo"></param>
        public void SetDVRMessageCallBack(DeviceInfo deviceInfo, CHCNetSDK.MSGCallBack_V31 msgCallback)
        {
            try
            {
                BaseHelper hikVisionHelper = new HikVisionHelper();

                if (!FaceRecognizeSignForm.isInit)
                {
                    FaceRecognizeSignForm.isInit = hikVisionHelper.Init();
                }

                if (FaceRecognizeSignForm.isInit)
                {
                    bool isSuccess = hikVisionHelper.SetDVRMessageCallBack(msgCallback);

                    if (!isSuccess)
                    {
                        RecordErrorCode(string.Format("{0} 设置报警上传回调函数失败", deviceInfo.deviceIp));
                    }
                }
                else
                {
                    RecordErrorCode("初始化失败");
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 报警布防
        /// </summary>
        /// <param name="deviceInfo"></param>
        public void SetupAlarm(DeviceInfo deviceInfo)
        {
            try
            {
                BaseHelper hikVisionHelper = new HikVisionHelper();

                if (!FaceRecognizeSignForm.isInit)
                {
                    FaceRecognizeSignForm.isInit = hikVisionHelper.Init();
                }

                if (FaceRecognizeSignForm.isInit)
                {
                    if (deviceInfo.loginId < 0)
                    {
                        hikVisionHelper.Login(deviceInfo);
                    }

                    if (deviceInfo.loginId < 0)
                    {
                        RecordErrorCode(string.Format("{0} 登录失败", deviceInfo.deviceIp));
                    }
                    else
                    {
                        hikVisionHelper.SetupAlarmChan(deviceInfo);

                        if (deviceInfo.alarmHandle < 0)
                        {
                            RecordErrorCode(string.Format("{0} 建立报警上传通道失败", deviceInfo.deviceIp));
                        }                       
                    }
                }
                else
                {
                    RecordErrorCode("初始化失败");
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 预览
        /// </summary>
        /// <param name="deviceInfo"></param>
        public void Prievew(DeviceInfo deviceInfo)
        {
            try
            {
                BaseHelper hikVisionHelper = new HikVisionHelper();

                if (!FaceRecognizeSignForm.isInit)
                {
                    FaceRecognizeSignForm.isInit = hikVisionHelper.Init();
                }

                if (FaceRecognizeSignForm.isInit)
                {
                    if (deviceInfo.loginId < 0)
                    {
                        hikVisionHelper.Login(deviceInfo);
                    }

                    if (deviceInfo.loginId > -1)
                    {
                        hikVisionHelper.Preview(deviceInfo, null);

                        if (deviceInfo.realHandle < 0)
                        {
                            RecordErrorCode(string.Format("{0} 预览失败", deviceInfo.deviceIp));
                        }
                    }
                    else
                    {
                        RecordErrorCode(string.Format("{0} 登录失败", deviceInfo.deviceIp));
                    }
                }
                else
                {
                    RecordErrorCode("初始化失败");
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 关闭预览
        /// </summary>
        /// <param name="deviceInfo"></param>
        public void StopPreview(DeviceInfo deviceInfo)
        {
            try
            {
                if (deviceInfo.realHandle > -1)
                {
                    BaseHelper hikVisionHelper = new HikVisionHelper();
                    bool isSuccess = hikVisionHelper.StopPreview(deviceInfo);
                    if (!isSuccess)
                    {
                        RecordErrorCode(string.Format("{0} 关闭预览失败", deviceInfo.deviceIp));
                    }
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }           
        }

        /// <summary>
        /// 注销用户
        /// </summary>
        /// <param name="deviceInfo"></param>
        public void LogOut(DeviceInfo deviceInfo)
        {
            try
            {
                if (deviceInfo.loginId > -1)
                {
                    BaseHelper hikVisionHelper = new HikVisionHelper();
                    bool isSuccess = hikVisionHelper.LogOut(deviceInfo);
                    if (!isSuccess)
                    {
                        RecordErrorCode(string.Format("{0} 注销失败", deviceInfo.deviceIp));
                    }
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns></returns>
        public void Cleanup()
        {
            try
            {
                BaseHelper hikVisionHelper = new HikVisionHelper();
                bool result = hikVisionHelper.Cleanup();

                if (!result)
                {
                    RecordErrorCode("释放资源失败");
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 记录错误码
        /// </summary>
        public void RecordErrorCode(string str)
        {
            try
            {
                BaseHelper hikVisionHelper = new HikVisionHelper();
                uint errorCode = hikVisionHelper.GetLastError();
                string fileName = string.Format("{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd"), MethodBase.GetCurrentMethod().Name);
                string content = string.Format("{0} {1}.{2} {3}：{4}", DateTime.Now, this.GetType().ToString(),
                    MethodBase.GetCurrentMethod().Name, str, errorCode);
                ToolHelper.RecordLog(fileName, content);
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 报警上传回调函数
        /// </summary>
        /// <param name="lCommand"></param>
        /// <param name="pAlarmer"></param>
        /// <param name="pAlarmInfo"></param>
        /// <param name="dwBufLen"></param>
        /// <param name="pUser"></param>
        /// <returns></returns>
        public bool MsgCallback_V31(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            try
            {
                string strIP = pAlarmer.sDeviceIP;
                string infoStr = string.Format("{0}上传{1}类型报警", strIP, lCommand);
                ToolHelper.RecordSystemInfoLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, infoStr);

                //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
                switch (lCommand)
                {
                    case CHCNetSDK.COMM_SNAP_MATCH_ALARM://人脸比对结果信息
                        ToolHelper.RecordSystemInfoLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, "MsgCallback_V31");
                        ProcessCommAlarm_FaceMatch(pAlarmer, pAlarmInfo, dwBufLen, pUser);
                        break;
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return true; //回调函数需要有返回，表示正常接收到数据
        }

        private void ProcessCommAlarm_FaceMatch(CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            try
            {
                Dictionary<string, string> imagePath = new Dictionary<string, string>();
                CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM struFaceMatchAlarm = new CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM();
                struFaceMatchAlarm = (CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM));
                //保存图片
                SavePicture(struFaceMatchAlarm, "FaceCompareImage", imagePath);
                //更新xml
                UpdateXmlFile(pAlarmer, struFaceMatchAlarm, "FaceCompareXmlFile", "faceCompare", imagePath);
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void SavePicture(CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM struFaceMatchAlarm, string filePath, Dictionary<string, string> imagePath)
        {
            try
            {
                string bigImage = string.Empty;
                string faceImage = string.Empty;
                string compareImage = string.Empty;

                //抓拍大图
                if (struFaceMatchAlarm.dwSnapPicLen > 1)
                {
                    bigImage = SavePicture(struFaceMatchAlarm.dwSnapPicLen, struFaceMatchAlarm.pSnapPicBuffer, filePath, "BigImage");
                }
                //抓拍头像
                if ((struFaceMatchAlarm.struSnapInfo.dwSnapFacePicLen != 0) && (struFaceMatchAlarm.struSnapInfo.pBuffer1 != IntPtr.Zero))
                {
                    faceImage = SavePicture(struFaceMatchAlarm.struSnapInfo.dwSnapFacePicLen, struFaceMatchAlarm.struSnapInfo.pBuffer1,
                        filePath, "FaceImage");
                }
                //对比照片
                if ((struFaceMatchAlarm.struBlackListInfo.dwBlackListPicLen != 0) && (struFaceMatchAlarm.struBlackListInfo.pBuffer1 != IntPtr.Zero))
                {
                    compareImage = SavePicture(struFaceMatchAlarm.struBlackListInfo.dwBlackListPicLen, struFaceMatchAlarm.struBlackListInfo.pBuffer1,
                        filePath, "CompareImage");                    
                }

                imagePath.Add("bigImage", bigImage);
                imagePath.Add("faceImage", faceImage);
                imagePath.Add("compareImage", compareImage);
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void UpdateXmlFile(CHCNetSDK.NET_DVR_ALARMER pAlarmer, CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM struFaceMatchAlarm, string filePath, 
            string fileName, Dictionary<string, string> imagePath)
        {
            try
            {
                Dictionary<string, string> keyValue = new Dictionary<string, string>();
                string nodeType = string.Empty;
                string sex = string.Empty;
                string telephone = string.Empty;
                string personName = string.Empty;             

                //抓拍时间：年月日时分秒
                string strTimeYear = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 26) + 2000).ToString();
                string strTimeMonth = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 22) & 15).ToString("d2");
                string strTimeDay = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 17) & 31).ToString("d2");
                string strTimeHour = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 12) & 31).ToString("d2");
                string strTimeMinute = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 6) & 63).ToString("d2");
                string strTimeSecond = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 0) & 63).ToString("d2");
                string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", strTimeYear,  strTimeMonth, strTimeDay, strTimeHour, strTimeMinute, strTimeSecond);               

                //比对成功
                if ((struFaceMatchAlarm.struBlackListInfo.dwBlackListPicLen != 0) && (struFaceMatchAlarm.struBlackListInfo.pBuffer1 != IntPtr.Zero))
                {
                    nodeType = "compareSuccess";
                    string[] strArray = Encoding.Default.GetString(struFaceMatchAlarm.struBlackListInfo.struBlackListInfo.struAttribute.byName).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    if (strArray != null && strArray.Length > 2)
                    {
                        personName = strArray[0].Split('\0')[0];
                        sex = strArray[1].Split('\0')[0];
                        telephone = strArray[2].Split('\0')[0];
                    }
                    else
                    {
                        personName = Encoding.Default.GetString(struFaceMatchAlarm.struBlackListInfo.struBlackListInfo.struAttribute.byName).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                }
                else
                {
                    nodeType = "compareFail";
                }

                if (nodeType == "compareSuccess") //比对成功要去重
                {
                    if (faceCompareInfoList != null && faceCompareInfoList.Count > 0)
                    {
                        List<FaceCompareInfo> selectList = faceCompareInfoList.FindAll(x => x.PersonName == personName);
                        if (selectList != null && selectList.Count > 0)
                        {
                            return;
                        }
                    }
                }

                keyValue.Add("caputerDeviceIp", struFaceMatchAlarm.struSnapInfo.struDevInfo.struDevIP.sIpV4);//抓拍设备
                keyValue.Add("channel", struFaceMatchAlarm.struSnapInfo.struDevInfo.byChannel.ToString());//通道号
                keyValue.Add("captureTime", strTime);//抓拍时间
                keyValue.Add("similarityDegree", Convert.ToString(struFaceMatchAlarm.fSimilarity));//相似度          
                keyValue.Add("personName", personName);//姓名
                keyValue.Add("alarmTime", DateTime.Now.ToString());//报警时间
                keyValue.Add("alarmDeviceIp", pAlarmer.sDeviceIP);//报警设备
                keyValue.Add("sex", sex);//性别
                keyValue.Add("glasses", struFaceMatchAlarm.struSnapInfo.byGlasses == 1 ? "是" : (struFaceMatchAlarm.struSnapInfo.byGlasses == 2 ? "否" : "无法确认"));//眼镜
                keyValue.Add("faceScore", struFaceMatchAlarm.struSnapInfo.byFaceScore.ToString());//人脸清晰
                keyValue.Add("bigImage", imagePath["bigImage"]);//抓拍大图
                keyValue.Add("faceImage", imagePath["faceImage"]);//抓拍头像
                keyValue.Add("compareImage", imagePath["compareImage"]);//人脸比对照片
                keyValue.Add("telePhone", telephone);//电话

                bool isSuccess = ToolHelper.SaveXmlFile(filePath, fileName, keyValue, nodeType);

                if (isSuccess)
                {
                    if (nodeType == "compareSuccess")
                    {
                        FaceCompareInfo faceCompareInfoItem = new FaceCompareInfo();
                        faceCompareInfoItem.CaputerDeviceIp = struFaceMatchAlarm.struSnapInfo.struDevInfo.struDevIP.sIpV4;
                        faceCompareInfoItem.Channel = struFaceMatchAlarm.struSnapInfo.struDevInfo.byChannel.ToString();
                        faceCompareInfoItem.CaptureTime = strTime;
                        faceCompareInfoItem.SimilarityDegree = Convert.ToString(struFaceMatchAlarm.fSimilarity);
                        faceCompareInfoItem.PersonName = personName;
                        faceCompareInfoItem.AlarmTime = DateTime.Now.ToString();
                        faceCompareInfoItem.AlarmDeviceIp = pAlarmer.sDeviceIP;
                        faceCompareInfoItem.Sex = sex;
                        faceCompareInfoItem.Glasses = struFaceMatchAlarm.struSnapInfo.byGlasses == 1 ? "是" : (struFaceMatchAlarm.struSnapInfo.byGlasses == 2 ? "否" : "无法确认");
                        faceCompareInfoItem.FaceScore = struFaceMatchAlarm.struSnapInfo.byFaceScore.ToString();
                        faceCompareInfoItem.BigImage = imagePath["bigImage"];
                        faceCompareInfoItem.FaceImage = imagePath["faceImage"];
                        faceCompareInfoItem.CompareImage = imagePath["compareImage"];
                        faceCompareInfoItem.TelePhone = telephone;

                        faceCompareInfoList.Add(faceCompareInfoItem);

                        if (!timer1.Enabled)
                        {                           
                            timerCallBackAction(new object(), null);
                            timer1.Enabled = true;
                        }
                    }

                    if (!ToolHelper.SaveXmlFile(filePath, fileName, nodeType))
                    {
                        ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, "更新汇总XML文件失败", nodeType);
                    }
                }
                else
                {
                    ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, "更新XML文件失败", JsonConvert.SerializeObject(keyValue));
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private string SavePicture(uint picLen, IntPtr ptr, string filePath, string imgType)
        {
            string result = string.Empty;
            try
            {
                byte[] byteBuffer = new byte[picLen];
                Marshal.Copy(ptr, byteBuffer, 0, (int)picLen);
                result = ToolHelper.downloadImg(Encoding.Default.GetString(byteBuffer), filePath, imgType);

                if (string.IsNullOrWhiteSpace(result))
                {
                    ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, "下载图片失败", Encoding.Default.GetString(byteBuffer));
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public void Timer1CallBack(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (faceCompareInfoList != null && faceCompareInfoList.Count > 0)
                {
                    FaceCompareInfo faceCompareInfo = faceCompareInfoList.FirstOrDefault(x => !x.isShow);

                    if (faceCompareInfo != null && !string.IsNullOrWhiteSpace(faceCompareInfo.CompareImage))
                    {
                        if (!string.IsNullOrWhiteSpace(faceCompareInfo.CompareImage))
                        {
                            bool result = onProcessEvent(faceCompareInfo.CompareImage, faceCompareInfo.PersonName,
                            faceCompareInfo.Sex == "男" ? "先生" : (faceCompareInfo.Sex == "女" ? "女士" : ""));
                            faceCompareInfo.isShow = result;

                            if (result)
                            {
                                ToolHelper.RecordSystemInfoLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, string.Format("{0}显示成功", faceCompareInfo.PersonName));
                            }
                            else
                            {
                                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name,
                                    string.Format("{0}显示失败", faceCompareInfo.PersonName), JsonConvert.SerializeObject(faceCompareInfo));
                            }
                        }                      
                    }
                    else
                    {
                        if (timer1.Enabled)
                        {
                            timer1.Enabled = false;
                        }
                        ToolHelper.RecordSystemInfoLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(faceCompareInfo));
                    }
                }
                else
                {
                    if (timer1.Enabled)
                    {
                        timer1.Enabled = false;
                    }
                    ToolHelper.RecordSystemInfoLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, "faceCompareInfoList.Count为0");
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}
