using AutoMapper;
using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ConfirmCommand : IRequest<SignInCommandResult>
    {
        public Guid Id { get; set; }
        public string Code { get; set; }

        public class ConfirmCommandHandler : IRequestHandler<ConfirmCommand, SignInCommandResult>
        {
            private readonly IIdentityService _identityService;
            private readonly IAccountService _accountService;
            private readonly IMapper _mapper;

            public ConfirmCommandHandler(IIdentityService identityService, IAccountService accountService, IMapper mapper)
                => (_identityService, _accountService, _mapper) = (identityService, accountService, mapper);

            public async Task<SignInCommandResult> Handle(ConfirmCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Id);
                if (user == null)
                {
                    throw new NotFoundException(nameof(User), request.Id);
                }

                var result = await _identityService.ConfirmToken(request.Id, request.Code);
                if (!result.Succeeded)
                {
                    throw result.ToValidationException(nameof(ConfirmCommandHandler)); // TODO: map err codes.
                }

                return _mapper.Map<SignInCommandResult>(user);
            }
        }
    }
}
