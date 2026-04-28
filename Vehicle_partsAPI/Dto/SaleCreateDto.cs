using System.Collections.Generic;

namespace vehicle_parts.Dto
{
    public class SaleItemDto
    {
        public int PartID { get; set; }
        public int Quantity { get; set; }
    }

    public class SaleCreateDto
    {
        public int UserID { get; set; }
        public List<SaleItemDto> Items { get; set; }
        public string PaymentMethod { get; set; }
    }
}
