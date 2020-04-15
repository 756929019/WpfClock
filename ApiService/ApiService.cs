using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ApiService
{
    public class ApiService
    {
        /*
            http://weather.com.cn/data/city3jdata/china.html（省份列表）
            http://weather.com.cn/data/city3jdata/provshi/10121.html（地区列表） 
            http://weather.com.cn/data/city3jdata/station/1012101.html（县城列表）
            http://weather.com.cn/data/cityinfo/101210101.html（当天天气预报）
            http://weather.com.cn/data/sk/101210101.html（当前实时信息）
        */
        /// <summary>
        /// 省份列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetProvinceList()
        {
            JObject res = GetResponseData("http://weather.com.cn/data/city3jdata/china.html");
            if (res != null)
            {
                JToken record = res.Next;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var jp in res)
                {
                    dic.Add(jp.Key.ToString(), jp.Value.ToString());
                }
                return dic;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 地区列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetCityList(string province)
        {
            JObject res = GetResponseData(string.Format("http://weather.com.cn/data/city3jdata/provshi/{0}.html", province));
            if (res != null)
            {
                JToken record = res.Next;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var jp in res)
                {
                    dic.Add(province + jp.Key.ToString(), jp.Value.ToString());
                }
                return dic;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 县城列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetCountyList(string city)
        {
            JObject res = GetResponseData(string.Format("http://weather.com.cn/data/city3jdata/station/{0}.html", city));
            if (res != null)
            {
                JToken record = res.Next;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var jp in res)
                {
                    dic.Add(city + jp.Key.ToString(), jp.Value.ToString());
                }
                return dic;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 当天天气预报
        /// </summary>
        /// <returns></returns>
        public static WeatherRealTime GetWeatherToday(string county)
        {
            JObject res = GetResponseData(string.Format("http://weather.com.cn/data/cityinfo/{0}.html", county));
            if (res != null)
            {
                WeatherRealTime wrt = new WeatherRealTime();
                wrt = JsonConvert.DeserializeObject<WeatherRealTime>(res["weatherinfo"].ToString());

                return wrt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 当前实时信息
        /// </summary>
        /// <returns></returns>
        public static WeatherRealTime GetWeatherRealTime(string county)
        {
            JObject res = GetResponseData(string.Format("http://weather.com.cn/data/sk/{0}.html", county));
            if (res != null)
            {
                WeatherRealTime wrt = new WeatherRealTime();
                wrt = JsonConvert.DeserializeObject<WeatherRealTime>(res["weatherinfo"].ToString());

                return wrt;
            }
            else
             {
                return null;
            }
        }

        /// <summary>    
        /// 返回text数据
        /// </summary>    
        public static JObject GetResponseData(string Url)
        {
            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamReceive = response.GetResponseStream();

            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            string strResult = streamReader.ReadToEnd();
            streamReceive.Dispose();
            streamReader.Dispose();

            return JsonResult(strResult);
        }

        /// <summary>
        /// Json解析
        /// </summary>
        public static JObject JsonResult(string JsonText)
        {
            try
            {
                var json = (JObject)JsonConvert.DeserializeObject(JsonText);
                return json;
            }
            catch { return null; }
        }
    }

    public class WeatherRealTime : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string  city{get;set;}
        public string  cityid{get;set;}
        public string  temp{get;set;}
        public string  WD{get;set;}
        public string  WS{get;set;}
        public string  SD{get;set;}
        public string  WSE{get;set;}
        public string  time{get;set;}
        public string  isRadar{get;set;}
        public string  Radar{get;set;}
        public string  njd{get;set;}
        public string  qy{get;set;}
        public string  rain{get;set;}

        public string temp1 { get; set; }
        public string temp2 { get; set; }
        public string weather { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
        public string ptime { get; set; }
    }
}
