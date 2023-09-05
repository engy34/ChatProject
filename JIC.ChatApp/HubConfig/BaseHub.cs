using JIC.ChatApp.efmodels;
using JIC.ChatApp.HubConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace JIC.ChatApp.HubModels
{
    public class BaseHub : Hub<IChat>
    {
        private readonly JICChatAppContext _context;
        private static Dictionary<string, string> ChatGroups = new Dictionary<string, string>();
        public BaseHub(JICChatAppContext context)
        {
            _context = context;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Guid currUserId = _context.Connection.Where(c => c.SignalId == Context.ConnectionId).Select(c => c.PersonId).SingleOrDefault();
            _context.Connection.RemoveRange(_context.Connection.Where(x => x.PersonId == currUserId).ToList());
            ; _context.SaveChanges();
            Clients.Others.userOFF(currUserId);
            return base.OnDisconnectedAsync(exception);
        }
        public async Task authMe(PersonInfo personInfo)
        {
            string currSignalrID = Context.ConnectionId;
            Person tempPerson =await _context.Person.Where(p => p.Username == personInfo.userName && p.Password == personInfo.password)
                .SingleOrDefaultAsync();
          


            if (tempPerson != null) //if credentials are correct
            {
                Console.WriteLine("\n" + tempPerson.Name + " logged in" + "\nSignalrID: " + currSignalrID);

                Connection currUser = new Connection
                {
                    PersonId = tempPerson.Id,
                    SignalId = currSignalrID,
                    TimeStamp = DateTime.Now
                };
                await _context.Connection.AddAsync(currUser);
                await _context.SaveChangesAsync();

                User newUser = new User(tempPerson.Id, tempPerson.Name, currSignalrID);

                await Clients.Caller.authMeResponseSuccess(newUser); //for the sender himself
                await Clients.Others.userOn(newUser);//for others except user

            }

            else //if credentials are incorrect
            {
                await Clients.Caller.authMeResponseFail();
            }
        }
        public async Task askServer(string ClientText)
        {
            string tempString;
            if (ClientText == "key")
            {
                tempString = "message was 'key'";
            }
            else
            {
                tempString = "message was somethign else";
            }
            await Clients.Clients(this.Context.ConnectionId).askServerResponse();

        }

        public async Task reauthMe(Guid personId)
        {
            string currSignalrId = Context.ConnectionId;
            Person tempPerson =await _context.Person.Where(p => p.Id == personId).SingleOrDefaultAsync();
            var tempConn = await _context.Connection.Where(p => p.PersonId == personId).ToListAsync();
            List<string> GroupIds=await _context.GroupUsers.Where(p=>p.UserId==personId.ToString()).Select(p=> p.GroupId).ToListAsync();
            var groupNames = await _context.Groups.Where(p => GroupIds.Contains(p.GroupId.ToString())).Select(p => p.GroupName).ToListAsync();
              
            if (tempPerson != null)
            {   if(tempConn!=null)
                {
                   _context.Connection.RemoveRange(tempConn);
                    await _context.SaveChangesAsync();
                }
                Console.WriteLine("\n" + tempPerson.Name + "logged in " + "\n signalrId" + currSignalrId);
                Connection currUser = new Connection
                {
                    PersonId = tempPerson.Id,
                    SignalId = currSignalrId,
                    TimeStamp = DateTime.Now
                };

                await _context.Connection.AddAsync(currUser);
                await _context.SaveChangesAsync();

                User newUser = new User(tempPerson.Id, tempPerson.Name, currSignalrId);
                await Clients.Caller.reauthMeResponse(newUser);
                await Clients.Others.userOn(newUser);
                foreach (var name in groupNames)
                {
                    await Groups.AddToGroupAsync(currSignalrId, name);
                }
            }
        }
        public async Task getOnlineUsers()
        {
            Guid currUserId = _context.Connection.Where(c => c.SignalId == Context.ConnectionId).Select(c => c.PersonId).SingleOrDefault();
            List<MessageData> messageData = await _context.Messages.Where(p => p.UserId != currUserId
                 && string.IsNullOrEmpty(p.GroupId.ToString())).Select(
                 x => new MessageData
                 {
                     Id = x.Id,
                     UserId = x.UserId,
                     GroupId = x.GroupId,
                     Msg = x.Msg,
                     Time = x.Time
                 }).ToListAsync();
            List<User> onlineUsers = _context.Connection
                .Where(c => c.PersonId != currUserId)
                .Select(c =>
                    new User(c.PersonId, _context.Person.Where(p => p.Id == c.PersonId).Select(p => p.Name).SingleOrDefault(), c.SignalId,messageData)
                ).ToList();
            await Clients.Caller.getOnlineUsersResponse(onlineUsers);
        }


        public async Task getAvailableGroups(string userId)
        {
            List<string> RelatedUserGroups = _context.GroupUsers.Where(c => c.UserId == userId).Select(x => x.GroupId).Distinct().ToList();
            var AvailableGroups = _context.Groups.Where(x => RelatedUserGroups.Contains(x.GroupId.ToString())).Select(c =>
            new Groups { GroupId = c.GroupId, GroupName = c.GroupName }).ToList();
            await Clients.Caller.getAvailableGroupsResponse(AvailableGroups);

        }
        public void logOut(Guid personId)
        {

            _context.Connection.RemoveRange(_context.Connection.Where(p => p.PersonId == personId).ToList());
            _context.SaveChanges();
            Clients.Caller.logoutResponse();
            Clients.Others.userOFF(personId);


        }
        public async Task sendMsg(string connId, string msg)
        {

            await Clients.Client(connId).sendMsgResponse(Context.ConnectionId, msg);

        }
        public async Task SendGrpMsg(string senderId, string groupId, string message)
        {
            try
            {
                Messages newMsg = new Messages
                {
                    Id =  Guid.NewGuid(),
                    UserId = Guid.Parse(senderId),
                    GroupId = Guid.Parse(groupId),
                    Msg = message,
                    Time = DateTime.Now,
                };
                await _context.Messages.AddAsync(newMsg);
                await _context.SaveChangesAsync();
               
                var senderName=await _context.Person.Where(x=>x.Id==Guid.Parse(senderId)).Select(x => x.Name).FirstOrDefaultAsync();
                var groupName = await _context.Groups.Where(x => x.GroupId.ToString() == groupId).Select(x => x.GroupName).FirstOrDefaultAsync();
               await Clients.OthersInGroup(groupName).SendGrpMsgResponse(senderName, groupId, message);
            }
            catch (Exception ex) { 
            }
        }


        public async Task CreateGroup(List<GroupData> GroupData)
        {
            try
            {
                List<GroupUsers> userGroupList = new List<GroupUsers>();
                var connectionId = Context.ConnectionId;


                Groups CurrGroup = new Groups
                {
                    GroupId = Guid.NewGuid(),
                    GroupName = GroupData[0].GroupName,


                };
                await _context.Groups.AddAsync(CurrGroup);

                foreach (var data in GroupData)
                {
                    GroupUsers userGroup = new GroupUsers
                    {
                        Id = Guid.NewGuid(),
                        UserId = data.UserId.ToString(),
                        GroupId = CurrGroup.GroupId.ToString()
                    };
                    ChatGroups[data.ConnId] = data.GroupName;

                    await Groups.AddToGroupAsync(data.ConnId, data.GroupName);
                    
                    userGroupList.Add(userGroup);

                }
                await _context.GroupUsers.AddRangeAsync(userGroupList);
                await _context.SaveChangesAsync();


                await Clients.Group(CurrGroup.GroupName).GroupCreatedResponse(CurrGroup);
                //await Clients.Caller.CreateGroupResponseSuccess(CurrGroup); //for the sender himself
                //await Clients.Others.GroupCreated(CurrGroup);//for others except user
            }
            catch (Exception ex)
            {
                var tt = ex;
            }

        }
    }
}




