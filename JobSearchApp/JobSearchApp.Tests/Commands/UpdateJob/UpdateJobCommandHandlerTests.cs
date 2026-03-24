using FluentAssertions;
using JobSearchApp.Application.Command.UpdateJob;
using JobSearchApp.Domain.DTO.Job;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Service;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Commands.UpdateJob
{
    public class UpdateJobCommandHandlerTests
    {
        private readonly Mock<IJobService> _mockJobService;
        private readonly UpdateJobCommandHandler _handler;

        public UpdateJobCommandHandlerTests()
        {
            _mockJobService = new Mock<IJobService>();
            _handler = new UpdateJobCommandHandler(_mockJobService.Object);
        }

        [Fact]
        public async Task Handle_WithValidJobIdAndJobData_ShouldReturnSuccessResponse()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var dueDate = startDate.AddDays(10);
            const decimal budget = 5000m;
            const string description = "Updated job description";

            var existingJob = new Job
            {
                Id = jobId,
                StartDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                Budget = 1000m,
                Description = "Original description",
                Status = "Open",
                CreatedBy = customerId,
                CreatedAt = DateTime.UtcNow
            };

            var jobData = new JobData
            {
                StartDate = startDate,
                DueDate = dueDate,
                Budget = budget,
                Description = description
            };

            var command = new UpdateJobCommand(jobId, jobData);

            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            _mockJobService
                .Setup(x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().Be(jobId);
            result.Message.Should().Be("Job has been updated");
        }

        [Fact]
        public async Task Handle_WithValidJobIdAndJobData_ShouldUpdateJobProperties()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var dueDate = startDate.AddDays(10);
            const decimal budget = 7500m;
            const string description = "Updated job description";

            var existingJob = new Job
            {
                Id = jobId,
                StartDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                Budget = 1000m,
                Description = "Original description",
                Status = "Open",
                CreatedBy = customerId,
                CreatedAt = DateTime.UtcNow
            };

            var jobData = new JobData
            {
                StartDate = startDate,
                DueDate = dueDate,
                Budget = budget,
                Description = description
            };

            var command = new UpdateJobCommand(jobId, jobData);

            Job capturedJob = null;
            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            _mockJobService
                .Setup(x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
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
        }

        [Fact]
        public async Task Handle_WithValidJobIdAndJobData_ShouldSetUpdatedAtToUtcNow()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var beforeHandle = DateTime.UtcNow;

            var existingJob = new Job
            {
                Id = jobId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(5),
                Budget = 1000m,
                Description = "Original description",
                Status = "Open",
                CreatedBy = customerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 5000m,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            Job capturedJob = null;
            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            _mockJobService
                .Setup(x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .Callback<Job, CancellationToken>((job, ct) => capturedJob = job)
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterHandle = DateTime.UtcNow;

            // Assert
            capturedJob.UpdatedAt.Should().NotBeNull();
            capturedJob.UpdatedAt.Should().BeOnOrAfter(beforeHandle);
            capturedJob.UpdatedAt.Should().BeOnOrBefore(afterHandle);
        }

        [Fact]
        public async Task Handle_WithNonExistentJobId_ShouldReturnFailureResponse()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 5000m,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Job)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Job not found");
            _mockJobService.Verify(
                x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithValidJobId_ShouldCallGetJobByIdServiceWithCorrectJobId()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var existingJob = new Job
            {
                Id = jobId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(5),
                Budget = 1000m,
                Description = "Original description",
                Status = "Open",
                CreatedBy = customerId,
                CreatedAt = DateTime.UtcNow
            };

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 5000m,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            _mockJobService
                .Setup(x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobService.Verify(
                x => x.GetJobByIdAsync(
                    It.Is<Guid>(id => id == jobId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidJobId_ShouldCallUpdateJobServiceOnce()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var existingJob = new Job
            {
                Id = jobId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(5),
                Budget = 1000m,
                Description = "Original description",
                Status = "Open",
                CreatedBy = customerId,
                CreatedAt = DateTime.UtcNow
            };

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 5000m,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            _mockJobService
                .Setup(x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobService.Verify(
                x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToServices()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var jobId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var existingJob = new Job
            {
                Id = jobId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(5),
                Budget = 1000m,
                Description = "Original description",
                Status = "Open",
                CreatedBy = customerId,
                CreatedAt = DateTime.UtcNow
            };

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 5000m,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            _mockJobService
                .Setup(x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockJobService.Verify(
                x => x.GetJobByIdAsync(
                    It.IsAny<Guid>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);

            _mockJobService.Verify(
                x => x.UpdateJobAsync(
                    It.IsAny<Job>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithServiceException_ShouldThrowException()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 5000m,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            _mockJobService
                .Setup(x => x.GetJobByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task Handle_WithDifferentBudgets_ShouldUpdateBudgetCorrectly(decimal budget)
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var existingJob = new Job
            {
                Id = jobId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(5),
                Budget = 1000m,
                Description = "Original description",
                Status = "Open",
                CreatedBy = customerId,
                CreatedAt = DateTime.UtcNow
            };

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = budget,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            Job capturedJob = null;
            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            _mockJobService
                .Setup(x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .Callback<Job, CancellationToken>((job, ct) => capturedJob = job)
                .ReturnsAsync(jobId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJob.Budget.Should().Be(budget);
        }

        [Fact]
        public async Task Handle_WithNullJobFromService_ShouldReturnFailureResponse()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 5000m,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Job)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Job not found");
            result.Data.Should().Be(Guid.Empty);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdatedJobIdInResponseData()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var existingJob = new Job
            {
                Id = jobId,
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(5),
                Budget = 1000m,
                Description = "Original description",
                Status = "Open",
                CreatedBy = customerId,
                CreatedAt = DateTime.UtcNow
            };

            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 5000m,
                Description = "Updated description"
            };

            var command = new UpdateJobCommand(jobId, jobData);

            _mockJobService
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            _mockJobService
                .Setup(x => x.UpdateJobAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Data.Should().Be(jobId);
        }
    }
}