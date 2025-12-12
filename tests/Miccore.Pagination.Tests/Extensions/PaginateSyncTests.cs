using Miccore.Pagination.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Miccore.Pagination.Tests.Extensions;

public class PaginateSyncTests : TestBase
{
    public PaginateSyncTests()
    {
        SeedData(25);
    }

    [Fact]
    public void Paginate_Synchronous_ReturnsCorrectResults()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = true, Page = 1, Limit = 5 };

        // Act
        var result = Context.TestEntities.Paginate(query);

        // Assert
        result.Items.Should().HaveCount(5);
        result.TotalItems.Should().Be(25);
        result.TotalPages.Should().Be(5);
    }

    [Fact]
    public void Paginate_WithPaginationDisabled_ReturnsAllItems()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = false, Page = 1, Limit = 10 };

        // Act
        var result = Context.TestEntities.Paginate(query);

        // Assert
        result.Items.Should().HaveCount(25);
        result.TotalItems.Should().Be(25);
    }

    [Fact]
    public void Paginate_WithSorting_ReturnsSortedResults()
    {
        // Arrange
        var query = new PaginationQuery
        {
            Paginate = true,
            Page = 1,
            Limit = 5,
            OrderBy = "Name",
            OrderDirection = "asc"
        };

        // Act
        var result = Context.TestEntities.Paginate(query);

        // Assert
        result.Items.Should().HaveCount(5);
        result.Items.First().Name.Should().Be("Item 001");
        result.SortedBy.Should().Be("Name");
        result.SortDirection.Should().Be("asc");
    }

    [Fact]
    public void Paginate_LastPage_ReturnsRemainingItems()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = true, Page = 3, Limit = 10 };

        // Act
        var result = Context.TestEntities.Paginate(query);

        // Assert
        result.Items.Should().HaveCount(5); // 25 items, page 3 has 5 remaining
        result.CurrentPage.Should().Be(3);
        result.TotalPages.Should().Be(3);
    }
}
