using FluentAssertions;
using JobSearchApp.Application.Command.CreateJob;
using JobSearchApp.Domain.DTO.Job;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Service;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Commands.CreateJob
{
    public class CreateJobCommandHandlerTests
    {
        private readonly Mock<IJobService> _mockJobService;
        private readonly CreateJobCommandHandler _handler;

        public CreateJobCommandHandlerTests()
        {
            _mockJobService = new Mock<IJobService>();
            _handler = new CreateJobCommandHandler(_mockJobService.Object);
        }

        [Fact]
        public async Task Handle_WithValidJobData_ShouldReturnSuccessResponse()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var dueDate = startDate.AddDays(10);

            var jobData = new JobData
            {
                CustomerId = customerId,
                StartDate = startDate,
                DueDate = dueDate,
                Budget = 1000m,
                Description = "Test job description",
                Status = "Open"
            };

            var command = new CreateJobCommand(jobData);

            _mockJobService
                .Setup(x => x.CreateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().Be(jobId);
            result.Message.Should().Be("Job has been added");
            _mockJobService.Verify(
                x => x.CreateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidJobData_ShouldMapJobDataCorrectly()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var dueDate = startDate.AddDays(10);
            const decimal budget = 5000m;
            const string description = "Test job description";
            const string status = "Open";

            var jobData = new JobData
            {
                CustomerId = customerId,
                StartDate = startDate,
                DueDate = dueDate,
                Budget = budget,
                Description = description,
                Status = status
            };

            var command = new CreateJobCommand(jobData);

            Job capturedJob = null;
            _mockJobService
                .Setup(x => x.CreateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .Callback<Job, CancellationToken>((job, ct) => capturedJob = job)
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJob.Should().NotBeNull();
            capturedJob.StartDate.Should().Be(startDate);
            capturedJob.DueDate.Should().Be(dueDate);
            capturedJob.Budget.Should().Be(budget);
            capturedJob.Description.Should().Be(description);
            capturedJob.Status.Should().Be(status);
            capturedJob.CreatedBy.Should().Be(customerId);
            capturedJob.Id.Should().NotBeEmpty();
            capturedJob.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Handle_WithValidJobData_ShouldSetCreatedAtToUtcNow()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var beforeHandle = DateTime.UtcNow;

            var jobData = new JobData
            {
                CustomerId = customerId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(11),
                Budget = 1000m,
                Description = "Test job description",
                Status = "Open"
            };

            var command = new CreateJobCommand(jobData);

            Job capturedJob = null;
            _mockJobService
                .Setup(x => x.CreateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .Callback<Job, CancellationToken>((job, ct) => capturedJob = job)
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterHandle = DateTime.UtcNow;

            // Assert
            capturedJob.CreatedAt.Should().BeOnOrAfter(beforeHandle);
            capturedJob.CreatedAt.Should().BeOnOrBefore(afterHandle);
        }

        [Fact]
        public async Task Handle_WithValidJobData_ShouldGenerateNewJobId()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var jobData = new JobData
            {
                CustomerId = customerId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(11),
                Budget = 1000m,
                Description = "Test job description",
                Status = "Open"
            };

            var command = new CreateJobCommand(jobData);

            Job capturedJob = null;
            _mockJobService
                .Setup(x => x.CreateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .Callback<Job, CancellationToken>((job, ct) => capturedJob = job)
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJob.Id.Should().NotBeEmpty();
            capturedJob.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Handle_WithServiceException_ShouldThrowException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var jobData = new JobData
            {
                CustomerId = customerId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(11),
                Budget = 1000m,
                Description = "Test job description",
                Status = "Open"
            };

            var command = new CreateJobCommand(jobData);

            _mockJobService
                .Setup(x => x.CreateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToService()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var customerId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var jobData = new JobData
            {
                CustomerId = customerId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(11),
                Budget = 1000m,
                Description = "Test job description",
                Status = "Open"
            };

            var command = new CreateJobCommand(jobData);

            _mockJobService
                .Setup(x => x.CreateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockJobService.Verify(
                x => x.CreateJobAsync(
                    It.IsAny<Job>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Theory]
        [InlineData("Open")]
        [InlineData("Offered")]
        [InlineData("Accepted")]
        [InlineData("Completed")]
        [InlineData("Cancelled")]
        public async Task Handle_WithDifferentStatuses_ShouldMapStatusCorrectly(string status)
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var jobData = new JobData
            {
                CustomerId = customerId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(11),
                Budget = 1000m,
                Description = "Test job description",
                Status = status
            };

            var command = new CreateJobCommand(jobData);

            Job capturedJob = null;
            _mockJobService
                .Setup(x => x.CreateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .Callback<Job, CancellationToken>((job, ct) => capturedJob = job)
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJob.Status.Should().Be(status);
        }
    }
}
