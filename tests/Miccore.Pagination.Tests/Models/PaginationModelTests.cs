namespace Miccore.Pagination.Tests.Models;

public class PaginationModelTests
{
    [Fact]
    public void PaginationModel_PageSize_EnforcesMaximum()
    {
        // Arrange & Act
        var model = new PaginationModel<object> { PageSize = 150 };

        // Assert
        model.PageSize.Should().Be(100);
    }

    [Fact]
    public void PaginationModel_PageSize_AllowsValidValues()
    {
        // Arrange & Act
        var model = new PaginationModel<object> { PageSize = 50 };

        // Assert
        model.PageSize.Should().Be(50);
    }

    [Fact]
    public void PaginationModel_PageSize_AllowsMaximumValue()
    {
        // Arrange & Act
        var model = new PaginationModel<object> { PageSize = 100 };

        // Assert
        model.PageSize.Should().Be(100);
    }

    [Fact]
    public void PaginationModel_Items_InitializedAsEmptyList()
    {
        // Arrange & Act
        var model = new PaginationModel<object>();

        // Assert
        model.Items.Should().NotBeNull();
        model.Items.Should().BeEmpty();
    }

    [Fact]
    public void PaginationModel_NullableProperties_AreNullByDefault()
    {
        // Arrange & Act
        var model = new PaginationModel<object>();

        // Assert
        model.Prev.Should().BeNull();
        model.Next.Should().BeNull();
        model.SortedBy.Should().BeNull();
        model.SortDirection.Should().BeNull();
    }

    [Fact]
    public void PaginationModel_CanSetAllProperties()
    {
        // Arrange & Act
        var model = new PaginationModel<string>
        {
            PageSize = 20,
            CurrentPage = 3,
            TotalItems = 100,
            TotalPages = 5,
            Items = ["item1", "item2"],
            Prev = "/api/items?page=2",
            Next = "/api/items?page=4",
            SortedBy = "Name",
            SortDirection = "asc"
        };

        // Assert
        model.PageSize.Should().Be(20);
        model.CurrentPage.Should().Be(3);
        model.TotalItems.Should().Be(100);
        model.TotalPages.Should().Be(5);
        model.Items.Should().HaveCount(2);
        model.Prev.Should().Be("/api/items?page=2");
        model.Next.Should().Be("/api/items?page=4");
        model.SortedBy.Should().Be("Name");
        model.SortDirection.Should().Be("asc");
    }

    [Fact]
    public void PaginationModel_Items_CanBeModified()
    {
        // Arrange
        var model = new PaginationModel<int>();

        // Act
        model.Items.Add(1);
        model.Items.Add(2);

        // Assert
        model.Items.Should().HaveCount(2);
        model.Items.Should().Contain(1);
        model.Items.Should().Contain(2);
    }
}
