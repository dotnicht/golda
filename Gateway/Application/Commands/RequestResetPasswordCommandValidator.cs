using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Commands
{
   public class RequestResetPasswordCommandValidator : AbstractValidator<RequestResetPasswordCommand>
    {
        public RequestResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}
