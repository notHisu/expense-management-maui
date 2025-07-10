namespace ExpenseManagement.Maui.Models;

public class Currency
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    
    public static List<Currency> GetSupportedCurrencies()
    {
        return new List<Currency>
        {
            new() { Code = "USD", Name = "US Dollar", Symbol = "$" },
            new() { Code = "EUR", Name = "Euro", Symbol = "€" },
            new() { Code = "GBP", Name = "British Pound", Symbol = "£" },
            new() { Code = "JPY", Name = "Japanese Yen", Symbol = "¥" },
            new() { Code = "CAD", Name = "Canadian Dollar", Symbol = "C$" },
            new() { Code = "AUD", Name = "Australian Dollar", Symbol = "A$" },
            new() { Code = "CHF", Name = "Swiss Franc", Symbol = "CHF" },
            new() { Code = "CNY", Name = "Chinese Yuan", Symbol = "¥" },
            new() { Code = "INR", Name = "Indian Rupee", Symbol = "₹" },
            new() { Code = "KRW", Name = "South Korean Won", Symbol = "₩" }
        };
    }
}