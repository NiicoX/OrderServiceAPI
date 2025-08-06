using Microsoft.EntityFrameworkCore;
using OrderServiceAPI.Data;
using OrderServiceAPI.DTOs;
using OrderServiceAPI.Models;

namespace OrderServiceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDto orderDto)
        {
            var productIds = orderDto.OrderItems.Select(i => i.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            if (products.Count != orderDto.OrderItems.Count)
                throw new ArgumentException("Uno o más productos no existen.");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var itemDto in orderDto.OrderItems)
            {
                var product = products.First(p => p.ProductId == itemDto.ProductId);

                if (product.StockQuantity < itemDto.Quantity)
                    throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}.");

                var subtotal = itemDto.Quantity * itemDto.UnitPrice;
                totalAmount += subtotal;

                orderItems.Add(new OrderItem
                {
                    OrderItemId = Guid.NewGuid(),
                    ProductId = product.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Subtotal = subtotal
                });

                product.StockQuantity -= itemDto.Quantity;
            }

            var newOrder = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = orderDto.CustomerId,
                ShippingAddress = orderDto.ShippingAddress,
                BillingAddress = orderDto.BillingAddress,
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                OrderItems = orderItems
            };

            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            return newOrder;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? status, Guid? customerId, int pageNumber, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.OrderStatus == status);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Order?> UpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return null;

            var allowedStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!allowedStatuses.Contains(newStatus))
                throw new ArgumentException("Estado de orden inválido.");

            order.OrderStatus = newStatus;
            await _context.SaveChangesAsync();

            return order;
        }
    }
}