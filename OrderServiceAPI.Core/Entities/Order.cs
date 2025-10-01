using System.ComponentModel.DataAnnotations;

namespace OrderServiceAPI.Core.Entities
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }  // Simulado

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string OrderStatus { get; set; } = "Pending";

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string BillingAddress { get; set; }

        public string? Notes { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}