using MauimdApp.Components.MDComponents;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.RegularExpressions;

namespace MauimdApp.Extensions;

public static class MDExtensions
{
    public static RenderTreeBuilder AddDefaultComponent(this RenderTreeBuilder builder, string tag, string content)
    {
        builder.OpenElement(0, tag);
        content = builder.CheckOfCodeQuoteComponent(content);
        builder.AddContent(1, content);
        builder.CloseElement();

        return builder;
    }

    internal static RenderTreeBuilder AddCodeComponent(this RenderTreeBuilder builder, string lang, string title, string code)
    {
        builder.OpenComponent(0, typeof(MDCodeBlock));
        builder.AddAttribute(1, "Language", lang);
        builder.AddAttribute(2, "Title", title);
        builder.AddAttribute(3, "Code", code);
        builder.CloseComponent();

        return builder;
    }

    internal static RenderTreeBuilder AddUnorderedListComponent(this RenderTreeBuilder builder, string[] items)
    {
        builder.OpenElement(0, "ul");
        foreach (var item in items)
        {
            builder.OpenElement(1, "li");
            string content = builder.CheckOfCodeQuoteComponent(item);
            builder.AddContent(2, content);
            builder.CloseElement();
        }
        builder.CloseElement();
        return builder;
    }

    internal static RenderTreeBuilder AddOrderedListComponent(this RenderTreeBuilder builder, Dictionary<ushort, string> items)
    {
        builder.OpenElement(0, "ol");
        foreach (var item in items)
        {
            builder.OpenElement(1, "li");
            string content = builder.CheckOfCodeQuoteComponent(item.Value);
            builder.AddContent(2, content);
            builder.CloseElement();
        }
        builder.CloseElement();
        return builder;
    }

    internal static string CheckOfCodeQuoteComponent(this RenderTreeBuilder builder, string content)
    {
        var matches = MDRegexs.GetCodeQuoteRegex().Matches(content);

        foreach (Match match in matches)
        {
            var index = content.IndexOf(match.Value);

            builder.AddContent(1, content[..index]);
            builder.OpenElement(2, "code");
            builder.AddContent(3, match.Value[1..^1]);
            builder.CloseElement();
            content = content[(index + match.Value.Length)..];
        }

        return content;
    } 
}