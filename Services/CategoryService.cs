using ExpenseManagement.Maui.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Maui.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<List<Category>> GetActiveCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> AddCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> ToggleCategoryActiveStatusAsync(int id);
}

public class CategoryService : ICategoryService
{
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        using var context = new ExpenseDbContext();
        return await context.Categories
            .Include(c => c.Expenses)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Category>> GetActiveCategoriesAsync()
    {
        using var context = new ExpenseDbContext();
        return await context.Categories
            .Include(c => c.Expenses)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        using var context = new ExpenseDbContext();
        return await context.Categories
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        using var context = new ExpenseDbContext();
        category.CreatedAt = DateTime.UtcNow;
        
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        using var context = new ExpenseDbContext();
        
        context.Categories.Update(category);
        await context.SaveChangesAsync();
        
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        using var context = new ExpenseDbContext();
        var category = await context.Categories
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(c => c.Id == id);
            
        if (category == null) return false;
        
        // Don't allow deletion if category has expenses
        if (category.Expenses.Any())
        {
            return false;
        }
        
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleCategoryActiveStatusAsync(int id)
    {
        using var context = new ExpenseDbContext();
        var category = await context.Categories.FindAsync(id);
        if (category == null) return false;
        
        category.IsActive = !category.IsActive;
        await context.SaveChangesAsync();
        return true;
    }
}