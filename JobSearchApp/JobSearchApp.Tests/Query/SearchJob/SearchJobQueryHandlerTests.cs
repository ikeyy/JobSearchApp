using FluentAssertions;
using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Application.Query.SearchJob;
using JobSearchApp.Domain.DTO.Job;
using JobSearchApp.Domain.Interfaces.Repository;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Query.SearchJob
{
    public class SearchJobQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IJobRepository> _mockJobRepository;
        private readonly SearchJobQueryHandler _handler;

        public SearchJobQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockJobRepository = new Mock<IJobRepository>();
            _mockUnitOfWork.Setup(x => x.Jobs).Returns(_mockJobRepository.Object);
            _handler = new SearchJobQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidParameters_ShouldReturnPagedResult()
        {
            // Arrange
            var parameters = new JobSearchParams
            {
                Description = "Test job",
                Status = "Open",
                MinBudget = 100,
                MaxBudget = 1000,
                PageNumber = 1,
                PageSize = 10
            };

            var query = new SearchJobQuery(parameters);
            var jobData = new List<JobData>
            {
                new() { JobId = Guid.NewGuid(), Description = "Test job 1", Budget = 500 },
                new() { JobId = Guid.NewGuid(), Description = "Test job 2", Budget = 750 }
            };

            var expectedResult = new PagedResult<JobData>
            {
                Items = jobData,
                Total = 2,
                Page = 1,
                PageSize = 10
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Total.Should().Be(2);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task Handle_WithValidParameters_ShouldCallSearchJobsAsyncOnce()
        {
            // Arrange
            var parameters = new JobSearchParams
            {
                Description = "Test",
                PageNumber = 1,
                PageSize = 10
            };

            var query = new SearchJobQuery(parameters);
            var expectedResult = new PagedResult<JobData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockJobRepository.Verify(
                x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var parameters = new JobSearchParams();
            var query = new SearchJobQuery(parameters);
            var expectedResult = new PagedResult<JobData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _mockJobRepository.Verify(
                x => x.SearchJobsAsync(
                    parameters,
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyResults_ShouldReturnEmptyPagedResult()
        {
            // Arrange
            var parameters = new JobSearchParams { PageNumber = 2, PageSize = 10 };
            var query = new SearchJobQuery(parameters);
            var expectedResult = new PagedResult<JobData>
            {
                Items = new(),
                Total = 0,
                Page = 2,
                PageSize = 10
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.Total.Should().Be(0);
            result.Page.Should().Be(2);
        }

        [Fact]
        public async Task Handle_WithMultiplePages_ShouldReturnCorrectPageData()
        {
            // Arrange
            var parameters = new JobSearchParams { PageNumber = 2, PageSize = 5 };
            var query = new SearchJobQuery(parameters);
            var jobData = new List<JobData>
            {
                new() { JobId = Guid.NewGuid(), Description = "Job 6" },
                new() { JobId = Guid.NewGuid(), Description = "Job 7" },
                new() { JobId = Guid.NewGuid(), Description = "Job 8" },
                new() { JobId = Guid.NewGuid(), Description = "Job 9" },
                new() { JobId = Guid.NewGuid(), Description = "Job 10" }
            };

            var expectedResult = new PagedResult<JobData>
            {
                Items = jobData,
                Total = 25,
                Page = 2,
                PageSize = 5
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Page.Should().Be(2);
            result.Total.Should().Be(25);
            result.Items.Should().HaveCount(5);
        }

        [Theory]
        [InlineData("Open")]
        [InlineData("Offered")]
        [InlineData("Accepted")]
        [InlineData("Completed")]
        [InlineData("Cancelled")]
        public async Task Handle_WithDifferentStatuses_ShouldSearchWithCorrectStatus(string status)
        {
            // Arrange
            var parameters = new JobSearchParams { Status = status };
            var query = new SearchJobQuery(parameters);
            var expectedResult = new PagedResult<JobData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(
                    It.Is<JobSearchParams>(p => p.Status == status),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockJobRepository.Verify(
                x => x.SearchJobsAsync(
                    It.Is<JobSearchParams>(p => p.Status == status),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(100, 500)]
        [InlineData(1000, 5000)]
        [InlineData(0, 10000)]
        public async Task Handle_WithDifferentBudgetRanges_ShouldSearchWithCorrectRange(decimal minBudget, decimal maxBudget)
        {
            // Arrange
            var parameters = new JobSearchParams { MinBudget = minBudget, MaxBudget = maxBudget };
            var query = new SearchJobQuery(parameters);
            var expectedResult = new PagedResult<JobData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(
                    It.Is<JobSearchParams>(p => p.MinBudget == minBudget && p.MaxBudget == maxBudget),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockJobRepository.Verify(
                x => x.SearchJobsAsync(
                    It.Is<JobSearchParams>(p => p.MinBudget == minBudget && p.MaxBudget == maxBudget),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldThrowException()
        {
            // Arrange
            var parameters = new JobSearchParams();
            var query = new SearchJobQuery(parameters);

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnExactPagedResultFromRepository()
        {
            // Arrange
            var parameters = new JobSearchParams();
            var query = new SearchJobQuery(parameters);
            var jobIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var jobData = jobIds.Select(id => new JobData { JobId = id }).ToList();

            var expectedResult = new PagedResult<JobData>
            {
                Items = jobData,
                Total = 3,
                Page = 1,
                PageSize = 10
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
            result.Items.Select(j => j.JobId).Should().ContainInOrder(jobIds);
        }

        [Fact]
        public async Task Handle_WithNullableParameters_ShouldHandleGracefully()
        {
            // Arrange
            var parameters = new JobSearchParams
            {
                Description = null,
                Status = null,
                MinBudget = 0,
                MaxBudget = 0
            };
            var query = new SearchJobQuery(parameters);
            var expectedResult = new PagedResult<JobData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobRepository
                .Setup(x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockJobRepository.Verify(
                x => x.SearchJobsAsync(parameters, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}