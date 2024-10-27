using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class User
    {
        public User()
        {
            Files = new HashSet<File>();
            GroupMembers = new HashSet<GroupMember>();
            Groups = new HashSet<Group>();
            MessageReceivers = new HashSet<Message>();
            MessageSenders = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            Projects = new HashSet<Project>();
            Tasks = new HashSet<Task>();
            UserSessions = new HashSet<UserSession>();
        }

        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int? CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public string Phone { get; set; }
        public int? RoleId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<GroupMember> GroupMembers { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Message> MessageReceivers { get; set; }
        public virtual ICollection<Message> MessageSenders { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<UserSession> UserSessions { get; set; }
    }
}
