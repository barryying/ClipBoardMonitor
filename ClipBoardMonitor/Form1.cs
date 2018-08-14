using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        public Form1()
        {
            InitializeComponent();

            // 添加ListView表头标题
            this.listView1.Columns.Add("序号", 40, HorizontalAlignment.Left); //添加标题
            this.listView1.Columns.Add("复制的文本", 600, HorizontalAlignment.Left);
            this.listView1.View = System.Windows.Forms.View.Details;
            
            mNextClipBoardViewerHWnd = SetClipboardViewer(this.Handle);
        }
        //重写wndproc方法  定义热键使用
        #region Message Process
        //Override WndProc to get messages...
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
                            AddListView(Clipboard.GetText());
                            richTextBox1.Text = Clipboard.GetText();
                        }
                        //显示剪贴板中的图片信息
                        if (Clipboard.ContainsImage())
                        {
                            pictureBox1.Image = Clipboard.GetImage();
                            pictureBox1.Update();
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

        private void AddListView(string value)
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

            if (!value.Equals(""))
            {
                this.listView1.BeginUpdate(); //数据更新，UI暂时挂起

                // 添加序号
                ListViewItem lvi = new ListViewItem();
                lvi.Text = (this.listView1.Items.Count + 1).ToString();

                // 添加子项
                lvi.SubItems.Add(value.TrimStart().TrimEnd());
                this.listView1.Items.Add(lvi);

                this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
                this.label1.Text = "添加了一项记录！";

                // ListView获取焦点
                listView1.Focus();
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

        private void 清空历史ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
            this.label1.Text = "已清空了所有记录！";
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
    }
}
