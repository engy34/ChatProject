using System;

namespace JIC.ChatApp.HubModels
{
    public class MessageData
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? GroupId { get; set; }
        public string Msg { get; set; }
        public DateTime? Time { get; set; }
        private User user { get; set; }
    }
}
