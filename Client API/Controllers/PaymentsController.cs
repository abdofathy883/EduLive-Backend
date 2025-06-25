using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly IConfiguration config;
        public PaymentsController(IPaymentService _paymentService, IConfiguration config)
        {
            paymentService = _paymentService;
            this.config = config;
        }
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionDTO paymentRequest)
        {
            if (paymentRequest == null || string.IsNullOrEmpty(paymentRequest.StudentId) || paymentRequest.CourseId <= 0 || paymentRequest.Amount <= 0)
            {
                return BadRequest("Invalid payment request data.");
            }
            try
            {
                var sessionUrl = await paymentService.CreateCheckoutSessionAsync(paymentRequest.StudentId, paymentRequest.CourseId, paymentRequest.Amount);
                return Ok(new { SessionUrl = sessionUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating checkout session: {ex.Message}");
            }
        }

        
    }
}
