using FluentValidation;
using Jiggswap.Application.Emails;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Feedback.Commands
{
    public class SubmitFeedbackCommand : IRequest<bool>
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Comment { get; set; }
    }

    public class SubmitFeedbackValidator : AbstractValidator<SubmitFeedbackCommand>
    {
        public SubmitFeedbackValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty();

            RuleFor(v => v.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Must(email =>
                {
                    try
                    {
                        var m = new MailAddress(email);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                })
                .WithMessage("'Email' does not appear to be a valid email format.");

            RuleFor(v => v.Comment)
                .NotEmpty();
        }
    }

    public class SubmitFeedbackCommandHandler : IRequestHandler<SubmitFeedbackCommand, bool>
    {
        private readonly IJiggswapFeedbackEmail _emailer;

        public SubmitFeedbackCommandHandler(IJiggswapFeedbackEmail emailer)
        {
            _emailer = emailer;
        }

        public async Task<bool> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
        {
            await _emailer.SendForgotPasswordEmail(request.Name, request.Email, request.Comment);

            return true;
        }
    }
}