using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Common.Validation
{
    public interface IPasswordWithConfirmation
    {
        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }
    }

    public class PasswordValidator : AbstractValidator<IPasswordWithConfirmation>
    {
        public PasswordValidator()
        {
            RuleFor(v => v.Password)
                .NotNull()
                .Length(6, 256);

            RuleFor(v => v.PasswordConfirmation)
                .NotNull()
                .Length(6, 256)
                .Equal(v => v.Password)
                    .WithMessage("Password and Confirmation must be equal");
        }
    }
}