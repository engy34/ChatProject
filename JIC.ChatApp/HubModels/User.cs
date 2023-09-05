using JIC.ChatApp.efmodels;
using System;
using System.Collections.Generic;

namespace JIC.ChatApp.HubModels
{
    public class User
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string connId { get; set; }

        public ICollection<MessageData> messages { get; set; }
        public User(Guid someId,string someName,string someConn,List<MessageData> someMessages)
        {
            id = someId;
            name = someName;
            connId = someConn;
            messages=someMessages;
        }
        public User(Guid someId, string someName, string someConn)
        {
            id = someId;
            name = someName;
            connId = someConn;

        }
    }
}
