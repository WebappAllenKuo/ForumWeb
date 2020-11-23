using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models;

namespace ForumWebsite.Models
{
    public class HomeViewModel
    {
        public IList<board_Tb> boardList { get; set; }
        public IList<board_Tb> themeList { get; set; }
        public IList<article_Tb> articleList { get; set; }
        public IList<int> replyCountList { get; set; }
        public int replyCount { get; set; }
        public article_reply_Tb replyAritlceItem { get; set; }
        public IList<article_reply_Tb> replyAritlceList { get; set; }
        public IList<user_Tb> userInfoList { get; set; }
        public board_Tb boardItem { get; set; }
        public user_Tb userItem { get; set; }
        public article_Tb articleItem { get; set; }
    }
}