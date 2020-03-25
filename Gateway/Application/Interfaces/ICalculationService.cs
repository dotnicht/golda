using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ICalculationService
    {
        TimeSpan MiningRequestWindow { get; }
        TimeSpan WeeklyTimeout { get; }
        TimeSpan InstantTimeout { get; }
        decimal InstantMiningFee { get; }
        Dictionary<int, int> InstantBoostMapping { get; }
        Task<decimal> GenerateDefaultReward();
        Task<(decimal Amount, TransactionType Type)> GenerateWeeklyReward();
        Task<(decimal Amount, TransactionType Type)> GenerateBonusReward();
        Task<decimal> GenerateInstantReward();
        Task<Promotion> GeneratePromotion(int index);
    }
}
