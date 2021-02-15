using CentrisApiHomeBrew.Managers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CentrisPropertyAPI : ControllerBase
    {
        private readonly ILogger<CentrisPropertyAPI> _logger;

        public CentrisPropertyAPI(ILogger<CentrisPropertyAPI> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<Property> Get()
        {
            CentrisPropertyApiManager cpApiM = new CentrisPropertyApiManager();

            return cpApiM.GetPropertyBaseOnJson();
        }
    }
}
