using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ForumWebsite.Connection
{
    public class ConnectionFactory
    {
        private readonly string ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Forum_ConnectionString"].ConnectionString;
        public string _connectionString
        {
            get { return ConnectionString; }
        }
        public IDbConnection CreateConnection(string name = "default")
        {
            switch (name)
            {
                case "default":
                    {
                        return new SqlConnection(ConnectionString);
                    }
                default:
                    {
                        throw new Exception("name 不存在。");
                    }
            }
        }
    }
}