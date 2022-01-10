namespace Assignment_2C2P.Models
{
    public class InvoiceSearchRequest
    {
        public string Status { get; set; }
        public string Currency { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public class InvoiceTransactionResponse
    {
        public string Id { get; set; }
        public string Payment { get; set; }
        public string Status { get; set; }
    }
}
