using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common;

public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }

    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}