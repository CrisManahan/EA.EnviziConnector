using System;
using Xunit;
using EnviziCoreLibrary;
using CRMDBConnector;
using EnviziAPIClient;

namespace UnitTests
{
    public class UnitTest1
    {
        [Fact(DisplayName = "Test Fetching of Bill Data for upload")]
        public void TestGetBillDataFromCRM()
        {
            // 1. Arrange
            string connectionString = "Data Source=10.101.1.6;Initial Catalog=EABTP_PROD;Integrated Security=True;Pooling=False;";
            string crmDBName = "EABTP_UAT";
            string customerName = "Charter Hall";
            DBConnector dbConnector = new DBConnector(connectionString);
            dbConnector.CRMDatabaseName = crmDBName;
            dbConnector.CustomerName = customerName;
            dbConnector.TargetConnector = "Envizi";
            dbConnector.MonthID = DateTime.Now.AddMonths(-1).ToString("yyyyMM");

            // 2. Act
            dbConnector.FetchNewBillData();
            var invoiceBills = dbConnector.FetchBillDataForUpload();

            // 3. Assert
            Assert.NotEmpty(invoiceBills);
        }
    }
}
