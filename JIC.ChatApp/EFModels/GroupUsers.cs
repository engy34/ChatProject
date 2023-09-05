using System;
using System.Collections.Generic;

#nullable disable

namespace JIC.ChatApp.efmodels
{
    public partial class GroupUsers
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}
