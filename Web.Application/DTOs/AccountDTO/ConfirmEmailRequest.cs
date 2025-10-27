namespace Web.Application.DTOs.AccountDTO;

public record ConfirmEmailRequest(
    string email,
    string OTP
);