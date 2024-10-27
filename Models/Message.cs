using System;
using System.Collections.Generic;

#nullable disable

namespace BTL.Models
{
    public partial class Message
    {
        public Message()
        {
            Files = new HashSet<File>();
        }

        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int? GroupId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public string MessageType { get; set; }

        public virtual User Receiver { get; set; }
        public virtual User Sender { get; set; }
        public virtual ICollection<File> Files { get; set; }
    }
}
