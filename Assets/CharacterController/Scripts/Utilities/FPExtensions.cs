using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class FPExtensions
{
    public static void Send(this EventHandler handler, object sender)
        => handler?.Invoke(sender, EventArgs.Empty);

    // Used by FPStateMachineBase to derive state class names from enum type names.
    public static string FPRemoveSymbols(this string s)
        => s.Replace("'", "").Replace("\"", "").Replace("/", "")
             .Replace("(", "").Replace(")", "").Replace(" ", "")
             .Replace(".", "").Replace("-", "").Replace("+", "")
             .Replace("[", "").Replace("]", "").Replace("?", "");

    public static T RandomItem<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new IndexOutOfRangeException("Cannot select a random item from an empty list.");
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}
