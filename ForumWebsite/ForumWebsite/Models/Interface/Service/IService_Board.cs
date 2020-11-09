using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumWebsite.Models.Interface.Service
{
    public interface IService_Board
    {
        IList<board_Tb> ListBoard_Md();
    }
}