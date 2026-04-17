using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Faq;

public sealed class FaqQueryHandler(IFaqService faqService)
    : IRequestHandler<FaqQuery, Result<IReadOnlyList<FaqItem>>>
{
    public async Task<Result<IReadOnlyList<FaqItem>>> Handle(
        FaqQuery request,
        CancellationToken cancellationToken)
    {
        var items = faqService.GetFaqItems();

        if (items == null || !items.Any())
        {
            return Result.Failure<IReadOnlyList<FaqItem>>(FaqErrors.NotFound);
        }

        return Result.Success(items);
    }
}