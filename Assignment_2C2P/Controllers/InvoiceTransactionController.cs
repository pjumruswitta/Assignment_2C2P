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
        public async Task<IActionResult> GetInvoiceByStatus(string status = InvoiceConstant.INV_STATUS_APPROVED)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    return BadRequest("Status is required, [Approved, Failed, Rejected, Finished, Done]");
                }
                return Ok(await _invoiceService.GetInvoicesTransaction(INVOICE_SEARCH_MODE.STATUS, new InvoiceSearchRequest { Status = status }));
            }
            catch (AssignmentException ex)
            {
                if (ex.ExceptionData != null)
                    return BadRequest(ex.ExceptionData);

                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Something went wrong.");
            }
        }

        [HttpGet]
        [Route("Currency")]
        public async Task<IActionResult> GetInvoiceByCurrency(string currency = "USD")
        {
            try
            {
                return Ok(await _invoiceService.GetInvoicesTransaction(INVOICE_SEARCH_MODE.CURRENCY, new InvoiceSearchRequest { Currency = currency }));
            }
            catch (AssignmentException ex)
            {
                if (ex.ExceptionData != null)
                    return BadRequest(ex.ExceptionData);

                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Something went wrong.");
            }
        }

        [HttpGet]
        [Route("DateRange")]
        public async Task<IActionResult> GetInvoiceByDateRange(DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                if (!dateFrom.HasValue) return BadRequest("DateFrom is required.");
                if (!dateTo.HasValue) return BadRequest("DateTo is required.");
                return Ok(await _invoiceService.GetInvoicesTransaction(INVOICE_SEARCH_MODE.DATE_RANGE, new InvoiceSearchRequest { DateFrom = dateFrom.Value, DateTo = dateTo.Value }));
            }
            catch (AssignmentException ex)
            {
                if (ex.ExceptionData != null)
                    return BadRequest(ex.ExceptionData);

                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Something went wrong.");
            }
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                await _invoiceService.InsertInvoiceTransaction(file);
                return Ok();
            }
            catch (AssignmentException ex)
            {
                if (ex.ExceptionData != null)
                    return BadRequest(ex.ExceptionData);

                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Something went wrong.");
            }
        }

    }

}
