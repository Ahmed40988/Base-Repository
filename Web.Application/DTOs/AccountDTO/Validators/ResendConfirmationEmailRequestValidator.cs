namespace Web.Application.DTOs.AccountDTO.Validators;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}