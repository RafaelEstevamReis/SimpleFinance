namespace UnitTests.ManagerTests;

using Simple.Finance;
using Simple.Finance.Tables;
using Simple.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

public class BuildAndInitializeTests : IDisposable
{
    private readonly List<string> tempFiles = [];

    private string tempPath(string suffix = ".db")
    {
        var file = Path.Combine(Path.GetTempPath(), $"simplefinance_{Guid.NewGuid():N}{suffix}");
        tempFiles.Add(file);
        tempFiles.Add(file + ".gz");
        return file;
    }

    [Theory]
    [InlineData("ChangeLog")]
    [InlineData("ChangeLogItem")]
    [InlineData("Category")]
    [InlineData("Wallet")]
    [InlineData("Person")]
    [InlineData("Transac")]
    [InlineData("Scenario")]
    [InlineData("ScenarioItem")]
    public void Initialize_CreatesEveryTable(string tableName)
    {
        var file = tempPath();
        new Manager(file).Initialize();

        using var cnn = ConnectionFactory.FromFile(file).GetConnection();
        var count = cnn.ExecuteScalar<long>("SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @name", new { name = tableName });

        Assert.Equal(1, count);
    }

    [Fact]
    public void Initialize_IsIdempotent_AndPreservesData()
    {
        var file = tempPath();
        var mgr = new Manager(file);
        mgr.Initialize();
        var walletId = mgr.CreateUpdateWallet(new Wallet { Id = 0, Name = "Checking" });

        mgr.Initialize();

        var wallets = new List<Wallet>(mgr.GetWallets());
        Assert.Single(wallets);
        Assert.Equal(walletId, wallets[0].Id);
        Assert.Equal("Checking", wallets[0].Name);
    }

    [Fact]
    public void Initialize_WithBackup_AndNullName_Throws()
    {
        var mgr = new Manager(tempPath());

        var ex = Assert.Throws<ArgumentNullException>(() => mgr.Initialize(createBackup: true, backupName: null));
        Assert.Equal("backupName", ex.ParamName);
    }

    [Fact]
    public void Initialize_WithBackup_OnExternalDatabase_Throws()
    {
        var mgr = Manager.FromDatabase(ConnectionFactory.FromFile(tempPath()));

        Assert.Throws<ArgumentException>(() => mgr.Initialize(createBackup: true, backupName: tempPath(".bak")));
    }

    [Fact]
    public void Initialize_WithBackup_CompressesExistingDatabase()
    {
        var file = tempPath();
        var backup = tempPath(".bak");

        var mgr = new Manager(file);
        mgr.Initialize();
        mgr.CreateUpdateWallet(new Wallet { Id = 0, Name = "Checking" });

        mgr.Initialize(createBackup: true, backupName: backup);

        var gz = new FileInfo(backup + ".gz");
        Assert.True(gz.Exists, "backup file was not created");
        Assert.True(gz.Length > 0, "backup file is empty");
    }

    [Fact]
    public void Initialize_WithBackup_OnFirstRun_SkipsBackup()
    {
        var file = tempPath();
        var backup = tempPath(".bak");

        new Manager(file).Initialize(createBackup: true, backupName: backup);

        Assert.False(File.Exists(backup + ".gz"));
    }

    [Fact]
    public void FromDatabase_SharesTheUnderlyingDatabase()
    {
        var file = tempPath();
        var owner = new Manager(file);
        owner.Initialize();

        var external = Manager.FromDatabase(ConnectionFactory.FromFile(file));
        var walletId = external.CreateUpdateWallet(new Wallet { Id = 0, Name = "Savings" });

        Assert.Contains(owner.GetWallets(), w => w.Id == walletId && w.Name == "Savings");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var file in tempFiles)
        {
            try
            {
                File.Delete(file);
            }
            catch (IOException) { /* temp file, best effort */ }
        }
    }
}
