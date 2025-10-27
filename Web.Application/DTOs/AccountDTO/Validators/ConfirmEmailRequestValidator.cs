namespace Web.Application.DTOs.AccountDTO.Validators;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.email)
             .NotEmpty()
                .EmailAddress();

        RuleFor(x => x.OTP)
            .NotEmpty();
    }
}