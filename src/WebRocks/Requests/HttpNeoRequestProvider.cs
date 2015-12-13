using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebRocks.Requests
{
    public class HttpNeoRequestProvider : INeoRequestProvider
    {
        public const string RateLimitHeader = "X-RateLimit-Limit";
        public const string RateLimitRemainingHeader = "X-RateLimit-Remaining";

        public HttpNeoRequestProvider() { }

        public NeoResponse SendGetRequest(Uri uri)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            var response = new NeoResponse();

            using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                long length = webResponse.ContentLength;
                response.ResponseCode = (int)webResponse.StatusCode;

                if(response.ResponseCode >= 200 || response.ResponseCode < 300)
                {
                    response.RateLimitTotal = Int32.Parse(webResponse.Headers[RateLimitHeader]);
                    response.RateLimitRemaining = Int32.Parse(webResponse.Headers[RateLimitRemainingHeader]);

                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            response.ResponseText = reader.ReadToEnd();
                        }
                    }
                }
            }

            return response;
        }

        public Task<NeoResponse> SendGetRequestAsync(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
