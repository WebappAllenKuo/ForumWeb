using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models.Interface.Repository;
using ForumWebsite.Models.Interface.Service;
using ForumWebsite.Models.Repository;

namespace ForumWebsite.Models.Service
{
    public class Server_User : IServer_User
    {
        protected IRepository_Users Repository_Users_P { get; private set; }
        public Server_User() : this(null) { }
        public Server_User(IRepository_Users Repository_Users_Val)
        {
            Repository_Users_P = Repository_Users_Val ?? new Repository_Users();
        }
        public bool DeleteUser_Md(int deleteUserId)
        {
            return Repository_Users_P.DeleteUser_Md(deleteUserId);
        }

        public user_Tb GetUserInfo_Md(int id)
        {
            throw new NotImplementedException();
        }

        public user_Tb GetUserInfo_Md(string account)
        {
            throw new NotImplementedException();
        }

        public bool InsertUser_Md(user_Tb addUserInfo)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser_Md(user_Tb updateUserInfo)
        {
            throw new NotImplementedException();
        }

        public int UsersCount_Md()
        {
            throw new NotImplementedException();
        }
    }
}