namespace pizza_backend_api.DataTransferObjects
{
    public class PizzaTypeUploadDto
    {
        public string pizza_type_id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string ingredients { get; set; }
    }
}
