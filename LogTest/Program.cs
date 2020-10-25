﻿using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace LogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //获取AccessToken
            string accessToken = GetAccessToken();
            //获取外部接口日志
            string externalLog =  GetExternalLog(accessToken);
            //Console.WriteLine(externalLog);
        }
        //
        private static string GetExternalLog(string accessToken)
        {
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/getPushLog?accessToken=" +  accessToken;
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //页码信息
            JObject pageable = new JObject();
            pageable.Add("id", null);
            pageable.Add("pageSize", 100);
            pageable.Add("type", "first");
            //请求参数
            JObject postParam = new JObject();
            postParam.Add("pageable", pageable);
            postParam.Add("devType", "user");
            postParam.Add("startTime", ConvertDateTimeToInt(DateTime.Now));
            postParam.Add("endTime", ConvertDateTimeToInt(DateTime.Now));
            postParam.Add("pushType", "success");
            string jsonRequest = PostUrl(url, postParam.ToString(), "application/json");
            return jsonRequest.ToString();
        }

        private static string GetAccessToken()
        {
            JObject param = new JObject();
            param.Add("appId", "xxxxxx");
            param.Add("eid", "15452095");
            param.Add("secret", "xxxxxxxx");
            param.Add("timestamp", ConvertDateTimeToInt(DateTime.Now));
            param.Add("scope", "team");
            String url = "https://yunzhijia.com/gateway/oauth2/token/getAccessToken";
            string jsonRequest = PostUrl(url, param.ToString(), "application/json");
            JObject resGroupJson = JObject.Parse(jsonRequest);
            return resGroupJson["data"]["accessToken"].ToString();
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        private static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        /// <summary>
        /// Post提交数据
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="paramData">参数</param>
        /// <returns></returns>
        public static string PostUrl(string url, string postData, string contentType)
        {
            string result = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                req.Method = "POST";

                req.ContentType = contentType;// "application/x-www-form-urlencoded"

                req.Timeout = 800;//请求超时时间

                byte[] data = Encoding.UTF8.GetBytes(postData);

                req.ContentLength = data.Length;

                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);

                    reqStream.Close();
                }

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                Stream stream = resp.GetResponseStream();

                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e) { }

            return result;
        }
    }   
}
