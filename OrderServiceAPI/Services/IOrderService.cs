using OrderServiceAPI.DTOs;
using OrderServiceAPI.Models;

namespace OrderServiceAPI.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderDto orderDto);
        Task<IEnumerable<Order>> GetAllOrdersAsync(string? status, Guid? customerId, int pageNumber, int pageSize);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Order?> UpdateOrderStatusAsync(Guid orderId, string newStatus);
    }
}