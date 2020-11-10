using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models.Interface.Repository;
using ForumWebsite.Models.Interface.Service;
using ForumWebsite.Models.Repository;
using System.Web.UI;

namespace ForumWebsite.Models.Service
{
    public class Service_User : IService_User
    {
        //獲取網頁資訊
        private HttpContext httpVal = System.Web.HttpContext.Current;
        protected IRepository_Users Repository_Users_P { get; private set; }
        public Service_User() : this(null) { }
        public Service_User(IRepository_Users Repository_Users_Val)
        {
            Repository_Users_P = Repository_Users_Val ?? new Repository_Users();
        }
        public bool DeleteUser_Md(int deleteUserId)
        {
            return Repository_Users_P.DeleteUser_Md(deleteUserId);
        }

        /// <summary>
        /// 使用用戶編號查詢資料
        /// </summary>
        /// <param name="id">用戶編號</param>
        /// <returns></returns>
        public user_Tb GetUserInfo_Md(int id)
        {
            return Repository_Users_P.GetUserInfo_Md(id);
        }
        /// <summary>
        /// 使用用戶帳號查詢資料
        /// </summary>
        /// <param name="account">用戶帳號</param>
        /// <returns></returns>
        public user_Tb GetUserInfo_Md(string account)
        {
            return Repository_Users_P.GetUserInfo_Md(account);
        }

        public bool InsertUser_Md(user_Tb addUserInfo)
        {
            return Repository_Users_P.InsertUser_Md(addUserInfo);
        }

        public bool UpdateUser_Md(user_Tb updateUserInfo)
        {
            return Repository_Users_P.UpdateUser_Md(updateUserInfo);
        }

        public int UsersCount_Md()
        {
            return Repository_Users_P.UsersCount_Md();
        }
    }
}