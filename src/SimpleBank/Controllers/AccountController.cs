using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleBank.BusinessLogic.Abstraction;
using SimpleBank.DataAccess.DataContext;
using SimpleBank.DataAccess.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBank.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        public AccountController(ApplicationDbContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        protected string? GetUserId()
        {
            var userId = User.Identity.Name;
            return userId;

        }
        // GET: Account/Index
        public async Task<IActionResult> Index()
        {

            // Gets the current logged-in user's username or email
            var accounts = await  _accountService.GetUserAccountsAsync(GetUserId());
            return View(accounts);
        }

        // GET: Account/Create
        public async Task<IActionResult> Create()
        {
            var userId = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);
            ViewData["ApplicationUserId"] = user.Id;
            // Fetch the list of account types from the database
            var accountTypes = _context.AccountTypes.ToList();

            // Pass the list to the view using ViewData
            ViewData["AccountTypes"] = new SelectList(accountTypes, "Id", "TypeName");

            return View(new Account());
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountTypeId,Balance,AccountNumber,ApplicationUserId")] Account account)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

                if (user != null)
                {
                    
                    account.ApplicationUserId = user.Id; // Link account to current user
                    account = await _accountService.CreateAccountAsync(account);

                    return RedirectToAction(nameof(Index));
                }
            }
            // If the model state is invalid, repopulate the account types dropdown
            var accountTypes = _context.AccountTypes.ToList();
            ViewData["AccountTypes"] = new SelectList(accountTypes, "Id", "TypeName");
            return View(account);
        }

        // GET: Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.ApplicationUser) // Ensure ApplicationUser is loaded
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null || account.ApplicationUser == null || account.ApplicationUser.UserName != User.Identity.Name)
            {
                return NotFound();
            }

            // Pass the ApplicationUserId to the view
            ViewData["ApplicationUserId"] = account.ApplicationUser.Id;
            // Fetch the list of account types from the database
            var accountTypes = _context.AccountTypes.ToList();

            // Pass the list to the view using ViewData
            ViewData["AccountTypes"] = new SelectList(accountTypes, "Id", "TypeName", account.AccountTypeId);

            return View(account);
        }


        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountTypeId,AccountNumber,ApplicationUserId,Balance")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _accountService.UpdateAccountAsync(account);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            // If the model state is invalid, repopulate the account types dropdown
            var accountTypes = _context.AccountTypes.ToList();
            ViewData["AccountTypes"] = new SelectList(accountTypes, "Id", "TypeName", account.AccountTypeId);
            return View(account);
        }



        // GET: Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUser.UserName == User.Identity.Name);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }


        // POST: Account/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == id && a.ApplicationUser.UserName == User.Identity.Name);

            if (account == null)
            {
                return NotFound();
            }

            try
            {

                var result = await _accountService.DeleteAccountAsync(account.Id, GetUserId());

                TempData["SuccessMessage"] = "Account deleted successfully";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Error deleting account. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
