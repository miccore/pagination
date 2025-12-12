using Microsoft.EntityFrameworkCore;

namespace Miccore.Pagination.Tests.Common;

/// <summary>
/// Test entity for pagination tests.
/// </summary>
public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// In-memory database context for testing.
/// </summary>
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
}

/// <summary>
/// Base class for tests that need a database context.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly TestDbContext Context;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new TestDbContext(options);
    }

    protected void SeedData(int count = 25)
    {
        var entities = Enumerable.Range(1, count).Select(i => new TestEntity
        {
            Id = i,
            Name = $"Item {i:D3}",
            Price = i * 10.5m,
            CreatedAt = DateTime.UtcNow.AddDays(-count + i)
        }).ToList();

        Context.TestEntities.AddRange(entities);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}
