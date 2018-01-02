using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OuShangAwards
{
    public class BaseHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            return false;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="deviceInfo"></param>
        public virtual void Login(DeviceInfo deviceInfo)
        {
        }

        /// <summary>
        /// 预览
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="callBack"></param>
        public virtual void Preview(DeviceInfo deviceInfo, CHCNetSDK.REALDATACALLBACK callBack)
        {
        }

        /// <summary>
        /// 停止预览
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public virtual bool StopPreview(DeviceInfo deviceInfo)
        {
            return false;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public virtual bool LogOut(DeviceInfo deviceInfo)
        {
            return false;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns></returns>
        public virtual bool Cleanup()
        {
            return false;
        }

        /// <summary>
        /// 建立报警上传通道
        /// </summary>
        /// <param name="deviceInfo"></param>
        public virtual void SetupAlarmChan(DeviceInfo deviceInfo)
        {
        }

        /// <summary>
        /// 设置报警上传回调函数
        /// </summary>
        /// <param name="msgCallback"></param>
        /// <returns></returns>
        public virtual bool SetDVRMessageCallBack(CHCNetSDK.MSGCallBack_V31 msgCallback)
        {
            return false;
        }

        /// <summary>
        /// 撤销报警上传通道
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public virtual bool CloseAlarmChan(DeviceInfo deviceInfo)
        {
            return false;
        }

        /// <summary>
        /// 获取错误码
        /// </summary>
        /// <returns>错误码</returns>
        public virtual uint GetLastError()
        {
            return 0;
        }
    }
}
