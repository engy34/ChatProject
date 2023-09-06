using System;
using System.Collections.Generic;

#nullable disable

namespace JIC.ChatApp.efmodels
{
    public partial class Messages
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? GroupId { get; set; }
        public string Msg { get; set; }
        public DateTime? Time { get; set; }
    }
}
