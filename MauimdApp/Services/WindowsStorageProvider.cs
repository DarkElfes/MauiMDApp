using Windows.Security.Cryptography.Core;
using Windows.UI.Notifications;
using MauimdApp.Shared.Entities;
using MauimdApp.Shared.Services.Abstractions;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.VisualBasic;
using Results;

namespace MauimdApp.Services;

public class WindowsStorageProvider : IStorageProvider
{
    private readonly string SVaultPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Poison Storage\\");

    public Folder Root { get; private set; } = null!;
    public event Action? OnChangeRoot;


    #region Initialize
    /// <summary>
    /// Initializes the component asynchronously by loading the root folder from storage. It updates the root note and
    /// triggers a change event if successful.
    /// </summary>
    /// <returns>Returns a Result indicating success or failure along with any errors encountered.</returns>
    public async Task<Result> InitializeAsync()
    {
        var result = await LoadRootAsync();

        if (result.IsFailure) return result;

        Root = result.Value;
        OnChangeRoot?.Invoke();
        return Result.Success();
    }

    // Load root methods
    private async Task<Result<Folder>> LoadRootAsync()
    {
        try
        {
            Folder root = new("", null!);

            if (!Directory.Exists(SVaultPath))
            {
                Directory.CreateDirectory(SVaultPath);
            }

            await LoadFolderAsync(root);
            return root;
        }
        catch (Exception e)
        {
            return new Error(e.Message);
        }
    }
    private async Task LoadFolderAsync(Folder folder)
    {
        var childrenPaths = Directory.GetDirectories(GetFullPath(folder.RelativePath));

        foreach (var childPath in childrenPaths)
        {
            var child = new Folder(GetRelativePath(childPath), folder);
            await LoadFolderAsync(child);
            folder.AddChildFolder(child);
        }

        await LoadNotesFromFolderAsync(folder);
    }
    private async Task LoadNotesFromFolderAsync(Folder folder)
    {
        var filesPath = Directory.GetFiles(GetFullPath(folder.RelativePath), "*.md");

        await Parallel.ForEachAsync(filesPath, async (filePath, token) =>
        {
            try
            {
                var content = await File.ReadAllTextAsync(filePath, token);

                lock (folder)
                {
                    folder.AddNote(new Note(
                        relativePath: GetRelativePath(filePath),
                        parentFolder: folder,
                        content: content));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        });
    }
    #endregion


    #region Notes


    public Result RenameNote(Note note, string newName)
    {
        var fullPath = GetFullPath(note.RelativePath);

        var result = IsAvailableNoteName(fullPath, newName);

        if (result.IsFailure)
            return result;

        var newPath = result.Value;

        try
        {
            File.Move(fullPath, newPath);
            note.RelativePath = GetRelativePath(newPath);
            OnChangeRoot?.Invoke();

            return Result.Success();
        }
        catch (Exception e)
        {
            return new Error(e.Message);
        }
    }

    private static Result<string> IsAvailableNoteName(string fullPath, string newName)
    {
        var result = GetNewPath(fullPath, newName);

        if (result.IsFailure)
            return result;

        return File.Exists(result.Value) ?
            new Error($"Note with name `{newName}` already exists in current directory") :
            result;
    }
    private static Result<string> GetNewPath(string fullPath, string newName)
        => Result.Success((fullPath, newName))
            .TryCatch(x => Path.GetDirectoryName(x.fullPath))
            .Bind(DirectoryExist)
            .TryCatch(dirName => Path.Combine(dirName, $"{newName}.md"));
    private static Result<string> DirectoryExist(string? dirName)
        => Directory.Exists(dirName)
            ? Result.Success(dirName)
            : new Error($"Directory {dirName} does not exist");



    /// <summary>
    /// Create new note with default name available in the selected folder
    /// </summary>
    /// <param name="folder">Optional parameter - the folder in which the new note will be created, otherwise it will be created in the root folder by default</param>
    public Task<Result<Note>> CreateNoteAsync(Folder? folder = null)
    {
        folder ??= Root!;

        try
        {
            var notePath = GetAvailableNotePathInFolder(folder.RelativePath);
            using var _ = File.Create(notePath);

            var note = new Note(notePath, folder);
            folder.AddNote(note);

            OnChangeRoot?.Invoke();

            return Task.FromResult(Result.Success(note));

        }
        catch (Exception e)
        {
            return Task.FromResult(Result.Failure<Note>(new Error(e.Message)));
        }
    }

    /// <summary>
    /// Delete the note
    /// </summary>
    /// <param name="note"></param>
    public Task<Result> DeleteNoteAsync(Note note)
    {
        try
        {
            File.Delete(GetFullPath(note.RelativePath));
            note.ParentFolder.RemoveNote(note);

            OnChangeRoot?.Invoke();

            return Task.FromResult(Result.Success());
        }
        catch (Exception e)
        {
            return Task.FromResult(Result.Failure(new Error(e.Message)));
        }
    }

    /// <summary>
    /// Save the note content to the file. If the file does not exist, it will be created.
    /// </summary>
    /// <param name="note"></param>
    public async Task<Result> SaveNoteAsync(Note note)
    {
        var fullPath = GetFullPath(note.RelativePath);

        try
        {
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath);
            }

            await File.WriteAllTextAsync(fullPath, note.Content);

            return Result.Success();
        }
        catch (Exception e)
        {
            return new Error(e.Message);
        }
    }

