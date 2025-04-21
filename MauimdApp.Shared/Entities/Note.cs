namespace MauimdApp.Shared.Entities;

public class Note(
    string relativePath,
    Folder parentFolder,
    string? id = null
    ) : Entity, IComparable<Note>
{
    public override string Id { get; init; } = id ?? Guid.NewGuid().ToString();
    public string RelativePath { get; set; } = relativePath;
    public string Content { get; set; } = string.Empty;
    public Folder ParentFolder { get; } = parentFolder;
    public string Title => Path.GetFileNameWithoutExtension(RelativePath);


    public Note(string relativePath, Folder parentFolder, string content, string? id = null) 
        : this(relativePath, parentFolder, id)
        => Content = content;

    public int CompareTo(Note? other)
        => other == null
            ? 1
            : string.Compare(Title, other.Title, StringComparison.CurrentCulture);
}
