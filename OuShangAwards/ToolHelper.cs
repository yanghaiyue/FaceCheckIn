using DataHelper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

namespace OuShangAwards
{
    public class ToolHelper
    {
        /// <summary>
        /// 记录系统错误日志
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="exMessage"></param>
        /// <param name="stackTrace"></param>
        public static void RecordSystemErrorLog(string className, string methodName, string exMessage, string stackTrace)
        {
            try
            {
                string fileName = string.Format("{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd"), "SystemError");
                string content = string.Format("{0} {1}.{2} 错误：{3};详情：{4}", DateTime.Now, className,
                    methodName, exMessage, stackTrace);
                RecordLog(fileName, content);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 记录系统信息日志
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="info"></param>
        public static void RecordSystemInfoLog(string className, string methodName, string info)
        {
            try
            {
                string fileName = string.Format("{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd"), "SystemInfo");
                string content = string.Format("{0} {1}.{2} 信息：{3}", DateTime.Now, className, methodName, info);
                RecordLog(fileName, content);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void RecordLog(string fileName, string content)
        {
            try
            {
                CatchLog logs = new CatchLog(fileName);
                logs.WriteLine(content);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="imgType"></param>
        /// <returns></returns>
        public static string downloadImg(string url, string filePath, string imgType)
        {
            string result = string.Empty;

            try
            {
                string username = ConfigurationManager.AppSettings["userName"];
                string password = ConfigurationManager.AppSettings["userPwd"];

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Credentials = new NetworkCredential(username, password);

                WebResponse response = request.GetResponse();

                using (Stream reader = response.GetResponseStream())
                {
                    string str = string.Format(@"{0}\{1}\{2}\{3}", Environment.CurrentDirectory, filePath, DateTime.Today.ToString("yyyy-MM-dd"), imgType);

                    if (!Directory.Exists(str))
                    {
                        Directory.CreateDirectory(str);
                    }

                    str = string.Format(@"{0}\{1}{2}.jpg", str, DateTime.Now.ToString("HHmmss"), DateTime.Now.Millisecond);
                    Image imageTemp = new Bitmap(reader);
                    imageTemp.Save(str);
                    result = str;
                }
            }
            catch (Exception ex)
            {
                RecordSystemErrorLog(typeof(ToolHelper).FullName, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public static bool SaveXmlFile(string filePath, string fileName, Dictionary<string, string> keyValue, string nodeType)
        {
            bool result = false;

            try
            {
                string filePathStr = string.Format(@"{0}\{1}", Environment.CurrentDirectory, filePath);
                string fileNameStr = string.Format(@"{0}\{1}_{2}.xml", filePathStr, fileName, nodeType);

                if (!Directory.Exists(filePathStr))
                {
                    Directory.CreateDirectory(filePathStr);
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlNode root;

                //更新
                if (File.Exists(fileNameStr))
                {
                    xmlDoc.Load(fileNameStr);
                    root = xmlDoc.DocumentElement;
                }
                //创建
                else
                {
                    //创建类型声明节点
                    XmlNode typeNode = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    xmlDoc.AppendChild(typeNode);

                    //创建根节点
                    root = xmlDoc.CreateElement("Users");
                    xmlDoc.AppendChild(root);
                }

                XmlNode nodeChild = xmlDoc.CreateElement("User");
                root.AppendChild(nodeChild);

                foreach (var item in keyValue)
                {
                    CreateNode(xmlDoc, nodeChild, item.Key, item.Value);
                }

                xmlDoc.Save(fileNameStr);
                result = true;
            }
            catch (Exception ex)
            {
                RecordSystemErrorLog(typeof(ToolHelper).FullName, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        public static bool SaveXmlFile(string filePath, string fileName, string nodeType)
        {
            bool result = false;

            try
            {
                string filePathStr = string.Format(@"{0}\{1}", Environment.CurrentDirectory, filePath);
                string fileNameStr = string.Format(@"{0}\{1}_total.xml", filePathStr, fileName);

                if (!Directory.Exists(filePathStr))
                {
                    Directory.CreateDirectory(filePathStr);
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlNode root;

                //更新
                if (File.Exists(fileNameStr))
                {
                    xmlDoc.Load(fileNameStr);
                    root = xmlDoc.DocumentElement;
                }
                //创建
                else
                {
                    //创建类型声明节点
                    XmlNode typeNode = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    xmlDoc.AppendChild(typeNode);

                    //创建根节点
                    root = xmlDoc.CreateElement("Total");
                    xmlDoc.AppendChild(root);
                }

                XmlNode nodeChild = root.SelectSingleNode(nodeType);

                if (nodeChild == null)
                {
                    CreateNode(xmlDoc, root, nodeType, "1");
                }
                else if (string.IsNullOrWhiteSpace(nodeChild.InnerText))
                {
                    nodeChild.InnerText = "1";
                }
                else
                {
                    nodeChild.InnerText = (int.Parse(nodeChild.InnerText) + 1).ToString();
                }

                xmlDoc.Save(fileNameStr);
                result = true;
            }
            catch (Exception ex)
            {
                RecordSystemErrorLog(typeof(ToolHelper).FullName, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 加载xml数据
        /// </summary>
        public static List<FaceCompareInfo> FaceCompareInfoLoad(string fileName)
        {
            List<FaceCompareInfo> faceCompareInfoList = new List<FaceCompareInfo>();
            try
            {
                string filePathStr = string.Format(@"{0}\FaceCompareXmlFile", Environment.CurrentDirectory);
                string fileNameStr = string.Format(@"{0}\{1}", filePathStr, fileName);

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

                            faceCompareInfoList.Add(faceCompareInfoItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordSystemErrorLog(typeof(ToolHelper).FullName, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return faceCompareInfoList;
        }

        /// <summary>
        /// 加载xml数据
        /// </summary>
        public static FaceCompareInfo FaceCompareTotalInfoLoad(string fileName)
        {
            FaceCompareInfo faceCompareInfo = new FaceCompareInfo();
            try
            {
                string filePathStr = string.Format(@"{0}\FaceCompareXmlFile", Environment.CurrentDirectory);
                string fileNameStr = string.Format(@"{0}\{1}", filePathStr, fileName);

                if (File.Exists(fileNameStr))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(fileNameStr);
                    XmlNode success = xmlDoc.SelectSingleNode("Total/compareSuccess");
                    XmlNode Fail = xmlDoc.SelectSingleNode("Total/compareFail");

                    if (success != null && !string.IsNullOrWhiteSpace(success.InnerText))
                    {
                        faceCompareInfo.CompareSuccess = int.Parse(success.InnerText);
                    }

                    if (Fail != null && !string.IsNullOrWhiteSpace(Fail.InnerText))
                    {
                        faceCompareInfo.CompareFail = int.Parse(Fail.InnerText);
                    }
                }   
            }
            catch (Exception ex)
            {
                RecordSystemErrorLog(typeof(ToolHelper).FullName, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return faceCompareInfo;
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="dataDicList"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ExportExcel(List<NameValueCollection> dataDicList, string fileName)
        {
            bool result = false;

            try
            {
                if (dataDicList != null && dataDicList.Count > 0)
                {
                    HSSFWorkbook book = new HSSFWorkbook();
                    ISheet sheet = book.CreateSheet("test_01");
                    int rc = 0;

                    foreach (NameValueCollection item in dataDicList)
                    {
                        int cc = 0;
                        IRow row = sheet.CreateRow(rc);
                        foreach (string keyItem in item.Keys)
                        {
                            row.CreateCell(cc).SetCellValue(item[keyItem]);
                            cc++;
                        }
                        rc++;
                    }

                    FileStream fsm = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    book.Write(fsm);
                    fsm.Close();
                    fsm.Dispose();
                    book = null;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                RecordSystemErrorLog(typeof(ToolHelper).FullName, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return result;
        }

        /// <summary>  
        /// 创建节点  
        /// </summary>  
        /// <param name="xmldoc"></param>  xml文档
        /// <param name="parentnode"></param>父节点  
        /// <param name="name"></param>  节点名
        /// <param name="value"></param>  节点值
        /// 
        private static void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateElement(name);

            if (!String.IsNullOrWhiteSpace(value))
            {
                node.InnerText = value;
            }

            parentNode.AppendChild(node);
        }

        

        //public static void SerializableXML<T>(T myObject) where T : class, new()
        //{
        //    if (myObject != null)
        //    {
        //        XmlSerializer xs = new XmlSerializer(typeof(T));

        //        MemoryStream stream = new MemoryStream();
        //        XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
        //        writer.Formatting = Formatting.Indented;//缩进
        //        new FileStream(path, FileMode.Create);
        //        xs.Serialize(writer, myObject);
        //        writer.Close();
        //    }
        //}
    }
}
