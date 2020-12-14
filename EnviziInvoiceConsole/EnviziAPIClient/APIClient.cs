using System;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EnviziCoreLibrary;
using Newtonsoft.Json;

namespace EnviziAPIClient
{
    public class APIClient
    {
        HttpClient _httpClient = new HttpClient();
        InvoiceBillData _invoiceBillData = null;
        string _localPDFFile = string.Empty;
        bool _isConnected = false;

        public string ConnectorURL { get; set; }
        public string ThirdPartyURL { get; set; }
        public string ThirdPartyUsername { get; set; }
        public string ThirdPartyPassword { get; set; }

        public APIClient()
        {
            
        }

        /// <summary>
        /// Set the current invoice
        /// </summary>
        /// <param name="invoiceBill">Invoicebill data</param>
        public void SetInvoice(InvoiceBillData invoiceBill)
        {
            _invoiceBillData = invoiceBill;
            _localPDFFile = string.Empty;
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void Close()
        {
            Disconnect();
        }

        /// <summary>
        /// Upload the invoice data
        /// </summary>
        public string UploadInvoiceData()
        {
            if (_invoiceBillData == null)
                return string.Empty;

            string result = string.Empty;
            if (!_isConnected)
                _isConnected = Connect();

            if (_isConnected)
                result = SendCaptureData();

            return result;
        }

        /// <summary>
        /// Download the invoice PDF file from the web link
        /// </summary>
        private void GetPDF()
        {
            if (string.IsNullOrEmpty(_invoiceBillData.ScanFile))
                return;

            _localPDFFile = Path.GetTempPath() + _invoiceBillData.GetTempFileName();
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(_invoiceBillData.ScanFile, _localPDFFile);
            }
            catch { }
        }

        /// <summary>
        /// Connect to the Envizi connector
        /// </summary>
        private bool Connect()
        {
            _httpClient.BaseAddress = new Uri(ConnectorURL);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            LoginDetails loginDetails = new LoginDetails();
            loginDetails.Url = ThirdPartyURL;
            loginDetails.Username = ThirdPartyUsername;
            loginDetails.Password = ThirdPartyPassword;

            var postTask = _httpClient.PostAsJsonAsync("initialise", loginDetails);
            postTask.Wait();
            return postTask.Result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Retrieve the account number associated with the given reference
        /// </summary>
        private string GetAccountNumber(string reference)
        {
            string result = string.Empty;

            var getTask = _httpClient.GetAsync("accounts\\" + reference);
            getTask.Wait();
            if (getTask.Result.IsSuccessStatusCode)
            {
                var readTask = getTask.Result.Content.ReadAsAsync<AccountData[]>();
                readTask.Wait();
                var accounts = readTask.Result;
                if (accounts.Length > 0)
                {
                    AccountData accountData = accounts[0];
                    result = accountData.AccountNumber;
                }
            }

            return result;
        }

        /// <summary>
        /// Send the invoice data to the Envizi connector for uploading
        /// </summary>
        private string SendCaptureData()
        {
            if (string.IsNullOrEmpty(_localPDFFile))
                GetPDF();

            string result = "Not Found";
            CaptureData captureData = new CaptureData();
            captureData.AccountNumber = _invoiceBillData.NMI;
            //captureData.AccountNumber = GetAccountNumber(_invoiceBillData.AccountReference); // GetAccountNumber to be used in actual Charter Hall portal
            if (!string.IsNullOrEmpty(captureData.AccountNumber))
            {
                captureData.StartPeriod = _invoiceBillData.StartDate;
                captureData.EndPeriod = _invoiceBillData.EndDate;
                captureData.TotalConsumption = _invoiceBillData.TotalConsumption;
                captureData.TotalCost = _invoiceBillData.TotalCost;
                captureData.Reference = _invoiceBillData.GetReferenceText();
                captureData.InvoiceFile = _localPDFFile;

                var postTask = _httpClient.PostAsJsonAsync("accountdata", captureData);
                postTask.Wait();
                if (!postTask.Result.IsSuccessStatusCode)
                {
                    try
                    {
                        result = (string)JsonConvert.DeserializeObject<dynamic>(postTask.Result.Content.ReadAsStringAsync().Result);
                    }
                    catch { }
                }
                else
                    result = "Success";
            }

            return result;
        }

        /// <summary>
        /// Disconnect from the Envizi connector
        /// </summary>
        private void Disconnect()
        {
            if (_isConnected)
            {
                var postTask = _httpClient.PostAsJsonAsync("finish", new LoginDetails());
                postTask.Wait();
            }
        }
    }
}
