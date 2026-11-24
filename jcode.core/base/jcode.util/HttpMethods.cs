using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.11
    /// 描 述：http方法
    /// </summary>
    public class HttpMethods
    {
        /// <summary>
        /// 创建HttpClient
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateHttpClient(string url,TimeSpan timeout, IDictionary<string, string> cookies = null)
        {
            HttpClient httpclient;
            HttpClientHandler handler = new HttpClientHandler();
            var uri = new Uri(url);
            if (cookies != null)
            {
                foreach (var key in cookies.Keys)
                {
                    string one = key + "=" + cookies[key];
                    handler.CookieContainer.SetCookies(uri, one);
                }
            }
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                httpclient = new HttpClient(handler);
            }
            else
            {
                httpclient = new HttpClient(handler);
            }

            httpclient.Timeout = timeout;
            return httpclient;
        }
        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">请求参数</param>
        /// <param name="timeout">请求超时</param>
        /// <returns></returns>
        public static Task<string> Post(string url, string jsonData,TimeSpan timeout)
        {
            HttpClient httpClient = CreateHttpClient(url,timeout);
            var postData = new StringContent(jsonData);
            postData.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            Task<string> result = httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }
        
        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">请求参数</param>
        /// <returns></returns>
        public static Task<string> Post(string url, string jsonData)
        {
            return Post(url, jsonData,TimeSpan.FromSeconds(100));
        }

        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeout">请求超时</param>
        /// <returns></returns>
        public static Task<string> Post(string url,TimeSpan timeout)
        {
            HttpClient httpClient = CreateHttpClient(url,timeout);
            var postData = new StringContent("");
            postData.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            Task<string> result = httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }
        
        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public static Task<string> Post(string url)
        {
            HttpClient httpClient = CreateHttpClient(url,TimeSpan.FromSeconds(100));
            var postData = new StringContent("");
            postData.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            Task<string> result = httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }

        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="req">请求参数</param>
        /// <param name="timeout">请求超时</param>
        /// <returns></returns>
        public static Task<string> Post(string url, byte[] req,TimeSpan timeout)
        {
            HttpClient httpClient = CreateHttpClient(url,timeout);
            var postData = new ByteArrayContent(req);
            Task<string> result = httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }
        
        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public static Task<string> Post(string url, byte[] req)
        {
            HttpClient httpClient = CreateHttpClient(url,TimeSpan.FromSeconds(100));
            var postData = new ByteArrayContent(req);
            Task<string> result = httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }

        /// <summary>
        /// get 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeout">请求超时</param>
        /// <returns></returns>
        public static Task<string> Get(string url,TimeSpan timeout)
        {
            HttpClient httpClient = CreateHttpClient(url,timeout);
            Task<string> result = httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
            return result;
        }
        
        /// <summary>
        /// get 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public static Task<string> Get(string url)
        {
            HttpClient httpClient = CreateHttpClient(url,TimeSpan.FromSeconds(100));
            Task<string> result = httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
            return result;
        }

        #region 扩展方法----ADD BY SSY 20221122

        #region get 请求
        /// <summary>
        /// get 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">请求头参数</param>
        /// <param name="timeout">请求超时</param>
        /// <returns></returns>
        public static Task<string> Get(string url, Dictionary<string, string> headers,TimeSpan timeout)
        {
            HttpClient httpClient = CreateHttpClient(url,timeout);
            //序列化请求头参数
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            Task<string> result = httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
            return result;
        }
        
        /// <summary>
        /// get 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">请求头参数</param>
        /// <returns></returns>
        public static Task<string> Get(string url, Dictionary<string, string> headers)
        {
            HttpClient httpClient = CreateHttpClient(url,TimeSpan.FromSeconds(100));
            //序列化请求头参数
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            Task<string> result = httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
            return result;
        }
        #endregion

        #region post 请求
        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">请求参数</param>
        /// <param name="headers">请求头参数</param>
        /// <param name="ContentType">Body类型【(application/json),(multipart/form-data)】</param>
        /// <param name="timeout">请求超时</param>
        /// <returns></returns>
        public static Task<string> Post(string url, Dictionary<string, string> headers, string ContentType,string jsonData,TimeSpan timeout)
        {
            HttpClient httpClient = CreateHttpClient(url,timeout);
            //序列化请求头参数
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            var postData = new StringContent(jsonData);
            postData.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
            Task<string> result = httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }
        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">请求参数</param>
        /// <param name="headers">请求头参数</param>
        /// <param name="ContentType">Body类型【(application/json),(multipart/form-data)】</param>
        /// <returns></returns>
        public static Task<string> Post(string url, Dictionary<string, string> headers, string ContentType,string jsonData)
        {
            HttpClient httpClient = CreateHttpClient(url,TimeSpan.FromSeconds(100));
            //序列化请求头参数
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            var postData = new StringContent(jsonData);
            postData.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
            Task<string> result = httpClient.PostAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }
        #endregion

        #region put 请求
        /// <summary>
        /// put 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">请求参数</param>
        /// <param name="headers">请求头参数</param>
        /// <param name="ContentType">Body类型【(application/json),(multipart/form-data)】</param>
        /// <param name="timeout">请求超时</param>
        /// <returns></returns>
        public static Task<string> Put(string url, Dictionary<string, string> headers, string ContentType, string jsonData,TimeSpan timeout)
        {
            HttpClient httpClient = CreateHttpClient(url,timeout);
            //序列化请求头参数
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            var postData = new StringContent(jsonData);
            postData.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
            Task<string> result = httpClient.PutAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }
        
        /// <summary>
        /// put 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">请求参数</param>
        /// <param name="headers">请求头参数</param>
        /// <param name="ContentType">Body类型【(application/json),(multipart/form-data)】</param>
        /// <returns></returns>
        public static Task<string> Put(string url, Dictionary<string, string> headers, string ContentType, string jsonData)
        {
            HttpClient httpClient = CreateHttpClient(url,TimeSpan.FromSeconds(100));
            //序列化请求头参数
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            var postData = new StringContent(jsonData);
            postData.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
            Task<string> result = httpClient.PutAsync(url, postData).Result.Content.ReadAsStringAsync();
            return result;
        }
        #endregion

        #region delete 请求
        /// <summary>
        /// delete 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">请求头参数</param>
        /// <param name="timeout">请求超时</param>
        /// <returns></returns>
        public static Task<string> Delete(string url, Dictionary<string, string> headers,TimeSpan timeout)
        {
            HttpClient httpClient = CreateHttpClient(url,timeout);
            //序列化请求头参数
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            Task<string> result = httpClient.DeleteAsync(url).Result.Content.ReadAsStringAsync();
            return result;
        }
        
        /// <summary>
        /// delete 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">请求头参数</param>
        /// <returns></returns>
        public static Task<string> Delete(string url, Dictionary<string, string> headers)
        {
            HttpClient httpClient = CreateHttpClient(url,TimeSpan.FromSeconds(100));
            //序列化请求头参数
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            Task<string> result = httpClient.DeleteAsync(url).Result.Content.ReadAsStringAsync();
            return result;
        }
        #endregion

        #endregion
    }
}
