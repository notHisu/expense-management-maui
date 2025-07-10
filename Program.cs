using ExpenseManagement.Maui.Models;
using ExpenseManagement.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExpenseManagement.Maui;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup dependency injection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        // Initialize database
        var dbService = serviceProvider.GetRequiredService<IDatabaseService>();
        await dbService.InitializeAsync();
        
        // Ensure database is created with the main context too
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
        
        Console.WriteLine("=== Expense Management Console Demo ===");
        Console.WriteLine();
        
        // Test the services
        await DemonstrateExpenseManagement(serviceProvider);
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
    
    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddLogging(builder => builder.AddConsole());
        services.AddDbContext<ExpenseDbContext>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<IExpenseService, ExpenseService>();
        services.AddSingleton<ICategoryService, CategoryService>();
    }
    
    private static async Task DemonstrateExpenseManagement(ServiceProvider serviceProvider)
    {
        var expenseService = serviceProvider.GetRequiredService<IExpenseService>();
        var categoryService = serviceProvider.GetRequiredService<ICategoryService>();
        
        try
        {
            // Load categories
            Console.WriteLine("Loading categories...");
            var categories = await categoryService.GetActiveCategoriesAsync();
            Console.WriteLine($"Found {categories.Count} categories:");
            foreach (var category in categories.Take(5))
            {
                Console.WriteLine($"  - {category.Name} (Budget: {category.BudgetLimit:C})");
            }
            Console.WriteLine();
            
            // Add a sample expense
            Console.WriteLine("Adding sample expense...");
            var sampleExpense = new Expense
            {
                Title = "Sample Coffee Purchase",
                Description = "Morning coffee at local cafe",
                Amount = 4.50m,
                Date = DateTime.Now,
                CategoryId = categories.First().Id, // Use first category
                Currency = "USD"
            };
            
            var addedExpense = await expenseService.AddExpenseAsync(sampleExpense);
            Console.WriteLine($"Added expense: {addedExpense.Title} - {addedExpense.Amount:C}");
            Console.WriteLine();
            
            // Load all expenses
            Console.WriteLine("Loading all expenses...");
            var expenses = await expenseService.GetAllExpensesAsync();
            Console.WriteLine($"Total expenses found: {expenses.Count}");
            
            foreach (var expense in expenses.Take(10))
            {
                Console.WriteLine($"  {expense.Date:yyyy-MM-dd} | {expense.Title} | {expense.Amount:C} | {expense.Category?.Name}");
            }
            Console.WriteLine();
            
            // Calculate totals
            var totalExpenses = await expenseService.GetTotalExpensesAsync();
            var monthlyExpenses = await expenseService.GetTotalExpensesForMonthAsync(DateTime.Now);
            
            Console.WriteLine($"Total expenses: {totalExpenses:C}");
            Console.WriteLine($"This month: {monthlyExpenses:C}");
            Console.WriteLine();
            
            // Category breakdown
            Console.WriteLine("Category breakdown for this month:");
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var categoryBreakdown = await expenseService.GetCategoryExpenseSummaryAsync(startOfMonth, endOfMonth);
            
            foreach (var summary in categoryBreakdown)
            {
                Console.WriteLine($"  {summary.CategoryName}: {summary.TotalAmount:C} ({summary.Percentage:F1}%)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}