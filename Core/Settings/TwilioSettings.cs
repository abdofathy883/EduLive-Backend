using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Settings
{
    public class TwilioSettings
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string WhatsappFromNumber { get; set; }
        //public string WhatsappToNumber { get; set; }
        //public string WhatsappApiUrl { get; set; } = "https://api.twilio.com/2010-04-01/Accounts/{0}/Messages.json";
    }
}
