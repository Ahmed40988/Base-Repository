
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Web.Application.DTOs.AccountDTO;
using Web.Application.Errors;
using Web.Application.Helpers;

namespace Web.Infrastructure.Service
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memoryCache;
        private readonly int _refreshTokenExpiryDays = 14;

        public AccountService(
            AppDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            ITokenService tokenService,
            IMemoryCache memoryCache,
               IEmailSender emailSender


            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _memoryCache = memoryCache;
            _emailSender = emailSender;
        }

        public async Task<Result<string>> ForgotPasswordAsync(ForgetPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result.Failure<string>(UserErrors.UserNotFound);

            var otp = new Random().Next(100000, 999999).ToString();
            _memoryCache.Set($"ForgetPassword{request.Email}", otp, TimeSpan.FromMinutes(60));

            await _emailSender.SendEmailAsync(request.Email, "App Name", $"Your verification code is: {otp}");
            return Result.Success("Verification code sent to your email");
        }

        public async Task<Result<string>> VerfiyForgetPasswordOTP(VerfiyCodeDto verify)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(verify.Email);
                if (user == null)
                    return Result.Failure<string>(UserErrors.UserNotFound);

                var cachedOtp = _memoryCache.Get($"ForgetPassword{verify.Email}")?.ToString();

                if (string.IsNullOrEmpty(cachedOtp))
                    return Result.Failure<string>(UserErrors.OTPExpired);

                if (!string.Equals(verify.CodeOTP, cachedOtp, StringComparison.OrdinalIgnoreCase))
                    return Result.Failure<string>(UserErrors.InvalidOTP);


                _memoryCache.Remove($"ForgetPassword{verify.Email}");
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                return Result.Success(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VerifyOTPAsync: {ex.Message}");
                return Result.Failure<string>(UserErrors.UnexpectedServerError);
            }

        }

        public async Task<Result<TokenDTO>?> GetTokenAsync(LoginDTO loginDto)
        {

            if (await _userManager.FindByEmailAsync(loginDto.Email) is not { } user)
                return Result.Failure<TokenDTO>(UserErrors.InvalidCredentials);


            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, true);

            if (result.Succeeded)
            {
                var obj = await _tokenService.GenerateTokenAsync(user, _userManager);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    Expiereson = refreshTokenExpiration
                });

                await _userManager.UpdateAsync(user);

                var response = new TokenDTO
                {
                    UserId = user.Id,
                    Token = obj.Token,
                    expiresIn = obj.expiresIn,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshTokenExpiration
                };
                return Result.Success(response);
            }

            var error = result.IsNotAllowed
                ? UserErrors.EmailNotConfirmed
                : result.IsLockedOut
                ? UserErrors.LockedUser
                : UserErrors.InvalidCredentials;

            return Result.Failure<TokenDTO>(error);
        }

        public async Task<Result> RegisterAsync(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
                return Result.Failure<TokenDTO>(UserErrors.RegisterFailed);

            var isExist = await _userManager.Users.AnyAsync(p => p.Email == registerDTO.Email);
            if (isExist)
                return Result.Failure<TokenDTO>(UserErrors.DuplicatedEmail);

            var user = new AppUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                FullName = registerDTO.Name
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                var errorMsg = result.Errors.FirstOrDefault()?.Description ?? "Failed to create user.";
                return Result.Failure<TokenDTO>(new Error("User.RegisterFailed", errorMsg, 400));
            }

            var otp = new Random().Next(100000, 999999).ToString();
            _memoryCache.Set($"EmailOTP_{registerDTO.Email}", otp, TimeSpan.FromMinutes(5));
            await SendConfirmationEmail(user, otp);
           return Result.Success();
        }

        public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmEmailRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var otp = new Random().Next(100000, 999999).ToString();
            _memoryCache.Set($"EmailOTP_{request.Email}", otp, TimeSpan.FromMinutes(5));

            await SendConfirmationEmail(user, otp);

            return Result.Success();
        }

        public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPassword)
        {
            if (resetPassword.NewPassword != resetPassword.ConfirmNewPassword)
                return Result.Failure<bool>(UserErrors.PasswordMismatch);

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
            if (!result.Succeeded)
                return Result.Failure<bool>(UserErrors.ResetPasswordFailed);

            return Result.Success(true);
        }


        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.email) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);
            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var cachedOtp = _memoryCache.Get($"EmailOTP_{request.email}")?.ToString();

            if (string.IsNullOrEmpty(cachedOtp))
                return Result.Failure(UserErrors.OTPExpired);

            if (cachedOtp != request.OTP)
                return Result.Failure(UserErrors.InvalidOTP);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Result.Success();
            _memoryCache.Remove($"EmailOTP_{request.email}");
            var error = result.Errors.First();
            return Result.Failure(new Error(Code: error.Code,
                Description: error.Description, StatusCodes.Status400BadRequest));


        }


        public async Task<Result<TokenDTO>?> GetRefreshTokenAsync(string token, string refreshToken)
        {
            var userId = _tokenService.ValidateToken(token);

            if (userId is null)
                return Result.Failure<TokenDTO>((UserErrors.InvalidJwtToken));

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure<TokenDTO>((UserErrors.InvalidCredentials));

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.isActive);

            if (userRefreshToken is null)
                return Result.Failure<TokenDTO>((UserErrors.InvalidRefreshToken));

            userRefreshToken.Revokedon = DateTime.UtcNow;
            var obj = await _tokenService.GenerateTokenAsync(user, _userManager);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Expiereson = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new TokenDTO
            {
              UserId= user.Id,
               Token= obj.Token,
               expiresIn= obj.expiresIn,
               RefreshToken= newRefreshToken,
              RefreshTokenExpiration=  refreshTokenExpiration
            };
            return Result.Success(response);
        }


        public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken)
        {
            var userId = _tokenService.ValidateToken(token);


            if (userId is null)
                return Result.Failure((UserErrors.InvalidJwtToken));

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure((UserErrors.InvalidCredentials));


            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.isActive);

            if (userRefreshToken is null)
                return Result.Failure((UserErrors.InvalidRefreshToken));

            userRefreshToken.Revokedon = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private async Task SendResetPasswordEmail(AppUser user, string OTP)
        {

            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword", templateModel: new Dictionary<string, string>
                {
                    { "{{name}}", user.FullName },
                    { "{{OTP}}", $"{OTP}" }
                }
            );

          await _emailSender.SendEmailAsync(user.Email!, "✅ MEGO FOOD: Change Password", emailBody);

            await Task.CompletedTask;
        }

        private async Task SendConfirmationEmail(AppUser user, string OTP)
        {

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FullName },
                { "{{OTP}}", $"{OTP}" }
                }
            );
          await _emailSender.SendEmailAsync(user.Email!, "✅  MEGO FOOD: Email Confirmation", emailBody);

            await Task.CompletedTask;
        }
    }
}
