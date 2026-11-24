using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace jcode.util
{

    /// <summary>
    /// 获取ip位置信息
    /// </summary>
    public static class IPLocationHelper
    {
        private static readonly ConcurrentDictionary<string, string> ipList = new ConcurrentDictionary<string, string>();
        
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
        
        private static string HttpGet(string url)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            WebResponse response = null;
            string responseStr = null;
            

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }

        
        /// <summary>
        /// 获取IP地址信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetLocation(string ip)
        {
            if (ConfigHelper.GetConfig().GetIpNameClosed)
            {
                return "";
            }

            string res = "";
            ipList.TryGetValue(ip, out res);
            if (!string.IsNullOrEmpty(res))
            {
                return res;
            }

          

            try
            {
                string url = "https://sp0.baidu.com/8aQDcjqpAAV3otqbppnN2DJv/api.php?query=" + ip + "&resource_id=6006&ie=utf8&oe=utf8&format=json";
                res = HttpGet(url);
                var resjson = res.ToObject<obj>();
                if (resjson.data.Count == 0)
                {
                    res = "内网地址";
                }
                else
                {
                    res = resjson.data[0].location;
                }
            }
            catch
            {
                res = "";
            }

            ipList.TryAdd(ip, res);
            
            return res;
        }
        /// <summary>
        /// 接口数据
        /// </summary>
        private class obj
        {
            public List<dataone> data { get; set; }
        }
        private class dataone
        {
            public string location { get; set; }
        }
        
    }
}