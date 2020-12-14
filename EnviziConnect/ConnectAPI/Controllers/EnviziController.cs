using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using EnviziCoreLibrary;
using ConnectAPI.Models;

namespace ConnectAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public partial class EnviziController : ControllerBase
    {
        private readonly IWebAPI _webAPI = null;

        public EnviziController(IWebAPI webApi)
        {
            _webAPI = webApi;
        }

        /// <summary>
        /// Get method that serves as the starting point
        /// </summary>
        [HttpGet]
        public IActionResult Start()
        {
            return Ok("Envizi Connector is running!");
        }

        /// <summary>
        /// Post method for logging into Envizi
        /// </summary>
        /// <param name="data">Json data containing the url, username, and password to Envizi</param>
        [HttpPost]
        public IActionResult Initialise([FromBody] Object data)
        {
            IActionResult response = NotFound();

            LoginDetails loginDetails = JsonConvert.DeserializeObject<LoginDetails>(data.ToString());
            _webAPI.Initialize(loginDetails.Url, loginDetails.Username, loginDetails.Password);

            if (_webAPI.IsLoggedIn)
                response = Ok();

            return response;
        }

        /// <summary>
        /// Put method for selecting the organization in Envizi
        /// </summary>
        /// <param name="data">Json data containing the organization name to be selected</param>
        [HttpPut]
        public IActionResult Organizations([FromBody] Object data)
        {
            IActionResult response = NotFound();

            OrganizationData organizationData = JsonConvert.DeserializeObject<OrganizationData>(data.ToString());
            if (_webAPI.IsLoggedIn)
            {
                _webAPI.SetOrganization(organizationData.Name);
                response = Ok();
            }

            return response;
        }

        /// <summary>
        /// Post method for uploading invoice capture data into Envizi
        /// </summary>
        /// <param name="data">Json data containing the invoice data (account number, start and end periods, reference info, total consumption, total cost, and invoice file)</param>
        [HttpPost]
        public IActionResult AccountData([FromBody] Object data)
        {
            IActionResult response = NotFound();
            try
            {
                if (_webAPI.IsLoggedIn)
                {
                    CaptureData captureData = JsonConvert.DeserializeObject<CaptureData>(data.ToString());
                    string uploadResult = _webAPI.UploadAccountData(captureData);
                    if (!string.IsNullOrEmpty(uploadResult))
                    {
                        if (!uploadResult.Equals("Not Found"))
                            response = new ObjectResult(uploadResult) { StatusCode = (int)HttpStatusCode.InternalServerError };
                    }
                    else
                        response = Ok(captureData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }

            return response;
        }

        /// <summary>
        /// Get method for retrieving the list of groups for the selected organization
        /// </summary>
        [HttpGet]
        public IActionResult Groups()
        {
            IActionResult response = NotFound();
            if (_webAPI.IsLoggedIn)
            {
                IList<GroupData> data = _webAPI.GetGroups();
                if (data.Count() > 0)
                    response = Ok(data);
            }

            return response;
        }

        /// <summary>
        /// Get method for retrieving the list of locations for the selected organization
        /// </summary>
        [HttpGet]
        public IActionResult Locations()
        {
            IActionResult response = NotFound();
            if (_webAPI.IsLoggedIn)
            {
                IList<LocationData> data = _webAPI.GetLocations();
                if (data.Count() > 0)
                    response = Ok(data);
            }

            return response;
        }

        /// <summary>
        /// Get method for retrieving the list of accounts for the selected organization
        /// </summary>
        /// <param name="reference">Reference specific to an account; if set, method will return only the account data associated to the reference</param>
        [HttpGet("{reference?}")]
        public IActionResult Accounts(string reference = null)
        {
            IActionResult response = NotFound();
            if (_webAPI.IsLoggedIn)
            {
                IList<AccountData> data = _webAPI.GetAccounts(reference);
                if (data.Count() > 0)
                    response = Ok(data);
            }

            return response;
        }

        /// <summary>
        /// Get method for retrieving the list of meters for the selected organization
        /// </summary>
        [HttpGet]
        public IActionResult Meters()
        {
            IActionResult response = NotFound();
            if (_webAPI.IsLoggedIn)
            {
                IList<MeterData> data = _webAPI.GetMeters();
                if (data.Count() > 0)
                    response = Ok(data);
            }

            return response;
        }

        /// <summary>
        /// Post method to trigger logout from Envizi
        /// </summary>
        [HttpPost]
        public void Finish()
        {
            _webAPI.Close();
        }
    }
}
