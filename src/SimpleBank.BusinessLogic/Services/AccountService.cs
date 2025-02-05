// Services/AccountService.cs
using Microsoft.EntityFrameworkCore;
using SimpleBank.BusinessLogic.Abstraction;
using SimpleBank.DataAccess.DataContext;
using SimpleBank.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBank.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Account>> GetUserAccountsAsync(string userId)
        {
            return await _context.Accounts
                .Include(a => a.AccountType)
                .Where(a => a.ApplicationUser.UserName == userId)
                .ToListAsync();
        }

        public async Task<List<AccountType>> GetAccountTypesAsync()
        {
            return await _context.AccountTypes.ToListAsync();
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            _context.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> GetAccountByIdAsync(int id, string userId)
        {
            return await _context.Accounts
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUser.UserName == userId);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceResult> DeleteAccountAsync(int id, string userId)
        {
            var account = await _context.Accounts
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUser.UserName == userId);

            if (account == null)
            {
                return new ServiceResult { Success = false, Message = "Account not found" };
            }

            try
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return new ServiceResult { Success = true, Message = "Account deleted successfully" };
            }
            catch (DbUpdateException)
            {
                return new ServiceResult { Success = false, Message = "Error deleting account. Please try again." };
            }
        }

        public bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}