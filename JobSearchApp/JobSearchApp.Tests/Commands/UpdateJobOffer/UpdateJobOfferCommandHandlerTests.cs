using FluentAssertions;
using JobSearchApp.Application.Command.UpdateJobOffer;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Service;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Commands.UpdateJobOffer
{
    public class UpdateJobOfferCommandHandlerTests
    {
        private readonly Mock<IJobOfferService> _mockJobOfferService;
        private readonly UpdateJobOfferCommandHandler _handler;

        public UpdateJobOfferCommandHandlerTests()
        {
            _mockJobOfferService = new Mock<IJobOfferService>();
            _handler = new UpdateJobOfferCommandHandler(_mockJobOfferService.Object);
        }

        [Fact]
        public async Task Handle_WithValidJobOfferData_ShouldReturnSuccessResponse()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            const decimal price = 5000m;
            const string status = "Accepted";

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = price,
                Status = status
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().Be(jobOfferId);
            result.Message.Should().Be("Job Offer has been updated");
        }

        [Fact]
        public async Task Handle_WithValidJobOfferData_ShouldUpdateJobOfferProperties()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            const decimal price = 7500m;
            const string status = "Accepted";

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = price,
                Status = status
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.Should().NotBeNull();
            capturedJobOffer.JobId.Should().Be(jobId);
            capturedJobOffer.Price.Should().Be(price);
            capturedJobOffer.Status.Should().Be(status);
        }

        [Fact]
        public async Task Handle_WithValidJobOfferData_ShouldSetUpdatedAtToUtcNow()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var beforeHandle = DateTime.UtcNow;

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterHandle = DateTime.UtcNow;

            // Assert
            capturedJobOffer.UpdatedAt.Should().NotBeNull();
            capturedJobOffer.UpdatedAt.Should().BeOnOrAfter(beforeHandle);
            capturedJobOffer.UpdatedAt.Should().BeOnOrBefore(afterHandle);
        }

        [Fact]
        public async Task Handle_WithNonExistentJobOfferId_ShouldReturnFailureResponse()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobOffer)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("JobOffer not found");
            _mockJobOfferService.Verify(
                x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithValidJobOfferId_ShouldCallGetJobOfferByIdServiceWithCorrectJobOfferId()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobOfferService.Verify(
                x => x.GetJobOfferByIdAsync(
                    It.Is<Guid>(id => id == jobOfferId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidJobOfferId_ShouldCallUpdateJobOfferServiceOnce()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobOfferService.Verify(
                x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToServices()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockJobOfferService.Verify(
                x => x.GetJobOfferByIdAsync(
                    It.IsAny<Guid>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);

            _mockJobOfferService.Verify(
                x => x.UpdateJobOfferAsync(
                    It.IsAny<JobOffer>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithServiceException_ShouldThrowException()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("Accepted")]
        [InlineData("Rejected")]
        public async Task Handle_WithDifferentStatuses_ShouldUpdateStatusCorrectly(string status)
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = status
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.Status.Should().Be(status);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task Handle_WithDifferentPrices_ShouldUpdatePriceCorrectly(decimal price)
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = price,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.Price.Should().Be(price);
        }

        [Fact]
        public async Task Handle_WithNullJobOfferFromService_ShouldReturnFailureResponse()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobOffer)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("JobOffer not found");
            result.Data.Should().Be(Guid.Empty);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdatedJobOfferIdInResponseData()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Data.Should().Be(jobOfferId);
        }

        [Fact]
        public async Task Handle_ShouldPreserveContractorIdWhenUpdating()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = contractorId,
                Price = 3000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.ContractorId.Should().Be(contractorId);
        }

        [Fact]
        public async Task Handle_ShouldPreserveCreatedAtWhenUpdating()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow.AddDays(-5);

            var existingJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 3000m,
                Status = "Pending",
                CreatedAt = createdAt
            };

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = jobId,
                Price = 5000m,
                Status = "Accepted"
            };

            var command = new UpdateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJobOffer);

            _mockJobOfferService
                .Setup(x => x.UpdateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.CreatedAt.Should().Be(createdAt);
        }
    }
}