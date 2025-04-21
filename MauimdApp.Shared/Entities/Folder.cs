namespace MauimdApp.Shared.Entities;

public class Folder(
    string relativePath,
    Folder parentFolder,
    string? id = null
    ) : Entity, IComparable<Folder>
{
    public override string Id { get; init; } = id ?? Guid.NewGuid().ToString();
    public string RelativePath { get; } = relativePath;
    public Folder ParentFolder { get; } = parentFolder;
    public string Title => Path.GetFileName(RelativePath) ?? throw new ArgumentNullException();


    private readonly List<Folder> _children = [];
    public IReadOnlyList<Folder> Children => _children;

    private readonly List<Note> _notes = [];
    public IReadOnlyList<Note> Notes => _notes;



    public void AddChildFolder(Folder folder)
    {
        _children.Add(folder);
        _children.Sort();
    }

    public bool RemoveChildFolder(Folder folder)
        => _children.Remove(folder);

    public void AddNote(Note note)
    {
        _notes.Add(note);
        _children.Sort();
    }

    public bool RemoveNote(Note note)
        => _notes.Remove(note);


    public bool IsExistOtherNoteWithName(Note note, string newName)
        => _notes.FirstOrDefault(x => x.Title == newName) is { } existNote && existNote != note;


    public int CompareTo(Folder? other)
        => other == null
            ? 1
            : string.Compare(Title, other.Title, StringComparison.CurrentCulture);
}
