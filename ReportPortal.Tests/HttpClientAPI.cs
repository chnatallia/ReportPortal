using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPortal.Tests
{
    public class HttpClientAPI : IApiClient
    {
        HttpClient client = new HttpClient();
        public DeleteResponse Delete(string url)
        {
            var res =  client.DeleteAsync(url).GetAwaiter().GetResult();

            return new DeleteResponse
            {
                Json = res.Content.ReadAsStringAsync().GetAwaiter().GetResult(),
                StatusCode = (int)res.StatusCode
            };
        }

        public GetResponse Get(string url)
        {
            var res = client.GetAsync(url).GetAwaiter().GetResult();
            return new GetResponse
            {
                Json = res.Content.ReadAsStringAsync().GetAwaiter().GetResult(),
                StatusCode = (int)res.StatusCode,
                IsSucceed = res.IsSuccessStatusCode
            };
        }

        public PostResponse Post(string url, string json)
        {
            var res = client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
            return new PostResponse
            {
                Json = res.Content.ReadAsStringAsync().GetAwaiter().GetResult(),
                StatusCode = (int)res.StatusCode,
            };
        }

        public PutResponse Put(string url, string json)
        {
            var arr = new int[] { 1, 2, 3, 4, 5 };

            var res = client.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
            return new PutResponse
            {
                Json = res.Content.ReadAsStringAsync().GetAwaiter().GetResult(),
                StatusCode = (int)res.StatusCode,
            };
        }

        public void SetBaseUrl(string baseUrl)
        {
            client.BaseAddress = new Uri(baseUrl);
        }

        public void SetHeader(string name, string value)
        {
            client.DefaultRequestHeaders.Add(name, value);
        }
    }


}
