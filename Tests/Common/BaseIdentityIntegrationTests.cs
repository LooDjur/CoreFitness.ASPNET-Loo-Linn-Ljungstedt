using Application.Abstractions.Authentication;
using Infrastructure.Identit;
using Infrastructure.Identit.Services;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests.Common;

public abstract class BaseIdentityIntegrationTest : BaseIntegrationTest
{
    protected readonly UserManager<AppUser> UserManager;
    protected readonly RoleManager<AppRole> RoleManager;
    protected readonly IAuthService AuthService;

    protected BaseIdentityIntegrationTest() : base()
    {
        var userStore = new UserStore<AppUser, AppRole, ApplicationDbContext, string>(Context);
        var roleStore = new RoleStore<AppRole, ApplicationDbContext, string>(Context);

        UserManager = new UserManager<AppUser>(
            userStore, null!, new PasswordHasher<AppUser>(),
            [], [], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, Substitute.For<ILogger<UserManager<AppUser>>>());

        RoleManager = new RoleManager<AppRole>(
            roleStore, [], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), Substitute.For<ILogger<RoleManager<AppRole>>>());

        var signInManager = Substitute.ForPartsOf<SignInManager<AppUser>>(
            UserManager,
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(),
            Substitute.For<IOptions<IdentityOptions>>(),
            Substitute.For<ILogger<SignInManager<AppUser>>>(),
            Substitute.For<IAuthenticationSchemeProvider>(),
            Substitute.For<IUserConfirmation<AppUser>>());

        signInManager.When(x => x.RefreshSignInAsync(Arg.Any<AppUser>()))
                     .DoNotCallBase();

        signInManager.When(x => x.SignOutAsync())
                     .DoNotCallBase();

        AuthService = new IdentityAuthService(UserManager, RoleManager, signInManager);
    }
}