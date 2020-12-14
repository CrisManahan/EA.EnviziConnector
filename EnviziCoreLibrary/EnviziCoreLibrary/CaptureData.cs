using System;

namespace EnviziCoreLibrary
{
    public class CaptureData
    {
        public string AccountNumber { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public string Reference { get; set; }
        public decimal TotalConsumption { get; set; }

        public decimal TotalCost { get; set; }
        public string InvoiceFile { get; set; }
    }
}
