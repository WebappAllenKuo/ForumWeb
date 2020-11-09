using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForumWebsite.Models.Interface.Repository;
using ForumWebsite.Models.SQLHelper;
using ForumWebsite.Models.Other;
using System.Data.Entity;

namespace ForumWebsite.Models.Repository
{
    public class Repository_Users : IRepository_Users
    {
        //資料庫錯誤時的自訂回傳錯誤訊息
        public string DBError { set; get; }
        //Entity資料庫
        public ForumWebsiteDBEntities db { get; private set; }
        public Repository_Users()
        {
            Method_Cs s = new Method_Cs();
            db = Method_Cs.EntityConnention_Md();
        }

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
        /// <param name="id">用戶帳號</param>
        /// <returns></returns>
        public user_Tb GetUserInfo_Md(string account)
        {
            var items = db.user_Tb.FirstOrDefault(s => s.account == account);
            return items;
        }
        public int UsersCount_Md()
        {
            string strSql = "select count(*) from user_tb";
            return (int)SqlHelper.ExecuteScalar(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, null);

        }
        #region 資料異動
        public bool InsertUser_Md(user_Tb addUserInfo)
        {
            try
            {
                string strSql = "insert into user_Tb (account, password, username, gender, email, birthday, account_right) " +
                    "values(@account, @password, @username, @gender, @email, @birthday, @account_right)";
                    //"values('test4', 'test3', 'test3', 'true', 'abc@abc.com1', '2020-10-10', '200')";
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@account", SqlDbType.NVarChar),
                    new SqlParameter("@password", SqlDbType.NVarChar),
                    new SqlParameter("@username", SqlDbType.NVarChar),
                    new SqlParameter("@gender", SqlDbType.Bit),
                    new SqlParameter("@email", SqlDbType.NVarChar),
                    new SqlParameter("@birthday", SqlDbType.Date),
                    new SqlParameter("@account_right", SqlDbType.NVarChar),
                };
            sqlParameters[0].Value = addUserInfo.account;
            sqlParameters[1].Value = new Method_Cs().GetMD5_Md(addUserInfo.password);   //加密
            sqlParameters[2].Value = addUserInfo.username;
            sqlParameters[3].Value = (object)addUserInfo.gender ?? DBNull.Value;    //如果空則使用DB的null
            sqlParameters[4].Value = addUserInfo.email;
            sqlParameters[5].Value = (object)addUserInfo.birthday ?? DBNull.Value;  //如果空則使用DB的null
            sqlParameters[6].Value = addUserInfo.account_right;

            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) >= 1;
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
                string passwd = new Method_Cs().GetMD5_Md(updateUserInfo.password).Trim();
                string strSql = "update user_tb set password = @passwd where user_id = @userId";
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@passwd", SqlDbType.NVarChar),
                    new SqlParameter("@userId", SqlDbType.Int)
                };
                sqlParameters[0].Value = passwd;
                sqlParameters[1].Value = objItem.user_id;
                bool result = SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters)>=1;
                if (!result) return false;
                return true;

            }
            return false;
        }
        public bool DeleteUser_Md(int deleteUserId)
        {
            var objItem = GetUserInfo_Md(deleteUserId);
            //判斷是否有該資料集
            if (objItem == null) return false;

            string strSql = "delete from user_Tb where user_id = @id";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", SqlDbType.Int)
            };
            sqlParameters[0].Value = deleteUserId;
            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnectionStringLocalTransaction, CommandType.Text, strSql, sqlParameters) > 1;

        }

        #endregion

        #region 搬到Service
        
        #endregion
    }
}