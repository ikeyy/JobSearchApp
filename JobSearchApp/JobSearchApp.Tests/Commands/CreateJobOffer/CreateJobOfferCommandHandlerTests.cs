using FluentAssertions;
using JobSearchApp.Application.Command.CreateJobOffer;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using JobSearchApp.Domain.Interfaces.Service;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Commands.CreateJobOffer
{
    public class CreateJobOfferCommandHandlerTests
    {
        private readonly Mock<IJobOfferService> _mockJobOfferService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateJobOfferCommandHandler _handler;

        public CreateJobOfferCommandHandlerTests()
        {
            _mockJobOfferService = new Mock<IJobOfferService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new CreateJobOfferCommandHandler(
                _mockJobOfferService.Object,
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidJobOfferData_ShouldReturnSuccessResponse()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();
            const decimal price = 5000m;
            const string status = "Pending";

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = price,
                Status = status
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = price,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.JobOfferId.Should().Be(jobOfferId);
            result.JobId.Should().Be(jobId);
            result.ContractorId.Should().Be(contractorId);
            result.Price.Should().Be(price);
            result.Status.Should().Be(status);
            result.CreatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WithValidJobOfferData_ShouldMapJobOfferDataCorrectly()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();
            const decimal price = 7500m;
            const string status = "Pending";

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = price,
                Status = status
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = price,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.Should().NotBeNull();
            capturedJobOffer.JobId.Should().Be(jobId);
            capturedJobOffer.ContractorId.Should().Be(contractorId);
            capturedJobOffer.Price.Should().Be(price);
            capturedJobOffer.Status.Should().Be(status);
            capturedJobOffer.Id.Should().NotBeEmpty();
            capturedJobOffer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Handle_WithValidJobOfferData_ShouldSetCreatedAtToUtcNow()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();
            var beforeHandle = DateTime.UtcNow;

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = 3000m,
                Status = "Pending"
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = 3000m,
                Status = "Pending",
                CreatedAt = capturedJobOffer?.CreatedAt ?? DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterHandle = DateTime.UtcNow;

            // Assert
            capturedJobOffer.CreatedAt.Should().BeOnOrAfter(beforeHandle);
            capturedJobOffer.CreatedAt.Should().BeOnOrBefore(afterHandle);
        }

        [Fact]
        public async Task Handle_WithValidJobOfferData_ShouldGenerateNewJobOfferId()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending"
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.Id.Should().NotBeEmpty();
            capturedJobOffer.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Handle_WithServiceException_ShouldThrowException()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending"
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
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
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending"
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mockJobOfferService.Verify(
                x => x.CreateJobOfferAsync(
                    It.IsAny<JobOffer>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);

            _mockJobOfferService.Verify(
                x => x.GetJobOfferByIdAsync(
                    jobOfferId,
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("Accepted")]
        [InlineData("Rejected")]
        public async Task Handle_WithDifferentStatuses_ShouldMapStatusCorrectly(string status)
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = status
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.Status.Should().Be(status);
        }

        [Fact]
        public async Task Handle_ShouldCallCreateJobOfferServiceOnce()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending"
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobOfferService.Verify(
                x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallGetJobOfferByIdServiceOnce()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending"
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobOfferId);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = 5000m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockJobOfferService.Verify(
                x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task Handle_WithDifferentPrices_ShouldMapPriceCorrectly(decimal price)
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var jobOfferId = Guid.NewGuid();

            var jobOfferData = new JobOfferData
            {
                JobId = jobId,
                ContractorId = contractorId,
                Price = price,
                Status = "Pending"
            };

            var command = new CreateJobOfferCommand(jobOfferData);

            JobOffer capturedJobOffer = null;
            _mockJobOfferService
                .Setup(x => x.CreateJobOfferAsync(It.IsAny<JobOffer>(), It.IsAny<CancellationToken>()))
                .Callback<JobOffer, CancellationToken>((jobOffer, ct) => capturedJobOffer = jobOffer)
                .ReturnsAsync(jobOfferId);

            var createdJobOffer = new JobOffer
            {
                Id = jobOfferId,
                JobId = jobId,
                ContractorId = contractorId,
                Price = price,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _mockJobOfferService
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdJobOffer);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedJobOffer.Price.Should().Be(price);
        }
    }
}