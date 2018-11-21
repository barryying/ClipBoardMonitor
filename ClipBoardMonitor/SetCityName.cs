using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClipBoardMonitor
{
    public partial class SetCityName : Form
    {
        private static SetCityName frm = null;
        public SetCityName()
        {
            InitializeComponent();
        }

        public static SetCityName CreateInstrance()
        {
            if (frm == null || frm.IsDisposed)
            {
                frm = new SetCityName();   //单例
            }
            return frm;
        }
        //第一步：声明一个委托
        public delegate void MyDelegate(string cityname);
        //第二步：声明一个事件
        public event MyDelegate MyEvent;

        private string cityName = "";
        cn.com.webxml.www.WeatherWebService wws = new cn.com.webxml.www.WeatherWebService();
        
        private void SetCityName_Load(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem == null)
            {
                string[] arr = wws.getSupportProvince();
                comboBox1.DataSource = arr;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] arr = wws.getSupportCity(comboBox1.SelectedItem.ToString());
            comboBox2.DataSource = arr;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cityName = comboBox2.SelectedItem.ToString().Split(' ')[0];
            label3.Text = "当前城市为：" + cityName;
        }
        
        private void SetCityName_FormClosing(object sender, FormClosingEventArgs e)
        {
            //第五步：触发事件
            if (MyEvent != null)      //确保事件在Form1中已被绑定
                MyEvent(cityName);   //触发事件
        }
    }
}
