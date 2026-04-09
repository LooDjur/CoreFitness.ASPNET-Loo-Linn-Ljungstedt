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
        var contactResult = ContactRequestEntity.Create(
            FirstName.Create(request.FirstName).Value,
            LastName.Create(request.LastName).Value,
            Email.Create(request.Email).Value,
            PhoneNumber.Create(request.Phone).Value,
            MessageBody.Create(request.Message).Value);

        if (contactResult.IsFailure) return contactResult;

        await repository.AddAsync(contactResult.Value, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}