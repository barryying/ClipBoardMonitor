using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Resources;
using System.IO;

namespace ClipBoardMonitor
{
    public class AppConfigHelper
    {
        private static string FILE_PATH = System.Windows.Forms.Application.StartupPath;
        private const string FILE_NAME = "settings.txt";
        private static string FULL_PATH = FILE_PATH.Replace(@"\", "\\") + "\\" + FILE_NAME;

        // System.Reflection.Assembly为资源的主程序集，这里为Demo
        //static ResourceManager resManagerA = new ResourceManager("ClipBoardMonitor.strings", typeof(AppConfigHelper).Assembly);

        /// <summary>
        /// 获取AppSettings中某一节点值
        /// </summary>
        /// <param name="key">关键字</param>
        public static string GetConfigValue(string key)
        {
            string value = "";
            if (!File.Exists(FILE_NAME))
            {
                Console.WriteLine("{0} does not exist.", FILE_NAME);
                return value;
            }
            using (StreamReader sr = File.OpenText(FULL_PATH))
            {
                string[] lines = System.IO.File.ReadAllLines(FULL_PATH, Encoding.Default);
                //"c:\\users\\administrator\\desktop\\webapplication1\\webapplication1\\testtxt.txt"
                string[] str;//定义一个数组
                for (int i = 0; i < lines.Length; i++)
                {
                    str = lines[i].Split(',');//将单行数据以“,” 为界做截取并保存进str中
                    switch (key)
                    {
                        case "provincename":
                            value = str[0];
                            break;
                        case "cityname":
                            value = str[1];
                            break;
                        default:
                            break;
                    }                    
                }
                return value;
            }

            //string value = resManagerA.GetString(key);
            //return value;
        }
        /// <summary>
        /// 设置AppSettings中某一节点值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        public static bool SetConfigValue(string value1, string value2)
        {
            try
            {
                if (!File.Exists(FILE_NAME))
                {
                    Console.WriteLine("{0} does not exist.", FILE_NAME);
                    return false;
                }
                FileStream fs = new FileStream(FULL_PATH, FileMode.Open, FileAccess.Write);
                File.SetAttributes(FULL_PATH, FileAttributes.Hidden);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(value1 + "," + value2, true);//开始写入值             
                sw.Flush();  //刷新缓存
                sw.Close();
                fs.Close();   
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
