using FluentAssertions;
using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Application.Query.SearchCustomer;
using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.Interfaces.Repository;
using Moq;
using Xunit;

namespace JobSearchApp.Tests.Query.SearchCustomer
{
    public class SearchCustomerQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly SearchCustomerQueryHandler _handler;

        public SearchCustomerQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockUnitOfWork.Setup(x => x.Customers).Returns(_mockCustomerRepository.Object);
            _handler = new SearchCustomerQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidQuery_ShouldReturnPagedResult()
        {
            // Arrange
            var filter = "John";
            var pageNumber = 1;
            var pageSize = 10;
            var query = new SearchCustomerQuery(filter, pageNumber, pageSize);

            var customers = new List<CustomerData>
            {
                new() { CustomerId = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
                new() { CustomerId = Guid.NewGuid(), FirstName = "John", LastName = "Smith" }
            };

            var expectedResult = new PagedResult<CustomerData>
            {
                Items = customers,
                Total = 2,
                Page = pageNumber,
                PageSize = pageSize
            };

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(filter, pageNumber, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Total.Should().Be(2);
            result.Page.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.Items.Should().BeEquivalentTo(customers);
        }

        [Fact]
        public async Task Handle_WithEmptyFilter_ShouldReturnAllCustomers()
        {
            // Arrange
            var filter = string.Empty;
            var pageNumber = 1;
            var pageSize = 10;
            var query = new SearchCustomerQuery(filter, pageNumber, pageSize);

            var customers = new List<CustomerData>
            {
                new() { CustomerId = Guid.NewGuid(), FirstName = "Alice", LastName = "Johnson" },
                new() { CustomerId = Guid.NewGuid(), FirstName = "Bob", LastName = "Williams" },
                new() { CustomerId = Guid.NewGuid(), FirstName = "Charlie", LastName = "Brown" }
            };

            var expectedResult = new PagedResult<CustomerData>
            {
                Items = customers,
                Total = 3,
                Page = pageNumber,
                PageSize = pageSize
            };

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(filter, pageNumber, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.Total.Should().Be(3);
        }

        [Fact]
        public async Task Handle_WithNoResults_ShouldReturnEmptyPagedResult()
        {
            // Arrange
            var filter = "NonExistent";
            var pageNumber = 1;
            var pageSize = 10;
            var query = new SearchCustomerQuery(filter, pageNumber, pageSize);

            var expectedResult = new PagedResult<CustomerData>
            {
                Items = new List<CustomerData>(),
                Total = 0,
                Page = pageNumber,
                PageSize = pageSize
            };

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(filter, pageNumber, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.Total.Should().Be(0);
            result.Page.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public async Task Handle_ShouldCallSearchCustomersAsyncWithCorrectParameters()
        {
            // Arrange
            var filter = "Test";
            var pageNumber = 2;
            var pageSize = 20;
            var query = new SearchCustomerQuery(filter, pageNumber, pageSize);
            var cancellationToken = new CancellationToken();

            var pagedResult = new PagedResult<CustomerData>
            {
                Items = new List<CustomerData>(),
                Total = 0,
                Page = pageNumber,
                PageSize = pageSize
            };

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(filter, pageNumber, pageSize, cancellationToken))
                .ReturnsAsync(pagedResult);

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _mockCustomerRepository.Verify(
                x => x.SearchCustomersAsync(filter, pageNumber, pageSize, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var query = new SearchCustomerQuery("filter", 1, 10);

            var pagedResult = new PagedResult<CustomerData>
            {
                Items = new List<CustomerData>(),
                Total = 0,
                Page = 1,
                PageSize = 10
            };

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _mockCustomerRepository.Verify(
                x => x.SearchCustomersAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.Is<CancellationToken>(ct => ct == cancellationToken)),
                Times.Once);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 20)]
        [InlineData(5, 50)]
        public async Task Handle_WithDifferentPaginationParameters_ShouldReturnCorrectPagedResult(
            int pageNumber,
            int pageSize)
        {
            // Arrange
            var filter = "test";
            var query = new SearchCustomerQuery(filter, pageNumber, pageSize);

            var customers = new List<CustomerData>
            {
                new() { CustomerId = Guid.NewGuid(), FirstName = "Test", LastName = "User" }
            };

            var expectedResult = new PagedResult<CustomerData>
            {
                Items = customers,
                Total = 1,
                Page = pageNumber,
                PageSize = pageSize
            };

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(filter, pageNumber, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Page.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public async Task Handle_WithMultipleResults_ShouldReturnAllItems()
        {
            // Arrange
            var filter = "Smith";
            var pageNumber = 1;
            var pageSize = 100;
            var query = new SearchCustomerQuery(filter, pageNumber, pageSize);

            var customers = Enumerable.Range(1, 15)
                .Select(i => new CustomerData
                {
                    CustomerId = Guid.NewGuid(),
                    FirstName = $"John{i}",
                    LastName = "Smith"
                })
                .ToList();

            var expectedResult = new PagedResult<CustomerData>
            {
                Items = customers,
                Total = 15,
                Page = pageNumber,
                PageSize = pageSize
            };

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(filter, pageNumber, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(15);
            result.Total.Should().Be(15);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldThrowException()
        {
            // Arrange
            var query = new SearchCustomerQuery("filter", 1, 10);

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldPreserveCustomerDataAccuracy()
        {
            // Arrange
            var filter = "Accurate";
            var pageNumber = 1;
            var pageSize = 10;
            var query = new SearchCustomerQuery(filter, pageNumber, pageSize);

            var customerId = Guid.NewGuid();
            var customers = new List<CustomerData>
            {
                new()
                {
                    CustomerId = customerId,
                    FirstName = "John",
                    LastName = "Accurate"
                }
            };

            var expectedResult = new PagedResult<CustomerData>
            {
                Items = customers,
                Total = 1,
                Page = pageNumber,
                PageSize = pageSize
            };

            _mockCustomerRepository
                .Setup(x => x.SearchCustomersAsync(filter, pageNumber, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.First().CustomerId.Should().Be(customerId);
            result.Items.First().FirstName.Should().Be("John");
            result.Items.First().LastName.Should().Be("Accurate");
        }
    }
}