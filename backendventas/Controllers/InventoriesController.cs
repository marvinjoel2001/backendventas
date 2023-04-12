using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backendventas.Models;

namespace backendventas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoriesController : Controller
    {
        private readonly NetventasContext _context;

        public InventoriesController(NetventasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventories()
        {
            return await _context.Inventory.ToListAsync();
        }
        [HttpGet("{id}")]
        // GET: Inventories/Details/5
        public async Task<ActionResult<Inventory>> GetInventory(int? id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            return inventory;
            
        }

        [HttpPost]
        public async Task<ActionResult> CreateInventory(Inventory inventory)
        {
            var existingInventory = await _context.Inventory.FirstOrDefaultAsync(i => i.ProductId == inventory.ProductId);
            if (existingInventory != null)
            {
                // Si ya existe un registro de inventario para el mismo producto, simplemente actualizar la cantidad
                existingInventory.Quantity += inventory.Quantity;
            }
            else
            {
                // Si no existe un registro de inventario para el mismo producto, crear uno nuevo
                _context.Inventory.Add(inventory);
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetInventory", new { id = inventory.Id }, inventory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, Inventory inventory)
        {
            if (id != inventory.Id)
            {
                return BadRequest();
            }

            if (!await _context.Inventory.AnyAsync(i => i.Id == id))
            {
                return NotFound();
            }

            _context.Entry(inventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(inventory.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }
        // POST: Inventories/Delete/5
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
             _context.Inventory.Remove(inventory);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool InventoryExists(int id)
        {
          return (_context.Inventory?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
