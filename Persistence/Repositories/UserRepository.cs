using InvestmentApp.Core.Application.Helpers;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Core.Domain.Interfaces;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly InvestmentAppContext _dbContext;
        public UserRepository(InvestmentAppContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<User?> LoginAsync(string userName, string password)
        {
            string passwordEncrypt = PasswordEncryptation.ComputeSha256Hash(password);

            User? user = await _dbContext.Set<User>().FirstOrDefaultAsync
                (u => u.UserName == userName && u.Password == passwordEncrypt);
            return user;
        }
    }
}