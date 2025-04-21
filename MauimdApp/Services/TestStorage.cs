using CSharpFunctionalExtensions;
using MauimdApp.Shared.Entities;
using System.Xml.Linq;

namespace MauimdApp.Services;
internal class TestStorage
{


    //public Result RenameNote(Note note, string newName)
    //    => GetNewPath(note, newName)
    //    .Ensure(newPath => !File.Exists(newPath), "Note with this name already exists in current directory")
    //    .Bind(newPath => Result.Try(() =>
    //    {
    //        File.Move(note.FullPath, newPath);
    //        note.FullPath = newPath;
    //        return Result.Success();
    //    }))
    //    .Tap(_ => OnChangeRoot?.Invoke());

    //public Result RenameNote(Note note, string newName)
    //{
    //    var result = GetNewPath(note, newName);
    //    if (result.IsFailure)
    //        return result;

    //    var checkResult = IsNotExistPath(result.Value);
    //    if (checkResult.IsFailure)
    //        return result;

    //    try
    //    {
    //        File.Move(note.FullPath, result.Value);
    //        note.FullPath = result.Value;
    //        OnChangeRoot?.Invoke();

    //        return Result.Success();
    //    }
    //    catch (Exception e)
    //    {
    //        return Result.Failure(e.Message);
    //    }
    //}

    //public Result IsNotExistPath(string newPath)
    //    => Result.Success(newPath)
    //        .Ensure(x => !File.Exists(x), "Note with this name already exists in current directory");

    //private static Result<string> GetNewPath(Note note, string newName)
    //    => Result.Success((note.FullPath, newName))
    //        .BindTry<(string fullPath, string newName), (string? dirName, string newName)>(
    //            x => (Path.GetDirectoryName(x.fullPath), newName))
    //        .Ensure(x => DirectoryExist(x.newName))
    //        .BindTry<(string? dirName, string newName), string>(x => Path.Combine(x.dirName!, $"{x.newName}.md"));

    //private static Result DirectoryExist(string? dirName)
    //    => Directory.Exists(dirName)
    //        ? Result.Success()
    //        : Result.Failure($"Directory {dirName} does not exist");
}
