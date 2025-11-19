using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        Task<string?> AuthenticateAsync(string email, string password);
    }
}
