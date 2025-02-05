// Services/IAccountService.cs
using SimpleBank.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBank.BusinessLogic.Abstraction
{
    public interface IAccountService
    {


        Task<List<Account>> GetUserAccountsAsync(string userId);
        Task<List<AccountType>> GetAccountTypesAsync();
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> GetAccountByIdAsync(int id, string userId);
        Task UpdateAccountAsync(Account account);
        Task<ServiceResult> DeleteAccountAsync(int id, string userId);
        bool AccountExists(int id);
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}