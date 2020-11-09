using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumWebsite.Models.Other
{
    internal class InternalVal
    {
        //Session會員資訊名稱
        public readonly static string _SESSIONUSERINFO = "";
        public readonly static string _SESSIONACCOUNT = "account";
        public readonly static string _SESSIONNAME = "name";
        public readonly static string _SESSIONID = "sessionID";
        public readonly static string _SESSIONID_COMPARE = "sessionIdCompare";

        public readonly static string _COOKIEUSERINFO = "userInfo";
        public readonly static string _COOKIEACCOUNT = "account";
        public readonly static string _COOKIEANAME = "name";
        public readonly static string _COOKIE_ASP_SESSIONID = "ASP.NET_SessionId";

        public readonly static string _RESULTMSG = "message";
    }
}