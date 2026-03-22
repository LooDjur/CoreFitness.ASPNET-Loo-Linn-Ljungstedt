using Application.CustomerSupport.Inputs;
using Application.CustomerSupport.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CustomerSupport.Abstractions.Services;

public interface IContactRequestService
{
    Task<bool> CreateContactRequestAsync(ContactRequestInput input);
    Task<IReadOnlyList<ContactRequest>> GetContactRequestAsync();
}