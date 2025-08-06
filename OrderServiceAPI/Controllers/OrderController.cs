using Microsoft.AspNetCore.Mvc;
using OrderServiceAPI.DTOs;
using OrderServiceAPI.Models;
using OrderServiceAPI.Services;

namespace OrderServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // 1. Crear nueva orden
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 2. Obtener todas las órdenes con filtros opcionales
        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromQuery] string? status,
            [FromQuery] Guid? customerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetAllOrdersAsync(status, customerId, pageNumber, pageSize);
            return Ok(orders);
        }

        // 3. Obtener orden por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { error = "Orden no encontrada." });

            return Ok(order);
        }

        // 4. Cambiar estado de orden
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, dto.NewStatus);
                if (updatedOrder == null)
                    return NotFound(new { error = "Orden no encontrada." });

                return Ok(updatedOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}