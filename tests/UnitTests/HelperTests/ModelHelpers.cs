namespace UnitTests.HelperTests;

using Simple.Finance.Helpers;
using System;
using System.Globalization;
using System.Threading;
using Xunit;

public class ModelHelpersTests
{
    private record Model
    {
        public decimal Value { get; set; }
        public DateTime When { get; set; }
        public string? Text { get; set; }
        public double Rate { get; set; }
    }

    /// <summary>Runs an action pretending the machine is not in an invariant culture</summary>
    private static void onCulture(string culture, Action action)
    {
        var previous = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        try
        {
            action();
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previous;
        }
    }

    [Fact]
    public void ModelDiff_WithNullOlder_MarksEveryFieldAsNew()
    {
        var diff = ModelHelpers.ModelDiff(null, new Model { Value = 1m, Text = "a" });

        Assert.Equal(("[NL]", "1"), diff[nameof(Model.Value)]);
        Assert.Equal(("[NL]", "a"), diff[nameof(Model.Text)]);
    }

    [Fact]
    public void ModelDiff_WithNullProperty_UsesTheNullMarker()
    {
        var diff = ModelHelpers.ModelDiff(new Model { Text = "a" }, new Model { Text = null });

        Assert.Equal(("a", "[NL]"), diff[nameof(Model.Text)]);
    }

    [Fact]
    public void ModelDiff_IgnoresDecimalScale()
    {
        // A decimal read back from Sqlite carries a different scale than the one just built,
        // '0.0' against '0' is the same money and must not be logged as a change
        Assert.DoesNotContain(nameof(Model.Value), ModelHelpers.ModelDiff(new Model { Value = 0.0m }, new Model { Value = 0m }).Keys);
        Assert.DoesNotContain(nameof(Model.Value), ModelHelpers.ModelDiff(new Model { Value = -100.0m }, new Model { Value = -100m }).Keys);
        Assert.DoesNotContain(nameof(Model.Value), ModelHelpers.ModelDiff(new Model { Value = 1.50m }, new Model { Value = 1.5m }).Keys);
    }

    [Fact]
    public void ModelDiff_KeepsTenDecimalPlaces()
    {
        var diff = ModelHelpers.ModelDiff(new Model { Value = 0m }, new Model { Value = 0.0000000001m });

        Assert.Equal(("0", "0.0000000001"), diff[nameof(Model.Value)]);
    }

    [Fact]
    public void ModelDiff_RoundsBeyondTenDecimalPlaces()
    {
        var diff = ModelHelpers.ModelDiff(new Model { Value = 0m }, new Model { Value = 0.00000000001m });

        Assert.DoesNotContain(nameof(Model.Value), diff.Keys);
    }

    [Fact]
    public void ModelDiff_WritesDecimalsWithADot_OnAnyCulture()
        => onCulture("pt-BR", () =>
        {
            var diff = ModelHelpers.ModelDiff(new Model(), new Model { Value = -120.5m });

            Assert.Equal("-120.5", diff[nameof(Model.Value)].Item2);
        });

    [Fact]
    public void ModelDiff_WritesDoublesWithADot_OnAnyCulture()
        => onCulture("pt-BR", () =>
        {
            var diff = ModelHelpers.ModelDiff(new Model(), new Model { Rate = 1.25 });

            Assert.Equal("1.25", diff[nameof(Model.Rate)].Item2);
        });

    [Fact]
    public void ModelDiff_WritesDatesSortableAndWithoutMilliseconds()
        => onCulture("pt-BR", () =>
        {
            var diff = ModelHelpers.ModelDiff(new Model(), new Model { When = new DateTime(2026, 8, 9, 15, 26, 6, 519, DateTimeKind.Utc) });

            Assert.Equal("2026-08-09 15:26:06", diff[nameof(Model.When)].Item2);
        });

    [Fact]
    public void ModelDiff_IgnoresSubSecondDateChanges()
    {
        var moment = new DateTime(2026, 8, 9, 15, 26, 6, DateTimeKind.Utc);

        var diff = ModelHelpers.ModelDiff(new Model { When = moment }, new Model { When = moment.AddMilliseconds(400) });

        Assert.DoesNotContain(nameof(Model.When), diff.Keys);
    }
}
