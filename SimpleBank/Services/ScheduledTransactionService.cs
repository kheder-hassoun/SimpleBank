using Microsoft.EntityFrameworkCore;

using SimpleBank.Models;

public class ScheduledTransactionService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ScheduledTransactionService> _logger;

    public ScheduledTransactionService(
        IServiceProvider services,  
        ILogger<ScheduledTransactionService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var pendingTransactions = await context.Transactions
                    .Include(t => t.Account)
                    .Where(t => t.Status == "Pending" && t.ScheduledDate <= DateTime.Now)
                    .ToListAsync();

                foreach (var transaction in pendingTransactions)
                {
                    await ProcessTransaction(context, transaction);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled transactions");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check every minute
        }
    }

    private async Task ProcessTransaction(ApplicationDbContext context, Transaction transaction)
    {
        try
        {
            switch (transaction.TransactionType)
            {
                case "Withdrawal":
                    await ProcessWithdrawal(context, transaction);
                    break;
                case "Deposit":
                    await ProcessDeposit(context, transaction);
                    break;
                case "Transfer":
                    await ProcessTransfer(context, transaction);
                    break;
            }

            transaction.Status = "Completed";
        }
        catch (Exception ex)
        {
            transaction.Status = "Failed";
            _logger.LogError(ex, $"Failed to process transaction {transaction.Id}");
        }
        finally
        {
            await context.SaveChangesAsync();
        }
    }

    private async Task ProcessWithdrawal(ApplicationDbContext context, Transaction transaction)
    {
        var account = await context.Accounts.FindAsync(transaction.AccountId);
        if (account.Balance < transaction.Amount)
            throw new InvalidOperationException("Insufficient funds");

        account.Balance -= transaction.Amount;
    }

    private async Task ProcessDeposit(ApplicationDbContext context, Transaction transaction)
    {
        var account = await context.Accounts.FindAsync(transaction.AccountId);
        account.Balance += transaction.Amount;
    }

    private async Task ProcessTransfer(ApplicationDbContext context, Transaction transaction)
    {
        var fromAccount = await context.Accounts.FindAsync(transaction.AccountId);
        var toAccount = await context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == transaction.DestinationAccountNumber);

        if (fromAccount.Balance < transaction.Amount)
            throw new InvalidOperationException("Insufficient funds");

        fromAccount.Balance -= transaction.Amount;
        toAccount.Balance += transaction.Amount;
    }
}