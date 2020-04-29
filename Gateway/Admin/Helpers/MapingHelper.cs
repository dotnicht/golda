using Binebase.Exchange.Gateway.Admin.Models;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Binebase.Exchange.Gateway.Admin.Helpers
{
    public class MapingHelper
    {
        public static User MapToUser(ApplicationUser user)
          => user == null
              ? null
              : new User
              {
                  Id = user.Id,
                  UserName = user.UserName,
                  Registered = user.Registered,
                  PhoneNumber = user.PhoneNumber,
                  TwoFactorEnabled = user.TwoFactorEnabled,
                  EmailConfirmed = user.EmailConfirmed
              };
        public static EditUser MapToEditUser(ApplicationUser user)
         => user == null
             ? null
             : new EditUser(user)
             {
                 Id = user.Id,
                 UserName = user.UserName,
                 Registered = user.Registered,
                 PhoneNumber = user.PhoneNumber,
                 TwoFactorEnabled = user.TwoFactorEnabled,
                 EmailConfirmed = user.EmailConfirmed
             };

        public static List<TransactionExt> MapToTransactionExt(Binebase.Exchange.Gateway.Domain.Entities.Transaction[] transactions)
        {
            return MapToTransactionExt(transactions.ToList(), Guid.Empty);
        }
        public static List<TransactionExt> MapToTransactionExt(Binebase.Exchange.Gateway.Domain.Entities.Transaction[] transactions, Guid userId)
        {
            return MapToTransactionExt(transactions.ToList(), userId);
        }

        public static List<TransactionExt> MapToTransactionExt(List<Binebase.Exchange.Gateway.Domain.Entities.Transaction> transactions)
        {
            return MapToTransactionExt(transactions, Guid.Empty);
        }
        public static List<TransactionExt> MapToTransactionExt(List<Binebase.Exchange.Gateway.Domain.Entities.Transaction> transactions, Guid userId)
        {
            List<TransactionExt> result = new List<TransactionExt>();
            foreach (var item in transactions)
            {
                var mapedTrans = MapToTransactionExt(item);
                mapedTrans.UserId = userId;
                result.Add(mapedTrans);
            }
            return result;
        }
        public static TransactionExt MapToTransactionExt(Binebase.Exchange.Gateway.Domain.Entities.Transaction transaction)
         => transaction == null
             ? null
             : new TransactionExt(transaction)
             {
                 Currency = transaction.Currency,
                 Amount = transaction.Amount,
                 DateTime = transaction.DateTime,
                 Type = transaction.Type,
                 CreatedBy = transaction.CreatedBy,
                 LastModifiedBy = transaction.LastModifiedBy,
                 Id = transaction.Id,
                 Created = transaction.Created,
                 LastModified = transaction.LastModified,
                 Hash = transaction.Hash
             };
    }
}
