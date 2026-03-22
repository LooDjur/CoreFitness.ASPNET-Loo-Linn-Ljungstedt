using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Faq;

public record FaqItem
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}