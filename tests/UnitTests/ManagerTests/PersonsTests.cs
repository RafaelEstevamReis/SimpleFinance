namespace UnitTests.ManagerTests;

using Simple.Finance.Tables;
using System;
using System.Linq;
using Xunit;

public class PersonsTests : ManagerTestBase
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreatePerson_WithoutName_Throws(string? name)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => mgr.CreateUpdatePerson(new Person { Id = 0, Name = name! }));

        Assert.Contains(nameof(Person.Name), ex.Message);
        Assert.Empty(mgr.GetAllPersons());
    }

    [Fact]
    public void CreatePerson_AssignsIdAndPersists()
    {
        var id = mgr.CreateUpdatePerson(new Person { Id = 0, Name = "Alice" });

        Assert.True(id > 0);

        var stored = mgr.GetAllPersons().Single();
        Assert.Equal(id, stored.Id);
        Assert.Equal("Alice", stored.Name);
        Assert.False(stored.IsDeleted);
    }

    [Fact]
    public void UpdatePerson_KeepsIdAndDoesNotDuplicate()
    {
        var id = newPerson("Alice");

        var updatedId = mgr.CreateUpdatePerson(new Person { Id = id, Name = "Alice Smith" });

        Assert.Equal(id, updatedId);
        var stored = mgr.GetAllPersons().Single();
        Assert.Equal("Alice Smith", stored.Name);
    }

    [Fact]
    public void GetAllPersons_ReturnsSoftDeletedOnesToo()
    {
        var alive = newPerson("Alice");
        var deleted = mgr.CreateUpdatePerson(new Person { Id = 0, Name = "Bob", IsDeleted = true });

        var stored = mgr.GetAllPersons().ToDictionary(o => o.Id, o => o.IsDeleted);

        Assert.False(stored[alive]);
        Assert.True(stored[deleted]);
    }
}
