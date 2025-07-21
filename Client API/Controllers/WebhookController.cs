using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Client_API.Controllers
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly IConfiguration config;
        public WebhookController(IPaymentService _paymentService, IConfiguration config)
        {
            paymentService = _paymentService;
            this.config = config;
        }
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = Stripe.EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], config["Stripe:WebhookSecret"]);
            if (stripeEvent.Type == "Checkout.Session.Completed")
            {
                var session = stripeEvent.Data.Object as Session;
                await paymentService.HandlePaymentSuccessAsync(session.Id);
            }
            return Ok(new { Message = "Webhook received successfully." });
        }
    }
}
