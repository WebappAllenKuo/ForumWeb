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
    }
}