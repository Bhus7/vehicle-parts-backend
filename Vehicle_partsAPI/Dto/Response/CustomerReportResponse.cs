namespace vehicle_parts.Dto.Response
{
    public class CustomerReportResponse
    {
        public int UserID { get; set; }
        public required string FullName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal RemainingBalance { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
