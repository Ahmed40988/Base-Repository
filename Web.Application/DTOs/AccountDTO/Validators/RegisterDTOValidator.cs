using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Domain.Constants;
using Web.Domain.Enums;

namespace Web.Application.DTOs.AccountDTO.Validators
{
    public class RegisterDTOValidator:AbstractValidator<RegisterDTO>  
    {
        public RegisterDTOValidator()
        {
            RuleFor(x => x.Email)
              .NotEmpty()
              .EmailAddress();


            RuleFor(x => x.Password)
    .NotEmpty()
    .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
    .MaximumLength(12).WithMessage("Password must not exceed 12 characters")
    .Matches(PasswordRegexPatterns.Password).WithMessage("Password must contain uppercase and lowercase letters, numbers, and special characters");


            RuleFor(x => x.ConfirmPassword)
       .NotEmpty().WithMessage("Confirm Password is required.")
       .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.Name)
              .NotEmpty();

        }
    }
}
