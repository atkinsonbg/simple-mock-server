using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api
{
    public interface IMocks
    {
        List<JsonDocument> GetCollection();
        MockResponse GetResponse(string url, string method);
    }

    public class MockResponse
    {
        public string Content { get; set; }
        public int StatusCode { get; set; }

        public JsonElement Headers { get; set; }
    }

    public class Mocks : IMocks
    {
        private readonly ILogger<Mocks> _logger;
        public List<JsonDocument> Collection { get; set; }

        public Mocks(ILogger<Mocks> logger)
        {
            _logger = logger;
            this.Collection = new List<JsonDocument>();
            this.BuildCollection();
        }

        public List<JsonDocument> GetCollection()
        {
            return this.Collection;
        }

        private void BuildCollection()
        {
            _logger.LogInformation("Populating mock routes...");
            string[] mockFiles = Directory.GetFiles("Mocks", "*.json");
            _logger.LogInformation("Number of mock files found: " + mockFiles.Count());
            foreach(var mockFile in mockFiles)
            {
                using (StreamReader file = new StreamReader(mockFile))
                {
                    string json = file.ReadToEnd();
                    var allMocks = System.Text.Json.JsonDocument.Parse(json);

                    foreach (JsonElement j in allMocks.RootElement.EnumerateArray())
                    {
                        this.Collection.Add(System.Text.Json.JsonDocument.Parse(j.ToString()));
                    }
                }
            }
            _logger.LogInformation("Finished populating mock routes...");
            _logger.LogInformation("Number of mocks loaded: " + this.Collection.Count());
        }

        public MockResponse GetResponse(string url, string method)
        {
            var response = new MockResponse();

            try
            {
                var r = this.GetCollection().Where(
                    u => u.RootElement.GetProperty("url").ToString() == url &&
                        u.RootElement.GetProperty("method").ToString() == method).FirstOrDefault();
                if (r != null)
                {
                    response.Content = r.RootElement.GetProperty("response").GetProperty("body").ToString();
                    response.Headers = r.RootElement.GetProperty("response").GetProperty("headers");

                    int status = 200;
                    r.RootElement.GetProperty("response").GetProperty("statuscode").TryGetInt32(out status);
                    response.StatusCode = status;

                    return response;
                }
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);

                response.StatusCode = 599;
                response.Content = "Route not matched and an error occurred.";
                return response;
            }

            response.StatusCode = 400;
            response.Content = "Route not matched in mocks loaded.";
            return response;
        }
    }
}