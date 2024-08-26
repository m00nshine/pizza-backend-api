namespace pizza_backend_api.DataLayerAccess
{
    public class TransactionRepository
    {
        private readonly EhrlichContext _context;

        public TransactionRepository(EhrlichContext context)
        {
            _context = context;
        }

        public IQueryable GetAllTransactions() { 
            return _context.Transactions.Take(5);
        }
    }
}
