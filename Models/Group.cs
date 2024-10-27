using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class Group
    {
        public Group()
        {
            GroupMembers = new HashSet<GroupMember>();
        }

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual ICollection<GroupMember> GroupMembers { get; set; }
    }
}
