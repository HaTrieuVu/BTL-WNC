using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class SubscriptionPlan
    {
        public SubscriptionPlan()
        {
            CompanySubscriptions = new HashSet<CompanySubscription>();
        }

        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int? MaxUsers { get; set; }
        public int? MaxStorage { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<CompanySubscription> CompanySubscriptions { get; set; }
    }
}
