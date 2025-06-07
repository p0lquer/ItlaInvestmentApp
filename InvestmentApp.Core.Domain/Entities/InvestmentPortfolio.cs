using InvestmentApp.Core.Domain.Common;

namespace InvestmentApp.Core.Domain.Entities
{
    public class InvestmentPortfolio : BasicEntity<int>
    {
        public required int UserId { get; set; } // FK

        //navigation property
        public User? User { get; set; }

        public ICollection<InvestmentAssets>? InvestmentAssets { get; set; }
    }
}
