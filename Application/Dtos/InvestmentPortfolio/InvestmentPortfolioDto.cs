using InvestmentApp.Core.Application.Dtos.User;

namespace InvestmentApp.Core.Application.Dtos.InvestmentPortfolio
{
    public class InvestmentPortfolioDto : BasicDto<int>
    {
        public required int UserId { get; set; } // FK
        public UserDto? User { get; set; }
    }
}
