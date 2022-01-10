using Assignment_2C2P.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_2C2P.Repositories
{
    public class InvoiceRepositories
    {
        private readonly Assignment_2C2PContext _ctx;
        public InvoiceRepositories(Assignment_2C2PContext ctx)
        {
            _ctx = ctx;
        }
        private IQueryable<InvoiceTransaction> GetAllInvoiceTransactions()
            => _ctx.InvoiceTransactions.Where(trans => trans.IsActive);

        public async Task<List<InvoiceTransaction>> GetInvoiceTransactionsByCurrency(string currency)
        {
            return await GetAllInvoiceTransactions()
                .Where(trans => trans.Currency.ToUpper().Trim().Equals(currency.ToUpper().Trim()))
                .ToListAsync();
        }

        public async Task<List<InvoiceTransaction>> GetInvoiceTransactionsByStatus(string status)
        {
            return await GetAllInvoiceTransactions()
                .Where(trans => trans.Status.ToUpper().Trim().Equals(status.ToUpper().Trim()))
                .ToListAsync();
        }
        public async Task<List<InvoiceTransaction>> GetInvoiceTransactionsByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            return await GetAllInvoiceTransactions()
                .Where(trans
                    => trans.TransactionDate.CompareTo(dateFrom) >= 0
                    && trans.TransactionDate.CompareTo(dateTo) <= 0)
                .ToListAsync();
        }


        public async void InsertInvoiceTransactions(List<InvoiceTransaction> input)
        {
            var transaction = _ctx.Database.BeginTransaction();
            try
            {
                await _ctx.AddRangeAsync(input);
                await _ctx.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
