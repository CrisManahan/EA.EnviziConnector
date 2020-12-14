using System;

namespace EnviziCoreLibrary
{
    public class InvoiceBillData
    {
        public string NMI { get; set; }
        public string AccountReference { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalConsumption { get; set; }
        public decimal TotalCost { get; set; }
        public string ScanFile { get; set; }
        public string Supplier { get; set; }
        public string CustomerName { get; set; }

        public string GetReferenceText()
        {
            return string.Format("{0}_{1}", NMI, AccountReference);
        }

        public string GetTempFileName()
        {
            return string.Format("{0}_{1}-{2}.{3}", GetReferenceText(), StartDate.ToString("ddMMMyyyy"), EndDate.ToString("ddMMMyyyy"), "pdf");
        }
    }
}
