namespace Application.Faq;

public class FaqItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
}


public interface IFaqService
{
    IReadOnlyList<FaqItem> GetFaqItems();
}

public sealed class FaqService : IFaqService
{
    public IReadOnlyList<FaqItem> GetFaqItems()
    {
        var items = new List<FaqItem>
        {
            new() 
            { 
                Id = 1 , 
                Title = "Q1. Do I need prior gym experience to Join CoreFitness?",
                Description = "No, GymPro is designed for all fitness levels. Our trainers guide beginners with proper techniques and structured workout plans to help them start safely and confidently."
            },
            new()
            {
                Id = 2 ,
                Title = "Q2. What facilities are included with the membership?",
                Description = "No, GymPro is designed for all fitness levels. Our trainers guide beginners with proper techniques and structured workout plans to help them start safely and confidently."
            }
        };

        return items;
    }
}
