using FluentAssertions;
using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Application.Query.SearchContractor;
using JobSearchApp.Domain.DTO.Contractor;
using JobSearchApp.Domain.Interfaces.Repository;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Query.SearchContractor
{
    public class SearchContractorQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IContractorRepository> _mockContractorRepository;
        private readonly SearchContractorQueryHandler _handler;

        public SearchContractorQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockContractorRepository = new Mock<IContractorRepository>();
            _mockUnitOfWork.Setup(u => u.Contractors).Returns(_mockContractorRepository.Object);
            _handler = new SearchContractorQueryHandler(_mockUnitOfWork.Object);
        }

        private SearchContractorQuery CreateValidQuery(string filter = "", int pageNumber = 1, int pageSize = 10)
        {
            return new SearchContractorQuery(filter, pageNumber, pageSize);
        }

        private PagedResult<ContractorData> CreateMockPagedResult(int count = 5)
        {
            var contractors = Enumerable.Range(1, count)
                .Select(i => new ContractorData
                {
                    ContractorId = Guid.NewGuid(),
                    BusinessName = $"Contractor {i}",
                    Rating = i % 5 + 1
                })
                .ToList();

            return new PagedResult<ContractorData>
            {
                Items = contractors,
                Total = contractors.Count,
                Page = 1,
                PageSize = 10
            };
        }

        #region Basic Handler Tests

        [Fact]
        public async Task Handle_WithValidQuery_ShouldReturnPagedResults()
        {
            // Arrange
            var query = CreateValidQuery("test", 1, 10);
            var mockResult = CreateMockPagedResult(5);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(5);
            result.Total.Should().Be(5);
            _mockContractorRepository.Verify(
                r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyFilter_ShouldReturnAllContractors()
        {
            // Arrange
            var query = CreateValidQuery("", 1, 10);
            var mockResult = CreateMockPagedResult(3);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync("", query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
        }

        [Fact]
        public async Task Handle_WithNoResults_ShouldReturnEmptyPagedResult()
        {
            // Arrange
            var query = CreateValidQuery("nonexistent", 1, 10);
            var emptyResult = new PagedResult<ContractorData>
            {
                Items = new List<ContractorData>(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.Total.Should().Be(0);
        }

        #endregion

        #region Pagination Tests

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 10)]
        [InlineData(1, 5)]
        [InlineData(1, 20)]
        public async Task Handle_WithVariousPaginationParameters_ShouldPassCorrectValuesToRepository(int pageNumber, int pageSize)
        {
            // Arrange
            var query = CreateValidQuery("filter", pageNumber, pageSize);
            var mockResult = CreateMockPagedResult(1);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, pageNumber, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockContractorRepository.Verify(
                r => r.SearchContractorAsync(query.Filter, pageNumber, pageSize, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithFirstPage_ShouldReturnFirstPageResults()
        {
            // Arrange
            var query = CreateValidQuery("", 1, 10);
            var mockResult = CreateMockPagedResult(10);
            mockResult.Page = 1;
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Page.Should().Be(1);
            result.Items.Should().HaveCount(10);
        }

        [Fact]
        public async Task Handle_WithMultiplePages_ShouldReturnCorrectPageNumber()
        {
            // Arrange
            var query = CreateValidQuery("", 3, 10);
            var mockResult = new PagedResult<ContractorData>
            {
                Items = CreateMockPagedResult(10).Items,
                Total = 30,
                Page = 3,
                PageSize = 10
            };
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, 3, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Page.Should().Be(3);
            result.Total.Should().Be(30);
        }

        #endregion

        #region Filter Tests

        [Theory]
        [InlineData("John")]
        [InlineData("Smith")]
        [InlineData("john smith")]
        [InlineData("IT Specialist")]
        public async Task Handle_WithDifferentFilters_ShouldPassFilterToRepository(string filter)
        {
            // Arrange
            var query = CreateValidQuery(filter, 1, 10);
            var mockResult = CreateMockPagedResult(1);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(filter, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockContractorRepository.Verify(
                r => r.SearchContractorAsync(filter, 1, 10, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithSpecialCharactersInFilter_ShouldHandleCorrectly()
        {
            // Arrange
            var specialFilter = "Contractor & Co. @2024";
            var query = CreateValidQuery(specialFilter, 1, 10);
            var mockResult = CreateMockPagedResult(1);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(specialFilter, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockContractorRepository.Verify(
                r => r.SearchContractorAsync(specialFilter, 1, 10, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region CancellationToken Tests

        [Fact]
        public async Task Handle_WithValidCancellationToken_ShouldPassToRepository()
        {
            // Arrange
            var query = CreateValidQuery("", 1, 10);
            var cancellationToken = new CancellationToken();
            var mockResult = CreateMockPagedResult(1);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, cancellationToken))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            _mockContractorRepository.Verify(
                r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithCancelledCancellationToken_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var query = CreateValidQuery("", 1, 10);
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, cts.Token))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _handler.Handle(query, cts.Token));
        }

        #endregion

        #region Result Validation Tests

        [Fact]
        public async Task Handle_ShouldReturnContractorDataWithAllProperties()
        {
            // Arrange
            var contractorId = Guid.NewGuid();
            var query = CreateValidQuery("", 1, 10);
            var mockResult = new PagedResult<ContractorData>
            {
                Items = new List<ContractorData>
                {
                    new ContractorData
                    {
                        ContractorId = contractorId,
                        BusinessName = "Test Contractor",
                        Rating = 4
                    }
                },
                Total = 1,
                Page = 1,
                PageSize = 10
            };
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(1);
            var contractor = result.Items.First();
            contractor.ContractorId.Should().Be(contractorId);
            contractor.BusinessName.Should().Be("Test Contractor");
            contractor.Rating.Should().Be(4);
        }

        [Fact]
        public async Task Handle_WithMultipleResults_ShouldReturnAllItems()
        {
            // Arrange
            var query = CreateValidQuery("", 1, 10);
            var mockResult = CreateMockPagedResult(5);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(5);
            result.Items.Should().AllSatisfy(c => c.ContractorId.Should().NotBeEmpty());
        }

        #endregion

        #region Repository Interaction Tests

        [Fact]
        public async Task Handle_ShouldCallRepositoryExactlyOnce()
        {
            // Arrange
            var query = CreateValidQuery("", 1, 10);
            var mockResult = CreateMockPagedResult(1);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockContractorRepository.Verify(
                r => r.SearchContractorAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUseCorrectUnitOfWorkContractorsRepository()
        {
            // Arrange
            var query = CreateValidQuery("", 1, 10);
            var mockResult = CreateMockPagedResult(1);
            _mockContractorRepository
                .Setup(r => r.SearchContractorAsync(query.Filter, query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockUnitOfWork.Verify(u => u.Contractors, Times.AtLeastOnce);
        }

        #endregion
    }
}