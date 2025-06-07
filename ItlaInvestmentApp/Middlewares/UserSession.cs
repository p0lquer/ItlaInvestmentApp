using InvestmentApp.Core.Application.Helpers;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.User;
using InvestmentApp.Core.Domain.Common.Enums;

namespace ItlaInvestmentApp.Middlewares
{
    public class UserSession : IUserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HasUser()
        {
            UserViewModel? userViewModel = _httpContextAccessor.HttpContext?
                .Session.Get<UserViewModel>("User");

            if (userViewModel == null)
            {
                return false;
            }

            return true;
        }

        public UserViewModel? GetUserSession()
        {
            UserViewModel? userViewModel = _httpContextAccessor.HttpContext?
                .Session.Get<UserViewModel>("User");

            if (userViewModel == null)
            {
                return null;
            }

            return userViewModel;
        }

        public bool IsAdmin()
        {
            UserViewModel? userViewModel = _httpContextAccessor.HttpContext?
                .Session.Get<UserViewModel>("User");

            if (userViewModel == null)
            {
                return false;
            }

            return userViewModel.Role == (int)Role.ADMIN;
        }
    }
}
