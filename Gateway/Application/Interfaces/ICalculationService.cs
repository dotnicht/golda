using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ICalculationService
    {
        Task<decimal> GenerateDefaultReward();
        Task<(decimal Amount, MiningType Type)> GenerateWeeklyReward();
        Task<(decimal Amount, MiningType Type)> GenerateBonusReward();
        Task<decimal> GenerateInstantReward();
        Task<Promotion> GeneratePromotion(int index);
    }
}