    /// <summary>
    /// Generates a unique file path for a note in a specified directory. It checks for existing files and appends a
    /// number if necessary.
    /// </summary>
    /// <param name="dir">Specifies the directory where the note file will be created.</param>
    /// <returns>Returns a string representing the available file path for the note.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when a new note file cannot be created due to existing files.</exception>
    private static string GetAvailableNotePathInFolder(string dir)
    {
        const string name = "Kosiposha";

        var filePath = Path.Combine(dir, $"{name}.md");

        if (!File.Exists(filePath))
            return filePath;

        for (var i = 1; i < int.MaxValue; i++)
        {
            filePath = Path.Combine(dir, $"{name} {i}.md");

            if (!File.Exists(filePath))
                return filePath;
        }

        // Joke code
        throw new IndexOutOfRangeException("Cannot create new note in the current folder");
    }
    #endregion

    #region Folders
    /// <summary>
    /// Create new folder with default name available in the selected folder
    /// </summary>
    /// <param name="folder">Optional parameter - the folder in which the new folder will be created, otherwise it will be created in the root folder by default</param>
    public Task<Result<Folder>> CreateFolderAsync(Folder? folder = null)
    {
        folder ??= Root;

        try
        {
            var folderPath = GetAvailableFolderPathInFolder(GetFullPath(folder.RelativePath ));
            Directory.CreateDirectory(folderPath);

            var newFolder = new Folder(GetRelativePath(folderPath), folder);
            folder.AddChildFolder(newFolder);

            OnChangeRoot?.Invoke();

            return Task.FromResult(Result.Success(newFolder));
        }
        catch (Exception e)
        {
            return Task.FromResult(Result.Failure<Folder>(new Error(e.Message)));
        }
    }

    /// <summary>
    /// Delete the folder and all its contents
    /// </summary>
    /// <param name="folder">Folder that needs to be deleted</param>
    public Task<Result> DeleteFolderAsync(Folder folder)
    {
        var folderPath = GetFullPath(folder.RelativePath);

        try
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            if (directoryInfo.Attributes.HasFlag(FileAttributes.ReadOnly))
            {
                directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
            }

            Directory.Delete(folderPath, true);
            folder.ParentFolder.RemoveChildFolder(folder);

            OnChangeRoot?.Invoke();

            return Task.FromResult(Result.Success());
        }
        catch (Exception e)
        {
            return Task.FromResult(Result.Failure(new Error(e.Message)));
        }
    }

    /// <summary>
    /// Generates a unique folder path within a specified directory by appending a number to a base name.
    /// </summary>
    /// <param name="dir">Specifies the base directory where the new folder path will be created.</param>
    /// <returns>Returns a unique folder path that does not already exist in the specified directory.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when a new folder cannot be created due to existing directories.</exception>
    private static string GetAvailableFolderPathInFolder(string dir)
    {
        const string name = "Kosiposha";

        var dirPath = Path.Combine(dir, name);

        if (!Directory.Exists(dirPath))
            return dirPath;

        for (var i = 1; i < int.MaxValue; i++)
        {
            dirPath = Path.Combine(dir, $"{name} {i}");
            if (!Directory.Exists(dirPath))
                return dirPath;
        }

        // Joke code
        throw new IndexOutOfRangeException("Cannot create new folder in the current folder");
    }
    #endregion


    private string GetFullPath(string relativePath)
        => Path.Combine(SVaultPath, relativePath);

    private string GetRelativePath(string fullPath)
        => fullPath.Replace(SVaultPath, string.Empty);
}