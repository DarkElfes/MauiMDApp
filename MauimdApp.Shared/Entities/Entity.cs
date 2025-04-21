namespace MauimdApp.Shared.Entities;

public class Entity
{
    public virtual string Id { get; init; } = null!;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
}
