namespace Domain.Common;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }

    public bool IsDeleted { get; protected set; } = false;

    public virtual void UndoDelete() => IsDeleted = false;
}