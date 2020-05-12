using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jiggswap.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Jiggswap.Application.Common.Behaviors
{
    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUser;

        public RequestLogger(ILogger<TRequest> logger, ICurrentUserService currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Jiggswap Request: {Name}\n\tUser: {@User}", requestName, _currentUser.Username);

            return Task.CompletedTask;
        }
    }
}