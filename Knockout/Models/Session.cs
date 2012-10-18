using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Knockout.Demo.Models
{
    public class Session
    {
        public Session() {
            LogindDate = DateTime.Now.ToString();
        }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public string LogindDate { get; set; }
        public string Password { get; set; }
    }
}