using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ForumWebsite.Models;
using ForumWebsite.Models.Interface.Repositories;
using Dapper;
using ForumWebsite.Connection;
using System.Data.Entity;

namespace Repositories.Forum
{
    public class UserRepoBiz : IUserRepo
    {
        //資料庫錯誤時的自訂回傳錯誤訊息
        public string DBError { set; get; }
        //Entity資料庫
        public ForumWebsiteDBEntities db { get; private set; }
        //連線資訊
        private string _connection = new ConnectionFactory()._connectionString;
        /// <summary>
        /// 使用用戶編號查詢資料
        /// </summary>
        /// <param name="id">用戶編號</param>
        /// <returns></returns>
        public user_Tb GetUserInfo_Md(int id)
        {
            var items = db.user_Tb.FirstOrDefault(s => s.user_id == id);
            return items;
        }
        /// <summary>
        /// 使用用戶帳號查詢資料
        /// </summary>
        /// <param name="account">用戶帳號</param>
        /// <returns></returns>
        public user_Tb GetUserInfo_Md(string account)
        {
            var items = db.user_Tb.FirstOrDefault(s => s.account == account);
            return items;
        }
        public int UsersCount_Md()
        {
            using (var connection = new SqlConnection(_connection))
            {
                string strSql = "select count(*) from user_tb";
                return (int)connection.ExecuteScalar(strSql);
            }
        }
        #region 資料異動
        public bool InsertUser_Md(user_Tb addUserInfo)
        {
            try
            {
                using (var connection = new SqlConnection(_connection))
                {
                    string strSql = @"insert into user_Tb (
                                    account, password, username, gender, email, 
                                    birthday, account_right
                                    ) values(
                                    @account, @password, @username, @gender, @email, 
                                    @birthday, @account_right
                                )";
                    user_Tb userItem = new user_Tb
                    {
                        account = addUserInfo.account,
                        password = addUserInfo.password,   //加密
                        username = addUserInfo.username,
                        gender = (bool?)addUserInfo.gender,    //如果空則使用DB的null
                        email = addUserInfo.email,
                        birthday = (DateTime?)addUserInfo.birthday,  //如果空則使用DB的null
                        account_right = addUserInfo.account_right
                    };
                    return connection.Execute(strSql, userItem) > 0;
                }
            }
            catch
            {
                //插入資料失敗
                DBError = "資料庫新增失敗!";
                return false;
            }
        }
        public bool UpdateUser_Md(user_Tb updateUserInfo)
        {
            var objItem = GetUserInfo_Md(updateUserInfo.user_id);
            //判斷是否有該資料集 與 傳遞近來account是否與查詢ID的account是否相符
            if (objItem != null && updateUserInfo.account.Trim() == objItem.account)
            {
                objItem.username = updateUserInfo.username.Trim();
                objItem.gender = updateUserInfo.gender;
                objItem.birthday = updateUserInfo.birthday;
                objItem.email = updateUserInfo.email.Trim();
                try
                {
                    db.SaveChanges();
                }
                catch(Exception)
                {
                    throw;
                }
                //更新密碼，因為模型限制只能5-16字元
                //加密
                //string passwd = new Method_Cs().GetMD5_Md(updateUserInfo.password).Trim();
                //string strSql = "update user_tb set password = @passwd where user_id = @userId";
                //SqlParameter[] sqlParameters = new SqlParameter[]
                //{
                //    new SqlParameter("@passwd", SqlDbType.NVarChar),
                //    new SqlParameter("@userId", SqlDbType.Int)
                //};
                //sqlParameters[0].Value = passwd;
                //sqlParameters[1].Value = objItem.user_id;
                //bool result = SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters)>=1;
                //if (!result) return false;
                return true;

            }
            return false;
        }
        public bool DeleteUser_Md(int deleteUserId)
        {
            var objItem = GetUserInfo_Md(deleteUserId);
            //判斷是否有該資料集
            if (objItem == null) return false;
            using (var connection = new SqlConnection(_connection))
            {
                string strSql = "delete from user_Tb where user_id = @id";
                return connection.Execute(strSql, new { id = deleteUserId }) > 0;
            }
        }

        #endregion

    }
}