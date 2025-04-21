using MauimdApp.Shared.Extensions;
using MauimdApp.Shared.Services.Abstractions;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace MauimdApp.Shared.Services;

public class MDConvertor : IMDConvertor
{
    public RenderFragment Convert(string content)
    {
        return builder =>
        {
            builder.OpenElement(0, "div");
            builder.SetKey(Guid.NewGuid());
            var lines = content.Split("\n");
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (MDRegexs.GetTitleRegex().Match(line) is { Success: true } titleMatch)
                {
                    builder.AddTitle(titleMatch.Groups[1].Value.Length, titleMatch.Groups[2].Value);
                }
                else if (line.StartsWith("```"))
                {
                    var startCodeLineIndex = i;
                    line = line[3..];

                    while (!line.Contains("```") && i != lines.Length - 1)
                    {
                        line = lines[++i];
                    }

                    var code = string.Join("\n", lines[startCodeLineIndex..(i + 1)]);
                    var codeMatch = MatchCodeRegex(code);

                    if (codeMatch is null)
                    {
                        builder.AddParagraph(line);
                        i = startCodeLineIndex;
                    }
                    else
                    {
                        builder.AddCodeComponent(codeMatch.Groups[1].Value, codeMatch.Groups[2].Value,
                            codeMatch.Groups[3].Value);
                    }
                }
                else if (MDRegexs.GetUnorderedListRegex().Match(line) is { Success: true })
                {
                    var items = new List<(int, string)>();
                    while (MDRegexs.GetUnorderedListRegex().Match(line) is { Success: true } ulMatch)
                    {
                        items.Add(new(ulMatch.Groups[1].Value.Length, ulMatch.Groups[2].Value));

                        if (++i >= lines.Length) break;
                        line = lines[i];
                    }

                    i--;
                    builder.AddUnorderedListComponent(items.ToArray());
                }
                else if (MDRegexs.GetOrederedListRegex().Match(line) is { Success: true })
                {
                    var items = new Dictionary<ushort, string>();

                    while (MDRegexs.GetOrederedListRegex().Match(line) is { Success: true } olMatch)
                    {
                        items.Add(ushort.Parse(olMatch.Groups[1].Value), olMatch.Groups[2].Value);

                        if (++i >= lines.Length) break;
                        line = lines[i];
                    }

                    i--;
                    builder.AddOrderedListComponent(items);
                }
                else
                {
                    builder.AddParagraph(line);
                }
            }
            builder.CloseElement();
        };
    }



    private static Match? MatchCodeRegex(string line)
        => MDRegexs.GetCodeRegex().Match(line) is { Success: true } m ? m : null;
}
