using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.ContactReq.Entities;
using Domain.ContactReq.ValueObjects;
using Domain.Sessions.Entities;

namespace Domain.ContactReq.Repositories;

public interface IContactRequestRepository : IRepositoryBase<ContactRequestEntity, ContactRequestId>
{

}