using Google.Apis.Drive.v3;
using Google.Apis.Services;
using MauimdApp.Shared.Entities;
using MauimdApp.Shared.Services.Abstractions;
using Microsoft.AspNetCore.Components;
using Results;
using System.Diagnostics;

namespace MauimdApp.Web.Services;

public class GoogleStorageProvider(
    GoogleAuthService _googleAuth,
    NavigationManager _navigation
    ) : IStorageProvider
{
    public Folder? Root { get; private set; }
    public event Action? OnChangeRoot;

    private DriveService? _driveService;
 

    public async Task<Result> InitializeAsync()
    {
        if (await _googleAuth.IsAuthorizedAsync())
        {
            _driveService = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = _googleAuth.GetUserCredential(),
                ApplicationName = "MauimdApp",
                GZipEnabled = false,
            });

            Root = await LoadRootAsync();
            OnChangeRoot?.Invoke();
            return Result.Success();
        }

        _navigation.NavigateTo("auth");
        return Result.Failure(new("Not Authorized"));
    }

    private async Task<Folder> LoadRootAsync()
    {
        Root = new("\\", null!);

        var request = _driveService.Files.List();
        request.Fields = "nextPageToken, files(id, name, mimeType, parents)";
        var fileList = await request.ExecuteAsync();

        foreach (var file in fileList.Files)
        {
            if (file.MimeType == "application/vnd.google-apps.folder")
            {
                var folder = new Folder(file.Name, Root);
                Root.AddChildFolder(folder);
            }
            else
            {
                var note = new Note(file.Name, Root);
                Root.AddNote(note);
            }
        }

        return Root;
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

    public Result RenameNote(Note note, string name)
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
