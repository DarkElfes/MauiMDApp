using System.Text.RegularExpressions;

namespace MauimdApp.Shared.Extensions;

public static partial class MDRegexs
{
    [GeneratedRegex(@"```([\w+#/\-*]*)(?: *title:([^ \n]*))? *\n([\s\S]*?)\n?```")]
    private static partial Regex CodeRegex();
    public static Regex GetCodeRegex() => CodeRegex();


    [GeneratedRegex(@"`([^`]*)`")]
    private static partial Regex CodeQuoteRegex();
    public static Regex GetCodeQuoteRegex() => CodeQuoteRegex();


    [GeneratedRegex(@"^(#{1,6}) ([\s\S]+)$")]
    private static partial Regex TitleRegex();
    public static Regex GetTitleRegex() => TitleRegex();


    [GeneratedRegex(@"^(\d)+\. ([\s\S]*)$")]
    private static partial Regex OrderedListRegex();
    public static Regex GetOrederedListRegex() => OrderedListRegex();


    [GeneratedRegex(@"^(\t*)- ([\s\S]*)$")]
    private static partial Regex UnorderedListRegex();
    public static Regex GetUnorderedListRegex() => UnorderedListRegex();
}