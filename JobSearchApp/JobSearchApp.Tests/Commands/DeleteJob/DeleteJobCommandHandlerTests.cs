using FluentAssertions;
using JobSearchApp.Application.Command.DeleteJob;
using JobSearchApp.Domain.Interfaces.Service;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Commands.DeleteJob
{
    public class DeleteJobCommandHandlerTests
    {
        private readonly Mock<IJobService> _mockJobService;
        private readonly DeleteJobCommandHandler _handler;

        public DeleteJobCommandHandlerTests()
        {
            _mockJobService = new Mock<IJobService>();
            _handler = new DeleteJobCommandHandler(_mockJobService.Object);
        }

        [Fact]
        public async Task Handle_WithValidJobId_ShouldReturnSuccessResponse()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var command = new DeleteJobCommand(jobId);

            _mockJobService
                .Setup(x => x.DeleteJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().Be(jobId);
            result.Message.Should().Be("Job has been deleted");
        }

        [Fact]
        public async Task Handle_WithValidJobId_ShouldCallServiceWithCorrectJobId()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var command = new DeleteJobCommand(jobId);

            _mockJobService
                .Setup(x => x.DeleteJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobService.Verify(
                x => x.DeleteJobAsync(
                    It.Is<Guid>(id => id == jobId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();
            var command = new DeleteJobCommand(jobId);

            _mockJobService
                .Setup(x => x.DeleteJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockJobService.Verify(
                x => x.DeleteJobAsync(
                    It.IsAny<Guid>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithMultipleCalls_ShouldCallServiceMultipleTimes()
        {
            // Arrange
            var jobId1 = Guid.NewGuid();
            var jobId2 = Guid.NewGuid();

            _mockJobService
                .Setup(x => x.DeleteJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken ct) => id);

            // Act
            var command1 = new DeleteJobCommand(jobId1);
            var command2 = new DeleteJobCommand(jobId2);

            await _handler.Handle(command1, CancellationToken.None);
            await _handler.Handle(command2, CancellationToken.None);

            // Assert
            _mockJobService.Verify(
                x => x.DeleteJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_WithServiceException_ShouldThrowException()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var command = new DeleteJobCommand(jobId);

            _mockJobService
                .Setup(x => x.DeleteJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmptyJobId_ShouldPassEmptyGuidToService()
        {
            // Arrange
            var emptyJobId = Guid.Empty;
            var command = new DeleteJobCommand(emptyJobId);

            _mockJobService
                .Setup(x => x.DeleteJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyJobId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobService.Verify(
                x => x.DeleteJobAsync(
                    It.Is<Guid>(id => id == emptyJobId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            result.Data.Should().Be(emptyJobId);
        }

        [Fact]
        public async Task Handle_ShouldReturnDeletedJobIdInResponseData()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var command = new DeleteJobCommand(jobId);

            _mockJobService
                .Setup(x => x.DeleteJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Data.Should().Be(jobId);
        }
    }
}