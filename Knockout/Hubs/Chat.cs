using System;
using SignalR.Hubs;
using System.Linq;
using System.Web;
using Knockout.Demo.Brokers;
using Knockout.Demo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace Knockout.Demo.Hubs
{
     [HubName("demo")]
    public class Chat : Hub, IDisconnect, IConnected
    {
         public void CreateUserSession(Session chatSession)
         {
             var result = UserBroker.CreateSession(chatSession.UserName, chatSession.Password, Context.ConnectionId);
            if(result != null &&  !string.IsNullOrEmpty(chatSession.UserName)){
                 Clients[Context.ConnectionId].setCredentials(chatSession.UserName);
                 Clients[Context.ConnectionId].sendMeMessage("System: ", "Your session was successfully created");
                if(!result.hasManyOpenSessions)
                    Clients.broadcast(string.Format("{0} just joined the chat room",chatSession.UserName));
            }else{
                Clients[Context.ConnectionId].sendMeMessage("System", "Error creating your session");
            }
         }

         public void SignIn(Session chatSession)
         {
            var success = UserBroker.SigIn(chatSession);
            if (success)
                CreateUserSession(chatSession);
         }

        public void Send(string name,string message)
        {
            //Clients[Context.ConnectionId].sendMeMessage("Dave", message);
            Clients.broadcast(string.Format("{0}:{1}",name,message));
        }

        public System.Threading.Tasks.Task Connect()
        {
            Clients[Context.ConnectionId].resumeUserSession();
            return null;
        }

        public System.Threading.Tasks.Task Reconnect(IEnumerable<string> groups)
        {
            return null;
        }

        public System.Threading.Tasks.Task Disconnect()
        {
            var result = UserBroker.RemoveSession(Context.ConnectionId);
            if (result != null && !result.hasManyOpenSessions && !string.IsNullOrEmpty(result.UserName.ToString()))    
                Clients.broadcast(string.Format("{0} just left the chat room", result.UserName));
            return null;
        }
    }
}