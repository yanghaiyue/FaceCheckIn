using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace OuShangAwards
{
    public partial class FaceCompareInfoForm : Form
    {
        private List<FaceCompareInfo> faceCompareInfoList = null;

        public FaceCompareInfoForm()
        {
            InitializeComponent();
        }

        private void FaceCompareInfoForm_Load(object sender, EventArgs e)
        {
            this.button1.Visible = false;
            this.listView1.HideSelection = false;
            this.radioButton1.Checked = true;
            this.radioButton2.Checked = false;
            this.label1.Visible = false;
        }

        private void FaceCompareInfoForm_Shown(object sender, EventArgs e)
        {
            try
            {
                FaceCompareInfo faceCompareTotalInfo = ToolHelper.FaceCompareTotalInfoLoad("faceCompare_total.xml");

                if (faceCompareTotalInfo != null)
                {
                    this.label1.Visible = true;
                    this.label2.Text = string.Format(@"比对成功({0}),比对失败({1})", faceCompareTotalInfo.CompareSuccess, faceCompareTotalInfo.CompareFail);
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.listView1.SelectedItems.Count > 0 && this.radioButton1.Checked)
                {
                    this.pictureBox1.Image = new Bitmap(faceCompareInfoList[this.listView1.SelectedItems[0].Index].BigImage);
                    this.pictureBox2.Image = new Bitmap(faceCompareInfoList[this.listView1.SelectedItems[0].Index].FaceImage);
                    this.pictureBox3.Image = new Bitmap(faceCompareInfoList[this.listView1.SelectedItems[0].Index].CompareImage);
                }
                if (this.listView1.SelectedItems.Count > 0 && this.radioButton2.Checked)
                {
                    this.pictureBox1.Image = new Bitmap(faceCompareInfoList[this.listView1.SelectedItems[0].Index].BigImage);
                    this.pictureBox2.Image = new Bitmap(faceCompareInfoList[this.listView1.SelectedItems[0].Index].FaceImage);
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.radioButton1.Checked)
                {
                    return;
                }

                this.button1.Visible = false;
                this.pictureBox1.Image = null;
                this.pictureBox2.Image = null;
                this.pictureBox3.Image = null;

                if (this.listView1.Items != null && this.listView1.Items.Count > 0)
                {
                    this.listView1.Items.Clear();
                }
                
                faceCompareInfoList = ToolHelper.FaceCompareInfoLoad("faceCompare_compareSuccess.xml");

                if (faceCompareInfoList != null && faceCompareInfoList.Count > 0)
                {
                    ListViewItem listViewItem = null;
                    foreach (FaceCompareInfo item in faceCompareInfoList)
                    {
                        listViewItem = this.listView1.Items.Add(item.CaptureTime);
                        listViewItem.SubItems.Add(item.PersonName);
                        listViewItem.SubItems.Add(item.Sex);
                        listViewItem.SubItems.Add(item.TelePhone);
                        listViewItem.SubItems.Add("签到");
                    }
                    this.button1.Visible = true;
                    this.listView1.Items[0].Selected = true;
                    this.pictureBox1.Image = new Bitmap(faceCompareInfoList[0].BigImage);
                    this.pictureBox2.Image = new Bitmap(faceCompareInfoList[0].FaceImage);
                    this.pictureBox3.Image = new Bitmap(faceCompareInfoList[0].CompareImage);
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.radioButton2.Checked)
                {
                    return;
                }

                this.button1.Visible = false;
                this.pictureBox1.Image = null;
                this.pictureBox2.Image = null;
                this.pictureBox3.Image = null;

                if (this.listView1.Items != null && this.listView1.Items.Count > 0)
                {
                    this.listView1.Items.Clear();
                }

                faceCompareInfoList = ToolHelper.FaceCompareInfoLoad("faceCompare_compareFail.xml");

                if (faceCompareInfoList != null && faceCompareInfoList.Count > 0)
                {
                    ListViewItem listViewItem = null;
                    foreach (FaceCompareInfo item in faceCompareInfoList)
                    {
                        listViewItem = this.listView1.Items.Add(item.CaptureTime);
                        listViewItem.SubItems.Add(item.PersonName);
                        listViewItem.SubItems.Add(item.Sex);
                        listViewItem.SubItems.Add(item.TelePhone);
                        listViewItem.SubItems.Add("签到");
                    }
                    this.button1.Visible = true;
                    this.listView1.Items[0].Selected = true;
                    this.pictureBox1.Image = new Bitmap(faceCompareInfoList[0].BigImage);
                    this.pictureBox2.Image = new Bitmap(faceCompareInfoList[0].FaceImage);
                }
            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Excel表格（*.xls）|*.xls";

                //保存对话框是否记忆上次打开的目录 
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string localFilePath = dialog.FileName.ToString(); //获得文件路径 
                    string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径

                    #region Excel column名称处理

                    NameValueCollection dicData = (NameValueCollection)ConfigurationManager.GetSection("FaceCompareInfo");
                    List<NameValueCollection> dicDataList = new List<NameValueCollection>();
                    if (dicData != null && dicData.Count > 0)
                    {
                        dicDataList.Add(dicData);
                    }

                    #endregion

                    #region Excel数据处理

                    string dataType = string.Empty;

                    if (radioButton1.Checked)
                    {
                        dataType = "compareSuccess";
                    }
                    else if (radioButton2.Checked)
                    {
                        dataType = "compareFail";
                    }

                    List<FaceCompareInfo> faceCompareInfoList = ToolHelper.FaceCompareInfoLoad(string.Format("faceCompare_{0}.xml", dataType));

                    if (faceCompareInfoList != null && faceCompareInfoList.Count > 0)
                    {
                        foreach (FaceCompareInfo item in faceCompareInfoList)
                        {
                            dicData = new NameValueCollection();
                            dicData.Add("captureTime", item.CaptureTime);
                            dicData.Add("personName", item.PersonName);
                            dicData.Add("sex", item.Sex);
                            dicData.Add("telePhone", item.TelePhone);
                            dicData.Add("similarityDegree", item.SimilarityDegree);
                            dicData.Add("bigImage", item.BigImage);
                            dicData.Add("faceImage", item.FaceImage);
                            dicData.Add("compareImage", item.CompareImage);
                            dicDataList.Add(dicData);
                        }
                    }

                    bool isSuccess = false;

                    if (dicDataList != null && dicDataList.Count > 1)
                    {
                        isSuccess = ToolHelper.ExportExcel(dicDataList, localFilePath);
                    }

                    if (isSuccess)
                    {
                        MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("导出失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion
                }

            }
            catch (Exception ex)
            {
                ToolHelper.RecordSystemErrorLog(this.GetType().ToString(), MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}
