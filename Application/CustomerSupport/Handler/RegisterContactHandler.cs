using Application.CustomerSupport.Abstractions.Services;
using Application.CustomerSupport.Commands;
using Application.CustomerSupport.Inputs;
using Application.CustomerSupport.Output;
using Domain.Common;
using Domain.Common.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CustomerSupport.Handler;

public sealed class RegisterContactHandler(IContactRequestService contactService) 
    : IRequestHandler<RegisterContactCommand, Result>
{
    public async Task<Result> Handle(RegisterContactCommand request, CancellationToken ct)
    {
        var input = new ContactRequestInput(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Message
        );

        var success = await contactService.CreateContactRequestAsync(input);
        return success ? Result.Success() : Result.Failure(Error.Failure("Contact.Error", "Kunde inte spara"));
    }
}