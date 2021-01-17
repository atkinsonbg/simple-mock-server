using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        public ActionResult Handle()
        {
            var r = _mocks.GetResponse(Request.Path);
            return r;
        }

        [HttpGet]
        [Route("/mockserverconfig")]
        public ActionResult Config()
        {
            return Ok(_mocks.GetCollection());
        }
    }
}
