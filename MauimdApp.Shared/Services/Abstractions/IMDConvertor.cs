using Microsoft.AspNetCore.Components;

namespace MauimdApp.Shared.Services.Abstractions;

public interface IMDConvertor
{
    RenderFragment Convert(string content);
}
