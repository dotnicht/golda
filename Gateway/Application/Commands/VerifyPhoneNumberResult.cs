using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class VerifyPhoneNumberResult : IRequest
    {
        public bool Status { get; set; }
    }
}
