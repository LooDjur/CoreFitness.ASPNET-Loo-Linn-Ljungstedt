//using Domain.Common.Abstractions;
//using Domain.Common.ValueObjects.Shared;
//using Domain.Sessions;
//using FluentAssertions;
//using Infrastructure.Persistence;
//using Infrastructure.Persistence.Context;
//using Infrastructure.Repositories;
//using Microsoft.Data.Sqlite;
//using Microsoft.EntityFrameworkCore;

//namespace Tests.Sessions;

//public class SessionInfrastructureRepoUT : IDisposable
//{
//    private readonly ApplicationDbContext _context;
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly SqliteConnection _connection;

//    public SessionInfrastructureRepoUT()
//    {
//        _connection = new SqliteConnection("Filename=:memory:");
//        _connection.Open();

//        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//            .UseSqlite(_connection)
//            .Options;

//        _context = new ApplicationDbContext(options);
//        _context.Database.EnsureCreated();

//        _unitOfWork = new UnitOfWork(_context);
//    }

//    [Fact]
//    public async Task AddAsync_ShouldPersistSession_WhenUsingUnitOfWork()
//    {
//        // Arrange
//        var session = CreateValidSession("Yoga Basic");

//        // Act
//        await _unitOfWork.Sessions.AddAsync(session, CancellationToken.None);
//        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

//        // Assert
//        var dbSession = await _context.Sessions.FindAsync(session.Id);
//        dbSession.Should().NotBeNull();
//        dbSession!.Title.Value.Should().Be("Yoga Basic");
//    }

//    [Fact]
//    public async Task GetByIdAsync_WithValidStringGuid_ShouldReturnSession()
//    {
//        // Arrange
//        var session = CreateValidSession("Yoga for Beginners");
//        await _unitOfWork.Sessions.AddAsync(session, CancellationToken.None);
//        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

//        // Act
//        var result = await _unitOfWork.Sessions.GetByIdAsync(session.Id, CancellationToken.None);

//        // Assert
//        result.Should().NotBeNull();
//        result!.Id.Should().Be(session.Id);
//    }

//    [Fact]
//    public async Task GetByIdAsync_WithInvalidString_ShouldReturnNull()
//    {
//        var randomId = SessionId.New();

//        // Act
//        var result = await _unitOfWork.Sessions.GetByIdAsync(randomId, CancellationToken.None);

//        // Assert
//        result.Should().BeNull();
//    }

//    [Fact]
//    public async Task GetUpcomingSessionsAsync_ShouldFilterCorrectly()
//    {
//        // Arrange
//        var pastSession = CreateSessionAt(DateTime.UtcNow.AddDays(-1));
//        var futureSession = CreateSessionAt(DateTime.UtcNow.AddDays(1));
//        var deletedSession = CreateValidSession("Deleted Session");
//        deletedSession.Delete(); // Soft delete

//        await _context.Sessions.AddRangeAsync(pastSession, futureSession, deletedSession);
//        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

//        // Act
//        var result = await _unitOfWork.Sessions.GetUpcomingSessionsAsync(CancellationToken.None);

//        // Assert
//        result.Should().ContainSingle()
//            .Which.Id.Should().Be(futureSession.Id);
//    }

//    [Fact]
//    public async Task UpdateDetails_ShouldPersistAllChanges()
//    {
//        // Arrange
//        var session = CreateValidSession("Original Title");
//        await _unitOfWork.Sessions.AddAsync(session, CancellationToken.None);
//        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

//        var newTitle = Title.Create("New Title").Value;
//        var newInstructor = Instructor.Create("Sven-Erik").Value;
//        var newDesc = Description.Create("New description text here").Value;
//        var newCap = Capacity.Create(35).Value;
//        var newTime = TimeSlot.Create(DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(5).AddHours(1)).Value;

//        // Act
//        session.UpdateDetails(newTitle, newDesc, newInstructor, SessionCategory.Spinning, newTime, newCap);
//        _unitOfWork.Sessions.Update(session);
//        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

//        // Assert
//        // Vi skapar en ny context eller rensar tracker för att vara säkra på att vi läser från DB
//        _context.ChangeTracker.Clear();
//        var dbSession = await _context.Sessions.FindAsync(session.Id);

//        dbSession!.Title.Value.Should().Be("New Title");
//        dbSession.Category.Should().Be(SessionCategory.Spinning);
//        dbSession.MaxCapacity.Value.Should().Be(35);
//        dbSession.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
//    }

//    [Fact]
//    public async Task Delete_ShouldRemoveHardFromDatabase()
//    {
//        // Arrange
//        var session = CreateValidSession("TitleYoga");
//        await _unitOfWork.Sessions.AddAsync(session, CancellationToken.None);
//        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

//        // Act
//        _unitOfWork.Sessions.Delete(session);
//        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

//        // Assert
//        var dbSession = await _context.Sessions.FindAsync(session.Id);
//        dbSession.Should().BeNull();
//    }

//    // Helpers
//    private static SessionEntity CreateValidSession(string title)
//    {
//        var titleRes = Title.Create(title);
//        var instructorRes = Instructor.Create("Anders");
//        var descriptionRes = Description.Create("This is a very long and descriptive text to ensure it passes validation");
//        var capacityRes = Capacity.Create(10);
//        var timeRes = TimeSlot.Create(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1));

//        // Kontrollera om NÅGOT misslyckades innan vi anropar .Value
//        if (titleRes.IsFailure) throw new Exception($"Title Error: {titleRes.Error.Code}");
//        if (instructorRes.IsFailure) throw new Exception($"Instructor Error: {instructorRes.Error.Code}");
//        if (descriptionRes.IsFailure) throw new Exception($"Description Error: {descriptionRes.Error.Code}");
//        if (capacityRes.IsFailure) throw new Exception($"Capacity Error: {capacityRes.Error.Code}");
//        if (timeRes.IsFailure) throw new Exception($"TimeSlot Error: {timeRes.Error.Code}");

//        // Om alla Value Objects är OK, skapa entiteten
//        var sessionResult = SessionEntity.Create(
//            titleRes.Value,
//            descriptionRes.Value,
//            instructorRes.Value,
//            SessionCategory.Yoga,
//            timeRes.Value,
//            capacityRes.Value);
            

//        if (sessionResult.IsFailure)
//            throw new Exception($"SessionEntity Error: {sessionResult.Error.Code}");

//        return sessionResult.Value;
//    }
//    private static SessionEntity CreateSessionAt(DateTime startTime)
//    {
//        // Använd helpern ovan för att få ett giltigt grundobjekt
//        var session = CreateValidSession("Timed Session");

//        // Reflection för att tvinga in dåtid (eftersom domänen annars stoppar det)
//        var scheduleField = typeof(SessionEntity).GetProperty(nameof(SessionEntity.Schedule));

//        // Skapa TimeSlot via privat konstruktor
//        var pastTimeSlot = (TimeSlot)Activator.CreateInstance(typeof(TimeSlot), true)!;

//        typeof(TimeSlot).GetProperty(nameof(TimeSlot.StartTime))?.SetValue(pastTimeSlot, startTime);
//        typeof(TimeSlot).GetProperty(nameof(TimeSlot.EndTime))?.SetValue(pastTimeSlot, startTime.AddHours(1));

//        scheduleField?.SetValue(session, pastTimeSlot);
//        return session;
//    }

//    public void Dispose()
//    {
//        _connection.Dispose();
//        _context.Dispose();
//    }
//}
