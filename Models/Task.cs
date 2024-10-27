using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class Task
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedTo { get; set; }
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User AssignedToNavigation { get; set; }
        public virtual Project Project { get; set; }
    }
}
