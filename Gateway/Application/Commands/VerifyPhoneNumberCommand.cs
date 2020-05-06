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
    public class VerifyPhoneNumberCommand : IRequest<VerifyPhoneNumberResult>
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }

        public class VerifyPhoneNumberCommandHandler : IRequestHandler<VerifyPhoneNumberCommand, VerifyPhoneNumberResult>
        {
            private readonly IPhoneService _phoneService;
            private readonly IIdentityService _identityService;
            private readonly IMapper _mapper;

            public VerifyPhoneNumberCommandHandler(IIdentityService identityService, IPhoneService phoneService, IMapper mapper)
                => (_identityService, _phoneService, _mapper) = (identityService, phoneService, mapper);

            public async Task<VerifyPhoneNumberResult> Handle(VerifyPhoneNumberCommand request, CancellationToken cancellationToken)
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

                var (Sid, IsValid, Errors) = await _phoneService.CheckVerificationAsync(user.PhoneNumber, request.Code);
             
                if (IsValid)
                {
                    var updateResult = await _identityService.SetPhoneNumberVerify(user.Id, true);
                    return new VerifyPhoneNumberResult { Status = updateResult.Succeeded };
                }

                return new VerifyPhoneNumberResult { Status = IsValid };
            }
        }
    }
}
