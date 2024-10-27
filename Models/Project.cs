using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class Project
    {
        public Project()
        {
            Files = new HashSet<File>();
            Tasks = new HashSet<Task>();
        }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
