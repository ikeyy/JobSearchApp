using FluentAssertions;
using FluentValidation.TestHelper;
using JobSearchApp.Application.Command.UpdateJob;
using JobSearchApp.Domain.DTO.Job;
using Xunit;

namespace JobSearchApp.Tests.Commands.UpdateJob
{
    public class UpdateJobCommandValidatorTests
    {
        private readonly UpdateJobCommandValidator _validator;

        public UpdateJobCommandValidatorTests()
        {
            _validator = new UpdateJobCommandValidator();
        }

        private UpdateJobCommand CreateValidCommand()
        {
            var jobData = new JobData
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                DueDate = DateTime.UtcNow.AddDays(10),
                Budget = 1000m,
                Description = "This is a valid job description with sufficient length",
                Status = "Open"
            };

            return new UpdateJobCommand(Guid.NewGuid(), jobData);
        }

        #region JobId Tests

        [Fact]
        public void Validate_WithValidJobId_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobId);
        }

        [Fact]
        public void Validate_WithEmptyJobId_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobId = Guid.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobId)
                .WithErrorMessage("Job ID is required");
        }

        #endregion

        #region JobData Tests

        [Fact]
        public void Validate_WithValidJobData_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData);
        }

        [Fact]
        public async Task Validate_WithNullJobData_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData = null;

            // Act
            var result = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData)
                .WithErrorMessage("Job data is required");
        }

        #endregion

        #region StartDate Tests

        [Fact]
        public void Validate_WithValidStartDate_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.StartDate);
        }

        [Fact]
        public void Validate_WithEmptyStartDate_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.StartDate = default;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.StartDate)
                .WithErrorMessage("Start date is required");
        }

        [Fact]
        public void Validate_WithStartDateInPast_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.StartDate = DateTime.UtcNow.AddDays(-1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.StartDate)
                .WithErrorMessage("Start date must be today or in the future");
        }

        [Fact]
        public void Validate_WithStartDateToday_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.StartDate = DateTime.UtcNow.Date;
            command.JobData.DueDate = DateTime.UtcNow.AddDays(1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.StartDate);
        }

        [Fact]
        public void Validate_WithStartDateInFuture_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.StartDate = DateTime.UtcNow.AddDays(30);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.StartDate);
        }

        #endregion

        #region DueDate Tests

        [Fact]
        public void Validate_WithValidDueDate_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.DueDate);
        }

        [Fact]
        public void Validate_WithEmptyDueDate_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.DueDate = default;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.DueDate)
                .WithErrorMessage("Due date is required");
        }

        [Fact]
        public void Validate_WithDueDateBeforeStartDate_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            var startDate = DateTime.UtcNow.AddDays(10);
            command.JobData.StartDate = startDate;
            command.JobData.DueDate = startDate.AddDays(-5);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.DueDate)
                .WithErrorMessage("Due date must be after the start date");
        }

        [Fact]
        public void Validate_WithDueDateEqualToStartDate_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            var sameDate = DateTime.UtcNow.AddDays(5);
            command.JobData.StartDate = sameDate;
            command.JobData.DueDate = sameDate;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.DueDate)
                .WithErrorMessage("Due date must be after the start date");
        }

        [Fact]
        public void Validate_WithDueDateAfterStartDate_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            var startDate = DateTime.UtcNow.AddDays(5);
            command.JobData.StartDate = startDate;
            command.JobData.DueDate = startDate.AddDays(10);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.DueDate);
        }

        #endregion

        #region Budget Tests

        [Fact]
        public void Validate_WithValidBudget_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Budget = 1000m;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.Budget);
        }

        [Fact]
        public void Validate_WithZeroBudget_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Budget = 0m;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Budget)
                .WithErrorMessage("Budget must be greater than 0");
        }

        [Fact]
        public void Validate_WithNegativeBudget_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Budget = -100m;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Budget)
                .WithErrorMessage("Budget must be greater than 0");
        }

        [Fact]
        public void Validate_WithSmallPositiveBudget_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Budget = 0.01m;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.Budget);
        }

        [Fact]
        public void Validate_WithLargePositiveBudget_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Budget = 999999999.99m;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.Budget);
        }

        #endregion

        #region Description Tests

        [Fact]
        public void Validate_WithValidDescription_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Description = "This is a valid job description with sufficient length";

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.Description);
        }

        [Fact]
        public void Validate_WithEmptyDescription_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Description = string.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Description)
                .WithErrorMessage("Description is required");
        }

        [Fact]
        public void Validate_WithNullDescription_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Description = null;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Description)
                .WithErrorMessage("Description is required");
        }

        [Fact]
        public void Validate_WithDescriptionTooShort_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Description = "Short";

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Description)
                .WithErrorMessage("Description must be between 10 and 5000 characters");
        }

        [Fact]
        public void Validate_WithDescriptionExactlyMinimumLength_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Description = "1234567890"; // Exactly 10 characters

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.Description);
        }

        [Fact]
        public void Validate_WithDescriptionTooLong_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Description = new string('a', 5001);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Description)
                .WithErrorMessage("Description must be between 10 and 5000 characters");
        }

        [Fact]
        public void Validate_WithDescriptionExactlyMaximumLength_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Description = new string('a', 5000);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.Description);
        }

        #endregion

        #region Status Tests

        [Fact]
        public void Validate_WithValidStatus_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Status = "Open";

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.Status);
        }

        [Fact]
        public void Validate_WithEmptyStatus_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Status = string.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Status)
                .WithErrorMessage("Status is required");
        }

        [Fact]
        public void Validate_WithNullStatus_ShouldFail()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Status = null;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Status)
                .WithErrorMessage("Status is required");
        }

        [Theory]
        [InlineData("Open")]
        [InlineData("Offered")]
        [InlineData("Accepted")]
        [InlineData("Completed")]
        [InlineData("Cancelled")]
        public void Validate_WithAllValidStatuses_ShouldPass(string status)
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Status = status;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.JobData.Status);
        }

        [Theory]
        [InlineData("Invalid")]
        [InlineData("Pending")]
        [InlineData("InProgress")]
        [InlineData("open")] // Case sensitive
        [InlineData("OPEN")] // Case sensitive
        public void Validate_WithInvalidStatus_ShouldFail(string status)
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobData.Status = status;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.JobData.Status)
                .WithErrorMessage("Status must be one of: Open, Offered, Accepted, Completed, Cancelled");
        }

        #endregion

        #region Integration Tests (Multiple Fields)

        [Fact]
        public void Validate_WithAllFieldsValid_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithMultipleFieldsInvalid_ShouldFailForAllInvalidFields()
        {
            // Arrange
            var command = CreateValidCommand();
            command.JobId = Guid.Empty;
            command.JobData.StartDate = DateTime.UtcNow.AddDays(-1);
            command.JobData.DueDate = DateTime.UtcNow.AddDays(-2);
            command.JobData.Budget = -100m;
            command.JobData.Description = "Short";
            command.JobData.Status = "Invalid";

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.JobId);
            result.ShouldHaveValidationErrorFor(x => x.JobData.StartDate);
            result.ShouldHaveValidationErrorFor(x => x.JobData.DueDate);
            result.ShouldHaveValidationErrorFor(x => x.JobData.Budget);
            result.ShouldHaveValidationErrorFor(x => x.JobData.Description);
            result.ShouldHaveValidationErrorFor(x => x.JobData.Status);
        }

        [Fact]
        public void Validate_WithDateRangeOfOneDay_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            var startDate = DateTime.UtcNow.AddDays(1);
            command.JobData.StartDate = startDate;
            command.JobData.DueDate = startDate.AddHours(1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithLongDateRange_ShouldPass()
        {
            // Arrange
            var command = CreateValidCommand();
            var startDate = DateTime.UtcNow.AddDays(1);
            command.JobData.StartDate = startDate;
            command.JobData.DueDate = startDate.AddYears(1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        #endregion
    }
}