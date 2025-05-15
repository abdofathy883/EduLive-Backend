using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public enum SubscriptionStatus
    {
        Active,
        Canceled,
        Incomplete,
        IncompleteExpired,
        PastDue,
        Paused,
        Trialing,
        Unpaid
    }
}
