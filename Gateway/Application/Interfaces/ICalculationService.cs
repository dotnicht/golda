using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ICalculationService
    {
        Task<decimal> GenerateDefaultReward();
        Task<(decimal Amount, TransactionType Type)> GenerateBonusMiningReward();
        Task<decimal> GenerateInstantMiningReward();
        Task<Promotion> GeneratePromotion(decimal reward);
        Task<TimeSpan> GetMiningRequestWindow();
        Task<TimeSpan> GetBonusTimeout();
        Task<TimeSpan> GetInstantTimeout();
        Task<decimal> GetInstantMiningFee();
        Task<Dictionary<int, int>> GetInstantBoostMapping();
        Task<int> GetCurrentMiningCount();
    }
}
