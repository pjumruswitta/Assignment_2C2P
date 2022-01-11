using Assignment_2C2P.Models;
using Assignment_2C2P.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Assignment_2C2P.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class InvoiceTransactionController : ControllerBase
    {
        private IInvoiceServices _invoiceService;
        public InvoiceTransactionController(IInvoiceServices invoiceService)
        {
            this._invoiceService = invoiceService;
        }

        [HttpGet]
        [Route("Status")]
        public async Task<List<InvoiceTransactionResponse>> GetInvoiceByStatus(string status = InvoiceConstant.INV_STATUS_APPROVED)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentNullException(nameof(status), "Status is required, [Approved, Failed, Rejected, Finished, Done]");
            return await _invoiceService.GetInvoicesTransaction(INVOICE_SEARCH_MODE.STATUS, new InvoiceSearchRequest { Status = status });
        }

        [HttpGet]
        [Route("Currency")]
        public async Task<List<InvoiceTransactionResponse>> GetInvoiceByCurrency(string currency = "USD")
        {
            return await _invoiceService.GetInvoicesTransaction(INVOICE_SEARCH_MODE.CURRENCY, new InvoiceSearchRequest { Currency = currency });
        }

        [HttpGet]
        [Route("DateRange")]
        public async Task<List<InvoiceTransactionResponse>> GetInvoiceByDateRange(DateTime? dateFrom, DateTime? dateTo)
        {
            if (!dateFrom.HasValue) throw new ArgumentNullException(nameof(dateTo), "DateFrom is required.");
            if (!dateTo.HasValue) throw new ArgumentNullException(nameof(dateTo), "DateTo is required.");
            return await _invoiceService.GetInvoicesTransaction(INVOICE_SEARCH_MODE.DATE_RANGE, new InvoiceSearchRequest { DateFrom = dateFrom.Value, DateTo = dateTo.Value });
        }

        [HttpPost]
        [Route("Upload")]

        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null)
                    return BadRequest("file is required.");

                var insertList = new List<InvoiceTransaction>();
                var validateResult = new List<List<string>>();


                if (file.ContentType.Equals("text/xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Transactions));
                    using (XmlTextReader reader = new XmlTextReader(file.OpenReadStream()))
                    {
                        reader.WhitespaceHandling = WhitespaceHandling.None;
                        reader.Read();
                        Transactions? xmlResult = serializer.Deserialize(reader) as Transactions;
                        if (xmlResult != null)
                        {
                            xmlResult.Transaction.ForEach(t =>
                            {
                                var validateResultItem = ValidateXmlInput(t);
                                if (validateResultItem.Count > 0)
                                    validateResult.Add(validateResultItem);
                                else
                                    insertList.Add(new InvoiceTransaction()
                                    {
                                        TransactionId = t.Id,
                                        TransactionDate = t.TransactionDate.Value,
                                        Amount = t.PaymentDetails.Amount.Value,
                                        Currency = t.PaymentDetails.CurrencyCode,
                                        InputType = "XML",
                                        Status = t.Status,
                                        CreatedDate = DateTime.Now,
                                        IsActive = true
                                    });
                            });
                        }
                    }
                }
                else if (file.ContentType.Equals("text/csv"))
                {

                }
                else
                {
                    return BadRequest("Unknown format");
                }

                if (validateResult.Count > 0)
                    return BadRequest(validateResult);
                await _invoiceService.InsertInvoiceTransaction(insertList);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Sorry, something went wrong.");
            }
        }

        private List<string> ValidateXmlInput(Transaction input)
        {
            List<string> result = new List<string>();

            if (input != null)
            {
                if (string.IsNullOrWhiteSpace(input.Id))
                    result.Add("Transaction ID is missing.");
                if (string.IsNullOrWhiteSpace(input.Status))
                    result.Add("Transaction Status is missing.");
                if (input.TransactionDate == null)
                    result.Add("Transaction Date is missing.");
                if (input.PaymentDetails == null)
                {
                    result.Add("Transaction PaymentDetails is missing.");
                }
                else
                {
                    if (input.PaymentDetails.Amount == null)
                        result.Add("Transaction PaymentDetails Amount is missing.");
                    if (string.IsNullOrWhiteSpace(input.PaymentDetails.CurrencyCode))
                        result.Add("Transaction PaymentDetails CurrencyCode is missing.");
                }


                if (result.Count > 0)
                {
                    result.Insert(0, $"Transaction ID: {input.Id}");
                }
            }

            return result;
        }
    }

}
