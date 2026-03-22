using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Faq;

public sealed class FaqService : IFaqService
{
    public IReadOnlyList<FaqItem> GetFaqItems()
    {
        return
        [
            new()
            {
                Id = 1,
                Title = "Q1. Do I need prior gym experience to Join CoreFitness?",
                Description = "No, CoreFitness is designed for all fitness levels. Our trainers guide beginners with proper techniques and structured workout plans to help them start safely and confidently."
            },
            new()
            {
                Id = 2,
                Title = "Q2. What facilities are included with the membership?",
                Description = "Our membership includes access to all cardio and strength equipment, locker rooms, saunas, and a selection of group fitness classes held throughout the week."
            },
            new()
            {
                Id = 3,
                Title = "Q3. Can I freeze my membership if I'm away?",
                Description = "Yes, you can freeze your membership for up to 3 months per year for a small administrative fee. Just notify us at least 7 days in advance."
            }
        ];
    }
}