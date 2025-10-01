using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderServiceAPI.Core.DTOs;
using OrderServiceAPI.Core.Entities;
using OrderServiceAPI.Infrastructure.Data;
using OrderServiceAPI.Infrastructure.Services;
using Xunit;

namespace OrderServiceAPI.Tests
{
    public class OrderServiceTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // base de datos nueva por test
                .Options;

            var context = new AppDbContext(options);

            // Seed de productos con propiedades obligatorias
            context.Products.Add(new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Producto A",
                Description = "Descripción Producto A",
                StockQuantity = 5,
                CurrentUnitPrice = 100,
                InternalCode = "PRODA001",
                SKU = "SKU001"
            });
            context.Products.Add(new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Producto B",
                Description = "Descripción Producto B",
                StockQuantity = 10,
                CurrentUnitPrice = 200,
                InternalCode = "PRODB001",
                SKU = "SKU002"
            });
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task CreateOrder_ShouldCreateOrderSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = Mock.Of<ILogger<OrderService>>();
            var service = new OrderService(context, logger);

            var product = context.Products.First();

            var dto = new CreateOrderDto
            {
                CustomerId = Guid.NewGuid(),
                ShippingAddress = "Calle Falsa 123",
                BillingAddress = "Calle Falsa 123",
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = product.ProductId,
                        Quantity = 2,
                        UnitPrice = product.CurrentUnitPrice
                    }
                }
            };

            // Act
            var result = await service.CreateOrderAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.CustomerId, result.CustomerId);
            Assert.Single(result.OrderItems);
            Assert.Equal(2 * product.CurrentUnitPrice, result.TotalAmount);

            // Verificar que el stock se descontó
            var updatedProduct = await context.Products.FindAsync(product.ProductId);
            Assert.Equal(product.StockQuantity, updatedProduct.StockQuantity);
        }

        [Fact]
        public async Task CreateOrder_WithInsufficientStock_ShouldThrowException()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = Mock.Of<ILogger<OrderService>>();
            var service = new OrderService(context, logger);

            var product = context.Products.First();

            var dto = new CreateOrderDto
            {
                CustomerId = Guid.NewGuid(),
                ShippingAddress = "Calle Falsa 123",
                BillingAddress = "Calle Falsa 123",
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = product.ProductId,
                        Quantity = product.StockQuantity + 1, // más del stock disponible
                        UnitPrice = product.CurrentUnitPrice
                    }
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateOrderAsync(dto));
            Assert.Contains("Stock insuficiente", ex.Message);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldChangeStatus()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = Mock.Of<ILogger<OrderService>>();
            var service = new OrderService(context, logger);

            var order = new Core.Entities.Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                ShippingAddress = "123",
                BillingAddress = "123",
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow,
                TotalAmount = 100,
                OrderItems = new List<OrderItem>()
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Act
            var updated = await service.UpdateOrderStatusAsync(order.OrderId, "Shipped");

            // Assert
            Assert.NotNull(updated);
            Assert.Equal("Shipped", updated.OrderStatus);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOrder()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = Mock.Of<ILogger<OrderService>>();
            var service = new OrderService(context, logger);

            var order = new Core.Entities.Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                ShippingAddress = "123",
                BillingAddress = "123",
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow,
                TotalAmount = 100,
                OrderItems = new List<OrderItem>()
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetOrderByIdAsync(order.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
        }

        [Fact]
        public async Task GetAllOrders_ShouldReturnFilteredAndPaginatedOrders()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var logger = Mock.Of<ILogger<OrderService>>();
            var service = new OrderService(context, logger);

            // Crear varias órdenes
            var customer1 = Guid.NewGuid();
            var customer2 = Guid.NewGuid();

            context.Orders.AddRange(
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = customer1,
                    OrderStatus = "Pending",
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 100,
                    OrderItems = new List<OrderItem>(),
                    ShippingAddress = "Calle Falsa 123",
                    BillingAddress = "Calle Falsa 123"
                },
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = customer1,
                    OrderStatus = "Shipped",
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 200,
                    OrderItems = new List<OrderItem>(),
                    ShippingAddress = "Calle Falsa 123",
                    BillingAddress = "Calle Falsa 123"
                },
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = customer2,
                    OrderStatus = "Pending",
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 150,
                    OrderItems = new List<OrderItem>(),
                    ShippingAddress = "Calle Falsa 456",
                    BillingAddress = "Calle Falsa 456"
                }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllOrdersAsync(status: "Pending", customerId: customer1, pageNumber: 1, pageSize: 10);

            // Assert
            Assert.Single(result); // solo la orden pendiente del customer1
            Assert.All(result, o => Assert.Equal("Pending", o.OrderStatus));
            Assert.All(result, o => Assert.Equal(customer1, o.CustomerId));
        }
    }
}
