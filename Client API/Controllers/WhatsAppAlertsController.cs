using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppAlertsController : ControllerBase
    {
        private readonly E_LearningDbContext dbContext;
        private readonly ILogger<WhatsAppAlertsController> logger;
        private readonly IWhatsAppService whatsAppService;

        public WhatsAppAlertsController(E_LearningDbContext _dbContext, ILogger<WhatsAppAlertsController> _logger, IWhatsAppService _whatsAppService)
        {
            logger = _logger;
            dbContext = _dbContext;
            whatsAppService = _whatsAppService;
        }

        //[HttpGet("lesson/{lessonId}")]
    }
}
