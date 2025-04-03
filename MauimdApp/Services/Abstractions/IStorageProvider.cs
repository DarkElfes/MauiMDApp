using FluentResults;
using MauimdApp.Entities;

namespace MauimdApp.Services.Abstractions;

public interface IStorageProvider
{
    Task<Result<Folder>> LoadRootAsync();
    Task<Result> SaveNoteAsync(Note note);


    //Result<Folder> CreateFolder(string folderName);
    //Result<Note> CreateNote(string noteName);

    //Result RemoveFolder(string folderName);
    //Result RemoveNote(string noteName);
}
