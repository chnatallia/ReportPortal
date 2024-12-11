using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ReportPortal.Tests
{
    public class UnitTest1
    {
        private readonly string projectName;
        string invalidProjectName;
        string validLaunchUuid;
        int validLaunchId;
        int invalidLaunchId;
        //string apiKey = "mykey_PSuPAlX0Q8KDStLMq4jh_93E3tTHOnlDoUkMPKyAINAnkB8EP0MnOZ310YK30yX-";

        IConfiguration Configuration;
        IApiClient client;

        public UnitTest1()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            //client = new RestSharpApiClient("http://172.30.128.1:8080/api/v1/");
            client = new HttpClientAPI();
            client.SetBaseUrl(Configuration["baseUrl"]!);
            client.SetHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MzMzNTUxMTQsInVzZXJfbmFtZSI6InN1cGVyYWRtaW4iLCJhdXRob3JpdGllcyI6WyJST0xFX0FETUlOSVNUUkFUT1IiXSwianRpIjoiWFRpSE1RS0R0cXpBd2x3eXhheFBGcExUSzlZIiwiY2xpZW50X2lkIjoidWkiLCJzY29wZSI6WyJ1aSJdfQ.e-tt6lIlTSExoAKbligV2iYfWTi0cXLyOssa-Jafcjc");
            this.projectName = Configuration["projectName"]!;
            this.invalidProjectName = Configuration["invalidProjectName"]!;
            this.validLaunchId = Int32.Parse(Configuration.GetSection("validLaunch")["id"]!);
            this.validLaunchUuid = Configuration.GetSection("validLaunch")["uuid"]!;
            this.invalidLaunchId = Int32.Parse(Configuration.GetSection("invalidLaunch")["id"]!);
        }

        [Fact]
        public void GetSpecifiedLaunchByUUIDPositive()
        {
            GetResponse response = client.Get(projectName + "/launch/uuid/" + validLaunchUuid);
            Assert.True(response.IsSucceed);
            Launch result = JsonConvert.DeserializeObject<Launch>(response.Json);

            //Get uuid not deserializign all response <- Newtonsoft.Json

            Assert.Equal(result.uuid,validLaunchUuid);
        }

        [Fact]
        public void GetSpecifiedLaunchByIdNotFound()
        {
            GetResponse response = client.Get(projectName + "/launch/" + invalidLaunchId);
            Assert.True(response.StatusCode == (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public void UpdateLaunchForSpecificProjectValidLaunchId()
        {

            Payload payload = new Payload()
            {
                mode = "DEFAULT",
                description = "Test Demo",
                attributes = new List<Attribute> 
                { 
                    new Attribute
                    {
                        key = "platform",
                        value = "Windows 11"
                    }
                }
            };
            var json = JsonConvert.SerializeObject(payload);
            PutResponse response = client.Put(
            projectName + "/launch/" + validLaunchId + "/update", json);
            string jsonResponse =  response.Json;
            Assert.True(response.StatusCode == (int)HttpStatusCode.OK);
            ApiResponse  result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Launch with ID = '{validLaunchId}' successfully updated.", result.message);
        }

        [Fact]
        public void UpdateLaunchForSpecificProjectInvalidProject()
        {
            var payload = new
            {
                mode = "DEFAULT",
                description = "Test Demo",
                attributes = new Object[]
                {
                    new
                    {
                        key = "platform",
                        value ="Windows 11"
                    }
                }
            };
            var json = JsonConvert.SerializeObject(payload);
            PutResponse response = client.Put(
            invalidProjectName + "/launch/" + validLaunchId + "/update", json);
            string jsonResponse = response.Json;
            Assert.True(response.StatusCode == (int)HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Project '{invalidProjectName}' not found. Did you use correct project name?", result.message);

        }
        [Fact]
        public void UpdateLaunchForSpecificProjectInvalidLaunch()
        {
            var payload = new
            {
                mode = "DEFAULT",
                description = "Test Demo",
                attributes = new Object[]
                {
                    new
                    {
                        key = "platform",
                        value ="Windows 11"
                    }
                }
            };
            var json = JsonConvert.SerializeObject(payload);
            PutResponse response = client.Put(
            projectName + "/launch/" + invalidLaunchId + "/update",json);
            string jsonResponse = response.Json;
            Assert.True(response.StatusCode == (int)HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Launch '{invalidLaunchId}' not found. Did you use correct Launch ID?", result.message);

        }

        [Fact]
        public void StartLaunchAnalyzerOnDemandOk()
        {
            var payload = new
            {
                launchId = validLaunchId,
                analyzerMode = "ALL",
                analyzerTypeName = "autoAnalyzer",
                analyzeItemsMode = new string[] { "AUTO_ANALYZED" }

            };
            var json = JsonConvert.SerializeObject(payload);
            PostResponse response =  client.Post( projectName + "/launch/analyze", json);
            var jsonResponse = response.Json;
            Assert.True(response.StatusCode == (int)HttpStatusCode.OK);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"autoAnalyzer analysis for launch with ID='{validLaunchId}' started.", result.message);
        }

        [Fact]
        public void StartLaunchAnalyzerOnDemandForInvalidProject()
        {
            var payload = new
            {
                launchId = validLaunchId,
                analyzerMode = "ALL",
                analyzerTypeName = "autoAnalyzer",
                analyzeItemsMode = new string[] { "AUTO_ANALYZED" }

            };
            var json = JsonConvert.SerializeObject(payload);
            PostResponse response = client.Post( invalidProjectName + "/launch/analyze", json);
            var jsonResponse = response.Json;
            Assert.True(response.StatusCode == (int)HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Project '{invalidProjectName}' not found. Did you use correct project name?", result.message);
        }

        [Fact]
        public void StartLaunchAnalyzerOnDemandForInvalidLaunch()
        {
            var payload = new
            {
                launchId = invalidLaunchId,
                analyzerMode = "ALL",
                analyzerTypeName = "autoAnalyzer",
                analyzeItemsMode = new string[] { "AUTO_ANALYZED" }

            };
            string json = JsonConvert.SerializeObject(payload);
            PostResponse response = client.Post( projectName + "/launch/analyze", json);
            var jsonResponse = response.Json;
            Assert.True(response.StatusCode == (int)HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Launch '{invalidLaunchId}' not found. Did you use correct Launch ID?", result.message);
        }

        [Fact]
        public void DeleteLaunchNotValid()
        {

            DeleteResponse response = client.Delete(projectName + "/launch/"+invalidLaunchId);
            var jsonResponse = response.Json;
            Assert.True(response.StatusCode == (int)HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Launch '{invalidLaunchId}' not found. Did you use correct Launch ID?", result.message);
        }

    }
}