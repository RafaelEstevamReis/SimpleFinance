namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class CategoriesTests : ManagerTestBase
{
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
