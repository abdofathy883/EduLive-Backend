using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Background
{
    public class CertificateSerialNumberGenerator : ValueGenerator<string>
    {
        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();

        public override bool GeneratesTemporaryValues => false;
        private const string Prefix = "TQ";

        public override string Next(EntityEntry entry)
        {
            // Generate a unique 8-character serial number
            // Format: 2 letters followed by 6 numbers (e.g., AB123456)
            lock (_lock)
            {
                StringBuilder serialNumber = new StringBuilder();

                serialNumber.Append(Prefix);

                // Add 6 random digits
                for (int i = 0; i < 6; i++)
                {
                    int digit = _random.Next(10);
                    serialNumber.Append(digit);
                }

                return serialNumber.ToString();
            }
        }
    }
}
