using InvestmentApp.Core.Application.Dtos.AssetHistory;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IAssetHistoryService
    {
        Task<bool> AddAsync(AssetHistoryDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<AssetHistoryDto>> GetAll();
        Task<List<AssetHistoryDto>> GetAllWithInclude();
        Task<AssetHistoryDto?> GetById(int id);
        Task<bool> UpdateAsync(AssetHistoryDto dto);
    }
}