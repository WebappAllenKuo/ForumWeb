using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForumWebsite.Models
{
    [MetadataType(typeof(article_reply_TbMetaData))]
    public partial class article_reply_Tb
    {
        private class article_reply_TbMetaData
        {
            public string replyOrder { get; set; }
        }
    }
}