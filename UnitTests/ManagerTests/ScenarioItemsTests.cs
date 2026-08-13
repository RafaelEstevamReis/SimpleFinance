namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class ScenarioItemsTests : ManagerTestBase
{
    private readonly long scenarioId;
    private readonly long walletId;

    public ScenarioItemsTests()
    {
        scenarioId = mgr.CreateUpdateScenario(new Scenario { Id = 0, Name = "Car" });
        walletId = newWallet("Checking");
    }

    private ScenarioItem item(decimal value = -100m, long? scenario = null, long? wallet = null, long categoryId = 0)
        => new()
        {
            Id = 0,
            ScenarioId = scenario ?? scenarioId,
            WalletId = wallet ?? walletId,
            CategoryId = categoryId,
            Date = past,
            Value = value,
            Name = "installment",
            IsEnabled = true,
        };

    [Fact]
    public void CreateItem_AssignsIdAndPersists()
    {
        var categoryId = newCategory(isExpense: true);

        var id = mgr.CreateUpdateScenarioItem(new ScenarioItem
        {
            Id = 0,
            ScenarioId = scenarioId,
            WalletId = walletId,
            CategoryId = categoryId,
            Date = past,
            Value = -1250.55m,
            Name = "installment 3/12",
            IsEnabled = true,
            ExternalIdentifier = "ui-42",
        });

        Assert.True(id > 0);

        var stored = mgr.GetScenarioItems(scenarioId).Single();
        Assert.Equal(id, stored.Id);
        Assert.Equal(walletId, stored.WalletId);
        Assert.Equal(categoryId, stored.CategoryId);
        Assert.Equal(past, stored.Date);
        Assert.Equal(-1250.55m, stored.Value);
        Assert.Equal("installment 3/12", stored.Name);
        Assert.True(stored.IsEnabled);
        Assert.Equal("ui-42", stored.ExternalIdentifier);
    }

    [Fact]
    public void CreateItem_KeepsExternalIdentifierNull()
    {
        mgr.CreateUpdateScenarioItem(item());

        Assert.Null(mgr.GetScenarioItems(scenarioId).Single().ExternalIdentifier);
    }

    [Fact]
    public void UpdateItem_KeepsIdAndDoesNotDuplicate()
    {
        var id = mgr.CreateUpdateScenarioItem(item(-100m));

        var toUpdate = item(-250m);
        toUpdate.Id = id;
        toUpdate.IsEnabled = false;
        var updatedId = mgr.CreateUpdateScenarioItem(toUpdate);

        Assert.Equal(id, updatedId);
        var stored = mgr.GetScenarioItems(scenarioId).Single();
        Assert.Equal(-250m, stored.Value);
        Assert.False(stored.IsEnabled);
    }

    [Fact]
    public void CreateItem_WithUnknownScenario_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateScenarioItem(item(scenario: scenarioId + 999)));

        Assert.Contains(nameof(Scenario), ex.Message);
    }

    [Fact]
    public void CreateItem_WithUnknownWallet_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateScenarioItem(item(wallet: walletId + 999)));

        Assert.Contains(nameof(Wallet), ex.Message);
    }

    [Fact]
    public void CreateItem_WithUnknownCategory_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateScenarioItem(item(categoryId: 999)));

        Assert.Contains(nameof(Category), ex.Message);
    }

    [Fact]
    public void CreateItem_WithoutCategory_IsAccepted()
    {
        var id = mgr.CreateUpdateScenarioItem(item(categoryId: 0));

        Assert.Equal(0, mgr.GetScenarioItems(scenarioId).Single(o => o.Id == id).CategoryId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateItem_WithoutName_Throws(string? name)
    {
        var bad = item();
        bad.Name = name!;

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateScenarioItem(bad));

        Assert.Contains(nameof(ScenarioItem.Name), ex.Message);
        Assert.Empty(mgr.GetScenarioItems(scenarioId));
    }

    [Fact]
    public void CreateUpdateBulk_WithoutName_Throws()
    {
        var bad = item(-100m);
        bad.Name = string.Empty;

        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateBulkScenarioItem([bad]));

        Assert.Contains(nameof(ScenarioItem.Name), ex.Message);
        Assert.Empty(mgr.GetScenarioItems(scenarioId));
    }

    [Fact]
    public void CreateItem_WithZeroValue_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateScenarioItem(item(0m)));

        Assert.Contains(nameof(ScenarioItem.Value), ex.Message);
    }

    [Theory]
    [InlineData(250)]
    [InlineData(-250)]
    public void CreateItem_OnExpenseCategory_IsAlwaysNegative(decimal value)
    {
        var categoryId = newCategory(isExpense: true);

        var id = mgr.CreateUpdateScenarioItem(item(value, categoryId: categoryId));

        Assert.Equal(-250m, mgr.GetScenarioItems(scenarioId).Single(o => o.Id == id).Value);
    }

    [Theory]
    [InlineData(250)]
    [InlineData(-250)]
    public void CreateItem_OnIncomeCategory_IsAlwaysPositive(decimal value)
    {
        var categoryId = newCategory(isExpense: false);

        var id = mgr.CreateUpdateScenarioItem(item(value, categoryId: categoryId));

        Assert.Equal(250m, mgr.GetScenarioItems(scenarioId).Single(o => o.Id == id).Value);
    }

    [Theory]
    [InlineData(250)]
    [InlineData(-250)]
    public void CreateItem_WithoutCategory_KeepsCallerSign(decimal value)
    {
        var id = mgr.CreateUpdateScenarioItem(item(value, categoryId: 0));

        Assert.Equal(value, mgr.GetScenarioItems(scenarioId).Single(o => o.Id == id).Value);
    }

    [Fact]
    public void GetScenarioItems_ReturnsOnlyThatScenario()
    {
        var other = mgr.CreateUpdateScenario(new Scenario { Id = 0, Name = "Move" });
        mgr.CreateUpdateScenarioItem(item(-100m));
        mgr.CreateUpdateScenarioItem(item(-200m, scenario: other));

        Assert.Equal(-100m, mgr.GetScenarioItems(scenarioId).Single().Value);
        Assert.Equal(-200m, mgr.GetScenarioItems(other).Single().Value);
    }

    [Fact]
    public void GetScenarioItemById_ReturnsIt()
    {
        var id = mgr.CreateUpdateScenarioItem(item(-100m));

        Assert.Equal(-100m, mgr.GetScenarioItemById(id)!.Value);
    }

    [Fact]
    public void GetScenarioItemById_UnknownId_IsNull()
    {
        var id = mgr.CreateUpdateScenarioItem(item());

        Assert.Null(mgr.GetScenarioItemById(id + 999));
    }

    [Fact]
    public void CreateUpdateBulk_StoresEveryItemAndReturnsTheIds()
    {
        var ids = mgr.CreateUpdateBulkScenarioItem([item(-100m), item(-200m), item(-300m)]).ToArray();

        Assert.Equal(3, ids.Distinct().Count());
        Assert.Equal([-100m, -200m, -300m], ids.Select(o => mgr.GetScenarioItemById(o)!.Value));
    }

    [Fact]
    public void CreateUpdateBulk_ForcesSignOnEveryItem()
    {
        var categoryId = newCategory(isExpense: true);

        var ids = mgr.CreateUpdateBulkScenarioItem([item(250m, categoryId: categoryId), item(-250m, categoryId: categoryId)]);

        Assert.All(ids, id => Assert.Equal(-250m, mgr.GetScenarioItemById(id)!.Value));
    }

    [Fact]
    public void CreateUpdateBulk_AcceptsManyScenariosWalletsAndCategories()
    {
        var otherScenario = mgr.CreateUpdateScenario(new Scenario { Id = 0, Name = "Move" });
        var otherWallet = newWallet("Savings");
        var income = newCategory(isExpense: false, name: "Salary");
        var expense = newCategory(isExpense: true, name: "Rent");

        var ids = mgr.CreateUpdateBulkScenarioItem(
        [
            item(100m, categoryId: income),
            item(100m, scenario: otherScenario, wallet: otherWallet, categoryId: expense),
        ]).ToArray();

        Assert.Equal(100m, mgr.GetScenarioItemById(ids[0])!.Value);
        Assert.Equal(-100m, mgr.GetScenarioItemById(ids[1])!.Value);
    }

    [Fact]
    public void CreateUpdateBulk_UpdatesInPlaceWhenIdIsSet()
    {
        var id = mgr.CreateUpdateScenarioItem(item(-100m));
        var toUpdate = item(-999m);
        toUpdate.Id = id;

        mgr.CreateUpdateBulkScenarioItem([toUpdate]);

        Assert.Equal(-999m, mgr.GetScenarioItems(scenarioId).Single().Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void CreateUpdateBulk_WithUnknownScenario_Throws(int position)
    {
        var bad = item(-100m, scenario: scenarioId + 999);
        var good = item(-200m);
        var batch = position == 0 ? new[] { bad, good } : [good, bad];

        Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateBulkScenarioItem(batch));
    }

    [Fact]
    public void CreateUpdateBulk_WithUnknownWallet_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateBulkScenarioItem([item(wallet: walletId + 999)]));
    }

    [Fact]
    public void CreateUpdateBulk_WithUnknownCategory_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateBulkScenarioItem([item(categoryId: 999)]));
    }

    [Fact]
    public void CreateUpdateBulk_StopsOnInvalidItem_KeepingThePreviousOnes()
    {
        Assert.Throws<InvalidOperationException>(()
            => mgr.CreateUpdateBulkScenarioItem([item(-100m), item(0m), item(-300m)]));

        Assert.Equal(-100m, mgr.GetScenarioItems(scenarioId).Single().Value);
    }

    [Fact]
    public void DeleteScenarioItem_RemovesOnlyThatItem()
    {
        var kept = mgr.CreateUpdateScenarioItem(item(-100m));
        var dropped = mgr.CreateUpdateScenarioItem(item(-200m));

        mgr.DeleteScenarioItem(dropped);

        Assert.Equal(kept, mgr.GetScenarioItems(scenarioId).Single().Id);
    }

    [Fact]
    public void SetScenarioItemEnabled_TogglesOnlyTheInformedOnes()
    {
        var a = mgr.CreateUpdateScenarioItem(item(-100m));
        var b = mgr.CreateUpdateScenarioItem(item(-200m));
        var untouched = mgr.CreateUpdateScenarioItem(item(-300m));

        mgr.SetScenarioItemEnabled([a, b], false);

        var stored = mgr.GetScenarioItems(scenarioId).ToDictionary(o => o.Id, o => o.IsEnabled);
        Assert.False(stored[a]);
        Assert.False(stored[b]);
        Assert.True(stored[untouched]);
    }

    [Fact]
    public void SetScenarioItemEnabled_CrossesScenarios_AndKeepsEveryOtherField()
    {
        var other = mgr.CreateUpdateScenario(new Scenario { Id = 0, Name = "Move" });
        var here = mgr.CreateUpdateScenarioItem(item(-100m));
        var there = mgr.CreateUpdateScenarioItem(item(-200m, scenario: other));

        mgr.SetScenarioItemEnabled([here, there], false);

        var stored = mgr.GetScenarioItemById(here)!;
        Assert.False(stored.IsEnabled);
        Assert.Equal(-100m, stored.Value);
        Assert.Equal("installment", stored.Name);
        Assert.False(mgr.GetScenarioItemById(there)!.IsEnabled);
    }

    [Fact]
    public void SetScenarioItemEnabled_UnknownAndEmptyIds_DoNothing()
    {
        var id = mgr.CreateUpdateScenarioItem(item());

        mgr.SetScenarioItemEnabled([id + 999], false);
        mgr.SetScenarioItemEnabled([], false);

        Assert.True(mgr.GetScenarioItemById(id)!.IsEnabled);
    }

    [Fact]
    public void DeleteScenarioItem_UnknownId_DoesNothing()
    {
        var id = mgr.CreateUpdateScenarioItem(item());

        mgr.DeleteScenarioItem(id + 999);

        Assert.Single(mgr.GetScenarioItems(scenarioId));
    }

    [Fact]
    public void DeleteScenarioItem_KeepsTheScenario()
    {
        var id = mgr.CreateUpdateScenarioItem(item());

        mgr.DeleteScenarioItem(id);

        Assert.Single(mgr.GetScenarios());
        Assert.Empty(mgr.GetScenarioItems(scenarioId));
    }
}
