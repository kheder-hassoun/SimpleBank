using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBank.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBank.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transactions/Create
        public async Task<IActionResult> Create(int? accountId)
        {
            var userId = User.Identity.Name;
            var accounts = await _context.Accounts
                .Where(a => a.ApplicationUser.UserName == userId)
                .ToListAsync();

            // Preserve selected account through redirects
            if (accountId.HasValue)
            {
                ViewBag.SelectedAccountId = accountId.Value;
                TempData["SelectedAccountId"] = accountId.Value;
            }
            else if (TempData["SelectedAccountId"] is int savedId)
            {
                ViewBag.SelectedAccountId = savedId;
                TempData.Keep("SelectedAccountId");
            }

            // Handle messages and errors
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            if (TempData["Error"] != null)
            {
                ModelState.AddModelError("", TempData["Error"].ToString());
            }

            return View(accounts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return RedirectToAction("Create", new { accountId });
            }

            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                TempData["Error"] = "Account not found.";
                return RedirectToAction("Create");
            }

            if (account.Balance < amount)
            {
                TempData["Error"] = "Insufficient balance.";
                return RedirectToAction("Create", new { accountId });
            }

            account.Balance -= amount;
            _context.Update(account);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Withdrawal successful.";
            return RedirectToAction("Create", new { accountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(int accountId, decimal amount)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return RedirectToAction("Create", new { accountId });
            }

            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                TempData["Error"] = "Account not found.";
                return RedirectToAction("Create");
            }

            account.Balance += amount;
            _context.Update(account);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Deposit successful.";
            return RedirectToAction("Create", new { accountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(int fromAccountId, string toAccountNumber, decimal amount)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
            var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == toAccountNumber);

            if (fromAccount == null || toAccount == null)
            {
                TempData["Error"] = "Invalid account(s).";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            if (fromAccount.Balance < amount)
            {
                TempData["Error"] = "Insufficient balance.";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            _context.Update(fromAccount);
            _context.Update(toAccount);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Transfer successful.";
            return RedirectToAction("Create", new { accountId = fromAccountId });
        }
    }
}