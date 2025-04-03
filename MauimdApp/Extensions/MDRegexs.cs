using System.Text.RegularExpressions;

namespace MauimdApp.Extensions;

public static partial class MDRegexs
{
    [GeneratedRegex(@"```([\w+#/\-*]*)(?: *title:([^ \n]*))? *\n([\s\S]*?)\n?```")]
    private static partial Regex CodeRegex();
    public static Regex GetCodeRegex() => CodeRegex();


    [GeneratedRegex(@"`([^`]*)`")]
    private static partial Regex CodeQuoteRegex();
    public static Regex GetCodeQuoteRegex() => CodeQuoteRegex();


    [GeneratedRegex(@"^(\d)+. ([\s\S]*)$")]
    private static partial Regex NumberedListRegex();
    public static Regex GetNumberedListRegex() => NumberedListRegex();
}