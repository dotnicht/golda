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
    public class PhoneVerifyRequestCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }

        public class PhoneVerifyRequestCommandHandler : IRequestHandler<PhoneVerifyRequestCommand>
        {
            private readonly IPhoneService _phoneService;
            private readonly IIdentityService _identityService;
            private readonly IMapper _mapper;

            public PhoneVerifyRequestCommandHandler(IIdentityService identityService, IPhoneService phoneService, IMapper mapper)
                => (_identityService, _phoneService, _mapper) = (identityService, phoneService, mapper);

            public async Task<VerifyPhoneNumberResult> Handle(VerifyPhoneNumberCommand request, CancellationToken cancellationToken)
            {
                User user = await _identityService.GetUser(request.Id);

                if (user == null)
                {
                    throw new NotFoundException(nameof(User), request.Id);
                }

                var (Sid, IsValid, Errors) = await _phoneService.StartVerificationAsync(user.PhoneNumber);

                return new VerifyPhoneNumberResult { Status = IsValid };

            }

            public Task<Unit> Handle(PhoneVerifyRequestCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
