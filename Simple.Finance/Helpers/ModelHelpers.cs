namespace Simple.Finance.Helpers;

using System.Collections.Generic;
using System.Reflection;

public static class ModelHelpers
{
    public static Dictionary<string, (string, string)> ModelDiff<T>(T? older, T newer)
    {
        var type = typeof(T);
        var dic = new Dictionary<string, (string, string)>();

        foreach (var p in type.GetProperties())
        {
            var fOld = older == null ? null : p.GetValue(older);
            var fNew = p.GetValue(newer);

            var sOld = toString(p, fOld);
            var sNew = toString(p, fNew);

            if (string.Equals(sOld, sNew)) continue;

            dic[p.Name] = (sOld, sNew);
        }

        return dic;
    }

    private static string toString(PropertyInfo p, object? fOld)
    {
        if (fOld == null) return "[NL]";
        return fOld.ToString();
    }
}
