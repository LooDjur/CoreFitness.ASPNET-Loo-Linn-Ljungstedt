namespace Presentation.WebApp.Models.Common;

public class FaqViewModel
{
    public string SectionTitle { get; set; } = "Frequently Asked Questions";
    public string Title { get; set; } = "Common questions about gym and training";
    public IEnumerable<FaqItem> Items { get; set; } = [];
}

public class FaqItem
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
}
