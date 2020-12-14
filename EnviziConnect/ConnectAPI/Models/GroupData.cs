
namespace ConnectAPI.Models
{
    public class GroupData
    {
        public string Name { get; set; }
        public int Locations { get; set; }
        public string Style { get; set; }
        public string Description { get; set; }
        public string BelongsTo { get; set; }
        public decimal ReportPercent { get; set; }
        public bool OpenToPublic { get; set; }
    }
}
