using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ClipBoardMonitor
{
    public partial class Cutter : Form
    {
        public Cutter()
        {
            InitializeComponent();
        }

        private const int WS_EX_TOPMOST = 0x00000008;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// 判断窗口是否为TopMost
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool CheckIsTopMost(IntPtr hWnd)
        {
            int style = GetWindowLong(hWnd, WS_EX_TOPMOST);
            style = style & WS_EX_TOPMOST;
            if (style == WS_EX_TOPMOST)
            {
                return true;
            }
            return false;
        }

        #region 定义程序变量
        // 定义变量

        // 用来记录鼠标按下的坐标，用来确定绘图起点
        private Point DownPoint;

        // 用来表示是否截图完成
        private bool CatchFinished = false;

        // 用来表示截图开始
        private bool CatchStart = false;

        // 用来保存原始图像
        private Bitmap originBmp;

        // 用来保存截图的矩形
        private Rectangle CatchRectangle;
        #endregion


        /// <summary>
        /// 窗体初始化操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cutter_Load(object sender, EventArgs e)
        {
            // 设置控件样式为双缓冲，这样可以有效减少图片闪烁的问题
            // 第二个参数为true表示把第一个参数指定的样式应用于控件；false 表示不应用。
            // '|'表示位逻辑或运算
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            // 改变鼠标样式
            this.Cursor = Cursors.Cross;
            // 保存全屏图片
            originBmp = new Bitmap(this.BackgroundImage);
            if (!this.TopMost)
                this.TopMost = true;
                //this.BringToFront();
        }
        /// <summary>
        /// 右键点击退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cutter_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        /// <summary>
        /// 左键按下开始截屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cutter_MouseDown(object sender, MouseEventArgs e)
        {
            // 鼠标左键按下是开始画图，也就是截图
            if (e.Button == MouseButtons.Left)
            {
                // 如果捕捉没有开始
                if (!CatchStart)
                {
                    CatchStart = true;
                    // 保存此时鼠标按下坐标
                    DownPoint = new Point(e.X, e.Y);
                }
            }
        }
        /// <summary>
        /// 左键双击保存到剪贴板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cutter_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CatchFinished)
            {
                // 新建一个与矩形一样大小的空白图片
                Bitmap CatchedBmp = new Bitmap(CatchRectangle.Width, CatchRectangle.Height);

                Graphics g = Graphics.FromImage(CatchedBmp);

                // 把originBmp中指定部分按照指定大小画到空白图片上
                // CatchRectangle指定originBmp中指定部分
                // 第二个参数指定绘制到空白图片的位置和大小
                // 画完后CatchedBmp不再是空白图片了，而是具有与截取的图片一样的内容
                g.DrawImage(originBmp, new Rectangle(0, 0, CatchRectangle.Width, CatchRectangle.Height), CatchRectangle, GraphicsUnit.Pixel);

                // 将图片保存到剪切板中
                Clipboard.SetImage(CatchedBmp);
                g.Dispose();
                CatchFinished = false;
                this.BackgroundImage = originBmp;
                CatchedBmp.Dispose();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        /// <summary>
        /// 鼠标移动事件处理程序，即用户改变截图大小的处理
        ///  这个方法是截图功能的核心方法，也就是绘制截图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cutter_MouseMove(object sender, MouseEventArgs e)
        {
            // 确保截图开始
            if (CatchStart)
            {
                // 新建一个图片对象，让它与屏幕图片相同
                Bitmap copyBmp = (Bitmap)originBmp.Clone();

                // 获取鼠标按下的坐标
                Point newPoint = new Point(DownPoint.X, DownPoint.Y);

                // 新建画板和画笔
                Graphics g = Graphics.FromImage(copyBmp);
                Pen p = new Pen(Color.OrangeRed, 2);

                // 获取矩形的长宽
                int width = Math.Abs(Control.MousePosition.X - DownPoint.X);
                int height = Math.Abs(Control.MousePosition.Y - DownPoint.Y);
                if (Control.MousePosition.X < DownPoint.X)
                {
                    newPoint.X = Control.MousePosition.X;
                }
                if (Control.MousePosition.Y < DownPoint.Y)
                {
                    newPoint.Y = Control.MousePosition.Y;
                }

                CatchRectangle = new Rectangle(newPoint, new Size(width, height));

                // 将矩形画在画板上
                g.DrawRectangle(p, CatchRectangle);

                // 释放目前的画板
                g.Dispose();
                p.Dispose();
                // 从当前窗体创建新的画板
                Graphics g1 = this.CreateGraphics();

                // 将刚才所画的图片画到截图窗体上
                // 为什么不直接在当前窗体画图呢？
                // 如果自己解决将矩形画在窗体上，会造成图片抖动并且有无数个矩形
                // 这样实现也属于二次缓冲技术
                g1.DrawImage(copyBmp, new Point(0, 0));
                g1.Dispose();
                // 释放拷贝图片，防止内存被大量消耗
                copyBmp.Dispose();
            }
        }
        /// <summary>
        /// 左键弹起结束截屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cutter_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 如果截图已经开始，鼠标左键弹起设置截图完成
                if (CatchStart)
                {
                    CatchStart = false;
                    CatchFinished = true;
                }
            }
        }
    }
}
