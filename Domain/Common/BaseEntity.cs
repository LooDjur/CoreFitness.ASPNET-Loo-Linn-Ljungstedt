namespace Domain.Common;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTime Created { get; protected set; }
    public DateTime? Modified { get; protected set; }
    public bool IsDeleted { get; protected set; } = false;

    protected void Initialize(TId id, DateTime utcNow)
    {
        Id = id;
        Created = utcNow;
        Modified = utcNow;
    }

    public void UpdateModified(DateTime utcNow)
    {
        Modified = utcNow;
    }

    public virtual void UndoDelete() => IsDeleted = false;
}