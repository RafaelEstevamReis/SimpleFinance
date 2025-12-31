namespace Simple.Finance.Helpers;

using System;

public static class DateHelpers
{
    public static DateTime StartOfYear(this DateTime dt)
    {
        return new DateTime(dt.Year, 1, 1, 0, 0, 0, dt.Kind);
    }
    public static DateTime EndOfYear(this DateTime dt)
    {
        var start = StartOfYear(dt);
        return start.AddYears(1).AddMilliseconds(-1);
    }

    public static DateTime StartOfMonth(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind);
    }
    public static DateTime EndOfMonth(this DateTime dt)
    {
        var start = StartOfMonth(dt);
        return start.AddMonths(1).AddMilliseconds(-1);
    }

    public static DateTime StartOfDay(this DateTime dt)
    {
        return dt.Date;
    }
    public static DateTime EndOfDay(this DateTime dt)
    {
        var start = StartOfDay(dt);
        return start.AddDays(1).AddMilliseconds(-1);
    }

    public static DateTime StartOfHour(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, dt.Kind);
    }
    public static DateTime EndOfHour(this DateTime dt)
    {
        var start = StartOfHour(dt);
        return start.AddHours(1).AddMilliseconds(-1);
    }
}
