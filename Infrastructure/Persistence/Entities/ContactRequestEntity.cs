using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Entities;

public class ContactRequestEntity
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
