namespace Web.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Lock")]
        public async Task<IActionResult> LockUser(string email)
        {
            var result = await _userService.LockUserByEmailAsync(email);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }


        //  [Authorize(Roles = "Admin")]
        [HttpPost("Unlock")]
        public async Task<IActionResult> UnlockUser(string email)
        {
            var result = await _userService.UnlockUserByEmailAsync(email);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        // [Authorize(Roles = "Admin")] 
        [HttpDelete("DeleteAccountByEmail")]
        public async Task<IActionResult> DeleteUserByEmail(string email)
        {
            var result = await _userService.DeleteUserByEmailAsync(email);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        // [Authorize(Roles = "Admin")] 
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}
