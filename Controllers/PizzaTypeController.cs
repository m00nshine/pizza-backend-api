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
    public class PizzaTypeController : ControllerBase
    {
        private readonly EhrlichContext _context;

        public PizzaTypeController(EhrlichContext context)
        {
            _context = context;
        }

        // GET: api/PizzaType
        [HttpGet]
        public IEnumerable<PizzaTypeResponseDto> GetPizzaTypes([FromQuery] PageDetails page)
        {
            var filteredPizzaTypeIds = _context.PizzaTypes
                .Skip((page.pageCount - 1) * page.pageSize)
                .Take(page.pageSize)
                .OrderByDescending(x => x.Id)
                .Select(s => s.Id)
                .ToList();

            var result = _context.VwCompletePizzaTypeDetails
                .Where(p => filteredPizzaTypeIds.Contains(p.Id))
                .Select(x => new PizzaTypeResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                    Description = x.Description
                });

            return result;
        }

        // GET: api/PizzaType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaType>> GetPizzaType(int id)
        {
            var pizzaType = await _context.PizzaTypes.FindAsync(id);

            if (pizzaType == null)
            {
                return NotFound();
            }

            return pizzaType;
        }

        // PUT: api/PizzaType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPizzaType(int id, PizzaType pizzaType)
        {
            if (id != pizzaType.Id)
            {
                return BadRequest();
            }

            _context.Entry(pizzaType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PizzaTypeExists(id))
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

        // POST: api/PizzaType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PizzaType>> PostPizzaType(PizzaType pizzaType)
        {
            _context.PizzaTypes.Add(pizzaType);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        // DELETE: api/PizzaType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePizzaType(int id)
        {
            var pizzaType = await _context.PizzaTypes.FindAsync(id);
            if (pizzaType == null)
            {
                return NotFound();
            }

            _context.PizzaTypes.Remove(pizzaType);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        private bool PizzaTypeExists(int id)
        {
            return _context.PizzaTypes.Any(e => e.Id == id);
        }
    }
}
