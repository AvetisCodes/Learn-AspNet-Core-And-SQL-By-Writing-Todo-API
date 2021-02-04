using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using ToDo.Models.UserToDo;
using ToDo.Repository.UserToDo;

namespace ToDo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserToDoController : ControllerBase
    {
        private readonly IUserToDoRepository _userToDoRepository;

        public UserToDoController(IUserToDoRepository userToDoRepository)
        {
            _userToDoRepository = userToDoRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<UserToDo>> Upsert(UserToDoCreate userToDoCreate)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var foundUserToDo = await _userToDoRepository.GetAsync(userToDoCreate.UserToDoId);

            if (foundUserToDo != null && foundUserToDo.ApplicationUserId != applicationUserId)
            {
                return Unauthorized("Cannot update ToDo.");
            }

            var userToDo = await _userToDoRepository.UpsertAsync(userToDoCreate, applicationUserId);

            return Ok(userToDo);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserToDo>>> GetAllUserPending()
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var pendingToDos = await _userToDoRepository.GetAllByUserIdAsync(applicationUserId, false);

            return Ok(pendingToDos);
        }

        [Authorize]
        [HttpGet("completed")]
        public async Task<ActionResult<List<UserToDo>>> GetAllUserCompleted()
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var completedToDos = await _userToDoRepository.GetAllByUserIdAsync(applicationUserId, true);

            return Ok(completedToDos);
        }

        [Authorize]
        [HttpDelete("{userToDoId}")]
        public async Task<ActionResult<int>> Delete(int userToDoId)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var foundUserToDo = await _userToDoRepository.GetAsync(userToDoId);

            if (foundUserToDo == null)
            {
                return BadRequest("UserToDo does not exist.");
            }

            if (foundUserToDo != null && foundUserToDo.ApplicationUserId != applicationUserId)
            {
                return Unauthorized("Cannot delete ToDo.");
            }

            var affectedRows = await _userToDoRepository.DeleteAsync(userToDoId);

            return Ok(affectedRows);
        }
    }
}
