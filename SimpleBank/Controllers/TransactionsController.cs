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

            if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
            if (TempData["Error"] != null) ModelState.AddModelError("", TempData["Error"].ToString());

            return View(accounts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount, string description)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return RedirectToAction("Create", new { accountId });
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Description is required.";
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

            var transaction = new Transaction
            {
                TransactionType = "Withdrawal",
                Amount = amount,
                Date = DateTime.Now,
                ScheduledDate = DateTime.Now,
                Status = "Completed",
                SourceAccountNumber = account.AccountNumber,
                DestinationAccountNumber = account.AccountNumber,
                Description = description,
                AccountId = accountId
            };

            _context.Update(account);
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Withdrawal successful.";
            return RedirectToAction("Create", new { accountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(int accountId, decimal amount, string description)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return RedirectToAction("Create", new { accountId });
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Description is required.";
                return RedirectToAction("Create", new { accountId });
            }

            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                TempData["Error"] = "Account not found.";
                return RedirectToAction("Create");
            }

            account.Balance += amount;

            var transaction = new Transaction
            {
                TransactionType = "Deposit",
                Amount = amount,
                Date = DateTime.Now,
                ScheduledDate = DateTime.Now,
                Status = "Completed",
                SourceAccountNumber = account.AccountNumber,
                DestinationAccountNumber = account.AccountNumber,
                Description = description,
                AccountId = accountId
            };

            _context.Update(account);
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Deposit successful.";
            return RedirectToAction("Create", new { accountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(int fromAccountId, string toAccountNumber, decimal amount, string description)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Description is required.";
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

            var transaction = new Transaction
            {
                TransactionType = "Transfer",
                Amount = amount,
                Date = DateTime.Now,
                ScheduledDate = DateTime.Now,
                Status =   "Completed",
                SourceAccountNumber = fromAccount.AccountNumber,
                DestinationAccountNumber = toAccount.AccountNumber,
                Description = description,
                AccountId = fromAccountId
            };

            _context.Update(fromAccount);
            _context.Update(toAccount);
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Transfer successful.";
            return RedirectToAction("Create", new { accountId = fromAccountId });
        }

        // GET: Transactions/History/{accountId}
        public async Task<IActionResult> History(int accountId)
        {

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                TempData["Error"] = "Account not found or you do not have access to this account.";
                return RedirectToAction("Create");
            }

            // Fetch all transactions for the selected account
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            // Pass the account and transactions to the view
            ViewBag.AccountNumber = account.AccountNumber;
            return View(transactions);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduledTransfer(int fromAccountId, string toAccountNumber, decimal amount, string description, DateTime scheduledDate)
        {
            // Validate amount
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Description is required.";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            // Validate scheduled date
            if (scheduledDate <= DateTime.Now)
            {
                TempData["Error"] = "Scheduled date must be in the future.";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            // Fetch accounts
            var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
            var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == toAccountNumber);

            // Validate accounts
            if (fromAccount == null || toAccount == null)
            {
                TempData["Error"] = "Invalid account(s).";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            // Validate balance (optional: you can skip this for scheduled transactions)
            if (fromAccount.Balance < amount)
            {
                TempData["Error"] = "Insufficient balance.";
                return RedirectToAction("Create", new { accountId = fromAccountId });
            }

            // Create the scheduled transaction
            var transaction = new Transaction
            {
                TransactionType = "Transfer",
                Amount = amount,
                Date = DateTime.Now, // Current date (when the transaction is created)
                ScheduledDate = scheduledDate, // Future date (when the transaction should be executed)
                Status = "Pending", // Mark as pending until processed by the background service
                SourceAccountNumber = fromAccount.AccountNumber,
                DestinationAccountNumber = toAccount.AccountNumber,
                Description = description,
                AccountId = fromAccountId
            };

            // Save the transaction
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Transfer scheduled successfully for {scheduledDate.ToString("MMM dd, yyyy hh:mm tt")}.";
            return RedirectToAction("Create", new { accountId = fromAccountId });
        }

    }
}