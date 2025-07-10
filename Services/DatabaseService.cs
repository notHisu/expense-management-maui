using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ExpenseManagement.Maui.Services;

public interface IDatabaseService
{
    Task InitializeAsync();
    Task<bool> BackupDatabaseAsync(string backupPath);
    Task<bool> RestoreDatabaseAsync(string backupPath);
    Task<bool> ExportToCsvAsync(string filePath);
}

public class DatabaseService : IDatabaseService
{
    public async Task InitializeAsync()
    {
        using var context = new ExpenseDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    public Task<bool> BackupDatabaseAsync(string backupPath)
    {
        try
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "expenses.db");
            if (File.Exists(dbPath))
            {
                File.Copy(dbPath, backupPath, true);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> RestoreDatabaseAsync(string backupPath)
    {
        try
        {
            if (File.Exists(backupPath))
            {
                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "expenses.db");
                File.Copy(backupPath, dbPath, true);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<bool> ExportToCsvAsync(string filePath)
    {
        try
        {
            using var context = new ExpenseDbContext();
            var expenses = await context.Expenses
                .Include(e => e.Category)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Date,Title,Category,Amount,Currency,Description");

            foreach (var expense in expenses)
            {
                csv.AppendLine($"{expense.Date:yyyy-MM-dd},{expense.Title},{expense.Category?.Name},{expense.Amount},{expense.Currency},{expense.Description}");
            }

            await File.WriteAllTextAsync(filePath, csv.ToString());
            return true;
        }
        catch
        {
            return false;
        }
    }
}