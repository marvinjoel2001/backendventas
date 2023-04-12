using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backendventas.Models;
using Microsoft.AspNetCore.Authorization;

namespace backendventas.Controllers
{
    public class RegisterSaleRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : Controller
    {
        
        private readonly NetventasContext _context;

        public SalesController(NetventasContext context)
        {
            _context = context;
        }

        // GET: Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sales>>> GetSales()
        {
            return await _context.Sales.ToListAsync();

        }
        [HttpGet("{id}")]
        // GET: Sales/Details/5
        public async Task<ActionResult<Sales>> GetSale(int? id)
        {
            var sales = await _context.Sales.FindAsync(id);
            if (sales == null)
            {
                return NotFound();
            }

            return sales;
        }
        [HttpPost("registerVenta")]
        public async Task<IActionResult> RegisterSale([FromBody] RegisterSaleRequest saleRequest)
        {
            // Obtener el producto de la base de datos
            var product = await _context.Products.FindAsync(saleRequest.ProductId);

            // Obtener la cantidad actual del producto en el inventario
            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(i => i.ProductId == saleRequest.ProductId);
            if (inventory == null)
            {
                // Si el producto no existe en el inventario, crear un nuevo registro
                inventory = new Inventory
                {
                    ProductId = saleRequest.ProductId,
                    Quantity = saleRequest.Quantity
                };
                _context.Inventory.Add(inventory);
            }
            else
            {
                // Si el producto ya existe en el inventario, actualizar la cantidad
                inventory.Quantity -= saleRequest.Quantity;
            }

            // Crear un nuevo registro de venta
            var sale = new Sales
            {
                Date = DateTime.Now,
                SaleDetails = new List<SaleDetails>()
            };
            _context.Sales.Add(sale);

            // Crear un nuevo detalle de venta
            var saleDetail = new SaleDetails
            {
                SaleId = sale.Id,
                ProductId = saleRequest.ProductId,
                Quantity = saleRequest.Quantity,
                PricePerUnit = saleRequest.PricePerUnit,
                TotalPrice = saleRequest.Quantity * saleRequest.PricePerUnit
            };
            sale.SaleDetails.Add(saleDetail);

            // Actualizar el total de la venta
            sale.Total = sale.SaleDetails.Sum(sd => sd.TotalPrice);
            sale.Tax = sale.Total * 0.13m; // Suponiendo que el impuesto es del 18%

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<int>> CreateSales(Sales sales)
        {
            _context.Sales.Add(sales);
            await _context.SaveChangesAsync();
            return sales.Id;
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSales(int id, Sales sales)
        {
            if (id != sales.Id)
            {
                return NotFound();
            }
            _context.Entry(sales).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesExists(sales.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sales = await _context.Sales.FindAsync(id);
            if (sales == null)
            {
                return NoContent();
            }
            _context.Sales.Remove(sales);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool SalesExists(int id)
        {
          return (_context.Sales?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
