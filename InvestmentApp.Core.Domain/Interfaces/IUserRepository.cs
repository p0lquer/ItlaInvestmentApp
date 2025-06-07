using InvestmentApp.Core.Domain.Entities;

namespace InvestmentApp.Core.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> LoginAsync(string userName, string password);
    }
}