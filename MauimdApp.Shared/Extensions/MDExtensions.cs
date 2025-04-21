using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.RegularExpressions;
using MauimdApp.Shared.Components.MDComponents;

namespace MauimdApp.Shared.Extensions;

public static class MDExtensions
{

    #region  Typography
    internal static RenderTreeBuilder AddParagraph(this RenderTreeBuilder builder, string content)
    {
        builder.OpenComponent<Paragraph>(0);
        builder.AddAttribute(1, "ChildContent", (RenderFragment)(builder2 =>
        {
            content = builder2.CheckOfCodeQuoteComponent(content, 2);
            builder2.AddContent(5, content);
        }));
        builder.CloseComponent();
        return builder;
    }
    internal static RenderTreeBuilder AddTitle(this RenderTreeBuilder builder, int level, string content)
    {
        builder.OpenComponent<Title>(0);
        builder.AddAttribute(1, "Level", level);
        builder.AddAttribute(2, "ChildContent", (RenderFragment)(builder2 =>
        {
            content = builder2.CheckOfCodeQuoteComponent(content, 3);
            builder2.AddContent(6, content);
        }));
        builder.CloseComponent();

        return builder;
    }

    // Lists
    internal static RenderTreeBuilder AddOrderedListComponent(this RenderTreeBuilder builder, Dictionary<ushort, string> items)
    {
        builder.OpenComponent<Paragraph>(0);
        builder.AddAttribute(1, "ChildContent", (RenderFragment)(builder2 =>
        {
            builder2.OpenElement(2, "ol");
            foreach (var item in items)
            {
                builder2.OpenElement(3, "li");
                builder2.AddAttribute(4, "value", item.Key);
                var content = builder2.CheckOfCodeQuoteComponent(item.Value, 5);
                builder2.AddContent(8, content);
                builder2.CloseElement();
            }

            builder2.CloseElement();
        }));
        builder.CloseComponent();

        return builder;
    }
    internal static RenderTreeBuilder AddUnorderedListComponent(this RenderTreeBuilder builder, (int, string)[] items)
    {
        builder.OpenComponent<Paragraph>(0);
        builder.AddAttribute(1, "ChildContent", (RenderFragment)(builder2 =>
        {
            builder2.AddListItem(items);
        }));
        builder.CloseComponent();

        return builder;
    }

    private static RenderTreeBuilder AddListItem(this RenderTreeBuilder builder, (int, string)[] items)
    {
        var helper = new BuilderHelper();
        builder.BuildList(items, helper);
        return builder;
    }

    private class BuilderHelper
    {
        public int CurrentIndex { get; set; }
        public int CurrentLevel { get; set; }
        public int Sequence { get; set; }
    }

    private static RenderTreeBuilder BuildList(
        this RenderTreeBuilder builder,
        (int, string)[] items,
        BuilderHelper helper)
    {
        builder.OpenElement(helper.Sequence++, "ul");

        while (helper.CurrentIndex < items.Length)
        {
            var (currentItemLevel, currentItemValue) = items[helper.CurrentIndex];

            builder.OpenElement(helper.Sequence++, "li");

            if (currentItemLevel > helper.CurrentLevel)
            {
                helper.CurrentLevel++;
                builder.BuildList(items, helper);
                builder.CloseElement();
                continue;
            }

            builder.AddContent(helper.Sequence++, currentItemValue);
            builder.CloseElement();

            helper.CurrentIndex++;
            var isNextExist = helper.CurrentIndex < items.Length;

            if (!isNextExist) continue;

            var nexItemLevel = items[helper.CurrentIndex].Item1;
            if (nexItemLevel >= currentItemLevel) continue;

            helper.CurrentLevel--;
            break;
        }

        builder.CloseElement();
        return builder;
    }

    #endregion


    #region Code
    internal static RenderTreeBuilder AddCodeComponent(this RenderTreeBuilder builder, string lang, string title, string code)
    {
        builder.OpenComponent(0, typeof(MDCodeBlock));
        builder.AddAttribute(1, "Language", lang);
        builder.AddAttribute(2, "Title", title);
        builder.AddAttribute(3, "Code", code);
        builder.CloseComponent();

        return builder;
    }

    private static string CheckOfCodeQuoteComponent(this RenderTreeBuilder builder, string content, int sequence)
    {
        var matches = MDRegexs.GetCodeQuoteRegex().Matches(content);

        foreach (Match match in matches)
        {
            var index = content.IndexOf(match.Value, StringComparison.CurrentCulture);

            builder.AddContent(sequence, content[..index]);
            builder.OpenElement(++sequence, "code");
            builder.AddContent(++sequence, match.Value[1..^1]);
            builder.CloseElement();
            content = content[(index + match.Value.Length)..];
        }

        return content;
    }
    #endregion
}
