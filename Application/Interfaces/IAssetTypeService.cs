using InvestmentApp.Core.Application.Dtos.AssetType;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IAssetTypeService
    {
        Task<bool> AddAsync(AssetTypeDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<AssetTypeDto>> GetAll();
        Task<List<AssetTypeDto>> GetAllWithInclude();
        Task<AssetTypeDto?> GetById(int id);
        Task<bool> UpdateAsync(AssetTypeDto dto);
    }
}