using Application.CustomerSupport.Commands;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.ContactReq.Entities;
using Domain.ContactReq.Repositories;
using Domain.ContactReq.ValueObjects;
using MediatR;

namespace Application.CustomerSupport.Handler;

public sealed class RegisterContactCommandHandler(
    IContactRequestRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterContactCommand, Result>
{
    public async Task<Result> Handle(RegisterContactCommand request, CancellationToken ct)
    {
        var firstNameResult = FirstName.Create(request.FirstName);
        var lastNameResult = LastName.Create(request.LastName);
        var emailResult = Email.Create(request.Email);
        var phoneResult = PhoneNumber.Create(request.Phone);
        var messageResult = MessageBody.Create(request.Message);

        var validationResult = Result.FirstFailureOrSuccess(
            firstNameResult,
            lastNameResult,
            emailResult,
            phoneResult,
            messageResult);

        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        var contactResult = ContactRequestEntity.Create(
            firstNameResult.Value,
            lastNameResult.Value,
            emailResult.Value,
            phoneResult.Value,
            messageResult.Value);

        if (contactResult.IsFailure)
            return contactResult;

        await repository.AddAsync(contactResult.Value, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}