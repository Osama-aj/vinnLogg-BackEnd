

using System.Security.Claims;
using System.Threading.Tasks;
using basement.core.Entities.UserSection;
using basement.core.Models.UserModels;

namespace basement.infrastructure.Interfaces
{
    public interface IUserService
    {
        bool Create(AddUserModel model, ClaimsPrincipal user);
        bool Update(UpdateUserModel model, ClaimsPrincipal user);
        bool Delete(ClaimsPrincipal user);
        //User GetUserById(long id);

        string GetUserIdFromSession(ClaimsPrincipal user);
       // Task<string> GetUserIdFromFirebaseToken(string token);
        
    }
}