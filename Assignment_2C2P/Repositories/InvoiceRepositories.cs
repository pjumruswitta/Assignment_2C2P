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
        public async Task<List<InvoiceTransaction>> GetAllInvoiceTransactions()
        {
            return await _ctx.InvoiceTransactions
                .Where(trans => trans.IsActive)
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
