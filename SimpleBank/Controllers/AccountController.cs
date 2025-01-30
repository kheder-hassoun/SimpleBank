using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBank.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBank.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Index
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name; // Gets the current logged-in user's username or email
            var accounts = await _context.Accounts
                .Where(a => a.ApplicationUser.UserName == userId)
                .ToListAsync();

            return View(accounts);
        }

        // GET: Account/Create
        public async Task<IActionResult> Create()
        {
            var userId = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);
            ViewData["ApplicationUserId"] = user.Id;

            return View(new Account());
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountType,Balance,AccountNumber,ApplicationUserId")] Account account)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);

                if (user != null)
                {
                    account.ApplicationUserId = user.Id; // Link account to current user
                    _context.Add(account);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
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

            return View(account);
        }


        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountType,AccountNumber,ApplicationUserId,Balance")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
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
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
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
