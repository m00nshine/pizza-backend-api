namespace pizza_backend_api.DataTransferObjects
{
    public class PizzaPriceResponseDto
    {
        public int Id { get; set; }
        public string PizzaName { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
    }
}
