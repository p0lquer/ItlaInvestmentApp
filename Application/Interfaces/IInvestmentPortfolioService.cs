using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IInvestmentPortfolioService
    {
        Task<bool> AddAsync(InvestmentPortfolioDto dto);

        Task<bool> UpdateAsync(InvestmentPortfolioDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<InvestmentPortfolioDto>> GetAll();
        Task<List<InvestmentPortfolioDto>> GetAllWithIncludeByUser(int userId);
        Task<InvestmentPortfolioDto?> GetById(int id);     
    }
}