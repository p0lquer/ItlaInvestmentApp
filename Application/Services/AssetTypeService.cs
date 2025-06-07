using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class AssetTypeService : IAssetTypeService
    {
        private readonly IAssetTypeRepository _assetTypeRepository;

        public AssetTypeService(IAssetTypeRepository assetTypeRepository)
        {
            _assetTypeRepository = assetTypeRepository;
        }
        public async Task<bool> AddAsync(AssetTypeDto dto)
        {
            try
            {
                AssetType entity = new() { Id = 0, Name = dto.Name, Description = dto.Description };
                AssetType? returnEntity = await _assetTypeRepository.AddAsync(entity);
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
        public async Task<bool> UpdateAsync(AssetTypeDto dto)
        {
            try
            {
                AssetType entity = new() { Id = dto.Id, Name = dto.Name, Description = dto.Description };
                AssetType? returnEntity = await _assetTypeRepository.UpdateAsync(entity.Id, entity);
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
                await _assetTypeRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<AssetTypeDto?> GetById(int id)
        {
            try
            {
                var entity = await _assetTypeRepository.GetById(id);
                if (entity == null)
                {
                    return null;
                }

                AssetTypeDto dto = new() { Id = entity.Id, Name = entity.Name, Description = entity.Description };
                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<AssetTypeDto>> GetAll()
        {
            try
            {
                var listEntities = await _assetTypeRepository.GetAllList();

                var listEntityDtos = listEntities.Select(s =>
                new AssetTypeDto() { Id = s.Id, Name = s.Name, Description = s.Description }).ToList();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<List<AssetTypeDto>> GetAllWithInclude()
        {
            try
            {
                var listEntitiesQuery = _assetTypeRepository.GetAllQueryWithInclude(["Assets"]);           

                var listEntityDtos = await listEntitiesQuery.Select(s =>
                new AssetTypeDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    AssetQuantity = s.Assets != null ? s.Assets.Count : 0 //ternary operator
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
