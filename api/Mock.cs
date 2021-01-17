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
        ContentResult GetResponse(string url);
    }

    public class Mocks : IMocks
    {
        private readonly ILogger<Mocks> _logger;
        public List<JsonDocument> Collection { get; set; }

        public Mocks(ILogger<Mocks> logger)
        {
            _logger = logger;

            Collection = new List<JsonDocument>();

            logger.LogInformation("Populating mock routes...");
            string[] mocks = Directory.GetFiles("Mocks", "*.json");
            Console.WriteLine("Number of mock files found: " + mocks.Count());
            foreach(var mock in mocks)
            {
                using (StreamReader r = new StreamReader(mock))
                {
                    string json = r.ReadToEnd();
                    this.Collection.Add(System.Text.Json.JsonDocument.Parse(json));
                }
            }
            logger.LogInformation("Finished populating mock routes...");
        }

        public List<JsonDocument> GetCollection()
        {
            return this.Collection;
        }

        public ContentResult GetResponse(string url)
        {
            var response = new ContentResult();

            var r = this.GetCollection().Where(x => x.RootElement.GetProperty("url").ToString() == url).FirstOrDefault();
            if (r != null)
            {
                int status = 500;
                response.Content = r.RootElement.GetProperty("response").GetProperty("body").ToString();
                r.RootElement.GetProperty("response").GetProperty("statuscode").TryGetInt32(out status);
                response.StatusCode = status;
                response.ContentType = "application/json";
                return response;
            }
            
            response.StatusCode = 400;
            response.Content = "Route not matched in mocks loaded.";
            return response;
        }
    }
}