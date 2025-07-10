using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Maui.Models;

public class Category
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(7)]
    public string Color { get; set; } = "#512BD4";
    
    [MaxLength(50)]
    public string? Icon { get; set; }
    
    public decimal BudgetLimit { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    
    // Computed property for total expenses
    public decimal TotalExpenses => Expenses?.Sum(e => e.Amount) ?? 0;
    
    // Computed property for remaining budget
    public decimal RemainingBudget => BudgetLimit - TotalExpenses;
    
    // Computed property for budget utilization percentage
    public double BudgetUtilizationPercentage => BudgetLimit > 0 ? (double)(TotalExpenses / BudgetLimit) * 100 : 0;
}