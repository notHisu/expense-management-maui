# Expense Management MAUI App

A comprehensive expense management application built with .NET 8 and designed for cross-platform deployment using .NET MAUI. This project provides a complete backend infrastructure with SQLite database, comprehensive business logic, and extensive testing coverage.

## Features

### 🏗️ Core Infrastructure
- **SQLite Database** with Entity Framework Core
- **Clean Architecture** with separation of concerns
- **Dependency Injection** container setup
- **Comprehensive Error Handling** and validation
- **Unit Testing** with xUnit and in-memory databases

### 💰 Expense Management
- **Full CRUD Operations** for expenses
- **Category-based Organization** with 9 pre-seeded categories
- **Multi-Currency Support** (USD, EUR, GBP, JPY, etc.)
- **Date Range Filtering** and search functionality
- **Recurring Expenses** data model support

### 📊 Analytics & Reporting
- **Monthly/Yearly Totals** calculation
- **Category-wise Breakdowns** with percentages
- **Budget Tracking** per category
- **Expense Trends** analysis
- **Data Export** capabilities (CSV ready)

### 🗂️ Category Management
- **Budget Limits** per category with tracking
- **Color-coded Categories** for visual organization
- **Active/Inactive Status** management
- **Custom Category Creation**

### 💾 Data Management
- **Backup & Restore** functionality
- **Data Validation** with attributes
- **Migration Support** for database updates
- **Transaction Safety** with proper error handling

## Project Structure

```
ExpenseManagement.Maui/
├── Models/                    # Data models
│   ├── Category.cs           # Category entity with budget tracking
│   ├── Currency.cs           # Currency definitions and support
│   ├── Expense.cs            # Main expense entity
│   └── ReportModels.cs       # Analytics and reporting models
├── Services/                  # Business logic services
│   ├── CategoryService.cs    # Category management
│   ├── DatabaseService.cs    # Database operations
│   ├── ExpenseDbContext.cs   # Entity Framework context
│   ├── ExpenseService.cs     # Expense management
│   └── NavigationService.cs  # Navigation abstraction
├── Resources/                 # App resources
│   ├── AppIcon/              # Application icons
│   ├── Fonts/                # Custom fonts
│   └── Splash/               # Splash screen assets
├── Program.cs                 # Console demo application
└── GlobalUsings.cs           # Global using statements

Tests/
├── CategoryServiceTests.cs    # Category service unit tests
├── ExpenseServiceTests.cs     # Expense service unit tests
└── ExpenseManagement.Tests.csproj
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQLite (included with .NET)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/notHisu/expense-management-maui.git
   cd expense-management-maui
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the console demo**
   ```bash
   dotnet run
   ```

### Running Tests

```bash
cd Tests
dotnet test --verbosity normal
```

## Usage Examples

### Console Demo
The project includes a console application that demonstrates all core functionality:

```bash
dotnet run
```

This will:
- Initialize the SQLite database
- Seed 9 default categories
- Add sample expenses
- Display analytics and totals
- Show category breakdowns

### Core Services Usage

```csharp
// Setup dependency injection
var services = new ServiceCollection();
services.AddDbContext<ExpenseDbContext>();
services.AddScoped<IExpenseService, ExpenseService>();
services.AddScoped<ICategoryService, CategoryService>();

var serviceProvider = services.BuildServiceProvider();
var expenseService = serviceProvider.GetRequiredService<IExpenseService>();

// Add an expense
var expense = new Expense
{
    Title = "Coffee",
    Description = "Morning coffee",
    Amount = 4.50m,
    Date = DateTime.Now,
    CategoryId = 1, // Food & Dining
    Currency = "USD"
};

var savedExpense = await expenseService.AddExpenseAsync(expense);

// Get monthly totals
var monthlyTotal = await expenseService.GetTotalExpensesForMonthAsync(DateTime.Now);

// Get category breakdown
var breakdown = await expenseService.GetCategoryExpenseSummaryAsync(
    DateTime.Now.AddDays(-30), 
    DateTime.Now
);
```

## Database Schema

### Expenses Table
- `Id` (Primary Key)
- `Title` (Required, max 100 chars)
- `Description` (Optional, max 500 chars)
- `Amount` (Required, decimal)
- `Date` (Required)
- `CategoryId` (Foreign Key)
- `Currency` (Required, 3 chars)
- `ReceiptImagePath` (Optional)
- `IsRecurring` (Boolean)
- `RecurrenceType` (Enum)
- `NextRecurrenceDate` (Optional)
- `CreatedAt` / `UpdatedAt` (Timestamps)

### Categories Table
- `Id` (Primary Key)
- `Name` (Required, unique, max 50 chars)
- `Description` (Optional, max 200 chars)
- `Color` (Required, hex color)
- `Icon` (Optional emoji/icon)
- `BudgetLimit` (Decimal)
- `IsActive` (Boolean)
- `CreatedAt` (Timestamp)

## Pre-seeded Categories

The application comes with 9 default categories:

1. **Food & Dining** 🍽️ - Budget: $500
2. **Transportation** 🚗 - Budget: $300
3. **Entertainment** 🎬 - Budget: $200
4. **Bills & Utilities** 📄 - Budget: $400
5. **Shopping** 🛍️ - Budget: $300
6. **Healthcare** 🏥 - Budget: $250
7. **Education** 📚 - Budget: $150
8. **Travel** ✈️ - Budget: $600
9. **Other** 📦 - Budget: $100

## Testing

The project includes comprehensive unit tests covering:

- ✅ **Expense CRUD Operations**
- ✅ **Category Management**
- ✅ **Search and Filtering**
- ✅ **Date Range Queries**
- ✅ **Analytics Calculations**
- ✅ **Validation Rules**
- ✅ **Business Logic**

### Test Coverage
- 21+ unit tests
- Service layer testing
- Repository pattern testing
- In-memory database testing
- Edge case validation

## Architecture

### Design Patterns
- **Repository Pattern** - Data access abstraction
- **Service Layer Pattern** - Business logic separation
- **Dependency Injection** - Loose coupling
- **Entity Framework** - ORM with migrations

### Technologies Used
- **.NET 8** - Core framework
- **Entity Framework Core** - ORM
- **SQLite** - Database
- **xUnit** - Testing framework
- **Microsoft.Extensions.DependencyInjection** - DI container
- **System.ComponentModel.Annotations** - Validation

## Future MAUI Integration

This project is designed to be easily integrated with .NET MAUI when workloads become available:

1. **MVVM ViewModels** - Ready for UI binding
2. **Service Interfaces** - Clean separation for UI layer
3. **Navigation Service** - Abstract navigation for cross-platform
4. **Resource Structure** - MAUI-compatible resource organization
5. **Platform Support** - Designed for Windows, Android, iOS

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## License

This project is open source and available under the [MIT License](LICENSE).

## Roadmap

- [ ] **MAUI UI Implementation** - Modern cross-platform interface
- [ ] **Receipt Photo Capture** - Camera integration
- [ ] **Data Visualization** - Charts and graphs
- [ ] **Export Features** - PDF and advanced CSV export
- [ ] **Cloud Sync** - Optional cloud backup
- [ ] **Localization** - Multi-language support
- [ ] **Dark Theme** - Theme customization
- [ ] **Advanced Analytics** - Spending trends and insights
