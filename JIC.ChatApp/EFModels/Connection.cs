using System;
using System.Collections.Generic;

#nullable disable

namespace JIC.ChatApp.efmodels
{
    public partial class Connection
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string SignalId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
