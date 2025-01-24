using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBank.Models; // Adjust based on your project namespace
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

        // GET: Transactions/Create (Deposit/Withdraw/Transfer)
        public async Task<IActionResult> Create()
        {
            var userId = User.Identity.Name; // Gets the current logged-in user's username or email
            var accounts = await _context.Accounts
                .Where(a => a.ApplicationUser.UserName == userId)
                .ToListAsync();

            return View(accounts);
        }

        // GET: Transactions/Create (Deposit/Withdraw/Transfer)
        public async Task<IActionResult> Withdraw()
        {
            var userId = User.Identity.Name; // Gets the current logged-in user's username or email
            var accounts = await _context.Accounts
                .Where(a => a.ApplicationUser.UserName == userId)
                .ToListAsync();

            return View(accounts);
        }
        // POST: Transactions/Withdraw
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than zero.");
                return View("Create");
            }

            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                ModelState.AddModelError("", "Account not found.");
                return View("Create");
            }

            if (account.Balance < amount)
            {
                ModelState.AddModelError("", "Insufficient balance.");
                return View("Create");
            }

            // Deduct the amount and save
            account.Balance -= amount;
            _context.Update(account);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Withdrawal successful.";
            return RedirectToAction("Index", "Account");
        }
        // GET: Transactions/Create (Deposit/Withdraw/Transfer)
        public async Task<IActionResult> Deposit()
        {
            var userId = User.Identity.Name; // Gets the current logged-in user's username or email
            var accounts = await _context.Accounts
                .Where(a => a.ApplicationUser.UserName == userId)
                .ToListAsync();

            return View(accounts);
        }
        // POST: Transactions/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(int accountId, decimal amount)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than zero.");
                return View("Create");
            }

            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                ModelState.AddModelError("", "Account not found.");
                return View("Create");
            }

            // Add the amount and save
            account.Balance += amount;
            _context.Update(account);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Deposit successful.";
            return RedirectToAction("Index", "Account");
        }
        // GET: Transactions/Create (Deposit/Withdraw/Transfer)
        public async Task<IActionResult> Transfer()
        {
            var userId = User.Identity.Name; // Gets the current logged-in user's username or email
            var accounts = await _context.Accounts
                .Where(a => a.ApplicationUser.UserName == userId)
                .ToListAsync();

            return View(accounts);
        }
        // POST: Transactions/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(int fromAccountId, string toAccountNumber, decimal amount)
        {
            if (amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than zero.");
                return View("Create");
            }

            var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
            var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == toAccountNumber);

            if (fromAccount == null || toAccount == null)
            {
                ModelState.AddModelError("", "Invalid account(s).");
                return View("Create");
            }

            if (fromAccount.Balance < amount)
            {
                ModelState.AddModelError("", "Insufficient balance.");
                return View("Create");
            }

            // Perform transfer
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            _context.Update(fromAccount);
            _context.Update(toAccount);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Transfer successful.";
            return RedirectToAction("Index", "Account");
        }
    }
}
