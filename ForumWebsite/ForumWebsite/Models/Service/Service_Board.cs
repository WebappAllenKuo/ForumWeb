using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ForumWebsite.Models.Repository;
using ForumWebsite.Models.Interface.Repository;
using ForumWebsite.Models.Interface.Service;

namespace ForumWebsite.Models.Service
{
    public class Service_Board : IService_Board
    {
        protected IRepository_Board Repository_Board_P { get; private set; }
        public Service_Board() : this(null) { }
        public Service_Board(IRepository_Board Repository_Board_Val)
        {
            Repository_Board_P = Repository_Board_Val ?? new Repository_Board();
        }
        public IList<board_Tb> ListBoard_Md()
        {
            return Repository_Board_P.ListBoard_Md();
        }
    }
}