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
    public class SignInMultiFactorCommand : IRequest<SignInCommandResult>
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }

        public class SignInMultiFactorCommandHandler : IRequestHandler<SignInMultiFactorCommand, SignInCommandResult>
        {
            private readonly IIdentityService _identityService;
            private readonly IMapper _mapper;

            public SignInMultiFactorCommandHandler(IIdentityService identityService, IMapper mapper)
                => (_identityService, _mapper) = (identityService, mapper);

            public async Task<SignInCommandResult> Handle(SignInMultiFactorCommand request, CancellationToken cancellationToken)
            {
                var user = await _identityService.GetUser(request.Id);
                if (user == null)
                {
                    throw new NotFoundException(nameof(User), request.Id);
                }

                if (!await _identityService.GetTwoFactorEnabled(user.Id))
                {
                    throw new NotSupportedException(ErrorCode.MultiFactorRequired);
                }

                if (!await _identityService.CheckUserPassword(user.Id, request.Password))
                {
                    throw new SecurityException(ErrorCode.PasswordMismatch);
                }

                if (!await _identityService.VerifyTwoFactorToken(user.Id, request.Code))
                {
                    throw new SecurityException(ErrorCode.MultiFactor);
                }

                var result = _mapper.Map<SignInCommandResult>(user);
                result.Token = await _identityService.GenerateAuthToken(user);
                return result;
            }
        }
    }
}
