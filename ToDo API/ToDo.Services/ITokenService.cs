using System;
using ToDo.Models.Account;

namespace ToDo.Services
{
    public interface ITokenService
    {
        public string CreateToken(ApplicationUserIdentity user);
    }
}
