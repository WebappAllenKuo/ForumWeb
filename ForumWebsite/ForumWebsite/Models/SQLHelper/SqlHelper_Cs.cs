using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Collections;
using System.Data.SqlClient;

namespace ForumWebsite.Models.SQLHelper
{


    /// <summary>
    /// 數據庫的通用訪問代碼
    /// 此類為抽象類，不允許實例化，在應用時直接調用即可
    /// </summary>
    public abstract class SqlHelper
    {
        //獲取數據庫連接字符串，其屬於靜態變量且只讀，項目中所有文檔可以直接使用，但不能修改
        public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.ConnectionStrings["Forum_connectionString"].ConnectionString;

        // 哈希表用來存儲緩存的參數信息，哈希表可以存儲任意類型的參數。
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        ///執行一個不需要返回值的SqlCommand命令，通過指定專用的連接字符串。
        /// 使用參數數組形式提供參數列表 
        /// </summary>
        /// <remarks>
        /// 使用示例：
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一個有效的數據庫連接字符串</param>
        /// <param name="commandType">SqlCommand命令類型 (存儲過程， T-SQL語句， 等等。)</param>
        /// <param name="commandText">存儲過程的名字或者 T-SQL 語句</param>
        /// <param name="commandParameters">以數組形式提供SqlCommand命令中用到的參數列表</param>
        /// <returns>返回一個數值表示此SqlCommand命令執行后影響的行數</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //通過PrePareCommand方法將參數逐個加入到SqlCommand的參數集合中
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();

                //清空SqlCommand中的參數列表
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        ///執行一條不返回結果的SqlCommand，通過一個已經存在的數據庫連接 
        /// 使用參數數組提供參數
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        /// int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一個現有的數據庫連接</param>
        /// <param name="commandType">SqlCommand命令類型 (存儲過程， T-SQL語句， 等等。)</param>
        /// <param name="commandText">存儲過程的名字或者 T-SQL 語句</param>
        /// <param name="commandParameters">以數組形式提供SqlCommand命令中用到的參數列表</param>
        /// <returns>返回一個數值表示此SqlCommand命令執行后影響的行數</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 執行一條不返回結果的SqlCommand，通過一個已經存在的數據庫事物處理 
        /// 使用參數數組提供參數
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        /// int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一個存在的 sql 事物處理</param>
        /// <param name="commandType">SqlCommand命令類型 (存儲過程， T-SQL語句， 等等。)</param>
        /// <param name="commandText">存儲過程的名字或者 T-SQL 語句</param>
        /// <param name="commandParameters">以數組形式提供SqlCommand命令中用到的參數列表</param>
        /// <returns>返回一個數值表示此SqlCommand命令執行后影響的行數</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 執行一條返回結果集的SqlCommand命令，通過專用的連接字符串。
        /// 使用參數數組提供參數
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        /// SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一個有效的數據庫連接字符串</param>
        /// <param name="commandType">SqlCommand命令類型 (存儲過程， T-SQL語句， 等等。)</param>
        /// <param name="commandText">存儲過程的名字或者 T-SQL 語句</param>
        /// <param name="commandParameters">以數組形式提供SqlCommand命令中用到的參數列表</param>
        /// <returns>返回一個包含結果的SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);

            // 在這里使用try/catch處理是因為如果方法出現異常，則SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的語句就不會執行，觸發的異常由catch捕獲。
            //關閉數據庫連接，並通過throw再次引發捕捉到的異常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 執行一條返回第一條記錄第一列的SqlCommand命令，通過專用的連接字符串。 
        /// 使用參數數組提供參數
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        /// Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一個有效的數據庫連接字符串</param>
        /// <param name="commandType">SqlCommand命令類型 (存儲過程， T-SQL語句， 等等。)</param>
        /// <param name="commandText">存儲過程的名字或者 T-SQL 語句</param>
        /// <param name="commandParameters">以數組形式提供SqlCommand命令中用到的參數列表</param>
        /// <returns>返回一個object類型的數據，可以通過 Convert.To{Type}方法轉換類型</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 執行一條返回第一條記錄第一列的SqlCommand命令，通過已經存在的數據庫連接。
        /// 使用參數數組提供參數
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        /// Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一個已經存在的數據庫連接</param>
        /// <param name="commandType">SqlCommand命令類型 (存儲過程， T-SQL語句， 等等。)</param>
        /// <param name="commandText">存儲過程的名字或者 T-SQL 語句</param>
        /// <param name="commandParameters">以數組形式提供SqlCommand命令中用到的參數列表</param>
        /// <returns>返回一個object類型的數據，可以通過 Convert.To{Type}方法轉換類型</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 緩存參數數組
        /// </summary>
        /// <param name="cacheKey">參數緩存的鍵值</param>
        /// <param name="cmdParms">被緩存的參數列表</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 獲取被緩存的參數
        /// </summary>
        /// <param name="cacheKey">用於查找參數的KEY值</param>
        /// <returns>返回緩存的參數數組</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            //新建一個參數的克隆列表
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            //通過循環為克隆參數列表賦值
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                //使用clone方法復制參數列表中的參數
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        internal static DataSet ExecuteDataSet(string connectionStringLocalTransaction, CommandType text, string strSql, object p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 為執行命令准備參數
        /// </summary>
        /// <param name="cmd">SqlCommand 命令</param>
        /// <param name="conn">已經存在的數據庫連接</param>
        /// <param name="trans">數據庫事物處理</param>
        /// <param name="cmdType">SqlCommand命令類型 (存儲過程， T-SQL語句， 等等。)</param>
        /// <param name="cmdText">Command text，T-SQL語句 例如 Select * from Products</param>
        /// <param name="cmdParms">返回帶參數的命令</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            //判斷數據庫連接狀態
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            //判斷是否需要事物處理
            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        /// <summary>
        ///     A SqlConnection extension method that executes the data set operation.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>A DataSet.</returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, parameters);
                DataSet ds = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                sda.Fill(ds, "biao");
                cmd.Parameters.Clear();
                return ds;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }




    }
}