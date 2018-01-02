using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OuShangAwards
{
    public class DeviceInfo
    {
        private string _deviceId = string.Empty;//设备Id
        private string _deviceName = string.Empty;//设备名称
        private string _deviceCode = string.Empty;//设备Code
        private string _deviceIp = string.Empty;//设备Ip 
        private string _userName = string.Empty;//登录设备用户名
        private string _userPwd = string.Empty;//登录设备密码
        private string _deviceType = string.Empty;//设备类型
        private IntPtr _handle = new IntPtr(); //播放窗口的句柄

        public int devicePort;//设备端口号
        public int channel = -1;//通道
        public int loginId = -1;//登录Id
        public int realHandle = -1;//预览句柄
        public int alarmHandle = -1;//报警上传通道

        public string deviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }
        public string deviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }
        public string deviceCode
        {
            get { return _deviceCode; }
            set { _deviceCode = value; }
        }
        public string deviceIp
        {
            get { return _deviceIp; }
            set { _deviceIp = value; }
        }
        public string userName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public string userPwd
        {
            get { return _userPwd; }
            set { _userPwd = value; }
        }
        public string deviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }
        public IntPtr Handle
        {
            get { return _handle; }
            set { _handle = value; }
        }
    }
}
