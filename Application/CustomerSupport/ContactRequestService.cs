using Application.CustomerSupport.Abstractions.Repositories;
using Application.CustomerSupport.Abstractions.Services;
using Application.CustomerSupport.Commands;
using Application.CustomerSupport.Inputs;
using Application.CustomerSupport.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CustomerSupport;

public sealed class ContactRequestService(IContactRequestRepository repo) : IContactRequestService
{
    public async Task<bool> CreateContactRequestAsync(ContactRequestInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        input.SetId(Guid.NewGuid().ToString());
        input.SetDate(DateTime.UtcNow);

        var result = await repo.AddAsync(input);
        return result;
    }

    public async Task<IReadOnlyList<ContactRequest>> GetContactRequestAsync()
        => await repo.GetAllAsync();
}