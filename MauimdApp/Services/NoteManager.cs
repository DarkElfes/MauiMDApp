using FluentResults;
using MauimdApp.Entities;
using MauimdApp.Services.Abstractions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MauimdApp.Services;

public class NoteManager(
    IStorageProvider _noteStorageProvider
    ) : INoteManager
{
    public Folder? Root { get; private set; }
    public Note? SelectedNote { get; private set; }
    public bool IsNewNote { get; set; }

    public event Action? OnChangeSelectedNote;


    // Load the root folder
    public async Task<Result> InitializeAsync()
    {
        var result = await _noteStorageProvider.LoadRootAsync();

        if (result.IsSuccess)
        {
            Root = result.Value;
            return Result.Ok();
        }
        else
        {
            return Result.Fail(result.Errors);
        }
    }

    /// <summary>
    /// Set the selected note
    /// </summary>
    /// <param name="note"></param>
    public void SetSelectedNote(Note? note)
    {
        SelectedNote = note;
        OnChangeSelectedNote?.Invoke();
    }

    /// <summary>
    /// Rename the selected note
    /// </summary>
    /// <param name="newPath"></param>
    /// <returns></returns>
    public Result RenameSelectedNote(string newPath)
    {
        try
        {
            File.Move(SelectedNote!.FullPath, newPath);
            SelectedNote!.FullPath = newPath;
            OnChangeSelectedNote?.Invoke();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred while renaming note: {ex.Message}");
        }
    }


    /// <summary>
    /// Create new note width default name available in the selected folder
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public void CreateNewNote(Folder? folder = null)
    {
        folder ??= Root!;

        var notePath = GetAvailableNotePathInFolder(folder.FullPath);
        File.Create(notePath);

        SelectedNote = new Note(notePath, folder);
        IsNewNote = true;

        folder.Notes.Add(SelectedNote);
        OnChangeSelectedNote?.Invoke();
    }

    public void DeleteNote(Note note)
    {
        File.Delete(note.FullPath);
        if (SelectedNote == note)
        {
            SelectedNote = null;
        }

        note.ParentFolder.Notes.Remove(note);
        OnChangeSelectedNote?.Invoke();
    }


    // Create new folder
    public void CreateNewFolder()
    {
        throw new NotImplementedException();
    }


    private static string GetAvailableNotePathInFolder(string dir)
    {
        const string name = "Kosiposha";

        var filePath = Path.Combine(dir, $"{name}.md");

        if (!File.Exists(filePath))
            return filePath;

        for (var i = 0; i < int.MaxValue; i++)
        {
            filePath = Path.Combine(dir, $"{name} {i}.md");

            if (!File.Exists(filePath))
                return filePath;
        }

        // Joke code
        throw new Exception("Cannot create new note");
    }
}