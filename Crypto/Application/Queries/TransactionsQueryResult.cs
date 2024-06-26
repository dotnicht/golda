﻿using AutoMapper;
using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Domain.Enums;
using System;

namespace Binebase.Exchange.CryptoService.Application.Queries
{
    public class TransactionsQueryResult
    {
        public Transaction[] Transactions { get; set; }

        public class Transaction : IMapFrom<Domain.Entities.Transaction>
        {
            public Guid Id { get; set; }
            public Currency Currency { get; set; }
            public TransactionDirection Direction { get; set; }
            public TransactionStatus Status { get; set; }
            public DateTime DateTime { get; set; }
            public string Hash { get; set; }
            public decimal Amount { get; set; }

            public void MappingFrom(Profile profile)
                => profile.CreateMap<Domain.Entities.Transaction, Transaction>()
                    .ForMember(x => x.Currency, x => x.MapFrom(y => y.Address.Currency))
                    .ForMember(x => x.DateTime, x => x.MapFrom(y => y.Confirmed));
        }
    }
}
