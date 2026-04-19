using Domain.Common.Abstractions;
using Domain.ContactReq.Entities;
using Domain.ContactReq.ValueObjects;

namespace Domain.ContactReq.Repositories;

public interface IContactRequestRepository : IRepositoryBase<ContactRequestEntity, ContactRequestId>
{

}