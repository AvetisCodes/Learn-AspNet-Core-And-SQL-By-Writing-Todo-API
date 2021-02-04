using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Models.UserToDo;

namespace ToDo.Repository.UserToDo
{
    public class UserToDoRepository : IUserToDoRepository
    {
        private readonly IConfiguration _config;

        public UserToDoRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> DeleteAsync(int userToDoId)
        {
            int affectedRows = 0;
            
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                affectedRows = await connection.ExecuteAsync("UserToDo_Delete",
                    new { UserToDoId = userToDoId },
                    commandType: CommandType.StoredProcedure);
            }

            return affectedRows;
        }

        public async Task<List<Models.UserToDo.UserToDo>> GetAllByUserIdAsync(int applicationUserId, bool isComplete)
        {
            IEnumerable<Models.UserToDo.UserToDo> userToDos;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                userToDos = await connection.QueryAsync<Models.UserToDo.UserToDo>(
                    "UserToDo_GetByUserId",
                    new { ApplicationUserId = applicationUserId, IsComplete = isComplete },
                    commandType: CommandType.StoredProcedure
                    );
            }

            return userToDos.ToList();
        }

        public async Task<Models.UserToDo.UserToDo> GetAsync(int userToDoId)
        {
            Models.UserToDo.UserToDo userToDo;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                userToDo = await connection.QueryFirstOrDefaultAsync<Models.UserToDo.UserToDo>(
                    "UserToDo_Get",
                    new { UserToDoId = userToDoId },
                    commandType: CommandType.StoredProcedure
                    );
            }

            return userToDo;
        }

        public async Task<Models.UserToDo.UserToDo> UpsertAsync(UserToDoCreate userToDoCreate, int applicationUserId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserToDoId", typeof(int));
            dataTable.Columns.Add("CategoryId", typeof(int));
            dataTable.Columns.Add("Title", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("IsComplete", typeof(bool));

            dataTable.Rows.Add(
                userToDoCreate.UserToDoId,
                userToDoCreate.CategoryId,
                userToDoCreate.Title,
                userToDoCreate.Description,
                userToDoCreate.IsComplete
                );

            int? userToDoId;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                userToDoId = await connection.ExecuteScalarAsync<int?>(
                    "UserToDo_Upsert",
                    new
                    {
                        UserToDo = dataTable.AsTableValuedParameter("dbo.UserToDoType"),
                        ApplicationUserId = applicationUserId
                    },
                    commandType: CommandType.StoredProcedure
                    );
            }

            userToDoId = userToDoId ?? userToDoCreate.UserToDoId;

            Models.UserToDo.UserToDo userToDo = await GetAsync(userToDoId.Value);

            return userToDo;
        }
    }
}
