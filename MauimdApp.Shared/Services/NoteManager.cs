using MauimdApp.Shared.Entities;
using MauimdApp.Shared.Services.Abstractions;

namespace MauimdApp.Shared.Services;

public class NoteManager : INoteManager
{
    public Note? SelectedNote { get; private set; }
    public bool IsNewNote { get; set; }

    public event Action? OnChangeSelectedNote;

    /// <summary>
    /// Sets note like selected
    /// </summary>
    /// <param name="note"></param>
    /// <param name="isNewNote"></param>
    public void SetSelectedNote(Note? note, bool isNewNote = false)
    {
        SelectedNote = note;
        IsNewNote = isNewNote;
        OnChangeSelectedNote?.Invoke();
    }
}