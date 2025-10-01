using Microsoft.AspNetCore.Mvc;
using OrderServiceAPI.Infrastructure.Data;
using OrderServiceAPI.Core.Entities;

namespace OrderServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SeedProducts()
        {
            if (_context.Products.Any())
                return BadRequest("Ya existen productos.");
            //Métodos para registrar productos de forma manual
            var products = new List<Product>
            {
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    SKU = "SKU-001",
                    InternalCode = "PROD001",
                    Name = "Camiseta Negra",
                    Description = "Camiseta de algodón talla M",
                    CurrentUnitPrice = 2500.00m,
                    StockQuantity = 100
                },
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    SKU = "SKU-002",
                    InternalCode = "PROD002",
                    Name = "Zapatillas Urbanas",
                    Description = "Zapatillas deportivas unisex",
                    CurrentUnitPrice = 8500.00m,
                    StockQuantity = 50
                },
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    SKU = "SKU-003",
                    InternalCode = "PROD003",
                    Name = "Gorra Roja",
                    Description = "Gorra ajustable de algodón",
                    CurrentUnitPrice = 1200.00m,
                    StockQuantity = 75
                },
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    SKU = "SKU-004",
                    InternalCode = "PROD004",
                    Name = "Gorra Morada",
                    Description = "Gorra ajustable de lana",
                    CurrentUnitPrice = 1200.00m,
                    StockQuantity = 75
                }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();

            return Ok("Productos insertados correctamente.");
        }
    }
}