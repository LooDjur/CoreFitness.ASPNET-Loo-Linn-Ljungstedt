using Application.Abstractions.Authentication;
using Domain.Common.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Tests.Common;

public abstract class BaseIntegrationTest : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly ApplicationDbContext Context;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IAuthService AuthServiceMock = Substitute.For<IAuthService>();

    protected BaseIntegrationTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();

        UnitOfWork = new UnitOfWork(Context);
    }

    public void Dispose()
    {
        _connection.Close();
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}