using ErrorOr;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using WeatherApp.Application.Common.Interfaces;

namespace WeatherApp.Application.UnitTests.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<TestRequest>> _mockValidator;
    private readonly ValidationBehavior<TestRequest, string> _behavior;

    public ValidationBehaviorTests()
    {
        _mockValidator = new Mock<IValidator<TestRequest>>();
        _behavior = new ValidationBehavior<TestRequest, string>(
            new[] { _mockValidator.Object }
        );
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallNext()
    {
        // Arrange
        var request = new TestRequest();
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        bool nextCalled = false;
        RequestHandlerDelegate<ErrorOr<string>> next = (_) => { nextCalled = true; return Task.FromResult(ErrorOrFactory.From<string>("ok")); };

        // Act
        await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnErrors()
    {
        var request = new TestRequest();
        var failures = new List<ValidationFailure> { new("Prop", "Error message") };
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        ErrorOr<string> successResult = "ok";
        RequestHandlerDelegate<ErrorOr<string>> next = (_) =>
            Task.FromResult(successResult);

        var result = await _behavior.Handle(request, next, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Description.Should().Be("Error message");
    }

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallNextDirectly()
    {
        var behavior = new ValidationBehavior<TestRequest, string>(Enumerable.Empty<IValidator<TestRequest>>());
        bool nextCalled = false;
        RequestHandlerDelegate<ErrorOr<string>> next = (_) => { nextCalled = true; return Task.FromResult(ErrorOrFactory.From<string>("ok")); };

        await behavior.Handle(new TestRequest(), next, CancellationToken.None);

        nextCalled.Should().BeTrue();
    }
}