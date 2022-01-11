using Assignment_2C2P.Models;
using Assignment_2C2P.Repositories;

namespace Assignment_2C2P.Services
{
    public interface IInvoiceServices
    {
        public Task<List<InvoiceTransactionResponse>> GetInvoicesTransaction(INVOICE_SEARCH_MODE mode, InvoiceSearchRequest request);
        public Task InsertInvoiceTransaction(List<InvoiceTransaction> input);
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
                            throw new ArgumentOutOfRangeException("Invalid SearchStaus for InvoiceTransaction");
                        rawInvoiceTransactions = await _invoiceRepo.GetInvoiceTransactionsByStatus(request.Status.ToUpper());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), "Invalid SearchMode for InvoiceTransaction");
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
            catch (Exception)
            {
                throw;
            }
        }
        public async Task InsertInvoiceTransaction(List<InvoiceTransaction> input)
        {
            try
            {
                await _invoiceRepo.InsertInvoiceTransactions(input);
            }
            catch (Exception)
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

    }
}
