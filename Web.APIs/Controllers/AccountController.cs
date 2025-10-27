using Microsoft.AspNetCore.Identity.Data;
using Web.Domain.Constants;

namespace Web.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService,ILogger<AccountController>logger)
        {

            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            _logger.LogInformation("");
            var result = await _accountService.RegisterAsync(registerDto);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        [HttpPost("Login")]
        public async Task<ActionResult<Result<TokenDTO>>> Login(LoginDTO loginDto)
        {
            var result = await _accountService.GetTokenAsync(loginDto);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

        }
        [HttpPost("ForgetPassword")]
        public async Task<ActionResult<Result<string>>> ForgetPassword([FromBody] ForgetPasswordDto request)
        {

            var result = await _accountService.ForgotPasswordAsync(request);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request )
        {
            var result = await _accountService.ConfirmEmailAsync(request);

            return result!.IsSuccess ? Ok()
               :result.ToProblem();
        }


        [HttpPost("resend-confirmation-email- OTP")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmEmailRequest request)
        {
            var result = await _accountService.ResendConfirmationEmailAsync(request);

            return result!.IsSuccess ? Ok()
               : result.ToProblem();
        }

        [HttpPut("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            var result = await _accountService.ResetPasswordAsync(resetPassword);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request )
        {
            var result = await _accountService.GetRefreshTokenAsync(request.Token, request.RefreshToken);

            return result!.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request )
        {
            var result = await _accountService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken );

            return result.IsSuccess ? Ok() : result.ToProblem();
        }



    }
}
