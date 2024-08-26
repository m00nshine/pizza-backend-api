namespace pizza_backend_api.DataTransferObjects
{
    public class TransactionDetailUploadDto
    {
        public string order_details_id { get; set; }
        public string order_id { get; set; }
        public string pizza_id { get; set; }
        public string quantity { get; set; }
    }
}
