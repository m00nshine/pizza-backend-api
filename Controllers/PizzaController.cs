using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizza_backend_api.DataTransferObjects;
using pizza_backend_api.Models;

namespace pizza_backend_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly EhrlichContext _context;

        public PizzaController(EhrlichContext context)
        {
            _context = context;
        }

        // GET: api/Pizza
        [HttpGet]
        public IEnumerable<PizzaPriceResponseDto> GetPrices([FromQuery] PageDetails page)
        {

            var filteredPizzaPriceIds = _context.PizzaPrices
                .Skip((page.pageCount - 1) * page.pageSize)
                .Take(page.pageSize)
                .OrderByDescending(x => x.Id)
                .Select(s => s.Id)
                .ToList();

            var result = _context.VwCompletePizzaDetails
                .Where(p => filteredPizzaPriceIds.Contains(p.Id))
                .Select(x => new PizzaPriceResponseDto
                {
                    Id = x.Id,
                    PizzaName = x.Name,
                    Price = x.Price,
                    Size = x.Code
                });

            return result;
        }

        // GET: api/Pizza/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaPrice>> GetPrice(int id)
        {
            var price = await _context.PizzaPrices.FindAsync(id);

            if (price == null)
            {
                return NotFound();
            }

            return price;
        }

        // PUT: api/Pizza/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrice(int id, Price price)
        {
            if (id != price.Id)
            {
                return BadRequest();
            }

            _context.Entry(price).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PriceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Success");
        }

        // POST: api/Pizza
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PizzaPrice>> PostPrice(PizzaPrice price)
        {
            _context.PizzaPrices.Add(price);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        // DELETE: api/Pizza/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrice(int id)
        {
            var price = await _context.PizzaPrices.FindAsync(id);
            if (price == null)
            {
                return NotFound();
            }

            _context.PizzaPrices.Remove(price);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        private bool PriceExists(int id)
        {
            return _context.PizzaPrices.Any(e => e.Id == id);
        }
    }
}
