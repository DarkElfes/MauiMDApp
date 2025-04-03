using FluentResults;
using MauimdApp.Entities;

namespace MauimdApp.Services.Abstractions;

public interface INoteManager
{
    Folder? Root { get; }
    Note? SelectedNote { get; }
    bool IsNewNote { get; set; }

    event Action? OnChangeSelectedNote;


    Task<Result> InitializeAsync();


    void SetSelectedNote(Note? note);
    Result RenameSelectedNote(string newPath);


    void CreateNewNote(Folder? folder = null);
    void DeleteNote(Note note);

    void CreateNewFolder();
}
