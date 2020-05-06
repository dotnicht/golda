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
    public class VerifyPhoneNumberCommand : IRequest<SignInCommandResult>
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }

        public class VerifyPhoneNumberCommandHandler : IRequestHandler<VerifyPhoneNumberCommand, SignInCommandResult>
        {
            private readonly IPhoneService _phoneService;
            private readonly IIdentityService _identityService;
            private readonly IMapper _mapper;

            public VerifyPhoneNumberCommandHandler(IIdentityService identityService, IPhoneService phoneService, IMapper mapper)
                => (_identityService, _phoneService, _mapper) = (identityService, phoneService, mapper);

            public async Task<SignInCommandResult> Handle(VerifyPhoneNumberCommand request, CancellationToken cancellationToken)
            {
                User user = await _identityService.GetUser(request.Id);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), request.Id);
                }

                if (!await _identityService.CheckUserPassword(user.Id, request.Password))
                {
                    throw new SecurityException(ErrorCode.PasswordMismatch);
                }

                var (Sid, IsValid, Errors) = await _phoneService.CheckVerificationAsync(request.PhoneNumber, request.Code);
                if (!IsValid)
                    throw new NotSupportedException(string.Join(". ", Errors));

                var updatePhoneResult = await _identityService.UpdateUserPhoneNumber(request.Id, request.PhoneNumber);
                if (!updatePhoneResult.Succeeded)
                    throw new NotSupportedException(string.Join(". ", updatePhoneResult.Errors));

                var updateResult = await _identityService.SetPhoneNumberVerify(user.Id, true);
                if (!updateResult.Succeeded)
                    throw new NotSupportedException(string.Join(". ", Errors));

                SignInCommandResult result = _mapper.Map<SignInCommandResult>(user);
                var token = await _identityService.GenerateAuthToken(user);
                result.Token = token;

                return result;
            }
        }
    }
}
