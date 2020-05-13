using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Trades.Commands
{
    public class DeclineTradeCommand : IRequest<bool>
    {
        public Guid TradeId { get; set; }
    }

    public class DeclineTradeCommandValidator : AbstractValidator<DeclineTradeCommand>
    {
        public DeclineTradeCommandValidator()
        {
        }
    }

    public class DeclineTradeCommandHandler : IRequestHandler<DeclineTradeCommand, bool>
    {
        public Task<bool> Handle(DeclineTradeCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}