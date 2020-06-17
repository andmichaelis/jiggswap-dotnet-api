using FluentValidation;
using Jiggswap.Application.Emails;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Contact.Commands
{
    public class SubmitContactCommand : IRequest<bool>
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Comment { get; set; }
    }

    public class SubmitContactCommandValidator : AbstractValidator<SubmitContactCommand>
    {
        public SubmitContactCommandValidator()
        {
            RuleFor(v => v.Name)
                .MaximumLength(50);

            RuleFor(v => v.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(BeValidEmail)
                .WithMessage("'Email' does not appear to be a valid email format.");

            RuleFor(v => v.Comment)
                .NotEmpty();
        }

        private bool BeValidEmail(string email)
        {
            if (email.Length == 0)
            {
                return true;
            }

            try
            {
                var m = new MailAddress(email);
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }

    public class SubmitContactCommandHandler : IRequestHandler<SubmitContactCommand, bool>
    {
        private readonly IJiggswapEmailer _emailer;

        public SubmitContactCommandHandler(IJiggswapEmailer emailer)
        {
            _emailer = emailer;
        }

        public async Task<bool> Handle(SubmitContactCommand request, CancellationToken cancellationToken)
        {
            await _emailer.SendContactEmail(request.Name, request.Email, request.Comment);

            return true;
        }
    }
}