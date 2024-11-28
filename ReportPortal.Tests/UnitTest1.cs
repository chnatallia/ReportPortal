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

        HttpClient client;

        public UnitTest1()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            client = new HttpClient();
            client.BaseAddress = new Uri(Configuration["baseUrl"]!);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MzI2NDkwNDcsInVzZXJfbmFtZSI6InN1cGVyYWRtaW4iLCJhdXRob3JpdGllcyI6WyJST0xFX0FETUlOSVNUUkFUT1IiXSwianRpIjoiWEVtNnZyVkVPYVRIVF9KVHFDQ1NiTzR2VExZIiwiY2xpZW50X2lkIjoidWkiLCJzY29wZSI6WyJ1aSJdfQ.UH4R6mbTfpS6z4lqmRUdyxuJhOvR6Qy9NbHhge1gmx4");
            this.projectName = Configuration["projectName"]!;
            this.invalidProjectName = Configuration["invalidProjectName"]!;
            this.validLaunchId = Int32.Parse(Configuration.GetSection("validLaunch")["id"]!);
            this.validLaunchUuid = Configuration.GetSection("validLaunch")["uuid"]!;
            this.invalidLaunchId = Int32.Parse(Configuration.GetSection("invalidLaunch")["id"]!);
        }

        [Fact]
        public async Task GetSpecifiedLaunchByUUIDPositive()
        {
            HttpResponseMessage response = await client.GetAsync(projectName + "/launch/uuid/" + validLaunchUuid);
            string json = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode);
            Launch result = JsonConvert.DeserializeObject<Launch>(json);

            //Get uuid not deserializign all response <- Newtonsoft.Json

            Assert.Equal(result.uuid,validLaunchUuid);
        }

        [Fact]
        public async Task GetSpecifiedLaunchByIdNotFound()
        {
            HttpResponseMessage response = await client.GetAsync(projectName + "/launch/" + invalidLaunchId);
            string json = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateLaunchForSpecificProjectValidLaunchId()
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
            HttpResponseMessage response = await client.PutAsync(
            requestUri: projectName + "/launch/" + validLaunchId + "/update",
            content: new StringContent(json, Encoding.UTF8,"application/json"));
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            ApiResponse  result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Launch with ID = '{validLaunchId}' successfully updated.", result.message);
        }

        [Fact]
        public async Task UpdateLaunchForSpecificProjectInvalidProject()
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
            HttpResponseMessage response = await client.PutAsync(
            requestUri: invalidProjectName + "/launch/" + validLaunchId + "/update",
            content: new StringContent(json, Encoding.UTF8, "application/json"));
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Project '{invalidProjectName}' not found. Did you use correct project name?", result.message);

        }
        [Fact]
        public async Task UpdateLaunchForSpecificProjectInvalidLaunch()
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
            HttpResponseMessage response = await client.PutAsync(
            requestUri:  projectName + "/launch/" + invalidLaunchId + "/update",
            content: new StringContent(json, Encoding.UTF8, "application/json"));
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Launch '{invalidLaunchId}' not found. Did you use correct Launch ID?", result.message);

        }

        [Fact]
        public async Task StartLaunchAnalyzerOnDemandOk()
        {
            var payload = new
            {
                launchId = validLaunchId,
                analyzerMode = "ALL",
                analyzerTypeName = "autoAnalyzer",
                analyzeItemsMode = new string[] { "AUTO_ANALYZED" }

            };
            var json = JsonConvert.SerializeObject(payload);
            HttpResponseMessage response = await client.PostAsync( projectName + "/launch/analyze", new StringContent(json,Encoding.UTF8, "application/json"));
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"autoAnalyzer analysis for launch with ID='{validLaunchId}' started.", result.message);
        }

        [Fact]
        public async Task StartLaunchAnalyzerOnDemandForInvalidProject()
        {
            var payload = new
            {
                launchId = validLaunchId,
                analyzerMode = "ALL",
                analyzerTypeName = "autoAnalyzer",
                analyzeItemsMode = new string[] { "AUTO_ANALYZED" }

            };
            var json = JsonConvert.SerializeObject(payload);
            HttpResponseMessage response = await client.PostAsync( invalidProjectName + "/launch/analyze", new StringContent(json, Encoding.UTF8, "application/json"));
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Project '{invalidProjectName}' not found. Did you use correct project name?", result.message);
        }

        [Fact]
        public async Task StartLaunchAnalyzerOnDemandForInvalidLaunch()
        {
            var payload = new
            {
                launchId = invalidLaunchId,
                analyzerMode = "ALL",
                analyzerTypeName = "autoAnalyzer",
                analyzeItemsMode = new string[] { "AUTO_ANALYZED" }

            };
            var json = JsonConvert.SerializeObject(payload);
            HttpResponseMessage response = await client.PostAsync( projectName + "/launch/analyze", new StringContent(json, Encoding.UTF8, "application/json"));
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Launch '{invalidLaunchId}' not found. Did you use correct Launch ID?", result.message);
        }

        [Fact]
        public async Task DeleteLaunchNotValid()
        {

            HttpResponseMessage response = await client.DeleteAsync(projectName + "/launch/"+invalidLaunchId);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            Assert.Equal($"Launch '{invalidLaunchId}' not found. Did you use correct Launch ID?", result.message);
        }

    }
}