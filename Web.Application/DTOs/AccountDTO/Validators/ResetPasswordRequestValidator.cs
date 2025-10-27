namespace Web.Application.DTOs.AccountDTO.Validators;
public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.NewPassword)
     .NotEmpty()
     .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
     .MaximumLength(12).WithMessage("Password must not exceed 12 characters")
     .Matches(PasswordRegexPatterns.Password).WithMessage("Password must contain uppercase and lowercase letters, numbers, and special characters");

    }
}