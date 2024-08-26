using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using pizza_backend_api.DataLayerAccess;
using pizza_backend_api.DataTransferObjects;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace pizza_backend_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly EhrlichContext _context;

        public TransactionController(EhrlichContext data)
        {
            _context = data;
        }

        /// <summary>
        /// Get all recent transactions filtered by pagination
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<TransactionResponseDto> Get([FromQuery]PageDetails page)
        {
            var allTransactions = _context.Transactions
                .Skip((page.pageCount - 1) * page.pageSize)
                .Take(page.pageSize)
                .OrderByDescending(x => x.CreatedDate)
                .Select(s => s.Id)
                .ToList();

            var result = _context.VwMinimalTransactionDetails
                .Where(p => allTransactions.Contains(p.OrderId))
                .Select(x => new TransactionResponseDto
                {
                    OrderId = x.OrderId,
                    TotalPrice = x.TotalPrice,
                    TransactionDate = x.CreatedDate
                });

            return result;
        }

        [HttpGet("{OrderId}")]
        public async Task<ActionResult<Transaction>> GetTransactionDetail(int OrderId)
        {
            var result = _context.VwCompleteTransactionDetails
                .Where(w => w.OrderId == OrderId)
                .Select(x => new TransactionDetailResponseDto
                {
                    OrderId = x.OrderId,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    TotalPrice = x.Amount,
                    PizzaName = x.Name,
                    PizzaCategory = x.Category,
                    PizzaSize = x.Size
                })
                .ToList();

            if (result == null)
            {
                return NotFound();
            }
            //transaction.Where(x => x.OrderId == OrderId)

            return Ok(result);
        }

        [HttpPut("{OrderId}")]
        public async Task<IActionResult> PutTransaction(int OrderId, Transaction transaction)
        {
            if (OrderId != transaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(OrderId))
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

        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        [HttpDelete("{OrderId}")]
        public async Task<IActionResult> DeleteTransaction(int OrderId)
        {
            var transaction = await _context.Transactions.FindAsync(OrderId);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

        private bool TransactionExists(int OrderId)
        {
            return _context.Transactions.Any(e => e.Id == OrderId);
        }
    }
}
