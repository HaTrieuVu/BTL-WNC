using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class CompanySubscription
    {
        public int CompanyId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Company Company { get; set; }
        public virtual SubscriptionPlan Plan { get; set; }
    }
}
