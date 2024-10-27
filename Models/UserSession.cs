using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class UserSession
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public bool? IsActive { get; set; }

        public virtual User User { get; set; }
    }
}
