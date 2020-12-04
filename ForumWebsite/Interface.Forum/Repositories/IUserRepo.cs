using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models;

namespace Interface.Forum.Repositories
{
    public interface IUserRepo
    {
        //資料庫錯誤時的自訂回傳錯誤訊息
        string DBError { set; get; }
        /// <summary>
        /// 獲取用戶資訊
        /// </summary>
        /// <param name="id">用戶編號</param>
        /// <returns></returns>
        user_Tb GetUserInfo_Md(int id);/// <summary>
        /// 獲取用戶資訊
        /// </summary>
        /// <param name="id">用戶帳號</param>
        /// <returns></returns>
        user_Tb GetUserInfo_Md(string account);
        /// <summary>
        /// 用戶總數
        /// </summary>
        /// <returns></returns>
        int UsersCount_Md();

        #region 資料異動
        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="addUserInfo">新增會員資訊</param>
        /// <returns></returns>
        bool InsertUser_Md(user_Tb addUserInfo);
        /// <summary>
        /// 更新會員資料
        /// </summary>
        /// <param name="userId">會員資訊</param>
        /// <returns></returns>
        bool UpdateUser_Md(user_Tb updateUserInfo);
        /// <summary>
        /// 刪除會員
        /// </summary>
        /// <param name="userId">會員編號</param>
        /// <returns></returns>
        bool DeleteUser_Md(int deleteUserId);
        #endregion
    }
}