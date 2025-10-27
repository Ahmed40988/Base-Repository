
namespace Web.Application.DTOs.AccountDTO;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);