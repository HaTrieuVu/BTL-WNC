using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class Company
    {
        public Company()
        {
            Invoices = new HashSet<Invoice>();
            Projects = new HashSet<Project>();
            Users = new HashSet<User>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Domain { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual CompanySetting CompanySetting { get; set; }
        public virtual CompanySubscription CompanySubscription { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
