using System;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using Simple.Data.MongoDB;
using System.Collections.Generic;
using Knockout.Demo.Models;

namespace Knockout.Demo.Brokers
{
    public class UserBroker
    {
        public static dynamic CreateSession(string userName, string password, string connectionId) 
        {
            try {
                bool hasManySessions = false;
                var session = new Session {
                    UserName = userName,
                    ConnectionId = connectionId,
                    Password = password
                };
                var ctx = Proxy.Connect("kodemo");
                ctx.Sessions.Insert(session);
                var match = ctx.Users.FindByUserName(userName);
                //Check if user has many sessions open
                var query = ctx.Sessions.FindAllByUserName(userName);
                if (query != null)
                    hasManySessions = query.ToList().Count > 1;

                if(match == null && !string.IsNullOrWhiteSpace(password))
                    ctx.Users.Insert(new {
                        UserName = session.UserName,
                        Password = session.Password,
                        DateCreated = DateTime.Now.ToString()
                    });
                
                return new {
                    UserName = userName,
                    hasManyOpenSessions = hasManySessions
                };

            } catch{
                return null;
            }
        }

        public static bool SigIn(Session session) 
        {
            var ctx = Proxy.Connect("kodemo");
            try {
                var match = ctx.Users.FindByUserName(session.UserName);
                if (match == null)
                    return false;
                else
                    return ((Session)match).Password.Equals(session.Password);
                
            } catch{
                return false;
            }
        }


        public static dynamic RemoveSession(string connectionId)
        {
            try {
                string username=string.Empty;
                var ctx = Proxy.Connect("kodemo");
                var query = ctx.Sessions.FindByConnectionId(connectionId);
                if (query != null) {
                    username = ((Session)query).UserName;
                    ctx.Sessions.DeleteByConnectionId(connectionId);
                }
                //The user may have multiple sessions open
                var query2 = ctx.Sessions.FindAllByUserName(username);
                return new { 
                    UserName = username,
                    hasManyOpenSessions = query2 != null ? query2.ToList().Count > 0 : false
                };
            } catch {
                return null;
            }
        }
    }
}