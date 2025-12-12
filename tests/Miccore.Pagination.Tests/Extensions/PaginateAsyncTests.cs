using Miccore.Pagination.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Miccore.Pagination.Tests.Extensions;

public class PaginateAsyncTests : TestBase
{
    public PaginateAsyncTests()
    {
        SeedData(25);
    }

    [Fact]
    public async Task PaginateAsync_WithPaginationEnabled_ReturnsCorrectPage()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = true, Page = 2, Limit = 10 };

        // Act
        var result = await Context.TestEntities.PaginateAsync(query);

        // Assert
        result.Items.Should().HaveCount(10);
        result.CurrentPage.Should().Be(2);
        result.TotalItems.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.PageSize.Should().Be(10);
        result.Items.First().Id.Should().Be(11);
    }

    [Fact]
    public async Task PaginateAsync_WithPaginationDisabled_ReturnsAllItems()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = false, Page = 1, Limit = 10 };

        // Act
        var result = await Context.TestEntities.PaginateAsync(query);

        // Assert
        result.Items.Should().HaveCount(25);
        result.TotalItems.Should().Be(25);
    }

    [Fact]
    public async Task PaginateAsync_WithSorting_ReturnsSortedResults()
    {
        // Arrange
        var query = new PaginationQuery
        {
            Paginate = true,
            Page = 1,
            Limit = 5,
            OrderBy = "Price",
            OrderDirection = "desc"
        };

        // Act
        var result = await Context.TestEntities.PaginateAsync(query);

        // Assert
        result.Items.Should().HaveCount(5);
        result.Items.First().Id.Should().Be(25); // Highest price
        result.SortedBy.Should().Be("Price");
        result.SortDirection.Should().Be("desc");
    }

    [Fact]
    public async Task PaginateAsync_WithInvalidPage_DefaultsToPageOne()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = true, Page = -5, Limit = 10 };

        // Act
        var result = await Context.TestEntities.PaginateAsync(query);

        // Assert
        result.CurrentPage.Should().Be(1);
        result.Items.First().Id.Should().Be(1);
    }
}
