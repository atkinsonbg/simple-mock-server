using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;
        private readonly IMocks _mocks;

        public DefaultController(ILogger<DefaultController> logger, IMocks mocks)
        {
            _logger = logger;
            _mocks = mocks;
        }

        [HttpDelete]
        [HttpGet]
        [HttpHead]
        [HttpOptions]
        [HttpPatch]
        [HttpPost]
        [HttpPut]
        [Route("/{*.}")]
        public async Task<ActionResult> HandleAsync()
        {
            string requestBody;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            var r = _mocks.GetResponse(Request.Path + Request.QueryString.ToString(), Request.Method, requestBody);
            
            foreach (var h in r.Headers)
            {
                Response.Headers.Add(h.Key, h.Value);
            }

            var response = new ContentResult();
            response.Content = r.Content;
            response.StatusCode = r.StatusCode;

            return response;
        }

        [HttpGet]
        [Route("/mockserverconfig")]
        public ActionResult Config()
        {
            return Ok(_mocks.GetCollection());
        }
    }
}
