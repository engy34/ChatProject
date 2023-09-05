using System;

namespace JIC.ChatApp.HubModels
{
    public class GroupData
    {
        //public Guid id { get; set; }
        public string UserId { get; set; }
        //public string GroupId { get; set; }
        public string ConnId { get; set; }
        public string GroupName { get; set; }
      
        public GroupData( string userId/*,string groupId*/, string connId, string groupName)
        {
            UserId=userId;
            ConnId = connId;
            GroupName = groupName;
            //GroupId = groupId;
        }
        //public GroupData( string groupId, string groupName)
        //{
         
        //    GroupName = groupName;
        //    GroupId = groupId;
        //}
        //public GroupData(string groupId)
        //{

        //    GroupName = groupName;
        //    GroupId = groupId;
        //}
    }
}
