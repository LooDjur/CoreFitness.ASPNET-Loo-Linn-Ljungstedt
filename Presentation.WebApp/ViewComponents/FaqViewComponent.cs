using Application.Faq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.Common;

namespace Presentation.WebApp.ViewComponents;

public class FaqViewComponent(ISender sender) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await sender.Send(new FaqQuery());

        if (result.IsFailure)
        {
            return View(new FaqViewModel { Items = [] });
        }

        var viewModel = new FaqViewModel
        {
            Items = [.. result.Value.Select(x => new Presentation.WebApp.Models.Common.FaqItem
            {
                Id = x.Id.ToString(),
                Title = x.Title,
                Description = x.Description
            })]
        };

        return View(viewModel);
    }
}