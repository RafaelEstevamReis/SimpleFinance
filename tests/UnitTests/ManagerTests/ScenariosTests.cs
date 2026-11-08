namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class ScenariosTests : ManagerTestBase
{
    private long newScenario(string name = "Scenario", bool isActive = false)
        => mgr.CreateUpdateScenario(new Scenario
        {
            Id = 0,
            Name = name,
            IsActive = isActive,
        });

    private long newItem(long scenarioId, decimal value)
        => mgr.CreateUpdateScenarioItem(new ScenarioItem
        {
            Id = 0,
            ScenarioId = scenarioId,
            WalletId = newWallet(),
            CategoryId = 0,
            Date = past,
            Value = value,
            Name = "item",
            IsEnabled = true,
        });

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateScenario_WithoutName_Throws(string? name)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdateScenario(new Scenario { Id = 0, Name = name! }));

        Assert.Contains(nameof(Scenario.Name), ex.Message);
        Assert.Empty(mgr.GetScenarios());
    }

    [Fact]
    public void CreateScenario_AssignsIdAndPersists()
    {
        var id = mgr.CreateUpdateScenario(new Scenario
        {
            Id = 0,
            Name = "Car",
            Description = "cash or 12x",
            IsActive = true,
        });

        Assert.True(id > 0);

        var stored = mgr.GetScenarios().Single();
        Assert.Equal(id, stored.Id);
        Assert.Equal("Car", stored.Name);
        Assert.Equal("cash or 12x", stored.Description);
        Assert.True(stored.IsActive);
    }

    [Fact]
    public void CreateScenario_KeepsDescriptionNull()
    {
        var id = newScenario("Car");

        Assert.Null(mgr.GetScenarios().Single(o => o.Id == id).Description);
    }

    [Fact]
    public void GetScenarioById_ReturnsIt()
    {
        var id = newScenario("Car", isActive: true);
        newScenario("Move");

        var stored = mgr.GetScenarioById(id);

        Assert.Equal("Car", stored!.Name);
        Assert.True(stored.IsActive);
    }

    [Fact]
    public void GetScenarioById_UnknownId_IsNull()
    {
        var id = newScenario("Car");

        Assert.Null(mgr.GetScenarioById(id + 999));
    }

    [Fact]
    public void UpdateScenario_KeepsIdAndDoesNotDuplicate()
    {
        var id = newScenario("Car");

        var updatedId = mgr.CreateUpdateScenario(new Scenario { Id = id, Name = "New car", IsActive = true });

        Assert.Equal(id, updatedId);
        var stored = mgr.GetScenarios().Single();
        Assert.Equal("New car", stored.Name);
        Assert.True(stored.IsActive);
    }

    [Fact]
    public void GetScenarios_ReturnsInactiveOnesToo()
    {
        var active = newScenario("Car", isActive: true);
        var inactive = newScenario("Move", isActive: false);

        var stored = mgr.GetScenarios().ToDictionary(o => o.Id, o => o.IsActive);

        Assert.True(stored[active]);
        Assert.False(stored[inactive]);
    }

    [Fact]
    public void DeleteScenario_ReallyRemovesIt()
    {
        var kept = newScenario("Car");
        var dropped = newScenario("Move");

        mgr.DeleteScenario(dropped);

        Assert.Equal(kept, mgr.GetScenarios().Single().Id);
    }

    [Fact]
    public void DeleteScenario_RemovesItsItemsOnly()
    {
        var kept = newScenario("Car");
        var dropped = newScenario("Move");
        newItem(kept, 100m);
        newItem(dropped, -50m);
        newItem(dropped, -25m);

        mgr.DeleteScenario(dropped);

        Assert.Empty(mgr.GetScenarioItems(dropped));
        Assert.Equal(100m, mgr.GetScenarioItems(kept).Single().Value);
    }

    [Fact]
    public void DeleteScenario_UnknownId_DoesNothing()
    {
        var kept = newScenario("Car");
        newItem(kept, 100m);

        mgr.DeleteScenario(kept + 999);

        Assert.Single(mgr.GetScenarios());
        Assert.Single(mgr.GetScenarioItems(kept));
    }

    [Fact]
    public void SetScenarioActive_TogglesOnlyTheInformedOnes()
    {
        var a = newScenario("A");
        var b = newScenario("B");
        var untouched = newScenario("C", isActive: false);

        mgr.SetScenarioActive([a, b], true);

        var stored = mgr.GetScenarios().ToDictionary(o => o.Id, o => o.IsActive);
        Assert.True(stored[a]);
        Assert.True(stored[b]);
        Assert.False(stored[untouched]);
    }

    [Fact]
    public void SetScenarioActive_TurnsThemOffToo()
    {
        var a = newScenario("A", isActive: true);
        var b = newScenario("B", isActive: true);

        mgr.SetScenarioActive([a, b], false);

        Assert.All(mgr.GetScenarios(), o => Assert.False(o.IsActive));
    }

    [Fact]
    public void SetScenarioActive_KeepsEveryOtherField()
    {
        var id = mgr.CreateUpdateScenario(new Scenario { Id = 0, Name = "Car", Description = "12x", IsActive = false });

        mgr.SetScenarioActive([id], true);

        var stored = mgr.GetScenarioById(id)!;
        Assert.Equal("Car", stored.Name);
        Assert.Equal("12x", stored.Description);
        Assert.True(stored.IsActive);
    }

    [Fact]
    public void SetScenarioActive_UnknownAndEmptyIds_DoNothing()
    {
        var id = newScenario("Car", isActive: true);

        mgr.SetScenarioActive([id + 999], false);
        mgr.SetScenarioActive([], false);

        Assert.True(mgr.GetScenarioById(id)!.IsActive);
    }
}
