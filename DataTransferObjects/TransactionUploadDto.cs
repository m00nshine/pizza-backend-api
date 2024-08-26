namespace pizza_backend_api.DataTransferObjects
{
    public class TransactionUploadDto
    {
        public string order_id { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }
}
