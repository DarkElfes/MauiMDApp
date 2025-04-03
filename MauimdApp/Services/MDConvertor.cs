using MauimdApp.Extensions;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using MauimdApp.Services.Abstractions;

namespace MauimdApp.Services;

public class MDConvertor : IMDConvertor
{
    public RenderFragment Convert(string content)
    {
        return builder =>
        {
            var lines = content.Split("\n");
            for(var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("# "))
                {
                    builder.AddDefaultComponent("h1", line[2..]);
                }
                else if (line.StartsWith("## "))
                {
                    builder.AddDefaultComponent("h2", line[3..]);
                }
                else if (line.StartsWith("### "))
                {
                    builder.AddDefaultComponent("h3", line[4..]);
                }
                else if (line.StartsWith("#### "))
                {
                    builder.AddDefaultComponent("h4", line[5..]);
                }
                else if (line.StartsWith("##### "))
                {
                    builder.AddDefaultComponent("h5", line[6..]);
                }
                else if (line.StartsWith("###### "))
                {
                    builder.AddDefaultComponent("h6", line[7..]);
                }
                else if(line.StartsWith("```"))
                {
                    var startCodeLineIndex = i;
                    line = line[3..];

                    while (!line.Contains("```") && i != lines.Length - 1)
                    {
                        try
                        {
                            line = lines[++i];
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }

                    var code = string.Join("\n", lines[startCodeLineIndex..(i + 1)]);
                    var m = MDRegexs.GetCodeRegex().Match(code);

                    builder.AddCodeComponent(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
                }
                else if (line.StartsWith("- "))
                {
                    var items = new List<string>();
                    while (line.StartsWith("- "))
                    {
                        items.Add(line[2..]);
                        line = lines[++i];
                    }
                    i--;
                    builder.AddUnorderedListComponent(items.ToArray());
                }
                else if(IsNumberedListLine(line) is { })
                {
                    var items = new Dictionary<ushort, string>();

                    while (IsNumberedListLine(line) is { } m)
                    {
                        items.Add(ushort.Parse(m.Groups[1].Value), m.Groups[2].Value);
                        line = lines[++i];
                    }
                    i--;
                    builder. AddOrderedListComponent(items);
                }
                else
                {
                    builder.AddDefaultComponent("p", line);
                }
            }
        };
    }


    private static Match? IsNumberedListLine(string line)
        => MDRegexs.GetNumberedListRegex().Match(line) is { } m && m.Success ? m : null;
}
