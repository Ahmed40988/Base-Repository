namespace Web.Application.Interfaces
{
    public interface IAccountService
    {
          
            Task<Result<string>> ForgotPasswordAsync(ForgetPasswordDto request);


        Task<Result<TokenDTO>?> GetTokenAsync(LoginDTO loginDto);



            Task<Result> RegisterAsync(RegisterDTO registerDTO);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
            Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPassword);

        Task<Result> ResendConfirmationEmailAsync(ResendConfirmEmailRequest request);
        Task<Result<TokenDTO>?> GetRefreshTokenAsync(string token, string refreshToken);
        Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken);

        }
}
