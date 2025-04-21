using System.Data;
using MauimdApp.Shared.Entities;
using Results;

namespace MauimdApp.Shared.Services.Abstractions;

public interface IStorageProvider
{
    Folder? Root { get; }
    event Action? OnChangeRoot;


    Task<Result> InitializeAsync();

    Task<Result<Note>> CreateNoteAsync(Folder? folder = null);
    Task<Result> DeleteNoteAsync(Note note);
    Task<Result> SaveNoteAsync(Note note);
    Result RenameNote(Note note, string name);

    Task<Result<Folder>> CreateFolderAsync(Folder? folder = null);
    Task<Result> DeleteFolderAsync(Folder folder);
}
