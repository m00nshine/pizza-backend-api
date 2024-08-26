namespace pizza_backend_api.DataTransferObjects
{
    public class TransactionDetailResponseDto
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
        public string PizzaName { get; set; }
        public string PizzaCategory { get; set; }
        public string PizzaSize { get; set; }
    }
}
