using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ReportPortal.Tests
{
    public class RestSharpApiClient : IApiClient
    {
        RestClient client;

        public RestSharpApiClient(string baseUrl) {

            client = new RestClient(baseUrl);
        }
        public DeleteResponse Delete(string url)
        {
            var request = new RestRequest(url, Method.Delete);
            var response =  client.ExecuteAsync(request).GetAwaiter().GetResult();

            return new DeleteResponse
            {
                Json = response.Content,
                StatusCode = (int)response.StatusCode
            };
        }

        public GetResponse Get(string url)
        {
            var request = new RestRequest(url, Method.Get);
            var response = client.ExecuteAsync(request).GetAwaiter().GetResult();
            return new GetResponse
            {
                Json = response.Content,
                StatusCode = (int)response.StatusCode,
                IsSucceed = response.IsSuccessStatusCode
            };
        }

        public PostResponse Post(string url, string body)
        {
            var request = new RestRequest(url, Method.Post);
            request.AddBody(body);
            var response = client.ExecuteAsync(request).GetAwaiter().GetResult();
            return new PostResponse
            {
                Json = response.Content,
                StatusCode = (int)response.StatusCode
            };
        }

        public PutResponse Put(string url, string body)
        {
            var request = new RestRequest(url, Method.Put);
            request.AddBody(body);
            var response = client.ExecuteAsync(request).GetAwaiter().GetResult();
            return new PutResponse
            {
                Json = response.Content,
                StatusCode = (int)response.StatusCode
            };
        }

        public void SetBaseUrl(string baseUrl)
        {
    
        }

        public void SetHeader(string name, string value)
        {
            client.AddDefaultHeader(name, value);
        }
    }
}
