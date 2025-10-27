namespace Web.Application.Interfaces
{
    public interface ITokenService
    {
        Task<(string Token, int expiresIn)> GenerateTokenAsync(AppUser user, UserManager<AppUser> userManager);
        string? ValidateToken(string token);

    }
}
