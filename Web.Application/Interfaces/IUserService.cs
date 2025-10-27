using Web.Application.DTOs.UserDTO;

namespace Web.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<List<UserDto>>> GetAllUsersAsync();
        Task<Result<UserDto>> GetUserDetailsAsync(string userId);
        Task<Result<bool>> EditUserAsync(UserDto model);
        Task<Result<bool>> DeleteUserByEmailAsync(string email);
        Task<Result<bool>> LockUserByEmailAsync(string email);
        Task<Result<bool>> UnlockUserByEmailAsync(string email);
    }
}
