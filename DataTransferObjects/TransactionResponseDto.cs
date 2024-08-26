namespace pizza_backend_api.DataTransferObjects
{
    public class TransactionResponseDto
    {
        public int OrderId { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
