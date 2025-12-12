namespace Miccore.Pagination.Tests.Services;

public class RouteLinkTests
{
    [Fact]
    public void AddRouteLink_WithPaginationEnabled_AddsNextLink()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 1,
            TotalPages = 3,
            Items = []
        };
        var query = new PaginationQuery { Paginate = true, Page = 1, Limit = 10 };

        // Act
        model.AddRouteLink("/api/items", query);

        // Assert
        model.Next.Should().Be("/api/items?paginate=True&limit=10&page=2");
        model.Prev.Should().BeNull();
    }

    [Fact]
    public void AddRouteLink_WithPaginationEnabled_AddsPrevLink()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 2,
            TotalPages = 3,
            Items = []
        };
        var query = new PaginationQuery { Paginate = true, Page = 2, Limit = 10 };

        // Act
        model.AddRouteLink("/api/items", query);

        // Assert
        model.Prev.Should().Be("/api/items?paginate=True&limit=10&page=1");
        model.Next.Should().Be("/api/items?paginate=True&limit=10&page=3");
    }

    [Fact]
    public void AddRouteLink_OnLastPage_NoNextLink()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 3,
            TotalPages = 3,
            Items = []
        };
        var query = new PaginationQuery { Paginate = true, Page = 3, Limit = 10 };

        // Act
        model.AddRouteLink("/api/items", query);

        // Assert
        model.Prev.Should().Be("/api/items?paginate=True&limit=10&page=2");
        model.Next.Should().BeNull();
    }

    [Fact]
    public void AddRouteLink_OnFirstPage_NoPrevLink()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 1,
            TotalPages = 3,
            Items = []
        };
        var query = new PaginationQuery { Paginate = true, Page = 1, Limit = 10 };

        // Act
        model.AddRouteLink("/api/items", query);

        // Assert
        model.Prev.Should().BeNull();
        model.Next.Should().NotBeNull();
    }

    [Fact]
    public void AddRouteLink_WithPaginationDisabled_NoLinks()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 1,
            TotalPages = 1,
            Items = []
        };
        var query = new PaginationQuery { Paginate = false, Page = 1, Limit = 10 };

        // Act
        model.AddRouteLink("/api/items", query);

        // Assert
        model.Prev.Should().BeNull();
        model.Next.Should().BeNull();
    }

    [Fact]
    public void AddRouteLink_WithSorting_IncludesSortingParams()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 1,
            TotalPages = 3,
            Items = []
        };
        var query = new PaginationQuery
        {
            Paginate = true,
            Page = 1,
            Limit = 10,
            OrderBy = "Name",
            OrderDirection = "desc"
        };

        // Act
        model.AddRouteLink("/api/items", query);

        // Assert
        model.Next.Should().Contain("orderBy=Name");
        model.Next.Should().Contain("orderDirection=desc");
    }

    [Fact]
    public void AddRouteLink_WithSpecialCharsInOrderBy_EncodesCorrectly()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 1,
            TotalPages = 2,
            Items = []
        };
        var query = new PaginationQuery
        {
            Paginate = true,
            Page = 1,
            Limit = 10,
            OrderBy = "Created At",
            OrderDirection = "asc"
        };

        // Act
        model.AddRouteLink("/api/items", query);

        // Assert
        model.Next.Should().Contain("orderBy=Created%20At");
    }

    [Fact]
    public void AddRouteLink_SinglePage_NoLinks()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 1,
            TotalPages = 1,
            Items = []
        };
        var query = new PaginationQuery { Paginate = true, Page = 1, Limit = 10 };

        // Act
        model.AddRouteLink("/api/items", query);

        // Assert
        model.Prev.Should().BeNull();
        model.Next.Should().BeNull();
    }

    [Fact]
    public void AddRouteLink_ReturnsModelForChaining()
    {
        // Arrange
        var model = new PaginationModel<object>
        {
            CurrentPage = 1,
            TotalPages = 2,
            Items = []
        };
        var query = new PaginationQuery { Paginate = true, Page = 1, Limit = 10 };

        // Act
        var result = model.AddRouteLink("/api/items", query);

        // Assert
        result.Should().BeSameAs(model);
    }
}
