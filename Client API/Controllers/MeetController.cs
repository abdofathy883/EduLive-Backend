using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetController : ControllerBase
    {
        private IMeetService meetService;
        public MeetController(IMeetService meet)
        {
            meetService = meet;
        }

        public async Task CreateMeetAsync()
        {

        }

        public async Task UpdateMeetAsync()
        {

        }
    }
}
