using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Common.Validation
{
    public interface IHasPassword
    {
        public string Password { get; set; }
    }

    public class PasswordValidator : AbstractValidator<IHasPassword>
    {
        public PasswordValidator()
        {
            RuleFor(v => v.Password)
                .NotNull()
                .Length(6, 256);
        }
    }
}