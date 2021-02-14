using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZinRadioDesktop
{
    public static class NetworkHelper
    {

        private static HttpClient _httpClient;

        private static readonly object _httpClientLocker = new object();

        public static HttpClient HttpClient
        {
            get
            {
                if (_httpClient != null)
                {
                    return _httpClient;
                }

                lock (_httpClientLocker)
                {
                    if (_httpClient != null)
                    {
                        return _httpClient;
                    }

                    HttpClientHandler httpClientHandler = new HttpClientHandler();
                    _httpClient = new HttpClient(httpClientHandler);
                    _httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { NoCache = true };
                    return _httpClient;
                }
            }
        }

    }
}