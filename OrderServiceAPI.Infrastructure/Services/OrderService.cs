using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderServiceAPI.Core.DTOs;
using OrderServiceAPI.Core.Entities;
using OrderServiceAPI.Core.Interfaces;
using OrderServiceAPI.Infrastructure.Data;

namespace OrderServiceAPI.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDto orderDto)
        {
            try
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

                    var subtotal = itemDto.Quantity * product.CurrentUnitPrice;
                    totalAmount += subtotal;

                    orderItems.Add(new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        ProductId = product.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.CurrentUnitPrice,
                        Subtotal = subtotal
                    });

                    product.StockQuantity -= itemDto.Quantity;
                    _logger.LogInformation("Se descontó {Quantity} unidades del producto {ProductId}.", itemDto.Quantity, product.ProductId);
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

                _logger.LogInformation("Orden {OrderId} creada correctamente para el cliente {CustomerId}.", newOrder.OrderId, newOrder.CustomerId);

                return newOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden para el cliente {CustomerId}.", orderDto.CustomerId);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? status, Guid? customerId, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.OrderItems)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(o => o.OrderStatus == status);

                if (customerId.HasValue)
                    query = query.Where(o => o.CustomerId == customerId.Value);

                var orders = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("{Count} órdenes obtenidas (Status={Status}, CustomerId={CustomerId}).", orders.Count, status, customerId);

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las órdenes.");
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order != null)
                    _logger.LogInformation("Orden {OrderId} obtenida correctamente.", orderId);
                else
                    _logger.LogWarning("Orden {OrderId} no encontrada.", orderId);

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la orden {OrderId}.", orderId);
                throw;
            }
        }

        public async Task<Order?> UpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("No se encontró la orden {OrderId} para actualizar.", orderId);
                    return null;
                }

                var allowedStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
                if (!allowedStatuses.Contains(newStatus))
                    throw new ArgumentException("Estado de orden inválido.");

                var oldStatus = order.OrderStatus;
                order.OrderStatus = newStatus;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Orden {OrderId} actualizada: {OldStatus} -> {NewStatus}.", orderId, oldStatus, newStatus);

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado de la orden {OrderId}.", orderId);
                throw;
            }
        }
    }
}
