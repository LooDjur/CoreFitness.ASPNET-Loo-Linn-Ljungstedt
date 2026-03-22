using Application.CustomerSupport.Abstractions.Repositories;
using Application.CustomerSupport.Inputs;
using Application.CustomerSupport.Output;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class ContactRequestRepository(ApplicationDbContext context) : IContactRequestRepository
{
    public async Task<bool> AddAsync(ContactRequestInput model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = new ContactRequestEntity
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Message = model.Message,
            CreatedAt = model.CreatedAt
        };

        await context.ContactRequests.AddAsync(entity);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<IReadOnlyList<ContactRequest>> GetAllAsync()
    {
        var entities = await context.ContactRequests
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return [.. entities.Select(x => new ContactRequest(
            x.Id,
            x.FirstName,
            x.LastName,
            x.Email,
            x.Phone,
            x.Message,
            x.CreatedAt
        ))];
    }
}