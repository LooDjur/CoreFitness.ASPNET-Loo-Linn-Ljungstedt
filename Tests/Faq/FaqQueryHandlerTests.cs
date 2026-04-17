using Application.Faq;
using Domain.Common;
using FluentAssertions;
using NSubstitute;

namespace Tests.Faq;

public class FaqQueryHandlerTests
{
    private readonly IFaqService _faqServiceMock;
    private readonly FaqQueryHandler _handler;

    public FaqQueryHandlerTests()
    {
        _faqServiceMock = Substitute.For<IFaqService>();
        _handler = new FaqQueryHandler(_faqServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenItemsExist()
    {
        // --- Arrange ---
        var fakeItems = new List<FaqItem>
        {
            new() { Id = 1, Title = "Test", Description = "Test" }
        };
        _faqServiceMock.GetFaqItems().Returns(fakeItems);

        var query = new FaqQuery();

        // --- Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Title.Should().Be("Test");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenItemsAreEmpty()
    {
        // --- Arrange ---
        _faqServiceMock.GetFaqItems().Returns([]);

        var query = new FaqQuery();

        // --- Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FaqErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenServiceReturnsNull()
    {
        // --- Arrange ---
        _faqServiceMock.GetFaqItems().Returns((List<FaqItem>)null!);

        var query = new FaqQuery();

        // --- Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FaqErrors.NotFound);
    }
}