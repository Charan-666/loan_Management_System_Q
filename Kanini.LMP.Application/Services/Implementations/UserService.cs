using Kanini.LMP.Application.Services.Implementations;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.Enums;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Kanini.LMP.Database.Entities.CustomerEntities;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class UserService : Kanini.LMP.Data.Repositories.Interfaces.IUser
    {
        private readonly ILMPRepository<User, int> _userRepository;
        private readonly ILMPRepository<Customer, int> _customerRepository;
        private readonly IEmailService _emailService;
        private static readonly Dictionary<string, (string Token, DateTime Expiry)> _resetTokens = new();

        public UserService(ILMPRepository<User, int> userRepository, ILMPRepository<Customer, int> customerRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _emailService = emailService;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetAsync(u => u.Email == username);
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                PasswordHash = PasswordService.HashPassword(userDto.Password),
                Roles = userDto.Roles,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.AddAsync(user);
            return MapToDto(created);
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<IReadOnlyList<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDTO> RegisterCustomerAsync(CustomerRegistrationDTO registrationDto)
        {
            // Check if email already exists
            var existingUser = await _userRepository.GetAsync(u => u.Email == registrationDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already registered");
            }

            // Create User
            var user = new User
            {
                FullName = registrationDto.FullName,
                Email = registrationDto.Email,
                PasswordHash = PasswordService.HashPassword(registrationDto.Password),
                Roles = UserEnums.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);

            // Create Customer profile
            var customer = new Customer
            {
                UserId = createdUser.UserId,
                DateOfBirth = registrationDto.DateOfBirth,
                Gender = registrationDto.Gender,
                PhoneNumber = registrationDto.PhoneNumber,
                Occupation = registrationDto.Occupation,
                AnnualIncome = registrationDto.AnnualIncome,
                CreditScore = registrationDto.CreditScore,
                HomeOwnershipStatus = registrationDto.HomeOwnershipStatus,
                UpdatedAt = DateTime.UtcNow,
                ProfileImage = new byte[0] // Empty profile image initially
            };

            await _customerRepository.AddAsync(customer);

            return MapToDto(createdUser);
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetAsync(u => u.Email == email);
            if (user == null) return false;

            var resetToken = Guid.NewGuid().ToString();
            _resetTokens[email] = (resetToken, DateTime.UtcNow.AddHours(1));

            var emailDto = new Database.EntitiesDto.Email.EmailDto
            {
                ToEmail = email,
                Subject = EmailTemplates.PasswordResetSubject,
                Body = string.Format(EmailTemplates.PasswordResetBody, user.FullName, resetToken)
            };

            await _emailService.SendEmailAsync(emailDto);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string resetToken, string newPassword)
        {
            var user = await _userRepository.GetAsync(u => u.Email == email);
            if (user == null) return false;

            if (!_resetTokens.TryGetValue(email, out var tokenData) ||
                tokenData.Token != resetToken ||
                tokenData.Expiry < DateTime.UtcNow)
                return false;

            user.PasswordHash = PasswordService.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _resetTokens.Remove(email);

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!PasswordService.VerifyPassword(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = PasswordService.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        private UserDTO MapToDto(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Password = "***", // Never return actual password
                Roles = user.Roles,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
