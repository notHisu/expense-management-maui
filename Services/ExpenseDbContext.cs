using Microsoft.EntityFrameworkCore;
using ExpenseManagement.Maui.Models;

namespace ExpenseManagement.Maui.Services;

public class ExpenseDbContext : DbContext
{
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }

    public ExpenseDbContext() : base()
    {
    }

    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "expenses.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Expense entity
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Expenses)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.BudgetLimit).HasColumnType("decimal(18,2)");
            entity.HasIndex(c => c.Name).IsUnique();
        });

        // Seed default categories
        SeedDefaultCategories(modelBuilder);
    }

    private static void SeedDefaultCategories(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new Category { Id = 1, Name = "Food & Dining", Description = "Restaurants, groceries, and meals", Color = "#FF6B6B", Icon = "🍽️", BudgetLimit = 500, IsActive = true },
            new Category { Id = 2, Name = "Transportation", Description = "Gas, public transport, car maintenance", Color = "#4ECDC4", Icon = "🚗", BudgetLimit = 300, IsActive = true },
            new Category { Id = 3, Name = "Entertainment", Description = "Movies, games, subscriptions", Color = "#45B7D1", Icon = "🎬", BudgetLimit = 200, IsActive = true },
            new Category { Id = 4, Name = "Bills & Utilities", Description = "Electricity, water, internet, phone", Color = "#96CEB4", Icon = "📄", BudgetLimit = 400, IsActive = true },
            new Category { Id = 5, Name = "Shopping", Description = "Clothes, electronics, general shopping", Color = "#FFEAA7", Icon = "🛍️", BudgetLimit = 300, IsActive = true },
            new Category { Id = 6, Name = "Healthcare", Description = "Medical expenses, pharmacy, insurance", Color = "#DDA0DD", Icon = "🏥", BudgetLimit = 250, IsActive = true },
            new Category { Id = 7, Name = "Education", Description = "Books, courses, training", Color = "#FFA07A", Icon = "📚", BudgetLimit = 150, IsActive = true },
            new Category { Id = 8, Name = "Travel", Description = "Hotels, flights, vacation expenses", Color = "#98D8C8", Icon = "✈️", BudgetLimit = 600, IsActive = true },
            new Category { Id = 9, Name = "Other", Description = "Miscellaneous expenses", Color = "#A8A8A8", Icon = "📦", BudgetLimit = 100, IsActive = true }
        };

        modelBuilder.Entity<Category>().HasData(categories);
    }
}