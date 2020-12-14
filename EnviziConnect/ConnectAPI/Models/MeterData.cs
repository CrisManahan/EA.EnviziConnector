using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectAPI.Models
{
    public class MeterData
    {
        public string Location { get; set; }
        public string SerialNo { get; set; }
        public string Name { get; set; }
        public string ServicePoint { get; set; }
        public string DataType { get; set; }
        public string Component { get; set; }
        public string Type { get; set; }
        public string Supplier { get; set; }
    }
}
