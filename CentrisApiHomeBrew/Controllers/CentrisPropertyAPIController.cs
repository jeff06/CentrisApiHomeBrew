using CentrisApiHomeBrew.Managers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        [Route("Listing")]
        public List<Property> GetListing()
        {
            CentrisPropertyApiManager cpApiM = new CentrisPropertyApiManager();

            return cpApiM.GetPropertyBaseOnJson();
        }

        [HttpGet]
        [Route("AutomaticEmail")]
        public string AutomaticEmail()
        {
            CentrisPropertyApiManager cpApiM = new CentrisPropertyApiManager();
            int nbOfEmailSent = cpApiM.AutomaticEmail();
            if (nbOfEmailSent > 0)
            {
                return $"{nbOfEmailSent} propertys were sent at {ConfigurationManager.AppSettings.Get("email")}";
            }
            else
            {
                return "No new property were posted";
            }
        }
    }
}
