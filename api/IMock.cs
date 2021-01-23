using System.Collections.Generic;
using System.Text.Json;

namespace api
{
public interface IMocks
    {
        List<JsonDocument> GetCollection();
        MockResponse GetResponse(string url, string method, string requestBody, string requestHeaders);
    }
}