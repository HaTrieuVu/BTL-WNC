using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class GroupMember
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; }

        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
    }
}
