using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using Simple.Data;
using Simple.Data.MongoDB;
using System.Configuration;

namespace Knockout.Demo.Brokers
{
    public class Proxy
    {
        public static dynamic Connect(string dbName)
        {
            return Database.Opener.OpenMongo(string.Format(ConfigurationManager.AppSettings["mongoProxy"].ToString(), dbName));
        }
    }
}