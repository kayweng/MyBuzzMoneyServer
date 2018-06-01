using MyBuzzMoney.Library.Models;
using System.Threading.Tasks;

namespace MyBuzzMoney.Repository.Interfaces
{
    interface IUserRepository
    {
        Task<UserProfile> RetrieveUser(string username);

        Task<bool> SaveUser(UserProfile userProfile);
    }
}
