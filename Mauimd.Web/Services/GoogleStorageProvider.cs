using FluentResults;
using MauimdApp.Shared.Entities;
using MauimdApp.Shared.Services.Abstractions;

namespace Mauimd.Web.Services;

public class GoogleStorageProvider(
    GoogleAuthService authService
    ) : IStorageProvider
{
    public Folder? RootFolder { get; }

    public async Task<Result<Folder>> LoadRootAsync()
    {
        if (authService.IsAuthorized)
        {
            throw new NotImplementedException("LoadRootAsync is not implemented yet.");
        }
        else
        {
            return await authService.AuthorizeAsync();
        }
    }

    public Task<Result<Note>> CreateNoteAsync(Folder? folder = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteNoteAsync(Note note)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveNoteAsync(Note note)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Folder>> CreateFolderAsync(Folder? folder = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteFolderAsync(Folder folder)
    {
        throw new NotImplementedException();
    }
}
