using Application.Abstractions.Authentication;
using Application.Users.Commands.Create.User;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using static Domain.Common.DomainErrors;

namespace Infrastructure.Identit.Data;

internal class IdentityInitializer
{
    public static async Task AddDefaultAdminAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var command = new RegisterQuickCommand("admin@domain.com", "BytMig123!", "Admin");

        var result = await sender.Send(command);

        if (result.IsFailure)
        {
        }
    }
}
