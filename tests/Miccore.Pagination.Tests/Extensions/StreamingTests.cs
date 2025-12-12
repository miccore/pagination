using Miccore.Pagination.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Miccore.Pagination.Tests.Extensions;

public class StreamingTests : TestBase
{
    public StreamingTests()
    {
        SeedData(25);
    }

    [Fact]
    public async Task PaginateAsStreamAsync_ReturnsStreamedItems()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = true, Page = 1, Limit = 5 };
        var items = new List<TestEntity>();

        // Act
        await foreach (var item in Context.TestEntities.PaginateAsStreamAsync(query))
        {
            items.Add(item);
        }

        // Assert
        items.Should().HaveCount(5);
    }

    [Fact]
    public async Task PaginateAsStreamAsync_WithPaginationDisabled_ReturnsAllItems()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = false, Page = 1, Limit = 10 };
        var items = new List<TestEntity>();

        // Act
        await foreach (var item in Context.TestEntities.PaginateAsStreamAsync(query))
        {
            items.Add(item);
        }

        // Assert
        items.Should().HaveCount(25);
    }

    [Fact]
    public async Task AsStreamAsync_WithSorting_ReturnsAllItemsSorted()
    {
        // Arrange
        var items = new List<TestEntity>();

        // Act
        await foreach (var item in Context.TestEntities.AsStreamAsync("Name", "desc"))
        {
            items.Add(item);
        }

        // Assert
        items.Should().HaveCount(25);
        items.First().Name.Should().Be("Item 025");
    }

    [Fact]
    public async Task AsStreamAsync_WithoutSorting_ReturnsAllItems()
    {
        // Arrange
        var items = new List<TestEntity>();

        // Act
        await foreach (var item in Context.TestEntities.AsStreamAsync())
        {
            items.Add(item);
        }

        // Assert
        items.Should().HaveCount(25);
    }

    [Fact]
    public async Task PaginateAsStreamAsync_SecondPage_ReturnsCorrectItems()
    {
        // Arrange
        var query = new PaginationQuery { Paginate = true, Page = 2, Limit = 10 };
        var items = new List<TestEntity>();

        // Act
        await foreach (var item in Context.TestEntities.PaginateAsStreamAsync(query))
        {
            items.Add(item);
        }

        // Assert
        items.Should().HaveCount(10);
        items.First().Id.Should().Be(11);
    }

    [Fact]
    public async Task AsStreamAsync_CanBeCancelled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var items = new List<TestEntity>();
        var count = 0;

        // Act
        await foreach (var item in Context.TestEntities.AsStreamAsync(cancellationToken: cts.Token))
        {
            items.Add(item);
            count++;
            if (count >= 5)
            {
                cts.Cancel();
                break;
            }
        }

        // Assert
        items.Should().HaveCount(5);
    }
}
