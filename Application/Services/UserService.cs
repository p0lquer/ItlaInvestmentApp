using InvestmentApp.Core.Application.Dtos.User;
using InvestmentApp.Core.Application.Helpers;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserDto?> LoginAsync(LoginDto dto)
        {
            User? user = await _userRepository.LoginAsync(dto.UserName, dto.Password);

            if (user == null)
            {
                return null;
            }

            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                LastName = user.LastName,
                Name = user.Name,
                Role = user.Role,
                UserName = user.UserName,
                Phone = user.Phone,
                ProfileImage = user.ProfileImage,
            };

            return userDto;
        }
        public async Task<UserDto?> AddAsync(SaveUserDto dto)
        {
            try
            {
                User entity = new()
                {
                    Id = 0,
                    Name = dto.Name,
                    Email = dto.Email,
                    LastName = dto.LastName,
                    UserName = dto.UserName,
                    Password = PasswordEncryptation.ComputeSha256Hash(dto.Password),
                    Role = dto.Role,
                    Phone = dto.Phone,
                    ProfileImage = dto.ProfileImage
                };

                User? returnEntity = await _userRepository.AddAsync(entity);
                if (returnEntity == null)
                {
                    return null;
                }

                return new UserDto()
                {
                    Id = returnEntity.Id,
                    Name = returnEntity.Name,
                    LastName = returnEntity.LastName,
                    Role = returnEntity.Role,
                    UserName = returnEntity.UserName,
                    Email = returnEntity.Email
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<UserDto?> UpdateAsync(SaveUserDto dto)
        {
            try
            {
                var entityDb = await _userRepository.GetById(dto.Id);

                if (entityDb == null)
                {
                    return null;
                }

                User entity = new()
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Email = dto.Email,
                    LastName = dto.LastName,
                    UserName = dto.UserName,
                    Password = string.IsNullOrWhiteSpace(dto.Password) ? entityDb.Password : PasswordEncryptation.ComputeSha256Hash(dto.Password),
                    Role = dto.Role,
                    Phone = dto.Phone,
                    ProfileImage = dto.ProfileImage
                };

                User? returnEntity = await _userRepository.UpdateAsync(entity.Id, entity);
                if (returnEntity == null)
                {
                    return null;
                }

                return new UserDto()
                {
                    Id = returnEntity.Id,
                    Name = returnEntity.Name,
                    LastName = returnEntity.LastName,
                    Role = returnEntity.Role,
                    UserName = returnEntity.UserName,
                    Email = returnEntity.Email
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                await _userRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<UserDto?> GetById(int id)
        {
            try
            {
                var entity = await _userRepository.GetById(id);
                if (entity == null)
                {
                    return null;
                }

                UserDto dto = new()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Email = entity.Email,
                    UserName = entity.UserName,
                    LastName = entity.LastName,
                    Role = entity.Role,
                    Phone = entity.Phone,
                    ProfileImage = entity.ProfileImage
                };
                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<UserDto>> GetAll()
        {
            try
            {
                var listEntities = await _userRepository.GetAllList();

                var listEntityDtos = listEntities.Select(s =>
                new UserDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    UserName = s.UserName,
                    LastName = s.LastName,
                    Role = s.Role,
                    Phone = s.Phone,
                    ProfileImage = s.ProfileImage
                }).ToList();

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<List<UserDto>> GetAllWithInclude()
        {
            try
            {
                var listEntitiesQuery = _userRepository.GetAllQueryWithInclude(["InvestmentPortfolios"]);

                var listEntityDtos = await listEntitiesQuery.Select(s =>
                new UserDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    UserName = s.UserName,
                    LastName = s.LastName,
                    Role = s.Role,
                    Phone = s.Phone,
                    ProfileImage = s.ProfileImage
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
