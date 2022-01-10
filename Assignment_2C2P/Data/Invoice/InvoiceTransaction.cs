using System;
using System.Collections.Generic;

namespace Assignment_2C2P.Data.Invoice
{
    public partial class InvoiceTransaction
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string InputType { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
