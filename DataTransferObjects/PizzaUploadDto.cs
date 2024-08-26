namespace pizza_backend_api.DataTransferObjects
{
    public class PizzaUploadDto
    {
        public string pizza_id { get; set; }
        public string pizza_type_id { get; set; }
        public string size { get; set; }
        public string price { get; set; }
    }
}
