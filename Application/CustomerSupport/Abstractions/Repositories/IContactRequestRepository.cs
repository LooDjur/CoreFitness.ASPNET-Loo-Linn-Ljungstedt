using Application.CustomerSupport.Commands;
using Application.CustomerSupport.Inputs;
using Application.CustomerSupport.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CustomerSupport.Abstractions.Repositories;

public interface IContactRequestRepository
{
    Task<bool> AddAsync(ContactRequestInput model);
    Task<IReadOnlyList<ContactRequest>> GetAllAsync();
}