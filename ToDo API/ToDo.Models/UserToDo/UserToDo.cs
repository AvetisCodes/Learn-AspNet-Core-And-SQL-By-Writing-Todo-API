using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Models.UserToDo
{
    public class UserToDo : UserToDoCreate
    {
        public int ApplicationUserId { get; set; }

        public DateTime PublishDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
