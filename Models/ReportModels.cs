namespace ExpenseManagement.Maui.Models;

public class ExpenseSummary
{
    public DateTime Period { get; set; }
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class CategoryExpenseSummary
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = "#512BD4";
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal Percentage { get; set; }
}

public class MonthlyReport
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetAmount => TotalIncome - TotalExpenses;
    public List<CategoryExpenseSummary> CategoryBreakdown { get; set; } = new();
    public List<ExpenseSummary> DailyExpenses { get; set; } = new();
}

public class YearlyReport
{
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetAmount => TotalIncome - TotalExpenses;
    public List<ExpenseSummary> MonthlyExpenses { get; set; } = new();
    public List<CategoryExpenseSummary> CategoryBreakdown { get; set; } = new();
}