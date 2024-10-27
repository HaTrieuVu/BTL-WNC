using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class Invoice
    {
        public int InvoiceId { get; set; }
        public int CompanyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }

        public virtual Company Company { get; set; }
    }
}
