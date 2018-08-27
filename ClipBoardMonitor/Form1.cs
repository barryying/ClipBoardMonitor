﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
//using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipBoardMonitor
{
    public partial class Form1 : Form
    {
        #region Definitions
        //Constants for API Calls...
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;

        //Handle for next clipboard viewer...
        private IntPtr mNextClipBoardViewerHWnd;

        //API declarations...
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool ChangeClipboardChain(IntPtr HWnd, IntPtr HWndNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        #endregion
        private string imageDirectory = Application.StartupPath + "\\Images";

        string startuppath = Application.StartupPath.Replace(@"\\", @"\");
        RegistryKey RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        bool isdoubleclicked = false;
        
        public Form1()
        {
            InitializeComponent();

            if (!Is64Bit())
            {
                //MessageBox.Show("您的系统是32位系统,可能不支持此软件！");
                //Application.Exit();
            }

            // 添加到 当前登陆用户的 注册表启动项
            RKey.SetValue("ClipBoardMonitor", startuppath + @"\ClipBoardMonitor.exe");

            //// 添加到 所有用户的 注册表启动项
            //RegistryKey RKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            //RKey.SetValue("AppName", @"C:\AppName.exe");

            //控件大小随窗体大小等比例缩放 初始化
            x = this.Width;
            y = this.Height;
            setTag(this);

            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }
            else
            {
                DelectDir(imageDirectory);
            }
            // 添加ListView表头标题
            this.listView1.Columns.Add("序号", 60, HorizontalAlignment.Left); //添加标题
            this.listView1.Columns.Add("复制的文本", 400, HorizontalAlignment.Left);
            this.listView1.View = System.Windows.Forms.View.Details;
            
            mNextClipBoardViewerHWnd = SetClipboardViewer(this.Handle);
        }

        #region 重写wndproc方法获取Message  定义热键使用
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        //The clipboard has changed...
                        //##########################################################################
                        // Process Clipboard Here :)........................
                        //##########################################################################
                        SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());

                        //显示剪贴板中的文本信息
                        if (Clipboard.ContainsText())
                        {
                            AddTxtListView(Clipboard.GetText());
                            richTextBox1.Text = Clipboard.GetText();
                        }
                        //显示剪贴板中的图片信息
                        if (Clipboard.ContainsImage())
                        {
                            AddPictureListView(Clipboard.GetImage());
                            //pictureBox1.Image = Clipboard.GetImage();
                            //pictureBox1.Update();
                        }
                        break;
                    }
                case WM_CHANGECBCHAIN:
                    {
                        //Another clipboard viewer has removed itself...
                        if (m.WParam == (IntPtr)mNextClipBoardViewerHWnd)
                        {
                            mNextClipBoardViewerHWnd = m.LParam;
                        }
                        else
                        {
                            SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
                        }
                        break;
                    }
            }
            base.WndProc(ref m);
        }
        #endregion

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //从观察链中删除本观察窗口  
            ChangeClipboardChain(this.Handle, mNextClipBoardViewerHWnd);

            //将WM_DRAWCLIPBOARD消息传递到下一个观察链中的窗口   
            SendMessage(mNextClipBoardViewerHWnd, WM_CHANGECBCHAIN, this.Handle.ToInt32(), mNextClipBoardViewerHWnd.ToInt32());
        }

        private void AddTxtListView(string value)
        {
            value = value.TrimStart().TrimEnd();
            if (!value.Equals(""))
            {
                if (listView1.Items.Count > 0)
                {
                    foreach (ListViewItem lt in listView1.Items)
                    {
                        //MessageBox.Show(Convert.ToString(lt.SubItems[1].Text));
                        if (lt.SubItems[1].Text.Equals(value))     //比较每一行记录的第2个字段值
                        {
                            this.label1.Text = "这项记录已存在！";
                            return;
                        }
                    }
                }

                this.listView1.BeginUpdate(); //数据更新，UI暂时挂起

                // 添加序号
                ListViewItem lvi = new ListViewItem();
                lvi.Text = (this.listView1.Items.Count + 1).ToString();

                // 添加子项
                lvi.SubItems.Add(value);
                this.listView1.Items.Add(lvi);

                this.label1.Text = "添加了一个文本！";
                this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。

                // ListView获取焦点
                listView1.Focus();
            }
        }

        private void AddPictureListView(Image image)
        {
            if (image != null)
            {
                if (listView2.Items.Count > 0)
                {
                    int similarcount = 0;
                    foreach (ListViewItem lt in listView2.Items)
                    {
                        string imagePath = imageDirectory + "\\image" + (lt.ImageIndex + 1).ToString() + ".png";
                        //Image imageSource = Image.FromFile(imagePath);
                        SimilarPhoto similarPhotoSource = new SimilarPhoto(imagePath);
                        string imageSourceHash = similarPhotoSource.GetHash();

                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            SimilarPhoto similarPhotoAdd = new SimilarPhoto(ms);
                            string imageHash = similarPhotoAdd.GetHash();

                            //MessageBox.Show(Convert.ToString(lt.SubItems[1].Text));
                            if (SimilarPhoto.CalcSimilarDegree(imageSourceHash, imageHash) <= 5)
                            {
                                similarcount++;                                
                            }
                        }
                    }
                    if (!isdoubleclicked)
                    {
                        if(similarcount > 0)
                        {
                            DialogResult dr = MessageBox.Show("程序为您辨别此次截图与已存在的" + similarcount + "条记录相似，是否继续保存?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                            if (dr == DialogResult.OK)   //如果单击“是”按钮
                            {
                                //继续保存
                            }
                            else if (dr == DialogResult.Cancel)
                            {
                                //不保存
                                this.label1.Text = "这项记录已存在！";
                                return;
                            }
                        }
                        
                    }
                    else
                    {
                        //不保存
                        this.label1.Text = "这项记录已存在！";
                        return;
                    }
                }

                //listView2.Items.Clear();
                //imageList1.Images.Clear();
                this.listView2.BeginUpdate(); //数据更新，UI暂时挂起
                image.Save(imageDirectory + "\\image" + (imageList1.Images.Count + 1).ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                imageList1.ImageSize = new Size(256, 120);
                imageList1.ColorDepth = ColorDepth.Depth32Bit;
                this.imageList1.Images.Add(image);
                //imageList1.Images[imageList1.Images.Count - 1].Tag = imageList1.Images.Count;

                listView2.LargeImageList = imageList1;
                var lvi = new ListViewItem();
                lvi.ImageIndex = imageList1.Images.Count - 1;
                //lvi.Text = apptitlelist[i];//Path.GetFileNameWithoutExtension(filePath);//"P" + i;
                //lvi.ToolTipText = apppathlist[i];//Path.GetFileNameWithoutExtension(filePath);//"P" + i;
                listView2.Items.Add(lvi);
                this.label1.Text = "添加了一张图片！";
                this.listView2.EndUpdate();  //结束数据处理，UI界面一次性绘制。

                // ListView获取焦点
                listView2.Focus();
            }
        }

        // 测试代码
        private void button1_Click(object sender, EventArgs e)
        {
            mNextClipBoardViewerHWnd = SetClipboardViewer(this.Handle);
            //// GetDataObject检索当前剪贴板上的数据
            //IDataObject iData = Clipboard.GetDataObject();
            //// 将数据与指定的格式进行匹配，返回bool
            //if (iData.GetDataPresent(DataFormats.Text))
            //{
            //    // GetData检索数据并指定一个格式
            //    this.richTextBox1.Text = (string)iData.GetData(DataFormats.Text);
            //}
            //else
            //{
            //    MessageBox.Show("剪贴板中数据不可转换为文本", "错误");
            //}
        }

        public static void DelectDir(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
                return;

            //前提，listview禁止多选
            ListViewItem currentRow = listView1.SelectedItems[0];
            Clipboard.SetDataObject(currentRow.SubItems[1].Text,true);
            this.label1.Text = "已复制到剪贴板！";
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView2.SelectedItems.Count == 0)
                return;

            //标记双击
            isdoubleclicked = true;
            //前提，listview禁止多选
            ListViewItem currentRow = listView2.SelectedItems[0];
            //Image image = currentRow.ImageList.Images[currentRow.ImageIndex];

            string imagePath = imageDirectory + "\\image" + (currentRow.ImageIndex + 1).ToString() + ".png";

            FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            Image image = Image.FromStream(fs);
            fs.Close();

            //建立新的系统进程    
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            //设置文件名，此处为图片的真实路径+文件名    
            process.StartInfo.FileName = imagePath;
            //此为关键部分。设置进程运行参数，此时为最大化窗口显示图片。    
            process.StartInfo.Arguments = "rundll32.exe C://WINDOWS//system32//shimgvw.dll,ImageView_Fullscreen";
            //此项为是否使用Shell执行程序，因系统默认为true，此项也可不设，但若设置必须为true    
            process.StartInfo.UseShellExecute = true;
            //此处可以更改进程所打开窗体的显示样式，可以不设    
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            process.Start();
            process.Dispose();

            //image = pictureProcess(image, 1000, 600);
            //image = resizeImage(image, new Size(1000, 600));
            Clipboard.SetImage(image);
            //Form2 f2 = new Form2();
            //f2.Form2Value = image;
            //f2.ShowDialog();
            //pictureBox1.Image = image;
            //pictureBox1.Update();
            this.label1.Text = "已复制到剪贴板！";
            isdoubleclicked = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("退出后将只保留最后一条复制的内容，是否退出?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (dr == DialogResult.OK)   //如果单击“是”按钮
            {
                e.Cancel = false;                 //关闭窗体
            }
            else if (dr == DialogResult.Cancel)
            {
                e.Cancel = true;                  //不执行操作
            }
        }
        private static Image resizeImage(Image imgToResize, Size size)
        {
            //获取图片宽度
            int sourceWidth = imgToResize.Width;
            //获取图片高度
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //计算宽度的缩放比例
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //计算高度的缩放比例
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //期望的宽度
            int destWidth = (int)(sourceWidth * nPercent);
            //期望的高度
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }
        public static bool ImageCompareString(Image firstImage, Image secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;

            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());

            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region 控件大小随窗体大小等比例缩放
        private float x;//定义当前窗体的宽度
        private float y;//定义当前窗体的高度
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ";" + con.Height + ";" + con.Left + ";" + con.Top + ";" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    setTag(con);
                }
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            //遍历窗体中的控件，重新设置控件的值
            foreach (Control con in cons.Controls)
            {
                //获取控件的Tag属性值，并分割后存储字符串数组
                if (con.Tag != null)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ';' });
                    //根据窗体缩放的比例确定控件的值
                    con.Width = Convert.ToInt32(System.Convert.ToSingle(mytag[0]) * newx);//宽度
                    con.Height = Convert.ToInt32(System.Convert.ToSingle(mytag[1]) * newy);//高度
                    con.Left = Convert.ToInt32(System.Convert.ToSingle(mytag[2]) * newx);//左边距
                    con.Top = Convert.ToInt32(System.Convert.ToSingle(mytag[3]) * newy);//顶边距
                    //Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字体大小
                    //con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                    {
                        setControls(newx, newy, con);
                    }
                }
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / x;
            float newy = (this.Height) / y;
            setControls(newx, newy, this);
        }
        #endregion

        private void 清空历史ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("清空后将不保留所有内容(包括本地存放的图片文件)，是否清空?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (dr == DialogResult.OK)   //如果单击“是”按钮
            {
                this.listView1.Items.Clear();
                this.listView2.Items.Clear();
                this.imageList1.Images.Clear();
                if (Directory.Exists(imageDirectory))
                {
                    DelectDir(imageDirectory);
                }
                this.label1.Text = "已清空了所有记录！";
            }
            else if (dr == DialogResult.Cancel)
            {
                this.label1.Text = "您取消了清空历史的操作！";
            }
        }
        private void 打开图片存放位置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("ExpLorer", imageDirectory);
            this.label1.Text = "您打开了图片存放位置！";
        }

        private void 开机启动ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if(开机启动ToolStripMenuItem.Checked == false)
            {
                RKey.DeleteValue("ClipBoardMonitor");
                this.label1.Text = "您修改了开机不启动！";
            }
            else
            {
                // 添加到 当前登陆用户的 注册表启动项
                RKey.SetValue("ClipBoardMonitor", startuppath + @"\ClipBoardMonitor.exe");
                this.label1.Text = "您修改了开机启动！";
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            //ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            //ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("删除此条记录");
            //contextMenuStrip.Items.Add(toolStripMenuItem);
            //鼠标右键
            if (e.Button == MouseButtons.Right)
            {
                //filesList.ContextMenuStrip = contextMenuStrip1;
                //选中列表中数据才显示 空白处不显示
                String fileName = listView1.SelectedItems[0].Text; //获取选中文件名
                Point p = new Point(e.X, e.Y);
                contextMenuStrip1.Show(listView1, p);
            }
        }

        private void 删除此条记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.SelectedItems)
            {
                if (item.Selected)
                {
                    item.Remove();
                }
            }
            //重新排序
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Text = (i + 1).ToString();
            }
            listView1.Refresh(); //删除结束后刷新listView
        }
        
        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = this.listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                toolTip1.Show(item.SubItems[1].Text, listView1, new Point(e.X + 15, e.Y + 15), 1000);
                toolTip1.Active = true;
            }
            else
            {
                toolTip1.Active = false;
            }

            bool iscontains = false;
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                Rectangle rec = listView1.Items[i].GetBounds(ItemBoundsPortion.Entire);
                if (rec.Contains(e.Location))
                {
                    iscontains = true;
                    break;
                }
            }
            if (iscontains)
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewItem item = this.listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                item.Selected = true;
            }

            //Point curPos = this.listView1.PointToClient(Control.MousePosition);
            //ListViewItem lvwItem = this.listView1.GetItemAt(curPos.X, curPos.Y);


            //if (lvwItem != null)
            //{
            //    lvwItem.Checked = !lvwItem.Checked;
            //    listView1.Refresh();
            //}
        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            //鼠标右键
            if (e.Button == MouseButtons.Right)
            {
                //filesList.ContextMenuStrip = contextMenuStrip1;
                //选中列表中数据才显示 空白处不显示
                String fileName = listView2.SelectedItems[0].Text; //获取选中文件名
                Point p = new Point(e.X, e.Y);
                contextMenuStrip2.Show(listView2, p);
            }
        }

        private void 删除此条记录ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView2.SelectedItems)
            {
                if (item.Selected)
                {
                    item.Remove();
                    string imageindex = (item.ImageIndex + 1).ToString();
                    File.Delete(imageDirectory + "\\image" + imageindex + ".png");
                }
            }
            ////重新排序
            //for (int i = 0; i < listView2.Items.Count; i++)
            //{
            //    listView2.Items[i].ImageIndex = i;
            //}
            listView2.Refresh(); //删除结束后刷新listView
        }

        #region 判断系统是x86还是x64位
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);
        
        private static bool Is64Bit()
        {
            bool retVal;
            IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
            return retVal;
        }
        #endregion
    }
}
