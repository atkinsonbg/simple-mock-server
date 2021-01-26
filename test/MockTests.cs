using System;
using Xunit;
using api;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text.Json;

namespace test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMocks, Mocks>();
        }
    }
    
    public class MockTests
    {
        private IMocks _mocks;

        public MockTests(IMocks mocks)
        {
            _mocks = mocks;
        }

        [Fact]
        public void TestBuildCollection()
        {
            var c = _mocks.GetCollection();
            Assert.True(c.Count > 0);
        }

        [Fact]
        public void TestMocksAreMounted()
        {
            var c = _mocks.GetCollection();
            var m = c.FirstOrDefault();
            Assert.True(m.RootElement.GetProperty("url").ToString() == "/mockservertest/mock1");
        }

        [Fact]
        public void TestMockFoundByRouteStatusCode()
        {
            var m = _mocks.GetResponse("/mockservertest/mock1", "POST", null, null);
            Assert.True(m.StatusCode == 200);
        }

        [Fact]
        public void TestMockFoundByRouteContent()
        {
            var m = _mocks.GetResponse("/mockservertest/mock1", "GET", null, null);
            var json = m.Content.Replace(" ","").Replace("\n", "");
            Assert.True(json == "{\"method\":\"GET\",\"route\":\"mock2\"}");
        }

        [Fact]
        public void TestMockRouteNotFound()
        {
            var m = _mocks.GetResponse("/mockservertest/mock999999999", "GET", null, null);
            Assert.True(m.StatusCode == 499);
        }

        [Fact]
        public void TestMockFoundByRouteRequestContent()
        {
            var requestBody = "{\"field1\":\"Hello\",\"field2\":{\"field3\":\"World\"}}";
            var requestHeaders = "{}";
            var m = _mocks.GetResponse("/mockservertest/mock4", "POST", requestBody, requestHeaders);
            var json = m.Content.Replace(" ","").Replace("\n", "");
            Assert.True(json == "{\"method\":\"POST\",\"route\":\"mock3\",\"body-level\":\"two-layer\"}");
        }

        [Fact]
        public void TestMockFoundByRouteRequestHeadersContent()
        {
            var requestBody = "{\"field1\":\"Hello\",\"field2\":\"World\"}";
            var requestHeaders = "{\"accept\":\"application/json\",\"custom\":\"headertype2\",\"content-type\":\"application/json\"}";
            var m = _mocks.GetResponse("/mockservertest/mock4", "POST", requestBody, requestHeaders);
            var json = m.Content.Replace(" ","").Replace("\n", "");
            Assert.True(json == "{\"method\":\"POST\",\"route\":\"mock4-2\",\"body-level\":\"one-layer\"}");
        }

        [Fact]
        public void TestMockFoundByRouteRequestBadContent()
        {
            var m = _mocks.GetResponse("/mockservertest/mock4", "POST", "{\"field1\": \"Hello\",\"field2\":\"World\"", null);
            Assert.True(m.StatusCode == 599);
        }
    }
}