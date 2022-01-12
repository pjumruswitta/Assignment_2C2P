using Assignment_2C2P.Models;
using Assignment_2C2P.Repositories;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.VisualBasic.FileIO;

namespace Assignment_2C2P.Services
{
    public interface IInvoiceServices
    {
        public Task<List<InvoiceTransactionResponse>> GetInvoicesTransaction(INVOICE_SEARCH_MODE mode, InvoiceSearchRequest request);
        public Task InsertInvoiceTransaction(IFormFile file);
    }

    public class InvoiceServices : IInvoiceServices
    {

        private readonly InvoiceRepositories _invoiceRepo;

        public InvoiceServices(Assignment_2C2PContext invoiceContext)
        {
            this._invoiceRepo = new InvoiceRepositories(invoiceContext);
        }

        public async Task<List<InvoiceTransactionResponse>> GetInvoicesTransaction(INVOICE_SEARCH_MODE mode, InvoiceSearchRequest request)
        {
            try
            {
                var result = new List<InvoiceTransactionResponse>();

                List<InvoiceTransaction> rawInvoiceTransactions;

                switch (mode)
                {
                    case INVOICE_SEARCH_MODE.CURRENCY:
                        rawInvoiceTransactions = await _invoiceRepo.GetInvoiceTransactionsByCurrency(request.Currency.ToUpper());
                        break;
                    case INVOICE_SEARCH_MODE.DATE_RANGE:
                        rawInvoiceTransactions = await _invoiceRepo.GetInvoiceTransactionsByDateRange(request.DateFrom, request.DateTo);
                        break;
                    case INVOICE_SEARCH_MODE.STATUS:
                        if (!InvoiceConstant.INV_STATUS_LIST.Contains(request.Status.ToUpper()))
                            throw new AssignmentException("Invalid SearchStaus for InvoiceTransaction");
                        rawInvoiceTransactions = await _invoiceRepo.GetInvoiceTransactionsByStatus(request.Status.ToUpper());
                        break;
                    default:
                        throw new AssignmentException(nameof(mode), "Invalid SearchMode for InvoiceTransaction");
                }

                if (rawInvoiceTransactions.Any())
                {
                    rawInvoiceTransactions.ForEach(transaction =>
                    {
                        result.Add(new InvoiceTransactionResponse()
                        {
                            Id = transaction.TransactionId,
                            Payment = $"{transaction.Amount} {transaction.Currency}",
                            Status = DisplayStatusMapping(transaction.Status)
                        });
                    });
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
        public async Task InsertInvoiceTransaction(IFormFile file)
        {
            try
            {
                if (file == null)
                    throw new AssignmentException("file is required.");

                var insertList = new List<InvoiceTransaction>();
                var validateResult = new List<List<string>>();
                switch (file.ContentType)
                {
                    case "text/xml":
                        XmlSerializer serializer = new XmlSerializer(typeof(Transactions));
                        using (XmlTextReader reader = new XmlTextReader(file.OpenReadStream()))
                        {
                            reader.WhitespaceHandling = WhitespaceHandling.None;
                            reader.Read();
                            Transactions? xmlResult = serializer.Deserialize(reader) as Transactions;
                            if (xmlResult != null)
                            {
                                int seq = 1;
                                xmlResult.Transaction.ForEach(t =>
                                {
                                    var validateResultItem = ValidateXmlInput(t, seq);
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

                                    seq++;
                                });
                            }
                        }
                        break;
                    case "text/csv":
                    case "application/vnd.ms-excel":
                        using (TextFieldParser textFieldParser = new TextFieldParser(file.OpenReadStream()))
                        {
                            textFieldParser.TextFieldType = FieldType.Delimited;
                            textFieldParser.SetDelimiters(",");

                            int rowNo = 1;
                            while (!textFieldParser.EndOfData)
                            {
                                string[] rows = textFieldParser.ReadFields();

                                var validateResultItem = ValidateCsvInput(rows, rowNo);
                                if (validateResultItem.Count > 0)
                                {
                                    validateResult.Add(validateResultItem);
                                }
                                else
                                {
                                    var csv = InvoiceTransactionCsv.FromCsv(rows);
                                    insertList.Add(new InvoiceTransaction()
                                    {
                                        TransactionId = csv.TransactionId,
                                        TransactionDate = csv.TransactionDate.Value,
                                        Amount = csv.Amount.Value,
                                        Currency = csv.Currency,
                                        InputType = "CSV",
                                        Status = csv.Status,
                                        CreatedDate = DateTime.Now,
                                        IsActive = true
                                    });
                                }

                                rowNo++;
                            }
                        }
                        break;
                    default:
                        throw new AssignmentException("Unknown format");
                }

                if (validateResult.Count > 0)
                    throw new AssignmentException("Input validation failed.", validateResult);

                await _invoiceRepo.InsertInvoiceTransactions(insertList);
            }
            catch
            {
                throw;
            }
        }

        private string DisplayStatusMapping(string input)
        {
            switch (input)
            {
                case InvoiceConstant.INV_STATUS_APPROVED:
                    return InvoiceConstant.INV_DISP_STATUS_APPROVE;
                case InvoiceConstant.INV_STATUS_FAILED:
                case InvoiceConstant.INV_STATUS_REJECTED:
                    return InvoiceConstant.INV_DISP_STATUS_REJECTED;
                case InvoiceConstant.INV_STATUS_FINISHED:
                case InvoiceConstant.INV_STATUS_DONE:
                    return InvoiceConstant.INV_DISP_STATUS_DONE;
                default:
                    return input;
            }
        }
        private List<string> ValidateXmlInput(Transaction input, int sequenceNo)
        {
            List<string> result = new List<string>();

            if (input != null)
            {
                if (string.IsNullOrWhiteSpace(input.Id))
                    result.Add("Transaction ID is missing.");
                else if (input.Id.Length > 50)
                    result.Add("Maximum Transaction ID is 50 chars.");

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
                    result.Insert(0, $"Transaction Sequence NO: {sequenceNo}");
                }
            }

            return result;
        }
        private List<string> ValidateCsvInput(string[] input, int rowNo)
        {
            List<string> result = new List<string>();

            if (input != null && input.Count() == 5)
            {
                if (string.IsNullOrWhiteSpace(input[0]))
                    result.Add("Transaction ID is missing.");
                else if (input[0].Length > 50)
                    result.Add("Maximum Transaction ID is 50 chars.");

                if (string.IsNullOrWhiteSpace(input[1]))
                    result.Add("Transaction Amount is missing.");
                else if (!decimal.TryParse(input[1], out decimal res))
                    result.Add("Transaction Amount is invalid format.");

                if (string.IsNullOrWhiteSpace(input[2]))
                    result.Add("Transaction CurrencyCode is missing.");
                if (string.IsNullOrWhiteSpace(input[3]))
                    result.Add("Transaction Status is missing.");
                if (string.IsNullOrWhiteSpace(input[4]))
                    result.Add("Transaction Date is missing.");

            }
            else
            {
                result.Add("Invalid format.");
            }

            if (result.Count > 0)
            {
                result.Insert(0, $"Transaction Row Number: {rowNo}");
            }

            return result;
        }

    }
}
