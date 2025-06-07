using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class AssetHistoryService : IAssetHistoryService
    {
        private readonly IAssetHistoryRepository _assetHistoryRepository;

        public AssetHistoryService(IAssetHistoryRepository assetHistoryRepository)
        {
            _assetHistoryRepository = assetHistoryRepository;
        }
        public async Task<bool> AddAsync(AssetHistoryDto dto)
        {
            try
            {
                AssetHistory entity = new()
                {
                    Id = 0,
                    AssetId = dto.AssetId,
                    HistoryValueDate = dto.HistoryValueDate,
                    Value = dto.Value
                };

                AssetHistory? returnEntity = await _assetHistoryRepository.AddAsync(entity);
                if (returnEntity == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateAsync(AssetHistoryDto dto)
        {
            try
            {
                var entityDb = await _assetHistoryRepository.GetById(dto.Id);

                if (entityDb == null)
                {
                    return false;
                }

                AssetHistory entity = new()
                {
                    Id = dto.Id,
                    AssetId = entityDb.AssetId,
                    HistoryValueDate = entityDb.HistoryValueDate,
                    Value = dto.Value
                };
                AssetHistory? returnEntity = await _assetHistoryRepository.UpdateAsync(entity.Id, entity);
                if (returnEntity == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                await _assetHistoryRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<AssetHistoryDto?> GetById(int id)
        {
            try
            {
                var listEntitiesQuery = _assetHistoryRepository.GetAllQueryWithInclude(["Asset"]);

                var entity = await listEntitiesQuery.FirstOrDefaultAsync(a => a.Id == id);

                if (entity == null)
                {
                    return null;
                }

                var dto = new AssetHistoryDto()
                {
                    Id = entity.Id,
                    AssetId = entity.AssetId,
                    HistoryValueDate = entity.HistoryValueDate,
                    Value = entity.Value,
                    Asset = entity.Asset == null ? null : new AssetDto()
                    {
                        Id = entity.Asset.Id,
                        Name = entity.Asset.Name,
                        Description = entity.Asset.Description,
                        AssetTypeId = entity.Asset.AssetTypeId,
                        Symbol = entity.Asset.Symbol,
                    }
                };

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<AssetHistoryDto>> GetAll()
        {
            try
            {
                var listEntities = await _assetHistoryRepository.GetAllList();

                var listEntityDtos = listEntities.Select(s =>
                new AssetHistoryDto()
                {
                    Id = s.Id,
                    AssetId = s.AssetId,
                    HistoryValueDate = s.HistoryValueDate,
                    Value = s.Value,
                }).ToList();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<List<AssetHistoryDto>> GetAllWithInclude()
        {
            try
            {
                var listEntitiesQuery = _assetHistoryRepository.GetAllQueryWithInclude(["Asset"]);

                var listEntityDtos = await listEntitiesQuery.Select(s =>
                new AssetHistoryDto()
                {
                    Id = s.Id,
                    AssetId = s.AssetId,
                    HistoryValueDate = s.HistoryValueDate,
                    Value = s.Value,
                    Asset = s.Asset == null ? null : new AssetDto()
                    {
                        Id = s.Asset.Id,
                        Name = s.Asset.Name,
                        Description = s.Asset.Description,
                        AssetTypeId = s.Asset.AssetTypeId,
                        Symbol = s.Asset.Symbol,
                    }
                }).ToListAsync();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}