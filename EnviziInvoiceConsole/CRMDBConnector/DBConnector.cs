using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using EnviziCoreLibrary;

namespace CRMDBConnector
{
    public class DBConnector
    {
        // Query to fetch new bill data into the upload table
        private const string QueryNewBillData = 
                                "INSERT INTO [{0}].[dbo].[ThirdPartyBillDataUpload] " +
                                "(Target, MonthID, BillNMI, BillAccountReference, BillStartDate, BillEndDate, " +
                                "BillTotalConsumption, BillTotalCost, BillScanFile, Supplier, CustomerName, UploadResult, DateFirstUpload, DateRetryUpload) " +
                                "SELECT '{2}' AS Target, {3} AS MonthID, SPI.btp_NMIName, " +
                                "(SELECT TOP 1 AccountNumber FROM [urjanet].[dbo].[UrjanetEnactIntegrationStaging] WHERE SPID = SPI.btp_supplypointInfoname ORDER BY MthID DESC) AS AccountReference, " +
                                "BVS.btp_billsmystartdate, BVS.btp_billsmyenddate, BVS.btp_billsmyconsactual, BVS.btp_billsmytotalchg, BVS.btp_billsmyscanfile, SPI.btp_retailerName, " +
                                "(SELECT CUName FROM [Engauge].[dbo].[BDG_Customers] WHERE CUID IN (SELECT CUID FROM [Engauge].[dbo].[BDG_Cusps] WHERE SPID = SPI.btp_supplypointInfoname)) AS CustomerName, " +
                                "'New' AS Result, NULL AS DateFirstUpload, NULL AS DateRetryUpload " +
                                "FROM [{0}].[CRM].[BillValidationSummary] BVS INNER JOIN [{0}].[CRM].[SupplyPointInfo] SPI ON BVS.btp_spid = SPI.btp_supplypoint " +
                                "WHERE SPI.btp_supplypointInfoname IN (SELECT SPID FROM [Engauge].[dbo].[BDG_Cusps] WHERE CUID IN (SELECT CUID FROM [Engauge].[dbo].[BDG_Customers] WHERE CUName LIKE '{1}%')) " +
                                "AND BVS.statuscodename = 'Active' AND BVS.btp_billsmyconsactual IS NOT NULL AND BVS.btp_billsmytotalchg IS NOT NULL AND BVS.btp_mthid = {3} " +
                                "AND NOT EXISTS (SELECT Target, MonthID, BillNMI FROM [EABTP_UAT].[dbo].[ThirdPartyBillDataUpload] WHERE Target = '{2}' AND MonthID = {3} " +
                                "AND BillNMI = SPI.btp_NMIName AND BillTotalConsumption = BVS.btp_billsmyconsactual AND BillTotalCost = BVS.btp_billsmytotalchg AND Supplier = SPI.btp_retailerName);";

        // Query to fetch bill data for pushing to the connector
        private const string QueryPushBillData =
                                "SELECT BillNMI, BillAccountReference, BillStartDate, BillEndDate, BillTotalConsumption, BillTotalCost, BillScanFile, Supplier, CustomerName " +
                                "FROM [{0}].[dbo].[ThirdPartyBillDataUpload] WHERE UploadResult != 'Success' AND CustomerName LIKE '{1}%' AND Target = '{2}' AND MonthID = {3};";

        // Query to update results after the push process
        private const string QueryUpdatePushResult =
                                "UPDATE [{0}].[dbo].[ThirdPartyBillDataUpload] SET UploadResult = '{8}', " +
                                "DateFirstUpload = CASE WHEN UploadResult = 'New' THEN GETDATE() ELSE DateFirstUpload END, " +
                                "DateRetryUpload = CASE WHEN UploadResult = 'New' THEN DateRetryUpload ELSE GETDATE() END " +
                                "WHERE CustomerName LIKE '{1}%' AND Target = '{2}' AND MonthID = {3} AND BillNMI = '{4}' AND BillAccountReference = '{5}' " +
                                "AND BillTotalConsumption = {6} AND BillTotalCost = {7};";

        public string CRMDatabaseName { get; set; }
        public string CustomerName { get; set; }
        public string TargetConnector { get; set; }
        public string MonthID { get; set; }

        private string _connectionString = string.Empty;

        public DBConnector(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Fetch all new invoice data and save into the upload table
        /// </summary>
        public void FetchNewBillData()
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(QueryNewBillData, CRMDatabaseName, CustomerName, TargetConnector, MonthID), sqlConnection))
                {
                    command.ExecuteNonQuery();
                }
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Fetch all invoice bill data for uploading
        /// </summary>
        public InvoiceBillData[] FetchBillDataForUpload()
        {
            List<InvoiceBillData> invoiceBills = new List<InvoiceBillData>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(QueryPushBillData, CRMDatabaseName, CustomerName, TargetConnector, MonthID), sqlConnection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        try
                        {
                            InvoiceBillData invoiceData = new InvoiceBillData();
                            invoiceData.NMI = reader[0].ToString();
                            invoiceData.AccountReference = reader[1].ToString();
                            invoiceData.StartDate = Convert.ToDateTime(reader[2]);
                            invoiceData.EndDate = Convert.ToDateTime(reader[3]);
                            invoiceData.TotalConsumption = Convert.ToDecimal(reader[4]);
                            invoiceData.TotalCost = Convert.ToDecimal(reader[5]);
                            invoiceData.ScanFile = reader[6].ToString();
                            invoiceData.Supplier = reader[7].ToString();
                            invoiceData.CustomerName = reader[8].ToString();
                            invoiceBills.Add(invoiceData);
                        }
                        catch { }
                    }
                }
                sqlConnection.Close();
            }

            return invoiceBills.ToArray();
        }

        /// <summary>
        /// Save upload result in database
        /// </summary>
        /// <param name="invoiceBillData">Invoice bill data that was uploaded</param>
        /// <param name="uploadResult">Result of invoice upload</param>
        public void SaveUploadResult(InvoiceBillData invoiceBillData, string uploadResult)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(QueryUpdatePushResult, CRMDatabaseName, CustomerName, TargetConnector, MonthID, 
                        invoiceBillData.NMI, invoiceBillData.AccountReference, invoiceBillData.TotalConsumption, invoiceBillData.TotalCost, uploadResult), sqlConnection))
                {
                    command.ExecuteNonQuery();
                }
                sqlConnection.Close();
            }
        }
    }
}
