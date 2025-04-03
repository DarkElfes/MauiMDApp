using Microsoft.AspNetCore.Components;

namespace MauimdApp.Services.Abstractions;

public interface IMDConvertor
{
    RenderFragment Convert(string content);
}
