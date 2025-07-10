namespace ExpenseManagement.Maui.Services;

public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateToAsync(string route, IDictionary<string, object> parameters);
    Task GoBackAsync();
    Task PopToRootAsync();
}

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route)
    {
        Console.WriteLine($"Navigation: {route}");
        await Task.CompletedTask;
    }

    public async Task NavigateToAsync(string route, IDictionary<string, object> parameters)
    {
        Console.WriteLine($"Navigation: {route} with {parameters.Count} parameters");
        await Task.CompletedTask;
    }

    public async Task GoBackAsync()
    {
        Console.WriteLine("Navigation: Go back");
        await Task.CompletedTask;
    }

    public async Task PopToRootAsync()
    {
        Console.WriteLine("Navigation: Pop to root");
        await Task.CompletedTask;
    }
}