using Handlerwikifx.Common;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Handlerwikifx.Job
{
   public class WebHandlerJob: IJob
    {
        private static readonly object _locker = new object();
        public async Task Execute(IJobExecutionContext context)
        {
            // 启用一个线程处理
            Task task = new Task(() =>
            {

                try
                {
                    GetHtml();
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"{nameof(WebHandlerJob)} 线程内部报错,报错信息:{ex.Message}");
                }

            });
            task.Start();
            await task;
        }

        public void GetHtml()
        {
            
            string str = AppDomain.CurrentDomain.BaseDirectory + @"WIKIFX.csv";
            string settingPath = AppDomain.CurrentDomain.BaseDirectory + @"PathSetting.txt";
            StreamReader fileStream = new StreamReader(settingPath, Encoding.Default);
            string htmlPath = fileStream.ReadToEnd();
            var data = readCsvTxt(str);
            string FilePath="";
            string jsFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Common.js";
            foreach (DataRow dr in data.Rows)
            {
                string url = dr[1].ToString();
                string[] arry = url.Split('/');
                string path = arry[1];
                if (path == "fr_fr")
                {
                    path = "x_fr";
                }
                else if (path == "in_hi")
                {
                    path = "x_hi";
                }
                else if (path == "ph_zh-tw")
                {
                    path = "x_zh-tw";
                }
                else if (path == "sg_en")
                {
                    path = "x_en";
                }
                string htmlSstring =GetFilterHtml("https://" + url, path);
                StreamWriter sw = null;//写入流对象     
                try
                {
                    
                    fileStream = new StreamReader(jsFilePath, Encoding.Default);
                    string js = fileStream.ReadToEnd();
                    js = js.Replace("au_en", arry[1]);
                    Encoding code = Encoding.GetEncoding("UTF-8"); //声明文件编码
                    string htmlfilename = "index.html";
                    string jsName = "Common.js";

                    lock (_locker)
                    {
                        WriteFile(htmlPath, arry, htmlSstring, code, htmlfilename);
                        WriteFile(htmlPath, arry, js, code, jsName);
                    }

                }
                catch (Exception ex)
                {
                    var log = $" 错误信息:{ex.Message} AttachmentType:{path} data:{JsonConvert.SerializeObject(data)} htmlPath:{FilePath}";
                    LogHelper.Error(log);
                }
                finally
                {
                    if (sw!=null)
                    {
                     
                    }

                }
            }

        }

        private static void WriteFile(string htmlPath, string[] arry, string htmlSstring, Encoding code, string htmlfilename)
        {
            //创建文件夹               
            string FilePath = htmlPath + (arry[1] + @"\");
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            // 写入文件
            using (var fileStream1 = new FileStream(FilePath + @"\" + htmlfilename, FileMode.Create))
            {
                var sw1 = new StreamWriter(fileStream1, code);
                sw1.WriteLine(htmlSstring);
                sw1.Close();
            }
            //sw = new StreamWriter(FilePath + @"\" + htmlfilename, false, code);
        }



        /// <summary>  
        /// 获取网页的HTML码  
        /// </summary>  
        /// <param name="url">链接地址</param>  
        /// <param name="encoding">编码类型</param>  
        /// <returns></returns>  
        public static string GetHtmlStr(string url, string encoding)
        {
            string htmlStr = "";
            try
            {
                if (!String.IsNullOrEmpty(url))
                {
                    WebRequest request = WebRequest.Create(url);            //实例化WebRequest对象  
                    WebResponse response = request.GetResponse();           //创建WebResponse对象  
                    Stream datastream = response.GetResponseStream();       //创建流对象  
                    Encoding ec = Encoding.Default;
                    if (encoding == "UTF8")
                    {
                        ec = Encoding.UTF8;
                    }
                    else if (encoding == "Default")
                    {
                        ec = Encoding.Default;
                    }
                    StreamReader reader = new StreamReader(datastream, ec);
                    htmlStr = reader.ReadToEnd();                  //读取网页内容  
                    reader.Close();
                    datastream.Close();
                    response.Close();
                }
            }
            catch { }
            return htmlStr;
        }

        //根据Url地址得到网页的html源码 
        public static string GetWebContent(string Url)
        {
            string strResult = "";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                //声明一个HttpWebRequest请求 
                request.Timeout = 30000;
                //设置连接超时时间 
                request.Headers.Set("Pragma", "no-cache");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("utf-8");
                StreamReader streamReader = new StreamReader(streamReceive, encoding);
                strResult = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return strResult = ex.Message;
            }
            return strResult;
        }

        /// <summary>
        /// 获取Html字符串中指定标签的指定属性的值 
        /// </summary>
        /// <param name="html">Html字符</param>
        /// <param name="tag">指定标签名</param>
        /// <param name="attr">指定属性名</param>
        /// <returns></returns>
        public static List<string> GetHtmlAttr(string html, string tag, string attr)
        {

            Regex re = new Regex(@"(<" + tag + @"[\w\W].+?>)");
            MatchCollection imgreg = re.Matches(html);
            List<string> m_Attributes = new List<string>();
            Regex attrReg = new Regex(@"([a-zA-Z1-9_-]+)\s*=\s*(\x27|\x22)([^\x27\x22]*)(\x27|\x22)", RegexOptions.IgnoreCase);

            for (int i = 0; i < imgreg.Count; i++)
            {
                MatchCollection matchs = attrReg.Matches(imgreg[i].ToString());

                for (int j = 0; j < matchs.Count; j++)
                {
                    GroupCollection groups = matchs[j].Groups;

                    if (attr.ToUpper() == groups[1].Value.ToUpper())
                    {
                        m_Attributes.Add(groups[3].Value);
                        break;
                    }
                }

            }
            return m_Attributes;

        }
        /// <summary>
        /// 获取网页转换后的字符串
        /// </summary>
        /// <returns></returns>
        public static string GetFilterHtml(string url, string path)
        {
            try
            {
                string htmlSstring = GetWebContent(url);
                List<string> listJs = GetHtmlAttr(htmlSstring, "script", "src");
                List<string> listCs = GetHtmlAttr(htmlSstring, "link", "href");
                List<string> listImg = GetHtmlAttr(htmlSstring, "img", "src");
                List<string> listA = GetHtmlAttr(htmlSstring, "a", "href");
                List<string> Img = new List<string>();
                List<string> a = new List<string>();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (string item in listImg)
                {
                    if (!item.Contains("https") && item != "/Contentgj/images/default.png")
                    {
                        Img.Add(item);
                    }
                }
                foreach (string item in listA)
                {
                    if (!item.Contains("https") && !item.Contains("javascript"))
                    {
                        a.Add(item);
                    }
                }
                int i = 0;

                foreach (string item in listJs)
                {
                    i++;
                    dic.Add(i.ToString(), item + ")" + "https://www.wikifx.com" + item);
                }
                foreach (string item in listCs)
                {
                    i++;
                    dic.Add(i.ToString(), item + ")" + "https://www.wikifx.com" + item);
                }
                foreach (string item in Img)
                {
                    if (!item.Contains("https"))
                    {
                        i++;
                        if (!item.Contains("eimgjys.wikifx.com") && !item.Contains("//img.wikifx.com"))
                        {
                            dic.Add(i.ToString(), item + ")" + "https://www.wikifx.com" + item);
                        }
                        else
                        {
                            dic.Add(i.ToString(), item + ")" + "https:" + item);
                        }
                    }
                }
                foreach (string item in a)
                {
                    i++;
                    if (!item.Contains("//www.wikifx.com") && !item.Contains("//live.wikifx.com") && !item.Contains("//tools.wikifx.com") && !item.Contains("//survey.wikifx.com"))
                    {
                        dic.Add(i.ToString(), item + ")" + "https://www.wikifx.com" + item);
                    }
                    else
                    {
                        dic.Add(i.ToString(), item + ")" + "https:" + item);
                    }

                }
                foreach (var item in dic)
                {
                    string[] arry = item.Value.Split(')');
                    htmlSstring = htmlSstring.Replace(arry[0], arry[1]);
                }
                string settingPath = AppDomain.CurrentDomain.BaseDirectory + @"ReplaceHtml.csv";
                var data = readCsvTxt(settingPath);
                foreach (DataRow dr in data.Rows)
                {
                    htmlSstring = htmlSstring.Replace(dr[0].ToString().Replace("path", path), dr[1].ToString().Replace("path", path));
                }
                //判断是否是法国
                if (path == "x_fr")
                {
                    htmlSstring = htmlSstring.Replace("=/fr_fr/", "=https://www.wikifx.com/fr_fr/");
                }
                return htmlSstring;
            }
            catch (Exception e)
            {

                return e.Message;
            }

        }
        /// <summary>
        /// 从csv读取数据返回table
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static DataTable OpenCSV(string filePath)//从csv读取数据返回table
        {
            System.Text.Encoding encoding = GetType(filePath); //Encoding.ASCII;//
            DataTable dt = new DataTable();
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }

        /// <summary>
                /// 读取CSV文件通过文本格式
                /// </summary>
                /// <param name="strpath"></param>
                /// <returns></returns>
        public static DataTable readCsvTxt(string strpath)
        {
            int intColCount = 0;
            bool blnFlag = true;
            DataTable mydt = new DataTable("myTableName");

            DataColumn mydc;
            DataRow mydr;

            string strline;
            string[] aryline;

            System.IO.StreamReader mysr = new System.IO.StreamReader(strpath);

            while ((strline = mysr.ReadLine()) != null)
            {
                aryline = strline.Split(',');

                if (blnFlag)
                {
                    blnFlag = false;
                    intColCount = aryline.Length;
                    for (int i = 0; i < aryline.Length; i++)
                    {
                        mydc = new DataColumn(aryline[i]);
                        mydt.Columns.Add(mydc);
                    }
                }

                mydr = mydt.NewRow();
                for (int i = 0; i < intColCount; i++)
                {
                    mydr[i] = aryline[i];
                }
                mydt.Rows.Add(mydr);
            }

            return mydt;
        }


        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(System.IO.FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
    }
}
