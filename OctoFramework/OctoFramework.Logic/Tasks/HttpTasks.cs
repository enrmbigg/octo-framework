using System;
using System.Configuration;
using System.Net.Http;
using Umbraco.Core.Logging;

namespace OctoFramework.Logic.Tasks
{
    public static class HttpTasks
    {
        public static void RequestUrl(string url)
        {
            var baseUrl = ConfigurationManager.AppSettings["TaskRunnerBaseURL"].ToString();
            try
            {
                using (var client = new HttpClient(new HttpClientHandler()))
                {
                    client.BaseAddress = new Uri(baseUrl);
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Result: " + result);
                    LogHelper.Info<HttpClient>("Task:" + url + " Successful?:" + response.IsSuccessStatusCode);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info<HttpClient>("Task:" + url + " Successful?:Failed Reason:" + ex);
            }
        }
    }
}