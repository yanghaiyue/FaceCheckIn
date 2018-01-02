using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OuShangAwards
{
    public class FaceCompareInfo
    {
        private string _caputerDeviceIp = string.Empty;
        private string _channel = string.Empty;
        private string _captureTime = string.Empty;
        private string _similarityDegree = string.Empty;
        private string _personName = string.Empty;
        private string _alarmTime = string.Empty;
        private string _alarmDeviceIp = string.Empty;
        private string _sex = string.Empty;
        private string _glasses = string.Empty;
        private string _faceScore = string.Empty;
        private string _bigImage = string.Empty;
        private string _faceImage = string.Empty;
        private string _compareImage = string.Empty;
        private string _telePhone = string.Empty;
        public bool isShow = false;

        public int CompareSuccess { get; set; }
        public int CompareFail { get; set; }

        public string CaputerDeviceIp
        {
            get { return _caputerDeviceIp; }
            set { _caputerDeviceIp = value; }
        }

        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public string CaptureTime
        {
            get { return _captureTime; }
            set { _captureTime = value; }
        }

        public string SimilarityDegree
        {
            get { return _similarityDegree; }
            set { _similarityDegree = value; }
        }

        public string PersonName
        {
            get { return _personName; }
            set { _personName = value; }
        }

        public string AlarmTime
        {
            get { return _alarmTime; }
            set { _alarmTime = value; }
        }

        public string AlarmDeviceIp
        {
            get { return _alarmDeviceIp; }
            set { _alarmDeviceIp = value; }
        }

        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        public string Glasses
        {
            get { return _glasses; }
            set { _glasses = value; }
        }

        public string FaceScore
        {
            get { return _faceScore; }
            set { _faceScore = value; }
        }

        public string BigImage
        {
            get { return _bigImage; }
            set { _bigImage = value; }
        }

        public string FaceImage
        {
            get { return _faceImage; }
            set { _faceImage = value; }
        }

        public string CompareImage
        {
            get { return _compareImage; }
            set { _compareImage = value; }
        }

        public string TelePhone
        {
            get { return _telePhone; }
            set { _telePhone = value; }
        }
    }
}
