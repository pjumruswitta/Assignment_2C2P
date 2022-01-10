using System.ComponentModel;

namespace Assignment_2C2P.Models
{
    public static class InvoiceConstant
    {
        public static readonly List<string> INV_STATUS_LIST = new List<string>() { "APPROVED", "FAILED", "REJECTED", "FINISHED", "DONE" };

        public const string INV_STATUS_APPROVED = "Approved";
        public const string INV_STATUS_FAILED = "Failed";
        public const string INV_STATUS_REJECTED = "Rejected";
        public const string INV_STATUS_FINISHED = "Finished";
        public const string INV_STATUS_DONE = "Done";

        public const string INV_DISP_STATUS_APPROVE = "A";
        public const string INV_DISP_STATUS_REJECTED = "R";
        public const string INV_DISP_STATUS_DONE = "D";

        public const string INV_DISP_DATETIME_FORMAT = "yyyy-MM-ddThh:mm:ss";
    }

    public enum INVOICE_SEARCH_MODE
    {
        CURRENCY = 1,
        DATE_RANGE = 2,
        STATUS = 3
    }
}
