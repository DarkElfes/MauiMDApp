namespace MauimdApp.Entities;

public class Note(
    string fullPath,
    Folder parentFolder
    ) : IComparable<Note>
{
    public string FullPath { get; set; } = fullPath;
    public string Content { get; set; } = string.Empty;
    public Folder ParentFolder { get; set; } = parentFolder;

    public string Name => Path.GetFileNameWithoutExtension(FullPath);


    public Note(string fullPath, Folder parentFolder, string content) : this(fullPath, parentFolder)
        => Content = content;


    public int CompareTo(Note? other)
        => other == null
            ? 1
            : string.Compare(Name, other.Name, StringComparison.CurrentCulture);
}
