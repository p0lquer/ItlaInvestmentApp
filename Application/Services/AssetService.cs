using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Common.Enums;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IInvestmentAssetRepository _investmentAssetRepository;

        public AssetService(IAssetRepository assetRepository, IInvestmentAssetRepository investmentAssetRepository)
        {
            _assetRepository = assetRepository;
            _investmentAssetRepository = investmentAssetRepository;
        }
        public async Task<bool> AddAsync(AssetDto dto)
        {
            try
            {
                Asset entity = new()
                {
                    Id = 0,
                    Name = dto.Name,
                    Description = dto.Description,
                    AssetTypeId = dto.AssetTypeId,
                    Symbol = dto.Symbol
                };

                Asset? returnEntity = await _assetRepository.AddAsync(entity);
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
        public async Task<bool> UpdateAsync(AssetDto dto)
        {
            try
            {
                Asset entity = new()
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    AssetTypeId = dto.AssetTypeId,
                    Symbol = dto.Symbol
                };
                Asset? returnEntity = await _assetRepository.UpdateAsync(entity.Id, entity);
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
                await _assetRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<AssetDto?> GetById(int id)
        {
            try
            {
                var listEntitiesQuery = _assetRepository.GetAllQueryWithInclude(["AssetType"]);

                var entity = await listEntitiesQuery.FirstOrDefaultAsync(a => a.Id == id);

                if (entity == null)
                {
                    return null;
                }

                var dto = new AssetDto()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    Symbol = entity.Symbol,
                    AssetTypeId = entity.AssetTypeId,
                    AssetType = entity.AssetType == null ? null : new AssetTypeDto()
                    {
                        Id = entity.AssetType.Id,
                        Name = entity.AssetType.Name,
                        Description = entity.AssetType.Description
                    }
                };

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<AssetDto>> GetAll()
        {
            try
            {
                var listEntities = await _assetRepository.GetAllList();

                var listEntityDtos = listEntities.Select(s =>
                new AssetDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Symbol = s.Symbol,
                    AssetTypeId = s.AssetTypeId
                }).ToList();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<List<AssetDto>> GetAllWithInclude()
        {
            try
            {
                var listEntitiesQuery = _assetRepository.GetAllQueryWithInclude(["AssetType", "AssetHistories"]);

                var listEntityDtos = await listEntitiesQuery.Select(s =>
                new AssetDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Symbol = s.Symbol,
                    AssetTypeId = s.AssetTypeId,
                    AssetType = s.AssetType == null ? null : new AssetTypeDto()
                    {
                        Id = s.AssetType.Id,
                        Name = s.AssetType.Name,
                        Description = s.AssetType.Description
                    },
                    AssetHistories = s.AssetHistories == null
                    ? new List<AssetHistoryDto>()
                    : s.AssetHistories
                    .OrderByDescending(ah => ah.HistoryValueDate)
                    .Select(s => new AssetHistoryDto()
                    {
                        AssetId = s.AssetId,
                        Id = s.Id,
                        HistoryValueDate = s.HistoryValueDate,
                        Value = s.Value
                    }).ToList()

                }).ToListAsync();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<List<AssetForPortfolioDto>> GetAllAssetsByPortfolioId(int portfolioId,
            string? assetName = null, int? assetTypeId = null, int? assetOrderBy = null)
        {
            try
            {
                var assetIds = await _investmentAssetRepository
                    .GetAllQuery()
                    .Where(ia => ia.InvestmentPortfolioId == portfolioId)
                    .Select(s => s.AssetId).ToListAsync();

                if (assetIds.Count == 0)
                {
                    return [];
                }

                var listEntitiesQuery = _assetRepository
                    .GetAllQueryWithInclude(["AssetType", "AssetHistories"])
                    .Where(w => assetIds.Contains(w.Id));

                var listEntityDtos = listEntitiesQuery.Select(s =>
                new AssetForPortfolioDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Symbol = s.Symbol,
                    AssetTypeId = s.AssetTypeId,
                    AssetType = s.AssetType == null ? null : new AssetTypeDto()
                    {
                        Id = s.AssetType.Id,
                        Name = s.AssetType.Name,
                        Description = s.AssetType.Description
                    },
                    CurrentValue = s.AssetHistories != null && s.AssetHistories.Any()
                    ? s.AssetHistories
                    .OrderByDescending(ah => ah.HistoryValueDate)
                    .Select(s => new AssetHistoryDto()
                    {
                        AssetId = s.AssetId,
                        Id = s.Id,
                        HistoryValueDate = s.HistoryValueDate,
                        Value = s.Value
                    }).First().Value
                    : 0,
                });

                if (!string.IsNullOrWhiteSpace(assetName))
                {
                    listEntityDtos = listEntityDtos.Where(w => w.Name.Contains(assetName));
                }

                if (assetTypeId.HasValue)
                {
                    listEntityDtos = listEntityDtos.Where(w => w.AssetTypeId == assetTypeId);
                }

                var listDtos = await listEntityDtos.ToListAsync();

                if (assetOrderBy.HasValue)
                {
                    var listOrderDtos = assetOrderBy switch
                    {
                        (int) AssetOrdered.BY_NAME => listDtos.OrderBy(o => o.Name),
                        (int)AssetOrdered.BY_CURRENT_VALUE => listDtos.OrderByDescending(o => o.CurrentValue),
                        _ => listDtos.OrderBy(o => o.Name),
                    };

                    listDtos = listOrderDtos.ToList();
                }
                else
                {
                    listDtos = listDtos.OrderBy(o => o.Name).ToList();
                }

                return listDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}