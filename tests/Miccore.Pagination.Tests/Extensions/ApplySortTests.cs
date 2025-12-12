using Miccore.Pagination.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Miccore.Pagination.Tests.Extensions;

public class ApplySortTests : TestBase
{
    public ApplySortTests()
    {
        SeedData(10);
    }

    [Fact]
    public void ApplySort_WithValidProperty_SortsAscending()
    {
        // Arrange & Act
        var result = Context.TestEntities.ApplySort("Name", "asc").ToList();

        // Assert
        result.First().Name.Should().Be("Item 001");
        result.Last().Name.Should().Be("Item 010");
    }

    [Fact]
    public void ApplySort_WithValidProperty_SortsDescending()
    {
        // Arrange & Act
        var result = Context.TestEntities.ApplySort("Price", "desc").ToList();

        // Assert
        result.First().Price.Should().Be(105m); // Item 10
        result.Last().Price.Should().Be(10.5m); // Item 1
    }

    [Fact]
    public void ApplySort_WithNullOrderBy_ReturnsOriginalQuery()
    {
        // Arrange & Act
        var result = Context.TestEntities.ApplySort(null, "asc").ToList();

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public void ApplySort_WithEmptyOrderBy_ReturnsOriginalQuery()
    {
        // Arrange & Act
        var result = Context.TestEntities.ApplySort("", "asc").ToList();

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public void ApplySort_WithInvalidProperty_ReturnsOriginalQuery()
    {
        // Arrange & Act
        var result = Context.TestEntities.ApplySort("NonExistentProperty", "asc").ToList();

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public void ApplySort_IsCaseInsensitive()
    {
        // Arrange & Act
        var result = Context.TestEntities.ApplySort("NAME", "asc").ToList();

        // Assert
        result.First().Name.Should().Be("Item 001");
    }

    [Fact]
    public void ApplySort_WithDateProperty_SortsCorrectly()
    {
        // Arrange & Act
        var result = Context.TestEntities.ApplySort("CreatedAt", "desc").ToList();

        // Assert
        result.First().Id.Should().Be(10); // Most recent
        result.Last().Id.Should().Be(1); // Oldest
    }

    [Fact]
    public void ApplySort_WithDecimalProperty_SortsCorrectly()
    {
        // Arrange & Act
        var result = Context.TestEntities.ApplySort("Price", "asc").ToList();

        // Assert
        result.First().Price.Should().Be(10.5m);
        result.Last().Price.Should().Be(105m);
    }
}
