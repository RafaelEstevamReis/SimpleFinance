namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class ScenarioProjectionTests : ManagerTestBase
{
    private readonly long walletId;
    private readonly long otherWalletId;

    public ScenarioProjectionTests()
    {
        walletId = newWallet("Checking");
        otherWalletId = newWallet("Savings");
    }

    private long newScenario(bool isActive, string name = "Scenario")
        => mgr.CreateUpdateScenario(new Scenario { Id = 0, Name = name, IsActive = isActive });

    private long newItem(long scenarioId, DateTime date, decimal value = -100m, bool isEnabled = true, long? wallet = null)
        => mgr.CreateUpdateScenarioItem(new ScenarioItem
        {
            Id = 0,
            ScenarioId = scenarioId,
            WalletId = wallet ?? walletId,
            CategoryId = 0,
            Date = date,
            Value = value,
            Name = "item",
            IsEnabled = isEnabled,
        });

    [Fact]
    public void Project_ReturnsItemsOldestFirst()
    {
        var scenario = newScenario(isActive: true);
        newItem(scenario, past.AddDays(30), -30m);
        newItem(scenario, past.AddDays(10), -10m);
        newItem(scenario, past.AddDays(20), -20m);

        var values = mgr.ProjectScenariosItemsFor(walletId).Select(o => o.Value).ToArray();

        Assert.Equal([-10m, -20m, -30m], values);
    }

    [Fact]
    public void Project_ComposesEveryActiveScenario()
    {
        var car = newScenario(isActive: true, name: "Car");
        var move = newScenario(isActive: true, name: "Move");
        newItem(car, past.AddDays(10), -10m);
        newItem(move, past.AddDays(20), -20m);

        var values = mgr.ProjectScenariosItemsFor(walletId).Select(o => o.Value).ToArray();

        Assert.Equal([-10m, -20m], values);
    }

    [Fact]
    public void Project_SkipsInactiveScenarios()
    {
        var active = newScenario(isActive: true);
        var inactive = newScenario(isActive: false);
        var kept = newItem(active, past.AddDays(10));
        newItem(inactive, past.AddDays(20));

        Assert.Equal(kept, mgr.ProjectScenariosItemsFor(walletId).Single().Id);
    }

    [Fact]
    public void Project_SkipsDisabledItems()
    {
        var scenario = newScenario(isActive: true);
        var kept = newItem(scenario, past.AddDays(10), isEnabled: true);
        newItem(scenario, past.AddDays(20), isEnabled: false);

        Assert.Equal(kept, mgr.ProjectScenariosItemsFor(walletId).Single().Id);
    }

    [Fact]
    public void Project_SkipsOtherWallets()
    {
        var scenario = newScenario(isActive: true);
        var kept = newItem(scenario, past.AddDays(10));
        newItem(scenario, past.AddDays(20), wallet: otherWalletId);

        Assert.Equal(kept, mgr.ProjectScenariosItemsFor(walletId).Single().Id);
        Assert.Single(mgr.ProjectScenariosItemsFor(otherWalletId));
    }

    [Fact]
    public void Project_SameDate_FallsBackToId()
    {
        var scenario = newScenario(isActive: true);
        var first = newItem(scenario, past, -10m);
        var second = newItem(scenario, past, -20m);

        var ids = mgr.ProjectScenariosItemsFor(walletId).Select(o => o.Id).ToArray();

        Assert.Equal([first, second], ids);
    }

    [Fact]
    public void Project_WithoutAnything_IsEmpty()
    {
        Assert.Empty(mgr.ProjectScenariosItemsFor(walletId));
    }

    [Fact]
    public void ProjectWindow_KeepsOnlyTheWindow_BoundsIncluded()
    {
        var scenario = newScenario(isActive: true);
        newItem(scenario, past.AddDays(9), -9m);
        newItem(scenario, past.AddDays(10), -10m);
        newItem(scenario, past.AddDays(20), -20m);
        newItem(scenario, past.AddDays(21), -21m);

        var values = mgr.ProjectScenariosItems(past.AddDays(10), past.AddDays(20)).Select(o => o.Value);

        Assert.Equal([-10m, -20m], values);
    }

    [Fact]
    public void ProjectWindow_CoversEveryWallet_OldestFirst()
    {
        var scenario = newScenario(isActive: true);
        newItem(scenario, past.AddDays(20), -20m, wallet: otherWalletId);
        newItem(scenario, past.AddDays(10), -10m);

        var values = mgr.ProjectScenariosItems(past, past.AddDays(30)).Select(o => o.Value);

        Assert.Equal([-10m, -20m], values);
    }

    [Fact]
    public void ProjectWindow_ActiveTrue_IsScenarioActiveAndItemEnabled()
    {
        var active = newScenario(isActive: true);
        var inactive = newScenario(isActive: false);
        var kept = newItem(active, past.AddDays(10));
        newItem(active, past.AddDays(11), isEnabled: false);
        newItem(inactive, past.AddDays(12));
        newItem(inactive, past.AddDays(13), isEnabled: false);

        var items = mgr.ProjectScenariosItems(past, past.AddDays(30), isActive: true);

        Assert.Equal(kept, items.Single().Id);
    }

    [Fact]
    public void ProjectWindow_ActiveFalse_IsExactlyTheComplement()
    {
        var active = newScenario(isActive: true);
        var inactive = newScenario(isActive: false);
        newItem(active, past.AddDays(10));
        var disabled = newItem(active, past.AddDays(11), isEnabled: false);
        var onInactive = newItem(inactive, past.AddDays(12));
        var both = newItem(inactive, past.AddDays(13), isEnabled: false);

        var ids = mgr.ProjectScenariosItems(past, past.AddDays(30), isActive: false).Select(o => o.Id);

        Assert.Equal([disabled, onInactive, both], ids);
    }

    [Fact]
    public void ProjectWindow_ActiveNull_TakesEverything()
    {
        var active = newScenario(isActive: true);
        var inactive = newScenario(isActive: false);
        newItem(active, past.AddDays(10));
        newItem(active, past.AddDays(11), isEnabled: false);
        newItem(inactive, past.AddDays(12));

        Assert.Equal(3, mgr.ProjectScenariosItems(past, past.AddDays(30), isActive: null).Count());
    }

    [Fact]
    public void ProjectWindow_DefaultsToActiveOnly()
    {
        var active = newScenario(isActive: true);
        var inactive = newScenario(isActive: false);
        var kept = newItem(active, past.AddDays(10));
        newItem(inactive, past.AddDays(12));

        Assert.Equal(kept, mgr.ProjectScenariosItems(past, past.AddDays(30)).Single().Id);
    }
}
