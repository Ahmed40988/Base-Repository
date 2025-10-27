using Web.Application.DTOs.UserDTO;
using Web.Application.Errors;

namespace Web.Infrastructure.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<List<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Role = roles.FirstOrDefault()
                    });
                }

                return Result.Success(userDtos);
            }
            catch
            {
                return Result.Failure<List<UserDto>>(UserErrors.UnexpectedServerError);
            }
        }

        public async Task<Result<UserDto>> GetUserDetailsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure<UserDto>(UserErrors.UserNotFound);

            var userDTO = user.Adapt<UserDto>();
            return Result.Success(userDTO);
        }

        public async Task<Result<bool>> EditUserAsync([FromBody] UserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Result.Failure<bool>(UserErrors.UpdateFailed);

            return Result.Success(true);
        }

        public async Task<Result<bool>> DeleteUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Result.Failure<bool>(UserErrors.DeleteFailed);

            return Result.Success(true);
        }

        public async Task<Result<bool>> LockUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var setLockResult = await _userManager.SetLockoutEnabledAsync(user, true);
            var setDateResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            if (!setLockResult.Succeeded || !setDateResult.Succeeded)
                return Result.Failure<bool>(UserErrors.LockFailed);

            return Result.Success(true);
        }


        public async Task<Result<bool>> UnlockUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            if (!result.Succeeded)
                return Result.Failure<bool>(UserErrors.UnlockFailed);

            return Result.Success(true);
        }
    }
}
