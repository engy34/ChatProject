using JIC.ChatApp.efmodels;
using JIC.ChatApp.HubModels;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JIC.ChatApp.HubConfig
{
    public interface IChat
    {
        Task userOFF(Guid currUserId);
        Task userOn(User user);
        Task authMeResponseFail();
        Task reauthMeResponse(User newUser);
        Task authMeResponseSuccess(User user);
        Task getOnlineUsersResponse(List<User> users);
        Task getAvailableGroupsResponse(List<Groups> AvailableGroups);
        Task GetUserMsgsResponse(List<Messages> Messages );
        Task logoutResponse();
        Task sendMsgResponse(string connId,string msg);
        Task SendGrpMsgResponse(string senderName,string grpID,string msg);
        Task CreateGroupResponseSuccess(Groups CurrGroup);
        Task GroupCreatedResponse(Groups CurrGroup);
       
    }
}
