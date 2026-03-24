using FluentAssertions;
using FluentValidation.TestHelper;
using JobSearchApp.Application.Command.CreateJobOffer;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Commands.CreateJobOffer
{
    public class CreateJobOfferCommandValidatorTests
    {
        private readonly Mock<IJobRepository> _mockJobRepository;
        private readonly Mock<IContractorRepository> _mockContractorRepository;
        private readonly CreateJobOfferCommandValidator _validator;

        public CreateJobOfferCommandValidatorTests()
        {
            _mockJobRepository = new Mock<IJobRepository>();
            _mockContractorRepository = new Mock<IContractorRepository>();
            _validator = new CreateJobOfferCommandValidator(
                _mockJobRepository.Object,
                _mockContractorRepository.Object
            );
        }

        private CreateJobOfferCommand CreateValidCommand()
        {
            var jobOfferData = new JobOfferData
            {
                JobId = Guid.NewGuid(),
                ContractorId = Guid.NewGuid(),
                Price = 1000m,
                Status = "Pending"
            };

            SetupRepositoryMocks();
            return new CreateJobOfferCommand(jobOfferData);
        }

        private void SetupRepositoryMocks()
        {
            _mockJobRepository
                .Setup(x => x.GetJobByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(),false))
                .ReturnsAsync(new Job{ Id = Guid.NewGuid() });

            _mockContractorRepository
                .Setup(x => x.GetContractorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(),false))
                .ReturnsAsync(new Contractor{ Id = Guid.NewGuid() });
        }

        #region JobId Tests

        [Fact]
        public async Task Validate_WithValidJobId_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.JobId);
        }

        [Fact]
        public async Task Validate_WithEmptyJobId_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.JobId = Guid.Empty;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobId)
                .WithErrorMessage("JobId by must be a valid GUID");
        }

        [Fact]
        public async Task Validate_WithNonExistentJobId_ShouldFailAsync()
        {
            // Arrange
            var command = CreateValidCommand();
            var nonExistentJobId = Guid.NewGuid();
            command.JobOfferData.JobId = nonExistentJobId;

            _mockJobRepository
                .Setup(x => x.GetJobByIdAsync(nonExistentJobId, It.IsAny<CancellationToken>(),false))
                .ReturnsAsync((Job)null);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobId)
                .WithErrorMessage("Job does not exist");
        }

        [Fact]
        public async Task Validate_WithExistingJobId_ShouldPassAsync()
        {
            // Arrange
            var command = CreateValidCommand();
            var jobId = Guid.NewGuid();
            command.JobOfferData.JobId = jobId;

            _mockJobRepository
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new Job{ Id = jobId });

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.JobId);
        }

        #endregion

        #region ContractorId Tests

        [Fact]
        public async Task Validate_WithValidContractorId_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.ContractorId);
        }

        [Fact]
        public async Task Validate_WithEmptyContractorId_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.ContractorId = Guid.Empty;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.ContractorId)
                .WithErrorMessage("ContractorId by must be a valid GUID");
        }

        [Fact]
        public async Task Validate_WithNonExistentContractorId_ShouldFailAsync()
        {
            // Arrange
            var command = CreateValidCommand();
            var nonExistentContractorId = Guid.NewGuid();
            command.JobOfferData.ContractorId = nonExistentContractorId;

            _mockContractorRepository
                .Setup(x => x.GetContractorByIdAsync(nonExistentContractorId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync((Contractor)null);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.ContractorId)
                .WithErrorMessage("Contractor does not exist");
        }

        [Fact]
        public async Task Validate_WithExistingContractorId_ShouldPassAsync()
        {
            // Arrange
            var command = CreateValidCommand();
            var contractorId = Guid.NewGuid();
            command.JobOfferData.ContractorId = contractorId;

            _mockContractorRepository
                .Setup(x => x.GetContractorByIdAsync(contractorId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new Contractor{ Id = contractorId });

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.ContractorId);
        }

        #endregion

        #region Price Tests

        [Fact]
        public async Task Validate_WithValidPrice_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.Price = 1000m;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.Price);
        }

        [Fact]
        public async Task Validate_WithZeroPrice_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.Price = 0m;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.Price)
                .WithErrorMessage("Price must be greater than 0");
        }

        [Fact]
        public async Task Validate_WithNegativePrice_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.Price = -100m;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.Price)
                .WithErrorMessage("Price must be greater than 0");
        }

        [Fact]
        public async Task Validate_WithSmallPositivePrice_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.Price = 0.01m;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.Price);
        }

        [Fact]
        public async Task Validate_WithLargePrice_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.Price = 999999999.99m;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.Price);
        }

        [Fact]
        public async Task Validate_WithMaxDecimalPrice_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.Price = decimal.MaxValue;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobOfferData.Price);
        }

        #endregion

        #region Integration Tests (Multiple Fields)

        [Fact]
        public async Task Validate_WithAllFieldsValid_ShouldPassAsync()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithAllFieldsInvalid_ShouldFailForAllInvalidFieldsAsync()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobOfferData.JobId = Guid.Empty;
            command.JobOfferData.ContractorId = Guid.Empty;
            command.JobOfferData.Price = -100m;

            _mockJobRepository
                .Setup(x => x.GetJobByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), false))
                .ReturnsAsync((Job)null);

            _mockContractorRepository
                .Setup(x => x.GetContractorByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(),false))
                .ReturnsAsync((Contractor)null);

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobId);
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.ContractorId);
            result.ShouldHaveValidationErrorFor(x => x.JobOfferData.Price);
        }

        [Fact]
        public async Task Validate_RepositoriesCalledWithCorrectIds_ShouldCallRepositoriesAsync()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var command = CreateValidCommand();
            command.JobOfferData.JobId = jobId;
            command.JobOfferData.ContractorId = contractorId;

            _mockJobRepository
                .Setup(x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new Job{ Id = jobId });

            _mockContractorRepository
                .Setup(x => x.GetContractorByIdAsync(contractorId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new Contractor{ Id = contractorId });

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            _mockJobRepository.Verify(
                x => x.GetJobByIdAsync(jobId, It.IsAny<CancellationToken>(),false),
                Times.Once);

            _mockContractorRepository.Verify(
                x => x.GetContractorByIdAsync(contractorId, It.IsAny<CancellationToken>(), false),
                Times.Once);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithMultipleOffers_ShouldValidateDifferentlyAsync()
        {
            // Arrange
            var jobId1 = Guid.NewGuid();
            var jobId2 = Guid.NewGuid();
            var contractorId = Guid.NewGuid();

            var command1 = new CreateJobOfferCommand(new JobOfferData
            {
                JobId = jobId1,
                ContractorId = contractorId,
                Price = 1000m
            });

            var command2 = new CreateJobOfferCommand(new JobOfferData
            {
                JobId = jobId2,
                ContractorId = contractorId,
                Price = 2000m
            });

            _mockJobRepository
                .Setup(x => x.GetJobByIdAsync(jobId1, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new Job{ Id = jobId1 });

            _mockJobRepository
                .Setup(x => x.GetJobByIdAsync(jobId2, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync((Job)null);

            _mockContractorRepository
                .Setup(x => x.GetContractorByIdAsync(contractorId, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(new Contractor{ Id = contractorId });

            // Act
            var result1 = await _validator.TestValidateAsync(command1);
            var result2 = await _validator.TestValidateAsync(command2);

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
            result2.ShouldHaveValidationErrorFor(x => x.JobOfferData.JobId);
        }

        #endregion
    }
}