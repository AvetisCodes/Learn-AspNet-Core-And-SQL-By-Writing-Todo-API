using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Models.UserToDo
{
    public class PagedResults<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalCount { get; set; }
    }
}
