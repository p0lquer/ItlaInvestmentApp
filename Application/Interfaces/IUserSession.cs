using InvestmentApp.Core.Application.ViewModels.User;

namespace InvestmentApp.Core.Application.Interfaces
{
    public interface IUserSession
    {
        UserViewModel? GetUserSession();
        bool HasUser();

        bool IsAdmin();
    }
}