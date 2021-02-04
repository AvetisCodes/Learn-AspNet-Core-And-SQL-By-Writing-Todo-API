using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Repository.Category
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly IConfiguration _config;

        public CategoryRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<Models.Category.Category>> GetAll()
        {
            IEnumerable<Models.Category.Category> categories;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                categories = await connection.QueryAsync<Models.Category.Category>(
                    "Category_GetAll",
                    new { },
                    commandType: CommandType.StoredProcedure
                    );
            }

            return categories.ToList();
        }
    }
}
