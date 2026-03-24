using FluentAssertions;
using JobSearchApp.Application.Command.DeleteJobOffer;
using JobSearchApp.Domain.Interfaces.Service;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Commands.DeleteJobOffer
{
    public class DeleteJobOfferCommandHandlerTests
    {
        private readonly Mock<IJobOfferService> _mockJobOfferService;
        private readonly DeleteJobOfferCommandHandler _handler;

        public DeleteJobOfferCommandHandlerTests()
        {
            _mockJobOfferService = new Mock<IJobOfferService>();
            _handler = new DeleteJobOfferCommandHandler(_mockJobOfferService.Object);
        }

        [Fact]
        public async Task Handle_WithValidJobOfferId_ShouldReturnSuccessResponse()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new DeleteJobOfferCommand(jobOfferId);

            _mockJobOfferService
                .Setup(x => x.DeleteJobOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().Be(jobOfferId);
            result.Message.Should().Be("Job Offer has been deleted");
        }

        [Fact]
        public async Task Handle_WithValidJobOfferId_ShouldCallServiceWithCorrectJobOfferId()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new DeleteJobOfferCommand(jobOfferId);

            _mockJobOfferService
                .Setup(x => x.DeleteJobOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobOfferService.Verify(
                x => x.DeleteJobOfferAsync(
                    It.Is<Guid>(id => id == jobOfferId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();
            var command = new DeleteJobOfferCommand(jobOfferId);

            _mockJobOfferService
                .Setup(x => x.DeleteJobOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockJobOfferService.Verify(
                x => x.DeleteJobOfferAsync(
                    It.IsAny<Guid>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithMultipleCalls_ShouldCallServiceMultipleTimes()
        {
            // Arrange
            var jobOfferId1 = Guid.NewGuid();
            var jobOfferId2 = Guid.NewGuid();

            _mockJobOfferService
                .Setup(x => x.DeleteJobOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken ct) => id);

            // Act
            var command1 = new DeleteJobOfferCommand(jobOfferId1);
            var command2 = new DeleteJobOfferCommand(jobOfferId2);

            await _handler.Handle(command1, CancellationToken.None);
            await _handler.Handle(command2, CancellationToken.None);

            // Assert
            _mockJobOfferService.Verify(
                x => x.DeleteJobOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_WithServiceException_ShouldThrowException()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new DeleteJobOfferCommand(jobOfferId);

            _mockJobOfferService
                .Setup(x => x.DeleteJobOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmptyJobOfferId_ShouldPassEmptyGuidToService()
        {
            // Arrange
            var emptyJobOfferId = Guid.Empty;
            var command = new DeleteJobOfferCommand(emptyJobOfferId);

            _mockJobOfferService
                .Setup(x => x.DeleteJobOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyJobOfferId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobOfferService.Verify(
                x => x.DeleteJobOfferAsync(
                    It.Is<Guid>(id => id == emptyJobOfferId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            result.Data.Should().Be(emptyJobOfferId);
        }

        [Fact]
        public async Task Handle_ShouldReturnDeletedJobOfferIdInResponseData()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new DeleteJobOfferCommand(jobOfferId);

            _mockJobOfferService
                .Setup(x => x.DeleteJobOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Data.Should().Be(jobOfferId);
        }
    }
}