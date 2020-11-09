using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForumWebsite.Models
{
    /// <summary>
    /// 以免數據庫更新而參數被刪除
    /// </summary>
    [MetadataType(typeof(UserMetaData))]
    public partial class user_Tb
    {
        private class UserMetaData
        {
            [DisplayName("用戶編號")]
            public int user_id { get; set; }
            [DisplayName("帳號")]
            [Required(ErrorMessage = "請輸入帳號")]
            [RegularExpression("(\\w){5,16}", ErrorMessage = "只能輸入介於5~16個英數字_字元")]
            [StringLength(16, MinimumLength = 5, ErrorMessage = "帳號需介於5~16個字元")]
            public string account { get; set; }
            [DisplayName("密碼")]
            [Required(ErrorMessage = "請輸入密碼")]
            [RegularExpression(".{5,16}", ErrorMessage = "密碼需介於5~16個字元")]
            [StringLength(16, MinimumLength = 5, ErrorMessage = "密碼需介於5~16個字元")]
            public string password { get; set; }
            [DisplayName("用戶名稱")]
            [Required(ErrorMessage = "請輸入用戶名稱")]
            [MaxLength(20, ErrorMessage ="姓名不可超過20字元")]
            public string username { get; set; }
            [DisplayName("性別")]
            public Nullable<bool> gender { get; set; }
            [DisplayName("E-mail")]
            [Required(ErrorMessage = "請輸入E-mail")]
            [EmailAddress(ErrorMessage = "須符合信箱格式")]
            public string email { get; set; }
            [DisplayName("出生日期")]
            [DataType(DataType.Date, ErrorMessage = "格式須符合\"2020/1/1\"")]
            public Nullable<System.DateTime> birthday { get; set; }
            [DisplayName("帳號權限")]
            public string account_right { get; set; }
        }
    }
}