using Application.CustomerSupport.Commands;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.ContactReq.Entities;
using Domain.ContactReq.Repositories;
using Domain.ContactReq.ValueObjects;
using MediatR;

namespace Application.CustomerSupport.Handler;

public sealed class RegisterContactHandler(
    IContactRequestRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterContactCommand, Result>
{
    public async Task<Result> Handle(RegisterContactCommand request, CancellationToken ct)
    {
        var firstNameResult = FirstName.Create(request.FirstName);
        var lastNameResult = LastName.Create(request.LastName);
        var emailResult = Email.Create(request.Email);
        var messageResult = MessageBody.Create(request.Message);

        if (Result.FirstFailureOrSuccess(firstNameResult, lastNameResult, emailResult, messageResult) is Result failure && failure.IsFailure)
        {
            return failure;
        }

        var contactRequestResult = ContactRequestEntity.Create(
            firstNameResult.Value,
            lastNameResult.Value,
            emailResult.Value,
            request.Phone != null ? PhoneNumber.Create(request.Phone).Value : null,
            messageResult.Value);

        if (contactRequestResult.IsFailure)
        {
            return contactRequestResult;
        }

        await repository.AddAsync(contactRequestResult.Value, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}