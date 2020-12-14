using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConnectLogic;
using ConnectAPI.Models;
using EnviziCoreLibrary;

namespace ConnectAPI
{
    public class WebAPI : IWebAPI
    {
        private WebDriver _webDriver = null;
        private bool _isInitialized = false;

        /// <summary>
        /// Returns true if api has logged into target portal
        /// </summary>
        /// <param name="reference">Reference specific to the account; if set, method will return only the account data associated to the reference</param>
        public bool IsLoggedIn
        {
            get
            {
                return (_webDriver != null && _isInitialized);
            }
        }

        /// <summary>
        /// Log into the target portal indicated by the url
        /// </summary>
        /// <param name="url">Url of target portal</param>
        /// <param name="username">Portal username</param>
        /// <param name="password">Portal password</param>
        public void Initialize(string url, string username, string password)
        {
            try
            {
                _webDriver = new WebDriver();
                _webDriver.Url = url;
                _webDriver.EnterText(UIFieldElement.UsernameText, username);
                _webDriver.EnterText(UIFieldElement.PasswordText, password);
                _webDriver.ClickObject(UIFieldElement.LoginButton);
                _isInitialized = true;
            }
            catch { }
        }

        /// <summary>
        /// Set current organization
        /// </summary>
        /// <param name="name">Url of target portal</param>
        public void SetOrganization(string name)
        {
            _webDriver.ClickMenuItem(UIMenuItemElement.SelectOrganizationGeneral, name);
        }

        /// <summary>
        /// Get list of groups associated to current organization
        /// </summary>
        public List<Models.GroupData> GetGroups()
        {
            List<GroupData> groups = new List<GroupData>();

            _webDriver.ClickObject(UIFieldElement.MainHomeMenuButton);
            _webDriver.ClickMenuItem(UIMenuItemElement.OrganizationGroups);

            // Scroll through the grid rows to get the required data
            List<List<string>> rowValues = _webDriver.GetGridRows(UIMenuItemElement.OrganizationGroups);
            foreach (List<string> rowValue in rowValues)
            {
                groups.Add(new GroupData()
                {
                    Name = rowValue[0],
                    Locations = string.IsNullOrEmpty(rowValue[1]) ? 0 : Convert.ToInt32(rowValue[1]),
                    Style = rowValue[2],
                    Description = rowValue[3],
                    BelongsTo = rowValue[4],
                    ReportPercent = string.IsNullOrEmpty(rowValue[5]) ? 0 : Convert.ToDecimal(rowValue[5]),
                    OpenToPublic = rowValue[5].Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false
                });
            }

            return groups;
        }

        /// <summary>
        /// Get list of locations associated to current organization
        /// </summary>
        public List<Models.LocationData> GetLocations()
        {
            List<LocationData> locations = new List<LocationData>();

            _webDriver.ClickObject(UIFieldElement.MainHomeMenuButton);
            _webDriver.ClickMenuItem(UIMenuItemElement.OrganizationLocations);

            // Scroll through the grid rows to get the required data
            List<List<string>> rowValues = _webDriver.GetGridRows(UIMenuItemElement.OrganizationLocations);
            foreach (List<string> rowValue in rowValues)
            {
                locations.Add(new LocationData()
                {
                    Name = rowValue[0],
                    Reference = rowValue[1],
                    Type = rowValue[2],
                    Region = rowValue[3],
                    RegionType = rowValue[4],
                    Id = rowValue[5],
                    RefNo = rowValue[6],
                    NoOfAccounts = string.IsNullOrEmpty(rowValue[7]) ? 0 : Convert.ToInt32(rowValue[7])
                });
            }

            return locations;
        }

        /// <summary>
        /// Get list of accounts associated to current organization
        /// </summary>
        /// <param name="accountReference">Reference to a specific account; if set, only the account associated to the reference will be returned</param>
        public List<AccountData> GetAccounts(string accountReference = null)
        {
            List<AccountData> accounts = new List<AccountData>();

            _webDriver.ClickObject(UIFieldElement.MainHomeMenuButton);
            _webDriver.ClickMenuItem(UIMenuItemElement.OrganizationAccounts);

            // if reference is set, input it into the filter text box to narrow down grid rows
            if (!string.IsNullOrEmpty(accountReference))
#if SANDBOX
                _webDriver.EnterText(UIFieldElement.AccountReferenceFilterText, accountReference);
#else
                _webDriver.EnterText(UIFieldElement.AccountNumberFilterText, accountReference);
#endif

            // Scroll through the grid rows to get the required data
            List<List<string>> rowValues = _webDriver.GetGridRows(UIMenuItemElement.OrganizationAccounts);
            foreach (List<string> rowValue in rowValues)
            {
                accounts.Add(new AccountData()
                {
                    Location = rowValue[0],
                    AccountNumber = rowValue[1],
                    Reference = rowValue[2],
                    Supplier = rowValue[3],
                    Reader = rowValue[4],
                    DataType = rowValue[5],
                    AccountStyle = rowValue[6],
                    SubType = rowValue[7]
                });
            }

            return accounts;
        }

