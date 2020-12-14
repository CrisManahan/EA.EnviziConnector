using System;
using EnviziCoreLibrary;
using CRMDBConnector;
using EnviziAPIClient;
using System.Collections.Generic;
using System.Configuration;

namespace EnviziInvoiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool correctSyntax = false;
            if (args.Length >= 1)
            {
                string customerName = args[0];
                string monthID = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                if (args.Length >= 2)
                    monthID = args[1];

                int testMID;
                if (monthID.Length == 6 && Int32.TryParse(monthID, out testMID))
                {
                    correctSyntax = true;

                    Console.WriteLine("\nInvoice Uploader : Envizi");
                    Console.WriteLine("Please wait...\n");

                    string resultCount = UploadInvoices(customerName, monthID);
                    if (resultCount == "0 of 0")
                        Console.WriteLine(string.Format("No invoices found for \"{0}\" on {1}! Try changing the month id.", customerName, monthID));
                    else
                        Console.WriteLine("Successfully uploaded invoices: " + resultCount);
                }
            }
            
            if (!correctSyntax)
                Console.WriteLine("\nIncorrect syntax!\nExpected: " + System.AppDomain.CurrentDomain.FriendlyName + " [\"Customer Name\"] [YearMonth ID (yyyyMM)]");

            Console.WriteLine("\nGoodbye!");
        }

        static string UploadInvoices(string customerName, string monthID)
        {
            int successCount = 0;
            int totalCount = 0;
            
            APIClient apiClient = new APIClient();
            apiClient.ConnectorURL = ConfigurationManager.AppSettings.Get("ConnectorURL");
            apiClient.ThirdPartyURL = ConfigurationManager.AppSettings.Get("ThirdPartyURL");
            apiClient.ThirdPartyUsername = ConfigurationManager.AppSettings.Get("ThirdPartyUsername");
            apiClient.ThirdPartyPassword = ConfigurationManager.AppSettings.Get("ThirdPartyPassword");

            DBConnector dbConnector = new DBConnector(ConfigurationManager.ConnectionStrings["CRMDBConnection"].ConnectionString);
            dbConnector.CRMDatabaseName = ConfigurationManager.AppSettings.Get("CRMDatabaseName");

            dbConnector.CustomerName = customerName;
            dbConnector.TargetConnector = "Envizi";
            dbConnector.MonthID = monthID;

            dbConnector.FetchNewBillData();
            List<InvoiceBillData> invoiceBills = new List<InvoiceBillData>(dbConnector.FetchBillDataForUpload());
            totalCount = invoiceBills.Count;
            foreach (InvoiceBillData invoiceBill in invoiceBills)
            {
                apiClient.SetInvoice(invoiceBill);
                string uploadResult = apiClient.UploadInvoiceData();
                dbConnector.SaveUploadResult(invoiceBill, uploadResult);
                if (uploadResult.Equals("Success", StringComparison.OrdinalIgnoreCase))
                    ++successCount;
            }
            apiClient.Close();

            return string.Format("{0} of {1}", successCount, totalCount);
        }        
    }
}
