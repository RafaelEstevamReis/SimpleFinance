namespace Simple.Finance.Helpers;

using System;
using System.Collections.Generic;
using System.Globalization;

public static class ModelHelpers
{
    public static Dictionary<string, (string, string)> ModelDiff<T>(T? older, T newer)
    {
        var type = typeof(T);
        var dic = new Dictionary<string, (string, string)>();

        foreach (var p in type.GetProperties())
        {
            // Ignore Getter only
            if (!p.CanWrite) continue;

            var fOld = older == null ? null : p.GetValue(older);
            var fNew = p.GetValue(newer);

            var sOld = toString(fOld);
            var sNew = toString(fNew);

            if (string.Equals(sOld, sNew)) continue;

            dic[p.Name] = (sOld, sNew);
        }

        return dic;
    }

    /// <summary>
    /// Culture-independent rendering of a value for the change log.
    /// </summary>
    private static string toString(object? value)
    {
        if (value == null) return "[NL]";

        // A decimal read back from Sqlite can carry a different scale than the one just built ('0.0' against '0')
        if (value is decimal dec) return Math.Round(dec, 10).ToString("0.##########", CultureInfo.InvariantCulture);
        if (value is DateTime date) return date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        if (value is IFormattable formattable) return formattable.ToString(null, CultureInfo.InvariantCulture);

        return value.ToString();
    }
}
