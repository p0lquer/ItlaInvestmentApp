using InvestmentApp.Core.Application.Dtos.InvestmentAssets;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IInvestmentAssetsService
    {
        Task<bool> AddAsync(InvestmentAssetsDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<InvestmentAssetsDto>> GetAll();
        Task<List<InvestmentAssetsDto>> GetAllWithInclude();
        Task<InvestmentAssetsDto?> GetById(int id);
        Task<bool> UpdateAsync(InvestmentAssetsDto dto);

        Task<InvestmentAssetsDto?> GetByAssetAndPortfolioAsync(int assetId, int portfolioId);
    }
}