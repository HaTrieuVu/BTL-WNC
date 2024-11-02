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

        public int PlanId { get; set; }     //id gói dịch vụ
        public string PlanName { get; set; }    //tên gói dịch vụ
        public int? MaxUsers { get; set; }
        public int? MaxStorage { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<CompanySubscription> CompanySubscriptions { get; set; }
    }
}
