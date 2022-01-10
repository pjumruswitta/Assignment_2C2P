using Assignment_2C2P.Models;
using Assignment_2C2P.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<List<InvoiceTransactionResponse>> GetInvoiceByStatus(string status)
        {
            return await _invoiceService.GetInvoices(INVOICE_SEARCH_MODE.STATUS, new InvoiceSearchRequest { Status = status });
        }

        [HttpGet]
        [Route("Currency")]
        public async Task<List<InvoiceTransactionResponse>> GetInvoiceByCurrency(string currency)
        {
            return await _invoiceService.GetInvoices(INVOICE_SEARCH_MODE.CURRENCY, new InvoiceSearchRequest { Currency = currency });
        }

        [HttpGet]
        [Route("DateRange")]
        public async Task<List<InvoiceTransactionResponse>> GetInvoiceByDateRange(DateTime dateFrom, DateTime dateTo)
        {
            return await _invoiceService.GetInvoices(INVOICE_SEARCH_MODE.DATE_RANGE, new InvoiceSearchRequest { DateFrom = dateFrom, DateTo = dateTo });
        }
    }
}
