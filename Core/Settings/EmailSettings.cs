using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Settings
{
    public class EmailSettings
    {
        public string AppPassword { get; set; }
        public string AppEmail { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
    }
}
