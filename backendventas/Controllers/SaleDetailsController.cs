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
    public class SaleDetailsController : ControllerBase
    {
        private readonly NetventasContext _context;

        public SaleDetailsController(NetventasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetails>>> GetSaleDetails()
        {
            return await _context.SaleDetails
                .Include(s => s.Products)
                .Include(s => s.Sales)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDetails>> GetSaleDetails(int id)
        {
            var saleDetails = await _context.SaleDetails
                .Include(s => s.Products)
                .Include(s => s.Sales)
                .FirstOrDefaultAsync(m => m.SaleId == id);

            if (saleDetails == null)
            {
                return NotFound();
            }

            return saleDetails;
        }

        [HttpPost]
        public async Task<ActionResult<SaleDetails>> CreateSaleDetails(SaleDetails saleDetails)
        {
            _context.SaleDetails.Add(saleDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSaleDetails), new { id = saleDetails.SaleId }, saleDetails);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaleDetails(int id, SaleDetails saleDetails)
        {
            if (id != saleDetails.SaleId)
            {
                return BadRequest();
            }

            _context.Entry(saleDetails).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleDetails(int id)
        {
            var saleDetails = await _context.SaleDetails.FindAsync(id);

            if (saleDetails == null)
            {
                return NotFound();
            }

            _context.SaleDetails.Remove(saleDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleDetailsExists(int id)
        {
            return _context.SaleDetails.Any(e => e.SaleId == id);
        }
    }
}
