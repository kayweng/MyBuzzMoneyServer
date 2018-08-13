using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyBuzzMoney.Library.Models;

namespace MyBuzzMoney.Repository.Interfaces
{
    interface IVerificationRepository
    {
        Task<VerificationStatus> RetrieveVerificationStatus(string username);

        Task<bool> UpdateMobileVerification(string username, VerificationProcess process);
    }
}
