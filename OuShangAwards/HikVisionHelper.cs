using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OuShangAwards
{
    public class HikVisionHelper : BaseHelper
    {
        //static HikVisionHelper()
        //{
        //    CHCNetSDK.NET_DVR_Init();
        //}

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>true：初始化成功 false：初始化失败</returns>
        public override bool Init()
        {
            bool result = false;
            result = CHCNetSDK.NET_DVR_Init();
            return result;
        }

        /// <summary>
        /// 注册设备
        /// </summary>
        /// <returns>-1注册失败，否则注册成功</returns>
        public override void Login(DeviceInfo device)
        {
            string deviceIp = device.deviceIp;
            int devicePort = device.devicePort;
            string loginName = device.userName;
            string loginPassword = device.userPwd;

            CHCNetSDK.NET_DVR_DEVICEINFO_V30 deviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            device.loginId = CHCNetSDK.NET_DVR_Login_V30(deviceIp, devicePort, loginName, loginPassword, ref deviceInfo);
        }

        /// <summary>
        /// 实时预览
        /// </summary>
        /// <param name="deviceInfo">预览参数</param>
        /// <param name="callBack">码流数据回调函数</param>
        /// <returns>-1预览失败，否则预览成功</returns>
        public override void Preview(DeviceInfo deviceInfo, CHCNetSDK.REALDATACALLBACK callBack)
        {
            IntPtr intPtr = new IntPtr();

            CHCNetSDK.NET_DVR_PREVIEWINFO previewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            previewInfo.hPlayWnd = deviceInfo.Handle;
            //预te览的设备通道
            previewInfo.lChannel = deviceInfo.channel;
            //码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            previewInfo.dwStreamType = 0;
            //连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            previewInfo.dwLinkMode = 0;
            //0- 非阻塞取流，1- 阻塞取流
            previewInfo.bBlocked = true;

            deviceInfo.realHandle = CHCNetSDK.NET_DVR_RealPlay_V40(deviceInfo.loginId, ref previewInfo, callBack, intPtr);
        }

        /// <summary>
        /// 关闭实时预览
        /// </summary>
        /// <param name="realHandle">预览句柄</param>
        /// <returns>true：关闭成功 false：关闭失败</returns>
        public override bool StopPreview(DeviceInfo deviceInfo)
        {
            bool result = CHCNetSDK.NET_DVR_StopRealPlay(deviceInfo.realHandle);
            return result;
        }

        #region 注释代码

        ///// <summary>
        ///// 视频抓图保存成JPEG图片
        ///// </summary>
        ///// <param name="loginId">用户ID值，从HikVisionLogin获取</param>
        ///// <param name="channel">通道号</param>
        ///// <param name="pictureParam">JPEG图像参数</param>
        ///// <param name="pictureFilePath">保存JPEG图的文件路径</param>
        ///// <param name="deviceName">设备名称</param>
        ///// <returns></returns>
        //public string CaptureJPEGPicture(int loginId, int channel, ref CHCNetSDK.NET_DVR_JPEGPARA pictureParam, string pictureFilePath, string deviceName)
        //{
        //    string result = string.Empty;

        //    if (!Directory.Exists(pictureFilePath))
        //    {
        //        try
        //        {
        //            Directory.CreateDirectory(pictureFilePath);
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }

        //    string pictureFileName = string.Format("{0}{1}_{2}.jpg", DateTime.Now.ToString("HHmmss"), DateTime.Now.Millisecond.ToString(), deviceName);

        //    pictureFileName = string.Format(@"{0}\{1}", pictureFilePath, pictureFileName);

        //    bool isSuccess = CHCNetSDK.NET_DVR_CaptureJPEGPicture(loginId, channel, ref pictureParam, pictureFileName);

        //    if (isSuccess)
        //    {
        //        result = pictureFileName;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 视频抓图保存成JPEG图片
        ///// </summary>
        ///// <param name="loginId">用户ID值，从HikVisionLogin获取</param>
        ///// <param name="channel">通道号</param>
        ///// <param name="pictureParam">JPEG图像参数</param>
        ///// <param name="pictureFilePath">保存JPEG图的文件路径</param>
        ///// <param name="deviceName">设备名称</param>
        ///// <param name="fileName">图片文件</param>
        ///// <returns>true：抓图成功 false：抓图失败</returns>
        //public bool CaptureJPEGPicture(int loginId, int channel, ref CHCNetSDK.NET_DVR_JPEGPARA pictureParam, string pictureFilePath, string deviceName, out string fileName)
        //{
        //    bool result = false;
        //    fileName = string.Empty;

        //    if (!Directory.Exists(pictureFilePath))
        //    {
        //        try
        //        {
        //            Directory.CreateDirectory(pictureFilePath);
        //        }
        //        catch (Exception ex)
        //        {
        //            return result;
        //        }
        //    }

        //    string pictureFileName = string.Format("{0}{1}_{2}.jpg", DateTime.Now.ToString("HHmmss"), DateTime.Now.Millisecond.ToString(), deviceName);

        //    pictureFileName = string.Format(@"{0}\{1}", pictureFilePath, pictureFileName);

        //    bool isSuccess = CHCNetSDK.NET_DVR_CaptureJPEGPicture(loginId, channel, ref pictureParam, pictureFileName);

        //    if (isSuccess)
        //    {
        //        fileName = pictureFileName;
        //        result = true;
        //    }

        //    return result;
        //}

        #endregion

        /// <summary>
        /// 建立报警上传通道
        /// </summary>
        /// <param name="loginId">用户ID值，从HikVisionLogin获取</param>
        /// <returns>-1:建立失败，否则建立成功</returns>
        public override void SetupAlarmChan(DeviceInfo deviceInfo)
        {
            CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam = new CHCNetSDK.NET_DVR_SETUPALARM_PARAM();
            struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
            struAlarmParam.byLevel = 1; //0- 一级布防,1- 二级布防
            struAlarmParam.byAlarmInfoType = 1;//智能交通设备有效，新报警信息类型
            struAlarmParam.byFaceAlarmDetection = 1;//1-人脸侦测

            deviceInfo.alarmHandle = CHCNetSDK.NET_DVR_SetupAlarmChan_V41(deviceInfo.loginId, ref struAlarmParam);
        }

        /// <summary>
        /// 设置报警上传回调函数
        /// </summary>
        /// <returns></returns>
        public override bool SetDVRMessageCallBack(CHCNetSDK.MSGCallBack_V31 msgCallback)
        {
            bool result = false;

            result = CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(msgCallback, IntPtr.Zero);

            return result;
        }

        /// <summary>
        /// 撤销报警上传通道
        /// </summary>
        /// <param name="alarmHandle">上传通道值</param>
        /// <returns>true：撤销成功 false：撤销失败</returns>
        public override bool CloseAlarmChan(DeviceInfo deviceInfo)
        {
            bool result = false;

            result = CHCNetSDK.NET_DVR_CloseAlarmChan_V30(deviceInfo.alarmHandle);

            return result;
        }

        /// <summary>
        /// 用户注销
        /// </summary>
        /// <param name="loginId">用户ID值，从HikVisionLogin获取</param>
        /// <returns>true：注销成功 false：注销失败</returns>
        public override bool LogOut(DeviceInfo deviceInfo)
        {
            bool result = CHCNetSDK.NET_DVR_Logout(deviceInfo.loginId);

            return result;
        }

        /// <summary>
        /// 释放SDK资源
        /// </summary>
        /// <returns>true：释放成功 false：释放失败</returns>
        public override bool Cleanup()
        {
            bool result = CHCNetSDK.NET_DVR_Cleanup();
            return result;
        }

        /// <summary>
        /// 获取错误码
        /// </summary>
        /// <returns>错误码</returns>
        public override uint GetLastError()
        {
            uint result = CHCNetSDK.NET_DVR_GetLastError();
            return result;
        }
    }
}
