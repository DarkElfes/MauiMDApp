namespace MauimdApp.Entities;

public class Folder(
    string name,
    string fullPath
    ) : IComparable<Folder>
{
    public string Name { get; set; } = name;
    public string FullPath { get; set; } = fullPath;
    public List<Folder> Subfolders { get; set; } = [];
    public List<Note> Notes { get; set; } = [];


    public int CompareTo(Folder? other)
        => other == null
            ? 1
            : string.Compare(Name, other.Name, StringComparison.CurrentCulture);
}
