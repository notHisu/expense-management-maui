using ExpenseManagement.Maui.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ExpenseManagement.Maui.Services;

public interface IExpenseService
{
    Task<List<Expense>> GetAllExpensesAsync();
    Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<Expense>> GetExpensesByCategoryAsync(int categoryId);
    Task<List<Expense>> SearchExpensesAsync(string searchTerm);
    Task<Expense?> GetExpenseByIdAsync(int id);
    Task<Expense> AddExpenseAsync(Expense expense);
    Task<Expense> UpdateExpenseAsync(Expense expense);
    Task<bool> DeleteExpenseAsync(int id);
    Task<decimal> GetTotalExpensesAsync();
    Task<decimal> GetTotalExpensesForMonthAsync(DateTime month);
    Task<List<CategoryExpenseSummary>> GetCategoryExpenseSummaryAsync(DateTime startDate, DateTime endDate);
    Task<MonthlyReport> GetMonthlyReportAsync(int year, int month);
    Task<YearlyReport> GetYearlyReportAsync(int year);
    Task ProcessRecurringExpensesAsync();
}

public class ExpenseService : IExpenseService
{
    public async Task<List<Expense>> GetAllExpensesAsync()
    {
        using var context = new ExpenseDbContext();
        return await context.Expenses
            .Include(e => e.Category)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        using var context = new ExpenseDbContext();
        return await context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<Expense>> GetExpensesByCategoryAsync(int categoryId)
    {
        using var context = new ExpenseDbContext();
        return await context.Expenses
            .Include(e => e.Category)
            .Where(e => e.CategoryId == categoryId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<Expense>> SearchExpensesAsync(string searchTerm)
    {
        using var context = new ExpenseDbContext();
        return await context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Title.Contains(searchTerm) || 
                       (e.Description != null && e.Description.Contains(searchTerm)) ||
                       e.Category!.Name.Contains(searchTerm))
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        using var context = new ExpenseDbContext();
        return await context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Expense> AddExpenseAsync(Expense expense)
    {
        using var context = new ExpenseDbContext();
        expense.CreatedAt = DateTime.UtcNow;
        expense.UpdatedAt = DateTime.UtcNow;
        
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();
        
        return expense;
    }

    public async Task<Expense> UpdateExpenseAsync(Expense expense)
    {
        using var context = new ExpenseDbContext();
        expense.UpdatedAt = DateTime.UtcNow;
        
        context.Expenses.Update(expense);
        await context.SaveChangesAsync();
        
        return expense;
    }

    public async Task<bool> DeleteExpenseAsync(int id)
    {
        using var context = new ExpenseDbContext();
        var expense = await context.Expenses.FindAsync(id);
        if (expense == null) return false;
        
        context.Expenses.Remove(expense);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalExpensesAsync()
    {
        using var context = new ExpenseDbContext();
        var expenses = await context.Expenses.ToListAsync();
        return expenses.Sum(e => e.Amount);
    }

    public async Task<decimal> GetTotalExpensesForMonthAsync(DateTime month)
    {
        using var context = new ExpenseDbContext();
        var expenses = await context.Expenses
            .Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month)
            .ToListAsync();
        return expenses.Sum(e => e.Amount);
    }

    public async Task<List<CategoryExpenseSummary>> GetCategoryExpenseSummaryAsync(DateTime startDate, DateTime endDate)
    {
        using var context = new ExpenseDbContext();
        var expenses = await context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date)
            .ToListAsync();

        var totalAmount = expenses.Sum(e => e.Amount);
        
        return expenses
            .GroupBy(e => new { e.Category!.Name, e.Category.Color })
            .Select(g => new CategoryExpenseSummary
            {
                CategoryName = g.Key.Name,
                CategoryColor = g.Key.Color,
                TotalAmount = g.Sum(e => e.Amount),
                TransactionCount = g.Count(),
                Percentage = totalAmount > 0 ? (g.Sum(e => e.Amount) / totalAmount) * 100 : 0
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();
    }

    public async Task<MonthlyReport> GetMonthlyReportAsync(int year, int month)
    {
        using var context = new ExpenseDbContext();
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var expenses = await context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date)
            .ToListAsync();

        var categoryBreakdown = await GetCategoryExpenseSummaryAsync(startDate, endDate);
        
        var dailyExpenses = expenses
            .GroupBy(e => e.Date.Date)
            .Select(g => new ExpenseSummary
            {
                Period = g.Key,
                TotalAmount = g.Sum(e => e.Amount),
                TransactionCount = g.Count()
            })
            .OrderBy(e => e.Period)
            .ToList();

        return new MonthlyReport
        {
            Year = year,
            Month = month,
            TotalExpenses = expenses.Sum(e => e.Amount),
            CategoryBreakdown = categoryBreakdown,
            DailyExpenses = dailyExpenses
        };
    }

    public async Task<YearlyReport> GetYearlyReportAsync(int year)
    {
        using var context = new ExpenseDbContext();
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);

        var expenses = await context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date)
            .ToListAsync();

        var categoryBreakdown = await GetCategoryExpenseSummaryAsync(startDate, endDate);
        
        var monthlyExpenses = expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new ExpenseSummary
            {
                Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                TotalAmount = g.Sum(e => e.Amount),
                TransactionCount = g.Count()
            })
            .OrderBy(e => e.Period)
            .ToList();

        return new YearlyReport
        {
            Year = year,
            TotalExpenses = expenses.Sum(e => e.Amount),
            CategoryBreakdown = categoryBreakdown,
            MonthlyExpenses = monthlyExpenses
        };
    }

    public async Task ProcessRecurringExpensesAsync()
    {
        using var context = new ExpenseDbContext();
        var recurringExpenses = await context.Expenses
            .Where(e => e.IsRecurring && e.NextRecurrenceDate <= DateTime.Now)
            .ToListAsync();

        foreach (var expense in recurringExpenses)
        {
            var newExpense = new Expense
            {
                Title = expense.Title,
                Description = expense.Description,
                Amount = expense.Amount,
                Date = expense.NextRecurrenceDate ?? DateTime.Now,
                CategoryId = expense.CategoryId,
                Currency = expense.Currency,
                IsRecurring = expense.IsRecurring,
                RecurrenceType = expense.RecurrenceType
            };

            // Calculate next recurrence date
            newExpense.NextRecurrenceDate = expense.RecurrenceType switch
            {
                Models.RecurrenceType.Daily => newExpense.Date.AddDays(1),
                Models.RecurrenceType.Weekly => newExpense.Date.AddDays(7),
                Models.RecurrenceType.Monthly => newExpense.Date.AddMonths(1),
                Models.RecurrenceType.Yearly => newExpense.Date.AddYears(1),
                _ => null
            };

            context.Expenses.Add(newExpense);

            // Update original expense's next recurrence date
            expense.NextRecurrenceDate = newExpense.NextRecurrenceDate;
            context.Expenses.Update(expense);
        }

        await context.SaveChangesAsync();
    }
}