        /// <summary>
        /// Get list of meters associated to current organization
        /// </summary>
        public List<MeterData> GetMeters()
        {
            List<MeterData> meters = new List<MeterData>();

            _webDriver.ClickObject(UIFieldElement.MainHomeMenuButton);
            _webDriver.ClickMenuItem(UIMenuItemElement.OrganizationMeters);

            // Scroll through the grid rows to get the required data
            List<List<string>> rowValues = _webDriver.GetGridRows(UIMenuItemElement.OrganizationMeters);
            foreach (List<string> rowValue in rowValues)
            {
                meters.Add(new MeterData()
                {
                    Location = rowValue[0],
                    SerialNo = rowValue[1],
                    Name = rowValue[2],
                    ServicePoint = rowValue[3],
                    DataType = rowValue[4],
                    Component = rowValue[5],
                    Type = rowValue[6],
                    Supplier =  rowValue[7]
                });
            }

            return meters;
        }

        /// <summary>
        /// Upload invoice capture data into the portal
        /// </summary>
        /// <param name="captureData">Invoice data to be uploaded</param>
        public string UploadAccountData(CaptureData captureData)
        {
            string result = "Not Found";

            _webDriver.ClickObject(UIFieldElement.MainHomeMenuButton);
            _webDriver.ClickMenuItem(UIMenuItemElement.OrganizationAccounts, null, 3000);

            if (!string.IsNullOrEmpty(captureData.AccountNumber))
            {
                // Filter down to specific account
#if SANDBOX
                _webDriver.EnterText(UIFieldElement.AccountReferenceFilterText, captureData.AccountNumber);
#else
                _webDriver.EnterText(UIFieldElement.AccountNumberFilterText, captureData.AccountNumber);
#endif
                
                _webDriver.ClickObject(UIFieldElement.ScrollBarHorizontalRightButton);
                if (_webDriver.ClickObject(UIFieldElement.AccountNumberGridLink, 2000))
                {
                    // Ensure that the account number of the invoice data to be uploaded matches the selected account in the portal
                    if (captureData.AccountNumber.Equals(_webDriver.GetText(UIFieldElement.AccountNumberPageLabel)))
                    {
                        _webDriver.ClickObject(UIFieldElement.ActionsButton);
                        if (_webDriver.ClickObject(UIFieldElement.CaptureDataButton))
                        {
                            _webDriver.EnterCaptureData(UIFieldElement.CaptureDateStartPeriodText, captureData.StartPeriod.ToString("d"));
                            _webDriver.EnterCaptureData(UIFieldElement.CaptureDataEndPeriodText, captureData.EndPeriod.ToString("d"));
                            _webDriver.EnterCaptureData(UIFieldElement.CaptureDataReferenceText, captureData.Reference);
                            _webDriver.EnterCaptureData(UIFieldElement.CaptureDataTotalUnitskWhText, captureData.TotalConsumption.ToString());
                            _webDriver.EnterCaptureData(UIFieldElement.CaptureDataTotalCostText, captureData.TotalCost.ToString());
                            if (!string.IsNullOrEmpty(captureData.InvoiceFile))
                                _webDriver.EnterCaptureData(UIFieldElement.CaptureDataInvoiceFileText, captureData.InvoiceFile);

                            Thread.Sleep(500);

                            // If clicking the Save button was successful, clear the result message then wait before returning to allow the page to refresh
                            if (_webDriver.ClickObject(UIFieldElement.CaptureDataSaveButton))
                            {
                                result = string.Empty;
                                Thread.Sleep(500);
                            }
                            else
                                result = "Capture Save Fail";
                        }
                        else
                            result = "Capture Input Fail";
                    }
                    else
                        result = "Account Select Fail";
                }
            }

            return result;
        }

        /// <summary>
        /// Log out from the portal
        /// </summary>
        public void Close()
        {
            if (IsLoggedIn)
            {
                _webDriver.ClickMenuItem(UIMenuItemElement.PersonalLogoutItem);
                _webDriver.Close();
            }
        }        
    }
}
