using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Models.UserToDo;
using System.Threading.Tasks;

namespace ToDo.Repository.UserToDo
{
    public interface IUserToDoRepository
    {
        public Task<Models.UserToDo.UserToDo> UpsertAsync(UserToDoCreate userToDoCreate, int applicationUserId);

        public Task<Models.UserToDo.UserToDo> GetAsync(int userToDoId);

        public Task<List<Models.UserToDo.UserToDo>> GetAllByUserIdAsync(int applicationUserId, bool isComplete);

        public Task<int> DeleteAsync(int userToDoId);
    }
}
