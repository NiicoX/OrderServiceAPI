namespace OrderServiceAPI.DTOs
{
    public class CreateOrderDto
    {
        public Guid CustomerId { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; }
    }

    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}