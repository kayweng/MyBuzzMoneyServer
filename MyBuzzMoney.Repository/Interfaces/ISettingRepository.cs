using MyBuzzMoney.Library.Models;
using System.Threading.Tasks;

namespace MyBuzzMoney.Repository.Interfaces
{
    interface ISettingRepository
    {
        Task<UserSetting> RetrieveUserSetting(string username);

        Task<bool> UpdatePreferences(UserSetting userSetting);

        Task<bool> UpdateLinkedAccount(UserSetting userSetting);
    }
}
