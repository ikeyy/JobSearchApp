using FluentAssertions;
using FluentValidation.TestHelper;
using JobSearchApp.Application.Command.UpdateJobOffer;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using Moq;
using System.Runtime.CompilerServices;
using Xunit;

namespace JobSearchApp.Tests.Commands.UpdateJobOffer
{
    public class UpdateJobOfferCommandValidatorTests
    {
        private readonly Mock<IJobOfferRepository> _mockJobOfferRepository;
        private readonly UpdateJobOfferCommandValidator _validator;

        public UpdateJobOfferCommandValidatorTests()
        {
            _mockJobOfferRepository = new Mock<IJobOfferRepository>();
            _validator = new UpdateJobOfferCommandValidator(_mockJobOfferRepository.Object);
        }

        private UpdateJobOfferCommand CreateValidCommand()
        {
            var jobOfferData = new JobOfferData
            {
                JobOfferId = Guid.NewGuid(),
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 5000m,
                Status = "Accepted"
            };

            SetupRepositoryMocks();
            return new UpdateJobOfferCommand(jobOfferData);
        }

        private void SetupRepositoryMocks()
        {
            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new JobOffer
                {
                    Id = Guid.NewGuid(),
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = 3000m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });
        }

        #region JobOfferId Tests

        [Fact]
        public async Task Validate_WithValidJobOfferId_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.JobOfferId);
        }

        [Fact]
        public async Task Validate_WithEmptyJobOfferId_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.JobOfferId = Guid.Empty;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobOfferId)
                .WithErrorMessage("JobOfferId is required");
        }

        [Fact]
        public async Task Validate_WithNonExistentJobOfferId_ShouldFailAsync()
        {
            // Arrange
            var command = CreateValidCommand();
            var nonExistentJobOfferId = Guid.NewGuid();
            command.JobOfferData.JobOfferId = nonExistentJobOfferId;

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(nonExistentJobOfferId, It.IsAny<CancellationToken>(),false))
                .ReturnsAsync((JobOffer)null);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobOfferId)
                .WithErrorMessage("JobOfferId does not exist");
        }

        [Fact]
        public async Task Validate_WithExistingJobOfferId_ShouldPassAsync()
        {
            // Arrange
            var command = CreateValidCommand();
            var jobOfferId = Guid.NewGuid();
            command.JobOfferData.JobOfferId = jobOfferId;

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new JobOffer
                {
                    Id = jobOfferId,
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = 5000m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.JobOfferId);
        }

        [Fact]
        public async Task Validate_WithRepositoryReturningNull_ShouldFailAsync()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new UpdateJobOfferCommand(new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 5000m,
                Status = "Accepted"
            });

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync((JobOffer)null);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobOfferId)
                .WithErrorMessage("JobOfferId does not exist");
        }

        #endregion

        #region Repository Interaction Tests

        [Fact]
        public async Task Validate_ShouldCallRepositoryWithCorrectJobOfferIdAsync()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new UpdateJobOfferCommand(new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 5000m,
                Status = "Accepted"
            });

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new JobOffer
                {
                    Id = jobOfferId,
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = 5000m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            await _validator.TestValidateAsync(command);

            // Assert
            _mockJobOfferRepository.Verify(
                x => x.GetJobOfferByIdAsync(
                    It.Is<Guid>(id => id == jobOfferId),
                    It.IsAny<CancellationToken>(), false),
                Times.Once);
        }

        [Fact]
        public async Task Validate_ShouldPassCancellationTokenToRepositoryAsync()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var command = CreateValidCommand();

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(It.IsAny<Guid>(), cancellationToken,false))
                .ReturnsAsync(new JobOffer
                {
                    Id = Guid.NewGuid(),
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = 5000m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            await _validator.TestValidateAsync(command);

            // Assert
            _mockJobOfferRepository.Verify(
                x => x.GetJobOfferByIdAsync(
                    It.IsAny<Guid>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken),
                    false),
                Times.Once);
        }

        [Fact]
        public async Task Validate_RepositoryCalledOncePerValidationAsync()
        {
            // Arrange
            var command = CreateValidCommand();

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new JobOffer
                {
                    Id = Guid.NewGuid(),
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = 5000m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            await _validator.TestValidateAsync(command);

            // Assert
            _mockJobOfferRepository.Verify(
                x => x.GetJobOfferByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false),
                Times.Once);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Validate_WithAllValidData_ShouldPassAsync()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new UpdateJobOfferCommand(new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 5000m,
                Status = "Accepted"
            });

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new JobOffer
                {
                    Id = jobOfferId,
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = 5000m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("Accepted")]
        [InlineData("Rejected")]
        public async Task Validate_WithDifferentStatuses_ShouldPassAsync(string status)
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new UpdateJobOfferCommand(new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 5000m,
                Status = status
            });

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new JobOffer
                {
                    Id = jobOfferId,
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = 5000m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task Validate_WithDifferentPrices_ShouldPassAsync(decimal price)
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var command = new UpdateJobOfferCommand(new JobOfferData
            {
                JobOfferId = jobOfferId,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = price,
                Status = "Accepted"
            });

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new JobOffer
                {
                    Id = jobOfferId,
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = price,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithMultipleUpdates_ShouldValidateDifferentlyAsync()
        {
            // Arrange
            var jobOfferId1 = Guid.NewGuid();
            var jobOfferId2 = Guid.NewGuid();

            var command1 = new UpdateJobOfferCommand(new JobOfferData
            {
                JobOfferId = jobOfferId1,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 5000m,
                Status = "Accepted"
            });

            var command2 = new UpdateJobOfferCommand(new JobOfferData
            {
                JobOfferId = jobOfferId2,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 7000m,
                Status = "Rejected"
            });

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId1, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new JobOffer
                {
                    Id = jobOfferId1,
                    JobId = Guid.NewGuid(),
                    ContractorId = Guid.NewGuid(),
                    Price = 5000m,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(jobOfferId2, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync((JobOffer)null);

            // Act
            var result1 = await _validator.TestValidateAsync(command1);
            var result2 = await _validator.TestValidateAsync(command2);

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
            result2.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobOfferId)
                .WithErrorMessage("JobOfferId does not exist");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task Validate_WithRepositoryThrowingException_ShouldThrowAsync()
        {
            // Arrange
            var command = CreateValidCommand();

            _mockJobOfferRepository
                .Setup(x => x.GetJobOfferByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _validator.TestValidateAsync(command));
        }

        [Fact]
        public async Task Validate_WithMultipleInvalidCalls_ShouldFailForEmptyId()
        {
            // Arrange
            var command = new UpdateJobOfferCommand(new JobOfferData
            {
                JobOfferId = Guid.Empty,
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 5000m,
                Status = "Accepted"
            });

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobOfferId)
                .WithErrorMessage("JobOfferId is required");
        }

        #endregion
    }
}