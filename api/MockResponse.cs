using System.Collections.Generic;

namespace api
{
    public class MockResponse
    {
        public string Content { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}