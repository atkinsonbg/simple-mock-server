using System;
using Xunit;
using api;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text.Json;
using api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace test
{   
    public class ControllerTests
    {
        private IMocks _mocks;

        public ControllerTests(IMocks mocks)
        {
            _mocks = mocks;
        }

        [Fact]
        public void TestControllerConfigRoute()
        {
            var controller = new DefaultController(null, _mocks);
            var result = controller.Config();
            var viewResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void TestControllerAllRoute()
        {
            var controller = new DefaultController(null, _mocks);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Method = "GET";
            controller.ControllerContext.HttpContext.Request.Path = "/mockservertest/mock1";
            var result = controller.HandleAsync();
            var viewResult = Assert.IsType<ContentResult>(result.Result);
        }
    }
}