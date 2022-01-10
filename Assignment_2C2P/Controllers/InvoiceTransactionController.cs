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
        public async Task<List<InvoiceTransactionResponse>> Get()
        {
            return await _invoiceService.GetInvoices();
        }
    }
}
