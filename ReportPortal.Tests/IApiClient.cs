using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPortal.Tests
{
    public interface IApiClient
    {
        public void SetBaseUrl(string baseUrl);
        public void SetHeader(string name,string value);
        public GetResponse Get(string url);
        public PostResponse Post(string url, string body);
        public PutResponse Put(string url, string body);
        public DeleteResponse Delete (string url);
    }
}
