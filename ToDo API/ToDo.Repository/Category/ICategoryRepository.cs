using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Repository.Category
{
    public interface ICategoryRepository
    {
        public Task<List<Models.Category.Category>> GetAll();
    }
}
