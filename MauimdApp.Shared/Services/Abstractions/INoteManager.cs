using MauimdApp.Shared.Entities;

namespace MauimdApp.Shared.Services.Abstractions;

public interface INoteManager
{
    Note? SelectedNote { get; }
    bool IsNewNote { get; set; }

    event Action? OnChangeSelectedNote;

    void SetSelectedNote(Note? note, bool isNewNote = false);
}
