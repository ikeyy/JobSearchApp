using FluentAssertions;
using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Application.Query.SearchJobOffer;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Interfaces.Repository;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Query.SearchJobOffer
{
    public class SearchJobOfferHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IJobOfferRepository> _mockJobOfferRepository;
        private readonly SearchJobOfferQueryHandler _handler;

        public SearchJobOfferHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockJobOfferRepository = new Mock<IJobOfferRepository>();
            _mockUnitOfWork.Setup(x => x.JobOffers).Returns(_mockJobOfferRepository.Object);
            _handler = new SearchJobOfferQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidParameters_ShouldReturnPagedResult()
        {
            // Arrange
            var query = new SearchJobOfferQuery("contractor", 1, 10);
            var jobOfferData = new List<JobOfferData>
            {
                new() { JobOfferId = Guid.NewGuid(), JobId = Guid.NewGuid(), ContractorId = Guid.NewGuid(), ContractorName = "John Contractor", Price = 500, Status = "Pending" },
                new() { JobOfferId = Guid.NewGuid(), JobId = Guid.NewGuid(), ContractorId = Guid.NewGuid(), ContractorName = "Jane Contractor", Price = 750, Status = "Accepted" }
            };

            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = jobOfferData,
                Total = 2,
                Page = 1,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync("contractor", 1, 10, It.IsAny<CancellationToken>()))
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
        public async Task Handle_WithValidParameters_ShouldCallSearchJobOffersAsyncOnce()
        {
            // Arrange
            var query = new SearchJobOfferQuery("test", 1, 10);
            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync("test", 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockJobOfferRepository.Verify(
                x => x.SearchJobOffersAsync("test", 1, 10, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var query = new SearchJobOfferQuery("filter", 1, 10);
            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync("filter", 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _mockJobOfferRepository.Verify(
                x => x.SearchJobOffersAsync(
                    "filter",
                    1,
                    10,
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyResults_ShouldReturnEmptyPagedResult()
        {
            // Arrange
            var query = new SearchJobOfferQuery("nonexistent", 2, 10);
            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = new(),
                Total = 0,
                Page = 2,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync("nonexistent", 2, 10, It.IsAny<CancellationToken>()))
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
            var query = new SearchJobOfferQuery("job", 2, 5);
            var jobOfferData = new List<JobOfferData>
            {
                new() { JobOfferId = Guid.NewGuid(), JobId = Guid.NewGuid(), ContractorId = Guid.NewGuid(), ContractorName = "Contractor 6", Price = 600 },
                new() { JobOfferId = Guid.NewGuid(), JobId = Guid.NewGuid(), ContractorId = Guid.NewGuid(), ContractorName = "Contractor 7", Price = 700 },
                new() { JobOfferId = Guid.NewGuid(), JobId = Guid.NewGuid(), ContractorId = Guid.NewGuid(), ContractorName = "Contractor 8", Price = 800 },
                new() { JobOfferId = Guid.NewGuid(), JobId = Guid.NewGuid(), ContractorId = Guid.NewGuid(), ContractorName = "Contractor 9", Price = 900 },
                new() { JobOfferId = Guid.NewGuid(), JobId = Guid.NewGuid(), ContractorId = Guid.NewGuid(), ContractorName = "Contractor 10", Price = 1000 }
            };

            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = jobOfferData,
                Total = 25,
                Page = 2,
                PageSize = 5
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync("job", 2, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Page.Should().Be(2);
            result.Total.Should().Be(25);
            result.Items.Should().HaveCount(5);
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("Accepted")]
        [InlineData("Rejected")]
        [InlineData("Completed")]
        public async Task Handle_WithDifferentStatuses_ShouldSearchWithCorrectFilter(string status)
        {
            // Arrange
            var query = new SearchJobOfferQuery(status, 1, 10);
            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync(
                    It.Is<string>(f => f == status),
                    1,
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockJobOfferRepository.Verify(
                x => x.SearchJobOffersAsync(
                    It.Is<string>(f => f == status),
                    1,
                    10,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(2, 10)]
        [InlineData(3, 20)]
        public async Task Handle_WithDifferentPagination_ShouldSearchWithCorrectPageParameters(int pageNumber, int pageSize)
        {
            // Arrange
            var query = new SearchJobOfferQuery("filter", pageNumber, pageSize);
            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = new(),
                Total = 50,
                Page = pageNumber,
                PageSize = pageSize
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync(
                    "filter",
                    It.Is<int>(p => p == pageNumber),
                    It.Is<int>(ps => ps == pageSize),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockJobOfferRepository.Verify(
                x => x.SearchJobOffersAsync(
                    "filter",
                    It.Is<int>(p => p == pageNumber),
                    It.Is<int>(ps => ps == pageSize),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldThrowException()
        {
            // Arrange
            var query = new SearchJobOfferQuery("filter", 1, 10);

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync("filter", 1, 10, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnExactPagedResultFromRepository()
        {
            // Arrange
            var query = new SearchJobOfferQuery("search", 1, 10);
            var jobOfferIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var jobOfferData = jobOfferIds.Select(id => new JobOfferData { JobOfferId = id }).ToList();

            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = jobOfferData,
                Total = 3,
                Page = 1,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync("search", 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
            result.Items.Select(j => j.JobOfferId).Should().ContainInOrder(jobOfferIds);
        }

        [Fact]
        public async Task Handle_WithNullFilter_ShouldHandleGracefully()
        {
            // Arrange
            var query = new SearchJobOfferQuery(null, 1, 10);
            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = new(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync(null, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockJobOfferRepository.Verify(
                x => x.SearchJobOffersAsync(null, 1, 10, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyFilter_ShouldReturnAllJobOffers()
        {
            // Arrange
            var query = new SearchJobOfferQuery(string.Empty, 1, 10);
            var jobOfferData = new List<JobOfferData>
            {
                new() { JobOfferId = Guid.NewGuid(), ContractorName = "Contractor 1" },
                new() { JobOfferId = Guid.NewGuid(), ContractorName = "Contractor 2" }
            };

            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = jobOfferData,
                Total = 2,
                Page = 1,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync(string.Empty, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(2);
            result.Total.Should().Be(2);
        }

        [Fact]
        public async Task Handle_ShouldPreserveJobOfferProperties()
        {
            // Arrange
            var jobOfferId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var contractorId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

            var query = new SearchJobOfferQuery("test", 1, 10);
            var jobOfferData = new List<JobOfferData>
            {
                new()
                {
                    JobOfferId = jobOfferId,
                    JobId = jobId,
                    ContractorId = contractorId,
                    ContractorName = "Test Contractor",
                    Price = 1500,
                    Status = "Accepted",
                    CreatedAt = createdAt
                }
            };

            var expectedResult = new PagedResult<JobOfferData>
            {
                Items = jobOfferData,
                Total = 1,
                Page = 1,
                PageSize = 10
            };

            _mockJobOfferRepository
                .Setup(x => x.SearchJobOffersAsync("test", 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var offer = result.Items.First();
            offer.JobOfferId.Should().Be(jobOfferId);
            offer.JobId.Should().Be(jobId);
            offer.ContractorId.Should().Be(contractorId);
            offer.ContractorName.Should().Be("Test Contractor");
            offer.Price.Should().Be(1500);
            offer.Status.Should().Be("Accepted");
            offer.CreatedAt.Should().Be(createdAt);
        }
    }
}