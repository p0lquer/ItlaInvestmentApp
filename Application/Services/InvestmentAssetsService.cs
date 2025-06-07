using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class InvestmentAssetsService : IInvestmentAssetsService
    {
        private readonly IInvestmentAssetRepository _investmentAssetRepository;

        public InvestmentAssetsService(IInvestmentAssetRepository investmentAssetRepository)
        {
            _investmentAssetRepository = investmentAssetRepository;
        }
        public async Task<bool> AddAsync(InvestmentAssetsDto dto)
        {
            try
            {
                InvestmentAssets entity = new()
                {
                    Id = 0,
                    AssetId = dto.AssetId,
                    InvestmentPortfolioId = dto.InvestmentPortfolioId,
                    AssociationDate = DateTime.UtcNow
                };

                InvestmentAssets? returnEntity = await _investmentAssetRepository.AddAsync(entity);
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
        public async Task<bool> UpdateAsync(InvestmentAssetsDto dto)
        {
            try
            {
                InvestmentAssets entity = new()
                {
                    Id = dto.Id,
                    AssetId = dto.AssetId,
                    InvestmentPortfolioId = dto.InvestmentPortfolioId                    
                };

                InvestmentAssets? returnEntity = await _investmentAssetRepository.UpdateAsync(entity.Id, entity);
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
                await _investmentAssetRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<InvestmentAssetsDto?> GetByAssetAndPortfolioAsync(int assetId,int portfolioId)
        {
            try
            {
                var investmentAsset = await _investmentAssetRepository
                    .GetAllQueryWithInclude(["Asset"])
                    .FirstOrDefaultAsync(ia => ia.AssetId == assetId 
                    && ia.InvestmentPortfolioId == portfolioId);

                if (investmentAsset == null)
                {
                    return null;
                }

                InvestmentAssetsDto dto = new()
                {
                    AssetId = investmentAsset.AssetId,
                    Id = investmentAsset.Id,
                    InvestmentPortfolioId = investmentAsset.InvestmentPortfolioId,
                    Asset = investmentAsset.Asset == null ? null : new()
                    { 
                        Id = investmentAsset.Asset.Id,
                        AssetTypeId = investmentAsset.Asset.AssetTypeId,
                        Name = investmentAsset.Asset.Name,
                        Symbol = investmentAsset.Asset.Symbol
                    }
                };
                
                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<InvestmentAssetsDto?> GetById(int id)
        {
            try
            {
                var listEntitiesQuery = _investmentAssetRepository.GetAllQueryWithInclude(["Asset", "InvestmentPortfolio"]);

                var entity = await listEntitiesQuery.FirstOrDefaultAsync(a => a.Id == id);

                if (entity == null)
                {
                    return null;
                }

                var dto = new InvestmentAssetsDto()
                {
                    Id = entity.Id,  
                    AssetId = entity.AssetId,
                    InvestmentPortfolioId = entity.InvestmentPortfolioId,
                    AssociationDate = entity.AssociationDate,
                    Asset = entity.Asset == null ? null : new AssetDto()
                    {
                        Id = entity.Asset.Id,
                        Name = entity.Asset.Name,
                        Description = entity.Asset.Description,
                        AssetTypeId = entity.Asset.AssetTypeId,
                        Symbol = entity.Asset.Symbol
                    },
                    InvestmentPortfolio = entity.InvestmentPortfolio == null ? null : new InvestmentPortfolioDto()
                    {
                        Id = entity.InvestmentPortfolio.Id,
                        Name = entity.InvestmentPortfolio.Name,
                        Description = entity.InvestmentPortfolio.Description,
                        UserId = entity.InvestmentPortfolio.UserId,
                    },
                };

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<InvestmentAssetsDto>> GetAll()
        {
            try
            {
                var listEntities = await _investmentAssetRepository.GetAllList();

                var listEntityDtos = listEntities.Select(s =>
                new InvestmentAssetsDto()
                {
                    Id = s.Id,
                    AssetId = s.AssetId,
                    InvestmentPortfolioId = s.InvestmentPortfolioId,
                    AssociationDate = s.AssociationDate
                }).ToList();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<List<InvestmentAssetsDto>> GetAllWithInclude()
        {
            try
            {
                var listEntitiesQuery = _investmentAssetRepository.GetAllQueryWithInclude(["Asset", "InvestmentPortfolio"]);

                var listEntityDtos = await listEntitiesQuery.Select(s =>
                new InvestmentAssetsDto()
                {
                    Id = s.Id,
                    AssetId = s.AssetId,
                    InvestmentPortfolioId = s.InvestmentPortfolioId,
                    AssociationDate = s.AssociationDate,
                    Asset = s.Asset == null ? null : new AssetDto()
                    {
                        Id = s.Asset.Id,
                        Name = s.Asset.Name,
                        Description = s.Asset.Description,
                        AssetTypeId = s.Asset.AssetTypeId,
                        Symbol = s.Asset.Symbol
                    },
                    InvestmentPortfolio = s.InvestmentPortfolio == null ? null : new InvestmentPortfolioDto()
                    {
                        Id = s.InvestmentPortfolio.Id,
                        Name = s.InvestmentPortfolio.Name,
                        Description = s.InvestmentPortfolio.Description,
                        UserId = s.InvestmentPortfolio.UserId,
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