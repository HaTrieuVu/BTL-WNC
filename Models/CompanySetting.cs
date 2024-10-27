using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class CompanySetting
    {
        public int CompanyId { get; set; }
        public string Theme { get; set; }
        public string Logo { get; set; }

        public virtual Company Company { get; set; }
    }
}
