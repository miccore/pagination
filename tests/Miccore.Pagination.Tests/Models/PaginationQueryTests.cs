namespace Miccore.Pagination.Tests.Models;

public class PaginationQueryTests
{
    [Fact]
    public void PaginationQuery_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var query = new PaginationQuery();

        // Assert
        query.Paginate.Should().BeFalse();
        query.Page.Should().Be(1);
        query.Limit.Should().Be(10);
        query.OrderBy.Should().BeNull();
        query.OrderDirection.Should().Be("asc");
    }

    [Fact]
    public void PaginationQuery_CanSetAllProperties()
    {
        // Arrange & Act
        var query = new PaginationQuery
        {
            Paginate = true,
            Page = 5,
            Limit = 25,
            OrderBy = "Name",
            OrderDirection = "desc"
        };

        // Assert
        query.Paginate.Should().BeTrue();
        query.Page.Should().Be(5);
        query.Limit.Should().Be(25);
        query.OrderBy.Should().Be("Name");
        query.OrderDirection.Should().Be("desc");
    }

    [Fact]
    public void PaginationQuery_OrderDirection_DefaultsToAsc()
    {
        // Arrange & Act
        var query = new PaginationQuery { OrderBy = "Name" };

        // Assert
        query.OrderDirection.Should().Be("asc");
    }

    [Fact]
    public void PaginationQuery_Page_DefaultsToOne()
    {
        // Arrange & Act
        var query = new PaginationQuery();

        // Assert
        query.Page.Should().Be(1);
    }

    [Fact]
    public void PaginationQuery_Limit_DefaultsToTen()
    {
        // Arrange & Act
        var query = new PaginationQuery();

        // Assert
        query.Limit.Should().Be(10);
    }
}
