namespace UnitTests.HelperTests;

using Simple.Finance.Helpers;
using System;
using Xunit;

public class DateHelpersTests
{
    [Fact]
    public void StartOfYear_ReturnsFirstDayOfYearAtMidnight()
    {
        var dt = new DateTime(2025, 7, 15, 14, 30, 45, DateTimeKind.Utc);
        var result = dt.StartOfYear();

        Assert.Equal(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void EndOfYear_ReturnsLastInstantOfYear()
    {
        var dt = new DateTime(2025, 7, 15, 14, 30, 45, DateTimeKind.Local);
        var result = dt.EndOfYear();

        Assert.Equal(new DateTime(2025, 12, 31, 23, 59, 59, 999, DateTimeKind.Local), result);
    }

    [Fact]
    public void EndOfYear_OnDecember31_ReturnsSameYearEnd()
    {
        var dt = new DateTime(2025, 12, 31, 10, 0, 0, DateTimeKind.Unspecified);
        var result = dt.EndOfYear();

        Assert.Equal(new DateTime(2025, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void StartOfMonth_ReturnsFirstDayOfMonthAtMidnight()
    {
        var dt = new DateTime(2025, 3, 20, 8, 15, 0, DateTimeKind.Utc);
        var result = dt.StartOfMonth();

        Assert.Equal(new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void EndOfMonth_ReturnsLastInstantOfMonth()
    {
        // Month with 31 days
        var dt31 = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Local);
        Assert.Equal(new DateTime(2025, 1, 31, 23, 59, 59, 999, DateTimeKind.Local), dt31.EndOfMonth());

        // February (28 dias)
        var dtFebNonLeap = new DateTime(2023, 2, 10, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(new DateTime(2023, 2, 28, 23, 59, 59, 999, DateTimeKind.Utc), dtFebNonLeap.EndOfMonth());

        // February (leap year with 29 days)
        var dtFebLeap = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Unspecified);
        Assert.Equal(new DateTime(2024, 2, 29, 23, 59, 59, 999, DateTimeKind.Unspecified), dtFebLeap.EndOfMonth());

        // December
        var dtDec = new DateTime(2025, 12, 20, 0, 0, 0, DateTimeKind.Local);
        Assert.Equal(new DateTime(2025, 12, 31, 23, 59, 59, 999, DateTimeKind.Local), dtDec.EndOfMonth());
    }

    [Fact]
    public void StartOfDay_ReturnsMidnightOfTheDay()
    {
        var dt = new DateTime(2025, 12, 31, 14, 30, 45, 999, DateTimeKind.Utc);
        var result = dt.StartOfDay();

        Assert.Equal(new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void EndOfDay_ReturnsLastInstantOfTheDay()
    {
        var dt = new DateTime(2025, 12, 31, 14, 30, 45, DateTimeKind.Local);
        var result = dt.EndOfDay();

        Assert.Equal(new DateTime(2025, 12, 31, 23, 59, 59, 999, DateTimeKind.Local), result);
    }

    [Fact]
    public void StartOfHour_ReturnsStartOfTheHour()
    {
        var dt = new DateTime(2025, 12, 31, 14, 30, 45, DateTimeKind.Utc);
        var result = dt.StartOfHour();

        Assert.Equal(new DateTime(2025, 12, 31, 14, 0, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void EndOfHour_ReturnsLastInstantOfTheHour()
    {
        var dt = new DateTime(2025, 12, 31, 14, 30, 45, DateTimeKind.Local);
        var result = dt.EndOfHour();

        Assert.Equal(new DateTime(2025, 12, 31, 14, 59, 59, 999, DateTimeKind.Local), result);
    }

    [Fact]
    public void AllMethods_PreserveDateTimeKind()
    {
        var kinds = new[] { DateTimeKind.Utc, DateTimeKind.Local, DateTimeKind.Unspecified };

        foreach (var kind in kinds)
        {
            var dt = new DateTime(2025, 6, 15, 12, 30, 0, kind);

            Assert.Equal(kind, dt.StartOfYear().Kind);
            Assert.Equal(kind, dt.EndOfYear().Kind);
            Assert.Equal(kind, dt.StartOfMonth().Kind);
            Assert.Equal(kind, dt.EndOfMonth().Kind);
            Assert.Equal(kind, dt.StartOfDay().Kind);
            Assert.Equal(kind, dt.EndOfDay().Kind);
            Assert.Equal(kind, dt.StartOfHour().Kind);
            Assert.Equal(kind, dt.EndOfHour().Kind);
        }
    }
}