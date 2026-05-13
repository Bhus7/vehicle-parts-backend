namespace vehicle_parts.Dto.Response
{
    public class CustomerSearchResponse
    {
        public int UserID { get; set; }
        public required string FullName { get; set; }
        public required string Phone { get; set; }
        public string? Email { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public decimal LatestInvoiceAmount { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
