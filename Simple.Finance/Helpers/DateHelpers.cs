namespace Simple.Finance.Helpers;

using System;

public static class DateHelpers
{
    public static DateTime StartOfMonth(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind);
    }
    public static DateTime EndOfMonth(this DateTime dt)
    {
        var start = StartOfMonth(dt);
        return start.AddMonths(1).AddMilliseconds(-1);
    }
}
