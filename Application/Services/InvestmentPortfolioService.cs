using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Application.Dtos.User;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class InvestmentPortfolioService : IInvestmentPortfolioService
    {
        private readonly IInvestmentPortfolioRepository _investmentPortfolioRepository;

        public InvestmentPortfolioService(IInvestmentPortfolioRepository investmentPortfolioRepository)
        {
            _investmentPortfolioRepository = investmentPortfolioRepository;
        }
        public async Task<bool> AddAsync(InvestmentPortfolioDto dto)
        {
            try
            {
                InvestmentPortfolio entity = new()
                {
                    Id = 0,
                    Name = dto.Name,
                    Description = dto.Description,
                    UserId = dto.UserId,
                };

                InvestmentPortfolio? returnEntity = await _investmentPortfolioRepository.AddAsync(entity);
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
        public async Task<bool> UpdateAsync(InvestmentPortfolioDto dto)
        {
            try
            {
                InvestmentPortfolio entity = new()
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    UserId = dto.UserId
                };
                InvestmentPortfolio? returnEntity = await _investmentPortfolioRepository.UpdateAsync(entity.Id, entity);
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
                await _investmentPortfolioRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<InvestmentPortfolioDto?> GetById(int id)
        {
            try
            {
                var listEntitiesQuery = _investmentPortfolioRepository.GetAllQueryWithInclude(["User"]);

                var entity = await listEntitiesQuery.FirstOrDefaultAsync(a => a.Id == id);

                if (entity == null)
                {
                    return null;
                }

                var dto = new InvestmentPortfolioDto()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    UserId = entity.UserId,
                    User = entity.User == null ? null : new UserDto()
                    {
                        Id = entity.User.Id,
                        Name = entity.User.Name,
                        Email = entity.User.Email,
                        UserName = entity.User.UserName,
                        LastName = entity.User.LastName,
                        Role = entity.User.Role,
                        Phone = entity.User.Phone,
                        ProfileImage = entity.User.ProfileImage,                        
                    }
                };

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<InvestmentPortfolioDto>> GetAll()
        {
            try
            {
                var listEntities = await _investmentPortfolioRepository.GetAllList();

                var listEntityDtos = listEntities.Select(s =>
                new InvestmentPortfolioDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    UserId = s.UserId,                
                }).ToList();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<List<InvestmentPortfolioDto>> GetAllWithIncludeByUser(int userId)
        {
            try
            {
                var listEntitiesQuery = _investmentPortfolioRepository.GetAllQueryWithInclude(["User"])
                    .Where(ip=>ip.UserId == userId);

                var listEntityDtos = await listEntitiesQuery.Select(s =>
                new InvestmentPortfolioDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    UserId = s.UserId,
                    User = s.User == null ? null : new UserDto()
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Email = s.User.Email,
                        UserName = s.User.UserName,
                        LastName = s.User.LastName,
                        Role = s.User.Role,
                        Phone = s.User.Phone,
                        ProfileImage = s.User.ProfileImage,
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