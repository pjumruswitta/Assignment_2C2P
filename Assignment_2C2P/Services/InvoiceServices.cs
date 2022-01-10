using Assignment_2C2P.Models;
using Assignment_2C2P.Repositories;

namespace Assignment_2C2P.Services
{
    public interface IInvoiceServices
    {
        public Task<List<InvoiceTransactionResponse>> GetInvoices();
    }

    public class InvoiceServices : IInvoiceServices
    {

        private readonly InvoiceRepositories _invoiceRepo;

        public InvoiceServices(Assignment_2C2PContext invoiceContext)
        {
            this._invoiceRepo = new InvoiceRepositories(invoiceContext);
        }

        public async Task<List<InvoiceTransactionResponse>> GetInvoices()
        {
            var result = new List<InvoiceTransactionResponse>();

            var rawInvoiceTransactions = await _invoiceRepo.GetAllInvoiceTransactions();
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
