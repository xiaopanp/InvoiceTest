#region 命名空间
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;

#endregion

namespace WebSite.Common
{
    /// <summary>
    /// 调用Http/Https函数单元
    /// </summary>
    public class HttpUtil
    {
        #region 变量和属性

        /// <summary>
        /// POST/GET超时时间,1分钟
        /// </summary>
        public static readonly int MMaxTimeOut = 60000;

        /// <summary>
        /// POST/GET超时时间,1分钟
        /// </summary>
        public static readonly int MMaxReadWriteTimeOut = 60000;

        /// <summary>
        /// 4K内存大小
        /// </summary>
        public static readonly int MByteLen = 1024 * 4;

        #endregion

        static HttpUtil()
        {

        }

        #region Http Post调用

        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pars"></param> 
        /// <param name="isSsl"></param>
        /// <param name="encoding"></param>
        /// <param name="auth"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, string pars, bool isSsl, Encoding encoding, IHttpAuthorization auth = null)
        {
            //if(string.IsNullOrWhiteSpace(pars))
            //{
            //    return string.Empty;
            //}
            var rsb = new StringBuilder();
            //var data = Encoding.UTF8.GetBytes(pars);
            var data = Strings.ToUtf8ByteArray(pars);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            if (isSsl)
            {
                //验证服务器证书回调自动验证
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(ValidateServerCertificate);
            }
            request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "application/json";
            //如果为Window Mobile,只能采用Stream方式
            //request.ContentType = "text/plain";
            request.ContentLength = data.Length;
            request.KeepAlive = true;
            request.Pipelined = true;
            request.PreAuthenticate = true;
            //request.SendChunked = true;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Timeout = MMaxTimeOut;
            request.ReadWriteTimeout = MMaxReadWriteTimeOut;
            request.MaximumAutomaticRedirections = 2;
            //
            request.Accept = "*/*";
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Accept-Language", "zh-cn");
            //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
            if (auth != null)
            {
                auth.Set(request);
            }
            //
            Stream newStream = null;
            // Send the data.   
            if (data.Length > 0)
            {
                newStream = request.GetRequestStream();
                // Send the data.   
                newStream.Write(data, 0, data.Length);
                newStream.Close();
            }
            // 接收返回的页面
            var response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                    response.StatusDescription);
            }
            var responseStream = response.GetResponseStream();
            //Encoding.UTF8
            using (var reader = new StreamReader(responseStream, encoding))
            {
                var read = new Char[MByteLen];
                var count = reader.Read(read, 0, MByteLen);
                while (count > 0)
                {
                    var str = new string(read, 0, count);
                    rsb.Append(str);
                    count = reader.Read(read, 0, MByteLen);
                }
                //string lineStr;
                //while ((lineStr = reader.ReadLine()) != null)
                //{
                //    rsb.Append(lineStr);
                //}
                //srcString = rsb.ToString();
                //srcString = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                if (responseStream != null)
                {
                    responseStream.Dispose();
                    responseStream.Close();
                }
                response.Close();
            }
            if (newStream != null)
            {
                newStream.Dispose();
            }
            return rsb.ToString();
            //return srcString;
        }
        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pars"></param>
        /// <param name="isSsl"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, string pars, bool isSsl)
        {
            Encoding encoding = Encoding.UTF8;
            return HttpPostData(uri, pars, isSsl, encoding);
        }

        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pars"></param>
        /// <param name="auth"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, string pars, IHttpAuthorization auth = null)
        {
            Encoding encoding = Encoding.UTF8;
            return HttpPostData(uri, pars, false, encoding,auth);
        }

        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static void HttpPostDataAsync(string URI, string pars, IHttpAuthorization auth = null)
        {
            Task.Factory.StartNew(() =>
            {
                HttpPostData(URI, pars, auth);
            });
        }

        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="isSsl"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, byte[] data, bool isSsl, Encoding encoding)
        {
            var rsb = new StringBuilder();
            try
            {
                var request = WebRequest.Create(uri) as HttpWebRequest;
                if (isSsl == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                var response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                var responseStream = response.GetResponseStream();
                //Encoding.UTF8
                using (var reader = new StreamReader(responseStream, encoding))
                {
                    var read = new Char[MByteLen];
                    var count = reader.Read(read, 0, MByteLen);
                    while (count > 0)
                    {
                        var str = new string(read, 0, count);
                        rsb.Append(str);
                        count = reader.Read(read, 0, MByteLen);
                    }
                    //string lineStr;
                    //while ((lineStr = reader.ReadLine()) != null)
                    //{
                    //    rsb.Append(lineStr);
                    //}
                    //srcString = rsb.ToString();
                    //srcString = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    responseStream.Dispose();
                    responseStream.Close();
                    response.Close();
                }
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return rsb.ToString();
            //return srcString;
        }
        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pars"></param>
        /// <param name="isSsl"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, byte[] data, bool isSsl)
        {
            if (data == null || data.Length==0)
            {
                return string.Empty;
            }
            Encoding encoding = Encoding.UTF8;
            return HttpPostData(uri, data, isSsl, encoding);
        }
        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, byte[] data)
        {
            Encoding encoding = Encoding.UTF8;
            return HttpPostData(uri, data, false, encoding);
        }
        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="port"></param>
        /// <param name="pars"></param>
        /// <param name="isSsl"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, Stream data, bool isSsl, Encoding encoding)
        {
            if (data==null)
            {
                return string.Empty;
            }
            return HttpPostData(uri, Streams.ReadAll(data), isSsl, encoding);
        }

        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="isSsl"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, Stream data, bool isSsl)
        {
            Encoding encoding = Encoding.UTF8;
            return HttpPostData(uri, data, isSsl, encoding);
        }
        /// <summary>
        /// 获取Post后的string数据
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static string HttpPostData(string uri, Stream data)
        {
            Encoding encoding = Encoding.UTF8;
            return HttpPostData(uri, data, false, encoding);
        }


        /// <summary>
        /// 获取Post后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static byte[] HttpPostByte(string URI, string pars, bool isSSL)
        {
            byte[] rsb = null;
            Stream responseStream = null;
            try
            {
                var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                responseStream = response.GetResponseStream();
                rsb = Streams.ReadAll(responseStream);
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                if(responseStream!=null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                    responseStream = null;
                }
            }
            return rsb;
        }
        /// <summary>
        /// 获取Post后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] HttpPostByte(string URI, string pars)
        {
            return HttpPostByte(URI, pars, false);
        }
        /// <summary>
        /// 获取Post后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static byte[] HttpPostByte(string URI, byte[] data, bool isSSL)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }
            byte[] rsb = null;
            Stream responseStream = null;
            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                //request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                responseStream = response.GetResponseStream();
                rsb = Streams.ReadAll(responseStream);
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                    responseStream = null;
                }
            }
            return rsb;
        }
        /// <summary>
        /// 获取Post后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static OutT HttpPost<InT,OutT>(string URI, InT input)
        {
            string json = JsonConvert.SerializeObject(input);

            var btyes = Strings.ToUtf8ByteArray(json);

            byte[] result = HttpPostByte(URI, btyes);

            string resultStr = Strings.FromUtf8ByteArray(result);

            OutT resultObj = JsonConvert.DeserializeObject<OutT>(resultStr);

            return resultObj;
        }

        /// <summary>
        /// 获取Post后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] HttpPostByte(string URI, byte[] data)
        {
            return HttpPostByte(URI, data, false);
        }
        /// <summary>
        /// 获取Post后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static byte[] HttpPostByte(string URI, Stream data, bool isSSL)
        {
            if (data == null)
            {
                return null;
            }
            return HttpPostByte(URI, Streams.ReadAll(data), isSSL);
        }
        /// <summary>
        /// 获取Post后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] HttpPostByte(string URI, Stream data)
        {
            return HttpPostByte(URI, data, false);
        }


        /// <summary>
        /// 获取Post后的Stream数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static Stream HttpPostStream(string URI, string pars, bool isSSL)
        {
            Stream rsb = null;
            //Stream responseStream = null;
            try
            {
                var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                //responseStream = response.GetResponseStream();
                //rsb = Streams.ReadAll(responseStream);
                rsb = response.GetResponseStream();
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                //if (responseStream != null)
                //{
                //    responseStream.Close();
                //    responseStream.Dispose();
                //    responseStream = null;
                //}
            }
            return rsb;
        }
        /// <summary>
        /// 获取Post后的Stream数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream HttpPostStream(string URI, string pars)
        {
            return HttpPostStream(URI, pars, false);
        }
        /// <summary>
        /// 获取Post后的Stream数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static Stream HttpPostStream(string URI, byte[] data, bool isSSL)
        {
            Stream rsb = null;
            //Stream responseStream = null;
            try
            {
                //var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                //responseStream = response.GetResponseStream();
                //rsb = Streams.ReadAll(responseStream);
                rsb = response.GetResponseStream();
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                //if (responseStream != null)
                //{
                //    responseStream.Close();
                //    responseStream.Dispose();
                //    responseStream = null;
                //}
            }
            return rsb;
        }
        /// <summary>
        /// 获取Post后的Stream数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream HttpPostStream(string URI, byte[] data)
        {
            return HttpPostStream(URI, data, false);
        }
        /// <summary>
        /// 获取Post后的Stream数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="data"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static Stream HttpPostStream(string URI, Stream data, bool isSSL)
        {
            if (data == null)
            {
                return null;
            }
            return HttpPostStream(URI, Streams.ReadAll(data), isSSL);
        }
        /// <summary>
        /// 获取Post后的Stream数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream HttpPostStream(string URI, Stream data)
        {
            return HttpPostStream(URI, data, false);
        }

        #endregion

        #region Http Get调用

        /// <summary>
        /// 获取Get后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="isSSL"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string httpGetData(string URI, bool isSSL, Encoding encoding, bool UrlDontEscape = false, IHttpAuthorization auth =null)
        {
            var rsb = new StringBuilder();
            try
            {
                HttpWebRequest request;
                if (UrlDontEscape)
                {
                    Uri mUri = new Uri(URI, true);
                    request = WebRequest.Create(mUri) as HttpWebRequest;
                }
                else
                {
                    request = WebRequest.Create(URI) as HttpWebRequest;
                }
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "GET";
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                
                if (auth != null)
                {
                    auth.Set(request);
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                var responseStream = response.GetResponseStream();
                //Encoding.UTF8
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                {
                    var read = new Char[MByteLen];
                    var count = reader.Read(read, 0, MByteLen);
                    while (count > 0)
                    {
                        var str = new string(read, 0, count);
                        rsb.Append(str);
                        count = reader.Read(read, 0, MByteLen);
                    }
                    //srcString = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    responseStream.Dispose();
                    responseStream.Close();
                    response.Close();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            return rsb.ToString();
        }
        /// <summary>
        /// 获取Get后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpGetData(string URI, bool isSSL)
        {
            Encoding encoding = Encoding.UTF8;
            return httpGetData(URI, isSSL, encoding);
        }
        /// <summary>
        /// 获取Get后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <returns></returns>
        public static string httpGetData(string URI, IHttpAuthorization auth = null)
        {
            Encoding encoding = Encoding.UTF8;
            return httpGetData(URI, false, encoding,false, auth);
        }
        /// <summary>
        /// 获取Get后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static byte[] httpGetByte(string URI, bool isSSL)
        {
            byte[] rsb = null;
            Stream responseStream = null;
            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "GET";
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                responseStream = response.GetResponseStream();
                rsb = Streams.ReadAll(responseStream);
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                    responseStream = null;
                }
            }
            return rsb;
        }
        /// <summary>
        /// 获取Get后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <returns></returns>
        public static byte[] httpGetByte(string URI)
        {
            return httpGetByte(URI, false);
        }
        /// <summary>
        /// 获取Get后的Stream数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static Stream httpGetStream(string URI, bool isSSL)
        {
            Stream rsb = null;
            //Stream responseStream = null;
            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "GET";
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                //responseStream = response.GetResponseStream();
                //rsb = Streams.ReadAll(responseStream);
                rsb = response.GetResponseStream();
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                //if (responseStream != null)
                //{
                //    responseStream.Close();
                //    responseStream.Dispose();
                //    responseStream = null;
                //}
            }
            return rsb;
        }
        /// <summary>
        /// 获取Get后的Stream数据
        /// </summary>
        /// <param name="URI"></param>
        /// <returns></returns>
        public static Stream httpGetStream(string URI)
        {
            return httpGetStream(URI, false);
        }

        #endregion

        #region Http Put调用

        /// <summary>
        /// 获取PUT后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="port"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, string pars, bool isSSL, Encoding encoding)
        {
            var rsb = new StringBuilder();
            try
            {
                var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "PUT";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                var responseStream = response.GetResponseStream();
                //Encoding.UTF8
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                {
                    var read = new Char[MByteLen];
                    var count = reader.Read(read, 0, MByteLen);
                    while (count > 0)
                    {
                        var str = new string(read, 0, count);
                        rsb.Append(str);
                        count = reader.Read(read, 0, MByteLen);
                    }
                    //string lineStr;
                    //while ((lineStr = reader.ReadLine()) != null)
                    //{
                    //    rsb.Append(lineStr);
                    //}
                    //srcString = rsb.ToString();
                    //srcString = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    responseStream.Dispose();
                    responseStream.Close();
                    response.Close();
                }
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            return rsb.ToString();
        }
        /// <summary>
        /// 获取Put后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, string pars, bool isSSL)
        {
            Encoding encoding = Encoding.UTF8;
            return httpPutData(URI, pars, isSSL, encoding);
        }
        /// <summary>
        /// 获取Put后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, string pars)
        {
            Encoding encoding = Encoding.UTF8;
            return httpPutData(URI, pars, false, encoding);
        }
        /// <summary>
        /// 获取PUT后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="port"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, byte[] data, bool isSSL, Encoding encoding)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }
            var rsb = new StringBuilder();
            try
            {
                //var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "PUT";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                var responseStream = response.GetResponseStream();
                //Encoding.UTF8
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                {
                    var read = new Char[MByteLen];
                    var count = reader.Read(read, 0, MByteLen);
                    while (count > 0)
                    {
                        var str = new string(read, 0, count);
                        rsb.Append(str);
                        count = reader.Read(read, 0, MByteLen);
                    }
                    //string lineStr;
                    //while ((lineStr = reader.ReadLine()) != null)
                    //{
                    //    rsb.Append(lineStr);
                    //}
                    //srcString = rsb.ToString();
                    //srcString = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    responseStream.Dispose();
                    responseStream.Close();
                    response.Close();
                }
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            return rsb.ToString();
        }
        /// <summary>
        /// 获取Put后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, byte[] data, bool isSSL)
        {
            Encoding encoding = Encoding.UTF8;
            return httpPutData(URI, data, isSSL, encoding);
        }
        /// <summary>
        /// 获取Put后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, byte[] data)
        {
            Encoding encoding = Encoding.UTF8;
            return httpPutData(URI, data, false, encoding);
        }
        /// <summary>
        /// 获取PUT后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="port"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, Stream data, bool isSSL, Encoding encoding)
        {
            if(data==null)
            {
                return string.Empty;
            }
            return httpPutData(URI, Streams.ReadAll(data), isSSL, encoding);
        }
        /// <summary>
        /// 获取Put后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, Stream data, bool isSSL)
        {
            Encoding encoding = Encoding.UTF8;
            return httpPutData(URI, data, isSSL, encoding);
        }
        /// <summary>
        /// 获取Put后的数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static string httpPutData(string URI, Stream data)
        {
            Encoding encoding = Encoding.UTF8;
            return httpPutData(URI, data, false, encoding);
        }

        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static byte[] httpPutByte(string URI, string pars, bool isSSL)
        {
            byte[] rsb = null;
            Stream responseStream = null;
            try
            {
                var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "PUT";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                responseStream = response.GetResponseStream();
                rsb = Streams.ReadAll(responseStream);
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                    responseStream = null;
                }
            }
            return rsb;
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] httpPutByte(string URI, string pars)
        {
            return httpPutByte(URI, pars, false);
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static byte[] httpPutByte(string URI, byte[] data, bool isSSL)
        {
            if(data==null || data.Length==0)
            {
                return null;
            }
            byte[] rsb = null;
            Stream responseStream = null;
            try
            {
                //var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "PUT";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                responseStream = response.GetResponseStream();
                rsb = Streams.ReadAll(responseStream);
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                    responseStream = null;
                }
            }
            return rsb;
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] httpPutByte(string URI, byte[] data)
        {
            return httpPutByte(URI, data, false);
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static byte[] httpPutByte(string URI, Stream data, bool isSSL)
        {
            if (data == null)
            {
                return null;
            }
            return httpPutByte(URI, Streams.ReadAll(data), isSSL);
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] httpPutByte(string URI, Stream data)
        {
            return httpPutByte(URI, data, false);
        }

        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static Stream httpPutStream(string URI, string pars, bool isSSL)
        {
            Stream rsb = null;
            //Stream responseStream = null;
            try
            {
                var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "PUT";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                //responseStream = response.GetResponseStream();
                //rsb = Streams.ReadAll(responseStream);
                rsb = response.GetResponseStream();
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                //if (responseStream != null)
                //{
                //    responseStream.Close();
                //    responseStream.Dispose();
                //    responseStream = null;
                //}
            }
            return rsb;
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream httpPutStream(string URI, string pars)
        {
            return httpPutStream(URI, pars, false);
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static Stream httpPutStream(string URI, byte[] data, bool isSSL)
        {
            Stream rsb = null;
            //Stream responseStream = null;
            try
            {
                //var data = Strings.ToUtf8ByteArray(pars);
                var request = WebRequest.Create(URI) as HttpWebRequest;
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.Method = "PUT";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.ContentType = "text/plain";
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                //
                Stream newStream = null;
                // Send the data.   
                if (data != null && data.Length > 0)
                {
                    newStream = request.GetRequestStream();
                    // Send the data.   
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                // 接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                //responseStream = response.GetResponseStream();
                //rsb = Streams.ReadAll(responseStream);
                rsb = response.GetResponseStream();
                if (newStream != null)
                {
                    newStream.Dispose();
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                //if (responseStream != null)
                //{
                //    responseStream.Close();
                //    responseStream.Dispose();
                //    responseStream = null;
                //}
            }
            return rsb;
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream httpPutStream(string URI, byte[] data)
        {
            return httpPutStream(URI, data, false);
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static Stream httpPutStream(string URI, Stream data, bool isSSL)
        {
            if(data==null)
            {
                return null;
            }
            return httpPutStream(URI,Streams.ReadAll(data),isSSL);
        }
        /// <summary>
        /// 获取PUT后的byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream httpPutStream(string URI, Stream data)
        {
            return httpPutStream(URI, data, false);
        }

        #endregion

        #region Http 所有类型调用


        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, string pars,string Method,
            string ContentType,bool isSSL, Encoding encoding)
        {
            #region 为空取默认值
            if (string.IsNullOrWhiteSpace(Method))
            {
                Method = "GET";
            }
            if (string.IsNullOrWhiteSpace(ContentType))
            {
                ContentType = "text/plain";
            }
            #endregion

            var rsb = new StringBuilder();

            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;

                #region WebRequest参数
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.Method = Method.ToUpper();
                request.ContentType = ContentType;
                //
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                
                #endregion

                #region 传送数据流
                Stream newStream = null;
                if (!string.IsNullOrWhiteSpace(pars))
                {
                    var data = Strings.ToUtf8ByteArray(pars);
                    request.ContentLength = data.Length;
                    newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                #endregion

                #region 接收返回数据
                
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                var responseStream = response.GetResponseStream();
                //Encoding.UTF8
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                {
                    var read = new Char[MByteLen];
                    var count = reader.Read(read, 0, MByteLen);
                    while (count > 0)
                    {
                        var str = new string(read, 0, count);
                        rsb.Append(str);
                        count = reader.Read(read, 0, MByteLen);
                    }
                    //string lineStr;
                    //while ((lineStr = reader.ReadLine()) != null)
                    //{
                    //    rsb.Append(lineStr);
                    //}
                    //srcString = rsb.ToString();
                    //srcString = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    responseStream.Dispose();
                    responseStream.Close();
                    response.Close();
                }
                
                #endregion

                if (newStream != null)
                {
                    newStream.Dispose();
                    newStream = null;
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            return rsb.ToString();
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <param name="ContentType"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, string pars,string Method, 
            string ContentType,bool isSSL)
        {
            return httpRequestData(URI, pars,Method,ContentType,isSSL,Encoding.UTF8);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, string pars, string Method, 
            bool isSSL)
        {
            return httpRequestData(URI, pars, Method, "text/plain", isSSL);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, string pars, string Method)
        {
            return httpRequestData(URI, pars, Method, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, byte[] data, string Method,
            string ContentType, bool isSSL, Encoding encoding)
        {
            #region 为空取默认值
            if (string.IsNullOrWhiteSpace(Method))
            {
                Method = "GET";
            }
            if (string.IsNullOrWhiteSpace(ContentType))
            {
                ContentType = "text/plain";
            }
            #endregion

            var rsb = new StringBuilder();

            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;

                #region WebRequest参数
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.Method = Method.ToUpper();
                request.ContentType = ContentType;
                //
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";

                #endregion

                #region 传送数据流
                Stream newStream = null;
                if (data != null && data.Length > 0)
                {
                    //var data = Strings.ToUtf8ByteArray(pars);
                    request.ContentLength = data.Length;
                    newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                #endregion

                #region 接收返回数据

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                var responseStream = response.GetResponseStream();
                //Encoding.UTF8
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                {
                    var read = new Char[MByteLen];
                    var count = reader.Read(read, 0, MByteLen);
                    while (count > 0)
                    {
                        var str = new string(read, 0, count);
                        rsb.Append(str);
                        count = reader.Read(read, 0, MByteLen);
                    }
                    //string lineStr;
                    //while ((lineStr = reader.ReadLine()) != null)
                    //{
                    //    rsb.Append(lineStr);
                    //}
                    //srcString = rsb.ToString();
                    //srcString = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    responseStream.Dispose();
                    responseStream.Close();
                    response.Close();
                }

                #endregion

                if (newStream != null)
                {
                    newStream.Dispose();
                    newStream = null;
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            return rsb.ToString();
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <param name="ContentType"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, byte[] data, string Method,
            string ContentType, bool isSSL)
        {
            return httpRequestData(URI, data, Method, ContentType, isSSL, Encoding.UTF8);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, byte[] data, string Method,
            bool isSSL)
        {
            return httpRequestData(URI, data, Method, "text/plain", isSSL);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, byte[] data, string Method)
        {
            return httpRequestData(URI, data, Method, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, Stream data, string Method,
            string ContentType, bool isSSL, Encoding encoding)
        {
            return httpRequestData(URI, Streams.ReadAll(data), Method, ContentType, isSSL, encoding);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <param name="ContentType"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, Stream data, string Method,
            string ContentType, bool isSSL)
        {
            return httpRequestData(URI, data, Method, ContentType, isSSL, Encoding.UTF8);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <param name="isSSL"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, Stream data, string Method,
            bool isSSL)
        {
            return httpRequestData(URI, data, Method, "text/plain", isSSL);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        public static string httpRequestData(string URI, Stream data, string Method)
        {
            return httpRequestData(URI, data, Method, false);
        }


        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI, string pars, string Method,
            string ContentType, bool isSSL)
        {
            #region 为空取默认值
            if (string.IsNullOrWhiteSpace(Method))
            {
                Method = "GET";
            }
            if (string.IsNullOrWhiteSpace(ContentType))
            {
                ContentType = "text/plain";
            }
            #endregion

            byte[] rsb = null;
            Stream responseStream = null;

            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;

                #region WebRequest参数
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.Method = Method.ToUpper();
                request.ContentType = ContentType;
                //
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";

                #endregion

                #region 传送数据流
                Stream newStream = null;
                if (!string.IsNullOrWhiteSpace(pars))
                {
                    var data = Strings.ToUtf8ByteArray(pars);
                    request.ContentLength = data.Length;
                    newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                #endregion

                #region 接收返回数据

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                responseStream = response.GetResponseStream();
                rsb = Streams.ReadAll(responseStream);

                #endregion

                if (newStream != null)
                {
                    newStream.Dispose();
                    newStream = null;
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                    responseStream = null;
                }
            }
            return rsb;
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI, string pars, string Method,
            string ContentType)
        {
            return httpRequestByte(URI, pars, Method, ContentType, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI, string pars)
        {
            return httpRequestByte(URI, pars, string.Empty, string.Empty, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI, byte[] data, string Method,
            string ContentType, bool isSSL)
        {
            #region 为空取默认值
            if (string.IsNullOrWhiteSpace(Method))
            {
                Method = "GET";
            }
            if (string.IsNullOrWhiteSpace(ContentType))
            {
                ContentType = "text/plain";
            }
            #endregion

            byte[] rsb = null;
            Stream responseStream = null;

            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;

                #region WebRequest参数
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.Method = Method.ToUpper();
                request.ContentType = ContentType;
                //
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";

                #endregion

                #region 传送数据流
                Stream newStream = null;
                if (data != null && data.Length > 0)
                {
                    //var data = Strings.ToUtf8ByteArray(pars);
                    request.ContentLength = data.Length;
                    newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                #endregion

                #region 接收返回数据

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                responseStream = response.GetResponseStream();
                rsb = Streams.ReadAll(responseStream);

                #endregion

                if (newStream != null)
                {
                    newStream.Dispose();
                    newStream = null;
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                    responseStream = null;
                }
            }
            return rsb;
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI,byte[] data, string Method,
            string ContentType)
        {
            return httpRequestByte(URI, data, Method, ContentType, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI, byte[] data)
        {
            return httpRequestByte(URI, data, string.Empty, string.Empty, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI, Stream data, string Method,
            string ContentType, bool isSSL)
        {
            return httpRequestByte(URI, Streams.ReadAll(data), Method,ContentType,isSSL);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI, Stream data, string Method,
            string ContentType)
        {
            return httpRequestByte(URI, data, Method, ContentType, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static byte[] httpRequestByte(string URI, Stream data)
        {
            return httpRequestByte(URI, data, string.Empty, string.Empty, false);
        }


        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, string pars, string Method,
            string ContentType, bool isSSL)
        {
            #region 为空取默认值
            if (string.IsNullOrWhiteSpace(Method))
            {
                Method = "GET";
            }
            if (string.IsNullOrWhiteSpace(ContentType))
            {
                ContentType = "text/plain";
            }
            #endregion

            Stream rsb = null;
            //Stream responseStream = null;
            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;

                #region WebRequest参数
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.Method = Method.ToUpper();
                request.ContentType = ContentType;
                //
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";

                #endregion

                #region 传送数据流
                Stream newStream = null;
                if (!string.IsNullOrWhiteSpace(pars))
                {
                    var data = Strings.ToUtf8ByteArray(pars);
                    request.ContentLength = data.Length;
                    newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                #endregion

                #region 接收返回数据

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                //responseStream = response.GetResponseStream();
                //rsb = Streams.ReadAll(responseStream);
                rsb = response.GetResponseStream();
                #endregion

                if (newStream != null)
                {
                    newStream.Dispose();
                    newStream = null;
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                //if (responseStream != null)
                //{
                //    responseStream.Close();
                //    responseStream.Dispose();
                //    responseStream = null;
                //}
            }
            return rsb;
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, string pars, string Method,
            string ContentType)
        {
            return httpRequestStream(URI, pars, Method, ContentType, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, string pars)
        {
            return httpRequestStream(URI, pars, string.Empty, string.Empty, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, byte[] data, string Method,
            string ContentType, bool isSSL)
        {
            #region 为空取默认值
            if (string.IsNullOrWhiteSpace(Method))
            {
                Method = "GET";
            }
            if (string.IsNullOrWhiteSpace(ContentType))
            {
                ContentType = "text/plain";
            }
            #endregion

            Stream rsb = null;
            //Stream responseStream = null;
            try
            {
                var request = WebRequest.Create(URI) as HttpWebRequest;

                #region WebRequest参数
                if (isSSL == true)
                {
                    //验证服务器证书回调自动验证
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "application/json";
                //如果为Window Mobile,只能采用Stream方式
                request.Method = Method.ToUpper();
                request.ContentType = ContentType;
                //
                request.KeepAlive = true;
                request.Pipelined = true;
                request.PreAuthenticate = true;
                //request.SendChunked = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = MMaxTimeOut;
                request.ReadWriteTimeout = MMaxReadWriteTimeOut;
                request.MaximumAutomaticRedirections = 2;
                //
                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip");
                request.Headers.Add("Accept-Language", "zh-cn");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; GTB6.5; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E; InfoPath.2)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";

                #endregion

                #region 传送数据流
                Stream newStream = null;
                if (data != null && data.Length>0)
                {
                    //var data = Strings.ToUtf8ByteArray(pars);
                    request.ContentLength = data.Length;
                    newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                #endregion

                #region 接收返回数据

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                        response.StatusDescription);
                }
                //responseStream = response.GetResponseStream();
                //rsb = Streams.ReadAll(responseStream);
                rsb = response.GetResponseStream();
                #endregion

                if (newStream != null)
                {
                    newStream.Dispose();
                    newStream = null;
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                //if (responseStream != null)
                //{
                //    responseStream.Close();
                //    responseStream.Dispose();
                //    responseStream = null;
                //}
            }
            return rsb;
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, byte[] data, string Method,
            string ContentType)
        {
            return httpRequestStream(URI, data, Method, ContentType, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, byte[] data)
        {
            return httpRequestStream(URI, data, string.Empty, string.Empty, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <param name="isSSL">是否SSL安全协议请求</param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, Stream data, string Method,
            string ContentType, bool isSSL)
        {
            return httpRequestStream(URI,Streams.ReadAll(data),Method,ContentType,isSSL);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <param name="Method">GET,POST,PUT,DELETE等</param>
        /// <param name="ContentType">text/plain</param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, Stream data, string Method,
            string ContentType)
        {
            return httpRequestStream(URI, data, Method, ContentType, false);
        }
        /// <summary>
        /// 获取HttpWebRequest请求后byte[]数据
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static Stream httpRequestStream(string URI, Stream data)
        {
            return httpRequestStream(URI, data, string.Empty, string.Empty, false);
        }

        #endregion

        #region Http远程读取文件

        /// <summary>
        /// 发送数据本地存储
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public static void localDataSave(string path,string data, string fileName)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }
            try
            {
                var nowdate = System.DateTime.Now;
                var ndate = nowdate.ToLongDateString();
                //var ntime = nowdate.ToString("yyyyMMdd HHmmssfffff");
                var ntime = nowdate.ToString("HHmmss_ffff");
                var mappath = path;//HttpContext.Current.Server.MapPath("~/App_Data/DataState/");
                var tPath = mappath + ndate;
                if (!Directory.Exists(tPath)) { Directory.CreateDirectory(tPath); }
                var realname = string.Empty;
                //暂时不做自动生成处理
                //if (string.IsNullOrWhiteSpace(fileName))
                //{
                //    fileName = Guid.NewGuid().ToString();
                //}
                realname = "[" + ntime + "]" + fileName;
                var fn = tPath + Path.AltDirectorySeparatorChar + realname;
                File.WriteAllText(fn, data, Encoding.UTF8);
                //var fpath = new FileInfo(realname);
                //if (!fpath.Exists)
                //{
                //    fpath.Create();
                //}
                //var stream=fpath.AppendText();
                //stream.Write(data);
                //stream.Flush();
                //stream.Close();
                //stream.Dispose();
            }
            catch
            {

            }
        }
        /// <summary>
        /// Http URL文件下载后转换为byte[]数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isSSL"></param>
        /// <param name="isImgThum"></param>
        /// <returns></returns>
        public static byte[] convertUrlFile(string savePath, string url, bool isSSL, bool isImgThum)
        {
            byte[] rbts = null;
            if (string.IsNullOrWhiteSpace(url))
            {
                return rbts;
            }
            using (var client = new WebClient())
            {
                var fileTag = url.Substring(url.LastIndexOf(".") + 1, url.Length - url.LastIndexOf(".") - 1);
                var guid_mid = Guid.NewGuid();
                //var localpath = HttpContext.Current.Server.MapPath("~/temp/") + guid_mid.ToString() + "." + fileTag;
                var localpath = savePath + guid_mid.ToString() + "." + fileTag;
                try
                {
                    client.DownloadFile(url, localpath);
                }
                catch
                {
                    return rbts;
                }
                //如果需要建立图片缩略图
                if (isImgThum == true)
                {

                }
                if (File.Exists(localpath))
                {
                    rbts = File.ReadAllBytes(localpath);
                    //FileStream stream = null;
                    //StreamReader reader = null;
                    //try
                    //{
                    //    stream = new FileStream(localpath, FileMode.Open);
                    //    reader = new StreamReader(stream);
                    //    rbts = new byte[stream.Length];
                    //    stream.Read(rbts, 0, rbts.Length);
                    //    stream.Seek(0, SeekOrigin.Begin);
                    //    stream.Flush();
                    //}
                    //catch{}
                    //finally
                    //{
                    //    if (reader != null)
                    //    {
                    //        reader.Close();
                    //        reader.Dispose();
                    //    }
                    //    if(stream!=null)
                    //    {
                    //        stream.Close();
                    //        stream.Dispose();
                    //    }
                    //}
                    File.Delete(localpath);
                }
                client.Dispose();
            }
            return rbts;
        }
        /// <summary>
        /// Http URL文件下载后转换为byte[]数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] convertUrlFile(string savePath,string url)
        {
            return convertUrlFile(savePath,url, false, false);
        }
        /// <summary>
        /// Http URL文件下载后转换为Base64String数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isSSL"></param>
        /// <param name="isImgThum"></param>
        /// <returns></returns>
        public static string convertUrlFile64String(string savePath,string url, bool isSSL, bool isImgThum)
        {
            var str = string.Empty;
            var bts = convertUrlFile(savePath,url, isSSL, isImgThum);
            if (bts != null && bts.Length > 0)
            {
                str = Convert.ToBase64String(bts);
                bts = null;
            }
            return str;
        }
        /// <summary>
        /// Http URL文件下载后转换为Base64String数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string convertUrlFile64String(string savePath,string url)
        {
            return convertUrlFile64String(savePath,url, false, false);
        }

        #endregion

        #region 静态私有函数

        /// <summary>
        /// 验证服务器证书回调自动验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //if (sslPolicyErrors == SslPolicyErrors.None) {
            //  return true;
            //}

            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            //// Do not allow this client to communicate with unauthenticated servers.
            //return false;
            // 总是接受 
            return true;
        }
        /// <summary>
        /// 异步调用函数
        /// </summary>
        /// <param name="iar"></param>
        private static void processHttpResponseAsync(IAsyncResult iar)
        {
            var rsb = new StringBuilder();
            var encoding = Encoding.UTF8;
            var request = iar.AsyncState as HttpWebRequest;
            // 接收返回的页面
            var response = request.EndGetResponse(iar) as HttpWebResponse;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new System.Exception(response.StatusCode + ":HttpStatus异常:" +
                    response.StatusDescription);
            }
            var responseStream = response.GetResponseStream();
            //Encoding.UTF8
            using (var reader = new System.IO.StreamReader(responseStream, encoding))
            {
                var read = new Char[MByteLen];
                var count = reader.Read(read, 0, MByteLen);
                while (count > 0)
                {
                    var str = new string(read, 0, count);
                    rsb.Append(str);
                    count = reader.Read(read, 0, MByteLen);
                }
                reader.Close();
                reader.Dispose();
                responseStream.Dispose();
                responseStream.Close();
                response.Close();
            }
            //Console替换为页面呈现容器即可
            Console.WriteLine(rsb.ToString());
            Console.ReadLine();
            /*
            异步调用例子
            IAsyncResult result = request.BeginGetResponse(new AsyncCallback(processHttpResponseAsync), request);
            //处理超时请求
            ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, 
            new WaitOrTimerCallback(asyncTimeout), request, 1000 * 60 * 10, true);
            */
        }
        /// <summary>
        /// 超时终止请求  
        /// </summary>
        /// <param name="state"></param>
        /// <param name="bTimeOut"></param>
        private static void asyncTimeout(object state, bool bTimeOut)
        {
            if (bTimeOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }
        #endregion
    }
}
