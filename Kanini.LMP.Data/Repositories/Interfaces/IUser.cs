using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Authentication;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface IUser
    {
        Task<User> GetByUsernameAsync(string username);
        Task<UserDTO> CreateUserAsync(UserDTO userDto);
        Task<UserDTO> GetUserByIdAsync(int userId);
        Task<IReadOnlyList<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> RegisterCustomerAsync(CustomerRegistrationDTO registrationDto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string resetToken, string newPassword);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}