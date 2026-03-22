using Domain.Common;
using MediatR;

namespace Application.Faq;

public interface IFaqService
{
    IReadOnlyList<FaqItem> GetFaqItems();
}