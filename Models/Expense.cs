using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.Maui.Models;

public class Expense
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime Date { get; set; } = DateTime.Now;
    
    [Required]
    public int CategoryId { get; set; }
    
    public Category? Category { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "USD";
    
    public string? ReceiptImagePath { get; set; }
    
    public bool IsRecurring { get; set; }
    
    public RecurrenceType? RecurrenceType { get; set; }
    
    public DateTime? NextRecurrenceDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum RecurrenceType
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}