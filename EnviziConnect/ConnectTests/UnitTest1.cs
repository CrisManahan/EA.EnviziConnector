using System;
using Xunit;
using EnviziCoreLibrary;
using ConnectAPI;

namespace ConnectTests
{
    public class UnitTest1
    {
        [Theory(DisplayName = "Test Login and Get Account")]
        [InlineData("00000")]
        [InlineData("1234567890")]        
        public void TestGetAccount(string accountNumber)
        {
            // 1. Arrange
            string url = "https://au001.envizi.com/home/default.aspx";
            string userName = "bruce.macfarlane@energyaction.envizi.com";
            string passWord = "Enviz1Pa33w0rd!#";
            WebAPI webApi = new WebAPI();

            // 2. Act
            webApi.Initialize(url, userName, passWord);
            var result = webApi.GetAccounts(accountNumber);

            // 3. Assert
            Assert.True(webApi.IsLoggedIn);
            if (accountNumber.Length == 10)
            {
                Assert.NotEmpty(result);
                Assert.Single(result);
                Assert.Equal(accountNumber, result[0].AccountNumber);
            }
            else
                Assert.Empty(result);

            webApi.Close();
        }

        [Theory(DisplayName = "Test Upload Account Data")]
        [InlineData("00000")]
        [InlineData("1234567890")]        
        public void TestPostAccount(string accountNumber)
        {
            // 1. Arrange
            string url = "https://au001.envizi.com/home/default.aspx";
            string userName = "bruce.macfarlane@energyaction.envizi.com";
            string passWord = "Enviz1Pa33w0rd!#";
            CaptureData captureData = new CaptureData();
            captureData.AccountNumber = accountNumber;
            captureData.StartPeriod = Convert.ToDateTime(string.Format("{0}-{1}-{2}", DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month.ToString("00"), "01"));
            captureData.EndPeriod = Convert.ToDateTime(string.Format("{0}-{1}-{2}", DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month.ToString("00"), 
                                                                                    DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month)));
            captureData.TotalConsumption = 0;
            captureData.TotalCost = 0;
            WebAPI webApi = new WebAPI();

            // 2. Act
            webApi.Initialize(url, userName, passWord);
            var result = webApi.UploadAccountData(captureData);

            // 3. Assert
            Assert.True(webApi.IsLoggedIn);
            if (accountNumber.Length == 10)
                Assert.True(string.IsNullOrEmpty(result));
            else
                Assert.False(string.IsNullOrEmpty(result));

            webApi.Close();
        }
    }
}
