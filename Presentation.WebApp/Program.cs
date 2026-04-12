using Application;
using Infrastructure.Extensions;
using Infrastructure.Identit.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication();

builder.Services.AddSession();

builder.Services.AddControllersWithViews();
builder.Services.AddRouting(x => x.LowercaseUrls = true);

var app = builder.Build();

await app.UseDatabaseInitialization();
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<Infrastructure.Persistence.Context.ApplicationDbContext>();
//        await context.Database.EnsureCreatedAsync();

//        await IdentityInitializer.AddDefaultAdminAsync(services);
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "Ett fel uppstod vid initiering av databasen.");
//    }
//}

app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
