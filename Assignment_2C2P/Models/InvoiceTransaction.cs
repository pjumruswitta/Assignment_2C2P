using System.Globalization;
using System.Xml.Serialization;

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

    #region | XML |
    [XmlRoot(ElementName = "PaymentDetails")]
    public class PaymentDetails
    {

        [XmlElement(ElementName = "Amount")]
        public decimal? Amount { get; set; }

        [XmlElement(ElementName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
    }

    [XmlRoot(ElementName = "Transaction")]
    public class Transaction
    {

        [XmlElement(ElementName = "TransactionDate")]
        public DateTime? TransactionDate { get; set; }

        [XmlElement(ElementName = "PaymentDetails")]
        public PaymentDetails PaymentDetails { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Transactions")]
    public class Transactions
    {

        [XmlElement(ElementName = "Transaction")]
        public List<Transaction> Transaction { get; set; }
    }
    #endregion

    #region | CSV |
    public class InvoiceTransactionCsv
    {
        public string TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Status { get; set; }

        /// <summary>
        /// Need to pass validation before use this method
        /// </summary>
        /// <param name="csvLine"></param>
        /// <returns></returns>
        public static InvoiceTransactionCsv FromCsv(string[] csvLine)
        {
            InvoiceTransactionCsv csv = new InvoiceTransactionCsv();
            csv.TransactionId = csvLine[0].Replace("\"", "");
            csv.Amount = decimal.TryParse(csvLine[1].Replace("\"", ""), out decimal transAmount) ? transAmount : null;
            csv.Currency = csvLine[2].Replace("\"", "");
            csv.TransactionDate = DateTime.TryParseExact(csvLine[3].Replace("\"", ""), "dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out DateTime transDate) ? transDate : null;
            csv.Status = csvLine[4].Replace("\"", "");
            return csv;
        }
    }
    #endregion
}
