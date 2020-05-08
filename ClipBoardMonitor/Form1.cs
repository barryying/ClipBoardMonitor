using Microsoft.Win32;
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
using System.Threading;
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
        
        [System.Runtime.InteropServices.DllImport("user32.dll")] //申明API函数 
        public static extern bool RegisterHotKey(
         IntPtr hWnd, // handle to window   
         int id, // hot key identifier   
         uint fsModifiers, // key-modifier options   
         Keys vk // virtual-key code   
        );

        [System.Runtime.InteropServices.DllImport("user32.dll")] //申明API函数  
        public static extern bool UnregisterHotKey(
         IntPtr hWnd, // handle to window   
         int id // hot key identifier   
        );
        public enum KeyModifiers //组合键枚举 
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }

        private string provinceAndcityName = AppConfigHelper.GetConfigValue("cityname");
        cn.com.webxml.www.WeatherWebService wws = new cn.com.webxml.www.WeatherWebService();

        private string imageDirectory = Application.StartupPath + "\\Images";
        private string imageFavoriteDirectory = Application.StartupPath + "\\Favorite";
        private string txtFavoriteDirectory = Application.StartupPath + "\\Favorite\\favorite.txt";

        string startuppath = Application.StartupPath.Replace(@"\\", @"\");
        RegistryKey RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        bool isdoubleclicked = false;

        private Boolean isPause = true;

        public Form1()
        {
            InitializeComponent();

            //设定按字体来缩放控件
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //设定字体大小为12px     
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));

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
            if (!Directory.Exists(imageFavoriteDirectory))
            {
                Directory.CreateDirectory(imageFavoriteDirectory);
            }
            if(!File.Exists(txtFavoriteDirectory))
            {
                File.Create(txtFavoriteDirectory);
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
            //如果m.Msg的值为0x0312那么表示用户按下了热键
            const int WM_HOTKEY = 0x0312;

            switch (m.Msg)
            {
                case WM_HOTKEY:
                    {
                        if (m.WParam.ToString() == "100")
                        {
                            GlobalKeyProcess();
                        }
                        break;
                    }
                case WM_DRAWCLIPBOARD:
                    {
                        if (isPause)
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
#if DEBUG
                                Console.WriteLine(Clipboard.GetImage());
#endif
                                AddPictureListView(Clipboard.GetImage());
                                //pictureBox1.Image = Clipboard.GetImage();
                                //pictureBox1.Update();
                            }
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
            // 将系统消息传递自父类的WndProc
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

                this.label1.Text = "添加了一个文本: \"" + value.Substring(0, value.Length > 10 ? 10 : value.Length) + "....\"！";
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
                        if(similarcount > 1)
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
                        else if (similarcount == 1)
                        {
                            //不保存
                            this.label1.Text = "这项记录已存在！";
                            return;
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
                this.imageList1.Images.Add(resizeImage(image, new Size(390, 160)));
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
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (Image)b;
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
            复制此条记录ToolStripMenuItem_Click(sender, e);
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            复制此条记录ToolStripMenuItem1_Click(sender, e);
        }

        /// <summary>
        /// 窗体关闭时处理程序
        /// 窗体关闭时取消热键注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("退出后将只保留最后一条复制的内容，是否退出?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (dr == DialogResult.OK)   //如果单击“是”按钮
            {
                this.timer1.Stop();
                // 卸载热键
                if(Handle != null)
                    UnregisterHotKey(Handle, 100);
                e.Cancel = false;                 //关闭窗体
            }
            else if (dr == DialogResult.Cancel)
            {
                e.Cancel = true;                  //不执行操作
            }
        }
        //private static Image resizeImage(Image imgToResize, Size size)
        //{
        //    //获取图片宽度
        //    int sourceWidth = imgToResize.Width;
        //    //获取图片高度
        //    int sourceHeight = imgToResize.Height;

        //    float nPercent = 0;
        //    float nPercentW = 0;
        //    float nPercentH = 0;
        //    //计算宽度的缩放比例
        //    nPercentW = ((float)size.Width / (float)sourceWidth);
        //    //计算高度的缩放比例
        //    nPercentH = ((float)size.Height / (float)sourceHeight);

        //    if (nPercentH < nPercentW)
        //        nPercent = nPercentH;
        //    else
        //        nPercent = nPercentW;
        //    //期望的宽度
        //    int destWidth = (int)(sourceWidth * nPercent);
        //    //期望的高度
        //    int destHeight = (int)(sourceHeight * nPercent);

        //    Bitmap b = new Bitmap(destWidth, destHeight);
        //    Graphics g = Graphics.FromImage((System.Drawing.Image)b);
        //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    //绘制图像
        //    g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
        //    g.Dispose();
        //    return b;
        //}
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
            DialogResult dr = MessageBox.Show("清空后将不保留所有内容(包括本地存放的图片文件，但收藏夹不会被清空)，是否清空?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

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
                    this.label1.Text = "您已将 \"" + item.SubItems[1].Text.Substring(0, item.SubItems[1].Text.Length > 10 ? 10 : item.SubItems[1].Text.Length) + "....\" 删除了！";
                }
            }
            //重新排序
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Text = (i + 1).ToString();
            }
            listView1.Refresh(); //删除结束后刷新listView
        }

        private void 复制此条记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
                return;

            //前提，listview禁止多选
            ListViewItem currentRow = listView1.SelectedItems[0];
            Clipboard.SetDataObject(currentRow.SubItems[1].Text, true);
            this.label1.Text = "已复制到剪贴板！";
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = this.listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                toolTip1.Show(item.SubItems[1].Text, listView1, new Point(e.X + 15, e.Y + 15), 10000); //参数duration设置为大一点可以避免tooltip连续闪烁
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
                    this.label1.Text = "您已将 image" + imageindex + ".png 删除了！";
                }
            }
            ////重新排序
            //for (int i = 0; i < listView2.Items.Count; i++)
            //{
            //    listView2.Items[i].ImageIndex = i;
            //}
            listView2.Refresh(); //删除结束后刷新listView
        }
        private void 复制此条记录ToolStripMenuItem1_Click(object sender, EventArgs e)
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

        private void 开始截图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //96 DPI = 100% scaling
            //120 DPI = 125 % scaling
            //144 DPI = 150 % scaling
            //192 DPI = 200 % scaling
            // C# code to get dpi setting:
            //float dpiX, dpiY;
            //Graphics graphics = this.CreateGraphics();
            //dpiX = graphics.DpiX;
            //dpiY = graphics.DpiY;
            float dpiX, dpiY;
            dpiX = PrimaryScreen.ScaleX;
            dpiY = PrimaryScreen.ScaleY;

            this.Hide();   //隐藏当前窗体

            Thread.Sleep(400);
            //新建一个和屏幕大小相同的图片
            Bitmap CatchBmp = new Bitmap((int)(Screen.AllScreens[0].Bounds.Width * dpiX), (int)(Screen.AllScreens[0].Bounds.Height * dpiY));
            //Bitmap CatchBmp = new Bitmap((int)(Screen.AllScreens[0].Bounds.Width), (int)(Screen.AllScreens[0].Bounds.Height));
            
            // 创建一个画板，让我们可以在画板上画图
            // 这个画板也就是和屏幕大小一样大的图片
            // 我们可以通过Graphics这个类在这个空白图片上画图
            Graphics g = Graphics.FromImage(CatchBmp);

            // 把屏幕图片拷贝到我们创建的空白图片 CatchBmp中
            //g.CopyFromScreen(new Point(0, 0), new Point(0, 0), Screen.AllScreens[0].Bounds.Size);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), PrimaryScreen.DESKTOP);
            //g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size((int)(Screen.AllScreens[0].Bounds.Width * dpiX), (int)(Screen.AllScreens[0].Bounds.Height * dpiY)));

            // 创建截图窗体
            Cutter cutter = new Cutter();

            // 指示窗体的背景图片为屏幕图片
            cutter.BackgroundImage = CatchBmp;
            // 显示窗体
            //cutter.Show();
            // 如果Cutter窗体结束，则从剪切板获得截取的图片，并显示在聊天窗体的发送框中
            if (cutter.ShowDialog() == DialogResult.OK)
            {
                //IDataObject iData = Clipboard.GetDataObject();

                //if (iData.GetDataPresent(DataFormats.Bitmap))
                //{
                //    richTextBox1.Paste();

                //    // 清楚剪贴板的图片
                //    Clipboard.Clear();
                //}
                label1.Text = "您已截取图片！";
                this.Show();//重新显示窗体
            }
            else
            {
                label1.Text = "您取消了截取图片！";
                this.Show();//重新显示窗体
            }
        }

        private void 开始截图ToolStripMenuItem_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.Show("截图说明：点击此处或按Ctrl+1进行截图操作，开始后按住左键拖动截图区域，松开后双击截图区域完成截图，右键取消截图。", menuStrip1, new Point(e.X + 115, e.Y + 15), 10000); //参数duration设置为大一点可以避免tooltip连续闪烁
            toolTip1.Active = true;
        }

        private void 开始截图ToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Active = false;
        }

        /// <summary>
        /// 窗体加载事件处理
        /// 在窗体加载时注册热键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            uint ctrlHotKey = (uint)(KeyModifiers.Control);
            // 注册热键为Alt+Ctrl+C, "100"为唯一标识热键
            RegisterHotKey(Handle, 100, ctrlHotKey, Keys.D1);

            this.listView1.ListViewItemSorter = new ListViewColumnSorter();
            this.listView1.ColumnClick += new ColumnClickEventHandler(ListViewHelper.ListView_ColumnClick);

            this.Text += "/" + System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1] + " - ";
            this.timer1.Start();
            this.timer1.Interval = 9000000;
            this.Text = this.Text.Split('-')[0];
            this.Text += " -  " + getWeather(provinceAndcityName);
        }

        #region 获取天气预报
        /// <summary>
        /// 获取天气预报
        /// </summary>
        /// <param name="theCityName">所在城市</param>
        /// <returns></returns>
        private string getWeather(string theCityName)
        {
            try
            {
                string[] arr = wws.getWeatherbyCityName(theCityName);
                //for(int i = 0; i < arr.Length; i++)
                //{
                //    weather += arr[i] + "\r\n";
                //}
                string weather = arr[0] + arr[1] + " " + arr[6] + " " + arr[5];
                return weather;
            }
            catch(Exception ex)
            {
                //MessageBox.Show("网络未连接..." + ex.Message);
                return "网络未连接..." + ex.Message;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = this.Text.Split('-')[0];
            this.Text += " -  " + getWeather(provinceAndcityName);
        }
        #endregion

        // 热键按下执行的方法
        private void GlobalKeyProcess()
        {
            //this.WindowState = FormWindowState.Minimized;
            // 窗口最小化也需要一定时间
            Thread.Sleep(200);
            开始截图ToolStripMenuItem.PerformClick();
        }

        #region 收藏功能

        //参数：
        //  string dir 指定的文件夹
        //  string ext 文件类型的扩展名，如".txt" , “.exe"
        static int GetFileCount(string dir, string ext)
        {
            int count = 0;
            DirectoryInfo d = new DirectoryInfo(dir);
            foreach (FileInfo fi in d.GetFiles())
            {
                if (fi.Extension.ToUpper() == ext.ToUpper())
                {
                    count++;
                }
            }
            return count;
        }

        private void 加入收藏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.SelectedItems)
            {
                if (item.Selected)
                {
                    string value = item.SubItems[1].Text;
                    //MessageBox.Show(value);
                    FileStream fs = new FileStream(txtFavoriteDirectory, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("--------------------------------------------------------------------------------------");
                    sw.WriteLine(value);
                    this.label1.Text = "您已将第 " + (item.Index + 1).ToString() + " 条记录加入了收藏！";
                    sw.Close();
                    fs.Close();
                }
            }
        }

        private void 加入收藏ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView2.SelectedItems)
            {
                if (item.Selected)
                {                    
                    string imageindex = (item.ImageIndex + 1).ToString();
                    File.Copy(imageDirectory + "\\image" + imageindex + ".png", imageFavoriteDirectory + "\\favoriteimage" + (GetFileCount(imageFavoriteDirectory, ".png") + 1) + ".png");
                    this.label1.Text = "您已将 favoriteimage" + GetFileCount(imageFavoriteDirectory, ".png") + ".png 加入了收藏！";
                }
            }
        }
        

        private void 打开文本收藏夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad", txtFavoriteDirectory);
            this.label1.Text = "您打开了文本收藏夹！";
        }

        private void 打开图片收藏夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("ExpLorer", imageFavoriteDirectory);
            this.label1.Text = "您打开了图片收藏夹！";
        }
        #endregion

        #region 设置城市窗体功能
        public class GenericSingleton<T> where T : Form, new()
        {
            private static T t = null;
            public static T CreateInstrance()
            {
                if (null == t || t.IsDisposed)
                {
                    t = new T();
                }
                return t;
            }
        }

        private void 设置城市ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //第四步：将要实现的方法绑定到委托事件
            SetCityName setcityname = GenericSingleton<SetCityName>.CreateInstrance();
            setcityname.MyEvent += new SetCityName.MyDelegate(b_MyEvent);//监听b窗体事件
            setcityname.ShowDialog();
        }

        //第三步：实现要做的事情
        void b_MyEvent(string SetCityName_provinceAndcityName)
        {
            provinceAndcityName = SetCityName_provinceAndcityName;
            //MessageBox.Show(message);
            //这里是刷新窗体的方法
            this.Text = this.Text.Split('-')[0];
            this.Text += " -  " + getWeather(provinceAndcityName.Split(',')[1].Split(' ')[0]);
            AppConfigHelper.SetConfigValue(provinceAndcityName.Split(',')[0], provinceAndcityName.Split(',')[1]);
        }
        #endregion

        private void 开启剪贴板功能ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (开启剪贴板功能ToolStripMenuItem.Checked == false)
            {
                isPause = false;
                this.label1.Text = "您关闭了剪贴板功能！";
            }
            else
            {
                isPause = true;
                this.label1.Text = "您开启了剪贴板功能！";
            }
        }

    }
}
