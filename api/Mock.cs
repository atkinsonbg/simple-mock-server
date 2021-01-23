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
    public class Mocks : IMocks
    {
        private readonly ILogger<Mocks> _logger;
        public List<JsonDocument> Collection { get; set; }
        private JsonDocument Mock { get; set; }

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
            string[] mockFiles = Directory.GetFiles("Mocks", "*.json", SearchOption.AllDirectories);
            _logger.LogInformation("Number of mock files found: " + mockFiles.Count());
            foreach(var mockFile in mockFiles)
            {
                using (StreamReader file = new StreamReader(mockFile))
                {
                    string json = file.ReadToEnd();
                    var allMocks = JsonDocument.Parse(json);

                    foreach (JsonElement j in allMocks.RootElement.EnumerateArray())
                    {
                        this.Collection.Add(JsonDocument.Parse(j.ToString()));
                    }
                }
            }
            _logger.LogInformation("Finished populating mock routes...");
            _logger.LogInformation("Number of mocks loaded: " + this.Collection.Count());
        }

        public MockResponse GetResponse(string url, string method, string requestBody, string requestHeaders)
        {
            this.Mock = null;

            var response = new MockResponse();
            response.Headers = new Dictionary<string, string>();

            try
            {
                // Initial match against URL and METHOD only
                var query = this.GetCollection().Where(
                    u => u.RootElement.GetProperty("url").ToString() == url &&
                        u.RootElement.GetProperty("method").ToString() == method);
                
                // If more than one Mock is found, match against REQUEST BODY and then HEADERS
                if (query != null && query.Count() > 1)
                {
                    var subQuery = this.MatchRequestBody(query, requestBody);
                    if (subQuery.Count() > 1)
                    {
                        //this.Mock = query.FirstOrDefault();
                        subQuery = this.MatchHeaders(subQuery, requestHeaders);
                        if (subQuery.Count() > 1)
                        {
                            response.StatusCode = 400;
                            response.Content = "Route matched more than one mocks loaded.";
                            return response;
                        }
                        else
                        {
                            this.Mock = subQuery.FirstOrDefault();
                        }
                    }
                    else
                    {
                        this.Mock = subQuery.FirstOrDefault();
                    }
                }
                else
                {
                    this.Mock = query.FirstOrDefault();
                }

                if (this.Mock != null)
                {
                    response.Content = this.Mock.RootElement.GetProperty("response").GetProperty("body").ToString();

                    var headers = this.Mock.RootElement.GetProperty("response").GetProperty("headers");
                    foreach (var header in headers.EnumerateArray())
                    {
                        var h = header.EnumerateObject();
                        response.Headers.Add(h.FirstOrDefault().Name, h.FirstOrDefault().Value.ToString());
                    }

                    int status = 200;
                    this.Mock.RootElement.GetProperty("response").GetProperty("statuscode").TryGetInt32(out status);
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

        private IEnumerable<JsonDocument> MatchRequestBody(IEnumerable<JsonDocument> query, string json)
        {
            var returnElements = new List<JsonDocument>();
            var comparer = new JsonElementComparer();
            foreach (var mock in query)
            {
                var jsonToTest = JsonDocument.Parse(mock.RootElement.GetProperty("request").GetProperty("body").ToString());
                var incomingJson = JsonDocument.Parse(json);
                if (comparer.Equals(jsonToTest.RootElement, incomingJson.RootElement))
                {
                    returnElements.Add(mock);
                }
            }
            
            return returnElements.AsEnumerable();
        }

        private IEnumerable<JsonDocument> MatchHeaders(IEnumerable<JsonDocument> query, string json)
        {
            Console.WriteLine("MATHCING HEADRES");
            var returnElements = new List<JsonDocument>();
            var comparer = new JsonElementComparer();
            foreach (var mock in query)
            {
                var jsonToTest = JsonDocument.Parse(mock.RootElement.GetProperty("request").GetProperty("headers").ToString());
                var incomingJson = JsonDocument.Parse(json);
                if (comparer.Equals(jsonToTest.RootElement, incomingJson.RootElement))
                {
                    returnElements.Add(mock);
                }
            }
            
            return returnElements.AsEnumerable();
        }
    }
}