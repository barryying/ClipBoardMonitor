﻿namespace ClipBoardMonitor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.清空历史ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开图片存放位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开收藏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开文本收藏夹ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开图片收藏夹ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.是否开机启动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.开机启动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.开启剪贴板功能ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.开始截图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置城市ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listView2 = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除此条记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加入收藏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除此条记录ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.加入收藏ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.复制此条记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制此条记录ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(1214, 369);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(227, 112);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1274, 166);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(7, 24);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(735, 535);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
            this.listView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseMove);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.清空历史ToolStripMenuItem,
            this.打开图片存放位置ToolStripMenuItem,
            this.打开收藏ToolStripMenuItem,
            this.是否开机启动ToolStripMenuItem,
            this.开始截图ToolStripMenuItem,
            this.设置城市ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1197, 28);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 清空历史ToolStripMenuItem
            // 
            this.清空历史ToolStripMenuItem.Name = "清空历史ToolStripMenuItem";
            this.清空历史ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.清空历史ToolStripMenuItem.Text = "清空历史";
            this.清空历史ToolStripMenuItem.Click += new System.EventHandler(this.清空历史ToolStripMenuItem_Click);
            // 
            // 打开图片存放位置ToolStripMenuItem
            // 
            this.打开图片存放位置ToolStripMenuItem.Name = "打开图片存放位置ToolStripMenuItem";
            this.打开图片存放位置ToolStripMenuItem.Size = new System.Drawing.Size(141, 24);
            this.打开图片存放位置ToolStripMenuItem.Text = "打开图片存放位置";
            this.打开图片存放位置ToolStripMenuItem.Click += new System.EventHandler(this.打开图片存放位置ToolStripMenuItem_Click);
            // 
            // 打开收藏ToolStripMenuItem
            // 
            this.打开收藏ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开文本收藏夹ToolStripMenuItem,
            this.打开图片收藏夹ToolStripMenuItem});
            this.打开收藏ToolStripMenuItem.Name = "打开收藏ToolStripMenuItem";
            this.打开收藏ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.打开收藏ToolStripMenuItem.Text = "打开收藏";
            // 
            // 打开文本收藏夹ToolStripMenuItem
            // 
            this.打开文本收藏夹ToolStripMenuItem.Name = "打开文本收藏夹ToolStripMenuItem";
            this.打开文本收藏夹ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.打开文本收藏夹ToolStripMenuItem.Text = "打开文本收藏夹";
            this.打开文本收藏夹ToolStripMenuItem.Click += new System.EventHandler(this.打开文本收藏夹ToolStripMenuItem_Click);
            // 
            // 打开图片收藏夹ToolStripMenuItem
            // 
            this.打开图片收藏夹ToolStripMenuItem.Name = "打开图片收藏夹ToolStripMenuItem";
            this.打开图片收藏夹ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.打开图片收藏夹ToolStripMenuItem.Text = "打开图片收藏夹";
            this.打开图片收藏夹ToolStripMenuItem.Click += new System.EventHandler(this.打开图片收藏夹ToolStripMenuItem_Click);
            // 
            // 是否开机启动ToolStripMenuItem
            // 
            this.是否开机启动ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.开机启动ToolStripMenuItem,
            this.开启剪贴板功能ToolStripMenuItem});
            this.是否开机启动ToolStripMenuItem.Name = "是否开机启动ToolStripMenuItem";
            this.是否开机启动ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.是否开机启动ToolStripMenuItem.Text = "开关设置";
            // 
            // 开机启动ToolStripMenuItem
            // 
            this.开机启动ToolStripMenuItem.Checked = true;
            this.开机启动ToolStripMenuItem.CheckOnClick = true;
            this.开机启动ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.开机启动ToolStripMenuItem.Name = "开机启动ToolStripMenuItem";
            this.开机启动ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.开机启动ToolStripMenuItem.Text = "是否开机启动";
            this.开机启动ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.开机启动ToolStripMenuItem_CheckedChanged);
            // 
            // 开启剪贴板功能ToolStripMenuItem
            // 
            this.开启剪贴板功能ToolStripMenuItem.Checked = true;
            this.开启剪贴板功能ToolStripMenuItem.CheckOnClick = true;
            this.开启剪贴板功能ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.开启剪贴板功能ToolStripMenuItem.Name = "开启剪贴板功能ToolStripMenuItem";
            this.开启剪贴板功能ToolStripMenuItem.Size = new System.Drawing.Size(189, 26);
            this.开启剪贴板功能ToolStripMenuItem.Text = "开启剪贴板功能";
            this.开启剪贴板功能ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.开启剪贴板功能ToolStripMenuItem_CheckedChanged);
            // 
            // 开始截图ToolStripMenuItem
            // 
            this.开始截图ToolStripMenuItem.Name = "开始截图ToolStripMenuItem";
            this.开始截图ToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.开始截图ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.开始截图ToolStripMenuItem.Size = new System.Drawing.Size(137, 24);
            this.开始截图ToolStripMenuItem.Text = "开始截图(&Ctrl+1)";
            this.开始截图ToolStripMenuItem.ToolTipText = "按Ctrl+1进行截图操作";
            this.开始截图ToolStripMenuItem.Click += new System.EventHandler(this.开始截图ToolStripMenuItem_Click);
            this.开始截图ToolStripMenuItem.MouseLeave += new System.EventHandler(this.开始截图ToolStripMenuItem_MouseLeave);
            this.开始截图ToolStripMenuItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.开始截图ToolStripMenuItem_MouseMove);
            // 
            // 设置城市ToolStripMenuItem
            // 
            this.设置城市ToolStripMenuItem.Name = "设置城市ToolStripMenuItem";
            this.设置城市ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.设置城市ToolStripMenuItem.Text = "设置城市";
            this.设置城市ToolStripMenuItem.Click += new System.EventHandler(this.设置城市ToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Fuchsia;
            this.label1.Location = new System.Drawing.Point(695, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 15);
            this.label1.TabIndex = 6;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listView2
            // 
            this.listView2.Location = new System.Drawing.Point(6, 24);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(409, 535);
            this.listView2.TabIndex = 7;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.DoubleClick += new System.EventHandler(this.listView2_DoubleClick);
            this.listView2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView2_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(26, 615);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(278, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "* 双击上面的行记录可复制文本至剪贴板";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(770, 616);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(293, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "* 双击图片可打开查看大图并复制至剪贴板";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.复制此条记录ToolStripMenuItem,
            this.删除此条记录ToolStripMenuItem,
            this.加入收藏ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(169, 76);
            // 
            // 删除此条记录ToolStripMenuItem
            // 
            this.删除此条记录ToolStripMenuItem.Name = "删除此条记录ToolStripMenuItem";
            this.删除此条记录ToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.删除此条记录ToolStripMenuItem.Text = "删除此条记录";
            this.删除此条记录ToolStripMenuItem.Click += new System.EventHandler(this.删除此条记录ToolStripMenuItem_Click);
            // 
            // 加入收藏ToolStripMenuItem
            // 
            this.加入收藏ToolStripMenuItem.Name = "加入收藏ToolStripMenuItem";
            this.加入收藏ToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.加入收藏ToolStripMenuItem.Text = "加入收藏";
            this.加入收藏ToolStripMenuItem.Click += new System.EventHandler(this.加入收藏ToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.复制此条记录ToolStripMenuItem1,
            this.删除此条记录ToolStripMenuItem1,
            this.加入收藏ToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(176, 104);
            // 
            // 删除此条记录ToolStripMenuItem1
            // 
            this.删除此条记录ToolStripMenuItem1.Name = "删除此条记录ToolStripMenuItem1";
            this.删除此条记录ToolStripMenuItem1.Size = new System.Drawing.Size(168, 24);
            this.删除此条记录ToolStripMenuItem1.Text = "删除此条记录";
            this.删除此条记录ToolStripMenuItem1.Click += new System.EventHandler(this.删除此条记录ToolStripMenuItem1_Click);
            // 
            // 加入收藏ToolStripMenuItem1
            // 
            this.加入收藏ToolStripMenuItem1.Name = "加入收藏ToolStripMenuItem1";
            this.加入收藏ToolStripMenuItem1.Size = new System.Drawing.Size(168, 24);
            this.加入收藏ToolStripMenuItem1.Text = "加入收藏";
            this.加入收藏ToolStripMenuItem1.Click += new System.EventHandler(this.加入收藏ToolStripMenuItem1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Location = new System.Drawing.Point(12, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(748, 565);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文本区域";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listView2);
            this.groupBox2.Location = new System.Drawing.Point(766, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(421, 565);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "图片区域";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // 复制此条记录ToolStripMenuItem
            // 
            this.复制此条记录ToolStripMenuItem.Name = "复制此条记录ToolStripMenuItem";
            this.复制此条记录ToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.复制此条记录ToolStripMenuItem.Text = "复制此条记录";
            this.复制此条记录ToolStripMenuItem.Click += new System.EventHandler(this.复制此条记录ToolStripMenuItem_Click);
            // 
            // 复制此条记录ToolStripMenuItem1
            // 
            this.复制此条记录ToolStripMenuItem1.Name = "复制此条记录ToolStripMenuItem1";
            this.复制此条记录ToolStripMenuItem1.Size = new System.Drawing.Size(175, 24);
            this.复制此条记录ToolStripMenuItem1.Text = "复制此条记录";
            this.复制此条记录ToolStripMenuItem1.Click += new System.EventHandler(this.复制此条记录ToolStripMenuItem1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1197, 640);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "剪贴板监视器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 清空历史ToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem 打开图片存放位置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 是否开机启动ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 开机启动ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除此条记录ToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 删除此条记录ToolStripMenuItem1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripMenuItem 开始截图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加入收藏ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加入收藏ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 打开收藏ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开文本收藏夹ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开图片收藏夹ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置城市ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem 开启剪贴板功能ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 复制此条记录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 复制此条记录ToolStripMenuItem1;
    }
}

