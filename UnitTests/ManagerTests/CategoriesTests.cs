namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class CategoriesTests : ManagerTestBase
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateCategory_WithoutName_Throws(string? name)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateCategory(new Category
        {
            Id = 0,
            Name = name!,
            Description = "no name, still a category",
            IsExpense = true,
        }));

        Assert.Contains(nameof(Category.Name), ex.Message);
        Assert.Empty(mgr.GetCategories());
    }

    [Fact]
    public void CreateCategory_WithNegativeBudget_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateCategory(new Category
        {
            Id = 0,
            Name = "Rent",
            IsExpense = true,
            MonthlyBudget = -1200m,
        }));

        Assert.Contains(nameof(Category.MonthlyBudget), ex.Message);
        Assert.Empty(mgr.GetCategories());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1200)]
    public void CreateCategory_KeepsBudgetAsSent(decimal budget)
    {
        // an expense budget is a limit, not money: it stays positive even though its transactions are negative
        var id = mgr.CreateUpdateCategory(new Category
        {
            Id = 0,
            Name = "Rent",
            IsExpense = true,
            MonthlyBudget = budget,
        });

        Assert.Equal(budget, mgr.GetCategories().Single(o => o.Id == id).MonthlyBudget);
    }

    [Fact]
    public void CreateCategory_AssignsIdAndPersists()
    {
        var id = mgr.CreateUpdateCategory(new Category
        {
            Id = 0,
            Name = "Rent",
            Description = "House",
            IsExpense = true,
        });

        Assert.True(id > 0);

        var stored = mgr.GetCategories().Single();
        Assert.Equal(id, stored.Id);
        Assert.Equal("Rent", stored.Name);
        Assert.Equal("House", stored.Description);
        Assert.True(stored.IsExpense);
    }

    [Fact]
    public void UpdateCategory_KeepingKind_UpdatesFields()
    {
        var id = newCategory(isExpense: true, "Rent");

        var updatedId = mgr.CreateUpdateCategory(new Category
        {
            Id = id,
            Name = "Housing",
            Description = "Rent and taxes",
            IsExpense = true,
        });

        Assert.Equal(id, updatedId);
        var stored = mgr.GetCategories().Single();
        Assert.Equal("Housing", stored.Name);
        Assert.Equal("Rent and taxes", stored.Description);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UpdateCategory_ChangingIsExpense_Throws(bool originalIsExpense)
    {
        var id = newCategory(originalIsExpense, "Original");

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateCategory(new Category
        {
            Id = id,
            Name = "Flipped",
            IsExpense = !originalIsExpense,
        }));

        Assert.Contains(nameof(Category.IsExpense), ex.Message);

        var stored = mgr.GetCategories().Single();
        Assert.Equal(originalIsExpense, stored.IsExpense);
        Assert.Equal("Original", stored.Name);
    }

    [Fact]
    public void GetCategories_ReturnsExpensesAndIncomes()
    {
        var expense = newCategory(isExpense: true, "Rent");
        var income = newCategory(isExpense: false, "Salary");

        var stored = mgr.GetCategories().ToDictionary(o => o.Id, o => o.IsExpense);

        Assert.True(stored[expense]);
        Assert.False(stored[income]);
    }
}
