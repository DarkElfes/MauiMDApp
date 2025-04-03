using FluentResults;
using MauimdApp.Entities;
using MauimdApp.Services.Abstractions;

namespace MauimdApp.Services;

public class WindowsStorageProvider : IStorageProvider
{
    private static readonly string SDocumentsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Poison Storage");


    // Methods of creating and removing notes
    public Result<Note> CreateNote(string noteName)
    {
        throw new NotImplementedException();
    }
    public Result RemoveNote(string notePath)
    {
        try
        {
            if (File.Exists(notePath))
            {
                File.Delete(notePath);
            }

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail($"An error occurred while removing note: {e.Message}");
        }
    }

    // Save note method
    public async Task<Result> SaveNoteAsync(Note note)
    {
        try
        {
            if (!File.Exists(note.FullPath))
            {
                File.Create(note.FullPath);
            }

            await File.WriteAllTextAsync(note.FullPath, note.Content);

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }



    // Methods of creating and removing folders
    public Result<Folder> CreateFolder(string folderName)
    {
        throw new NotImplementedException();
        /*
        try
        {
            if (Directory.Exists(Path.Combine(SDocumentsPath, folderName)))
            {
                return Result.Fail("Folder already exists");
            }

            Directory.CreateDirectory(Path.Combine(SDocumentsPath, folderName));

            return Result.Ok(new Folder(folderName));
        }
        catch (Exception e)
        {
            return Result.Fail($"An error occurred while creating folder: {e.Message}");
        }*/
    }
    public Result RemoveFolder(string folderName)
    {
        throw new NotImplementedException();
    }



    // Load root methods
    public async Task<Result<Folder>> LoadRootAsync()
    {
        try
        {
            Folder root = new("Root", SDocumentsPath);

            if (!Directory.Exists(SDocumentsPath))
            {
                Directory.CreateDirectory(SDocumentsPath);
            }

            await LoadFolderAsync(root);
            return root;
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
    private static async Task LoadFolderAsync(Folder folder)
    {
        var subfolderPaths = Directory.GetDirectories(folder.FullPath);

        foreach(var subfolderPath in subfolderPaths)
        {
            var subfolder = new Folder(Path.GetFileName(subfolderPath), subfolderPath);
            await LoadFolderAsync(subfolder);
            folder.Subfolders.Add(subfolder);
        }
        folder.Subfolders.Sort();

        await LoadNotesFromFolderAsync(folder);
    }
    private static async Task LoadNotesFromFolderAsync(Folder folder)
    {
        var filesPath = Directory.GetFiles(folder.FullPath, "*.md");

        await Parallel.ForEachAsync(filesPath, async (filePath, token) =>
        {
            try
            {
                var content = await File.ReadAllTextAsync(filePath, token);

                lock (folder)
                {
                    folder.Notes.Add(new Note(
                        fullPath: filePath,
                        parentFolder: folder,
                        content: content));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        });

        folder.Notes.Sort();
    }



    // Helper methods
    private static string GetFilePath(string fileName)
        => Path.Combine(SDocumentsPath, fileName + ".md");
}