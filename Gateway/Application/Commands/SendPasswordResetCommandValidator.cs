using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Commands
{
   public class SendPasswordResetCommandValidator : AbstractValidator<SendPasswordResetCommand>
    {
        public SendPasswordResetCommandValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}
