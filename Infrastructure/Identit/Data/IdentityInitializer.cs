using Application.Users.Commands.Create.User;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identit.Data;

public class IdentityInitializer
{
    public static async Task AddDefaultAdminAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var command = new RegisterQuickCommand("admin@admin.com", "aA1!", "Admin");

        var result = await sender.Send(command);
    }
}
