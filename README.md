# Miccore .Net Pagination

[![NuGet](https://img.shields.io/nuget/v/Miccore.Net.Pagination.svg)](https://www.nuget.org/packages/Miccore.Net.Pagination)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Server-side pagination library for .NET with Entity Framework Core support.

**Requires .NET 8.0 or .NET 10.0**

## Features

- ✅ Async and sync pagination methods
- ✅ Dynamic sorting by property name
- ✅ IAsyncEnumerable streaming support
- ✅ Automatic navigation links (prev/next)
- ✅ Full nullable reference types support
- ✅ Multi-targeting .NET 8.0 and .NET 10.0

## Installation

### .NET CLI
```sh
dotnet add package Miccore.Net.Pagination
```

### Package Manager
```sh
Install-Package Miccore.Net.Pagination
```

---

## ⚠️ Migration Guide v1 → v2

Version 2.0 introduces breaking changes. Follow this guide to migrate from v1.x:

### Breaking Changes

| v1.x | v2.x | Action Required |
|------|------|-----------------|
| `Miccore.Pagination.Model` | `Miccore.Pagination` | Update namespace imports |
| `Miccore.Pagination.Service` | `Miccore.Pagination` | Update namespace imports |
| `RouterLink` class | `RouteLink` class | Rename class usage |
| `query.paginate` | `query.Paginate` | Update property names (PascalCase) |
| `query.page` | `query.Page` | Update property names (PascalCase) |
| `query.limit` | `query.Limit` | Update property names (PascalCase) |
| .NET 6.0 | .NET 8.0 / 10.0 | Upgrade target framework |

### Migration Steps

1. **Update your target framework** in your `.csproj`:
```xml
<TargetFramework>net8.0</TargetFramework>
```

2. **Update namespace imports**:
```csharp
// Before (v1)
using Miccore.Pagination.Models;
using Miccore.Pagination.Service;

// After (v2)
using Miccore.Pagination;
```

3. **Update PaginationQuery properties** to PascalCase:
```csharp
// Before (v1)
var query = new PaginationQuery { paginate = true, page = 1, limit = 10 };

// After (v2)
var query = new PaginationQuery { Paginate = true, Page = 1, Limit = 10 };
```

4. **Rename RouterLink to RouteLink** (if used directly):
```csharp
// Before (v1)
RouterLink.AddRouteLink(...)

// After (v2)
RouteLink.AddRouteLink(...)
// Or use as extension method:
response.AddRouteLink(url, query);
```

---

## 🔄 Deterministic Sorting

> **Important:** When using pagination with sorting, ensure your sort order is deterministic to avoid duplicated or missing items across pages.

### The Problem

If you sort by a non-unique field (e.g., `CreatedAt`), multiple items may have the same value. The database may return these items in an inconsistent order, causing:
- Items appearing on multiple pages
- Items being skipped entirely

### The Solution

Always add a secondary sort on a unique field (like `Id`):

```csharp
// ❌ Non-deterministic - may cause issues
var result = await _context.Products
    .ApplySort("CreatedAt", "desc")
    .PaginateAsync(query);

// ✅ Deterministic - safe for pagination
var result = await _context.Products
    .ApplySort("CreatedAt", "desc")
    .ThenBy(x => x.Id)  // Secondary sort ensures uniqueness
    .PaginateAsync(query);
```

---

## Quick Start

### Basic Usage

```csharp
using Miccore.Pagination;
using Microsoft.Extensions.DependencyInjection;

// In your repository or service
public async Task<PaginationModel<Product>> GetProductsAsync(PaginationQuery query)
{
    return await _context.Products.PaginateAsync(query);
}

// In your controller
[HttpGet]
public async Task<ActionResult<PaginationModel<ProductDto>>> GetProducts([FromQuery] PaginationQuery query)
{
    var products = await _productService.GetProductsAsync(query);
    var response = _mapper.Map<PaginationModel<ProductDto>>(products);
    
    if (query.Paginate)
    {
        response.AddRouteLink(Url.RouteUrl(nameof(GetProducts)), query);
    }
    
    return Ok(response);
}
```

### With Sorting

```csharp
// Request: GET /api/products?paginate=true&page=1&limit=10&orderBy=Price&orderDirection=desc
[HttpGet]
public async Task<ActionResult<PaginationModel<ProductDto>>> GetProducts([FromQuery] PaginationQuery query)
{
    // Sorting is automatically applied based on query parameters
    var products = await _context.Products
        .PaginateAsync(query);
    
    // Response includes sorting info
    // {
    //   "currentPage": 1,
    //   "totalPages": 5,
    //   "sortedBy": "Price",
    //   "sortDirection": "desc",
    //   "items": [...]
    // }
    
    return Ok(products);
}
```

### Synchronous Pagination

```csharp
// For scenarios where async is not suitable
var result = _context.Products.Paginate(query);
```

### Streaming with IAsyncEnumerable

```csharp
// Stream all items (no pagination)
await foreach (var product in _context.Products.AsStreamAsync("Name", "asc"))
{
    await ProcessProduct(product);
}

// Stream paginated items
await foreach (var product in _context.Products.PaginateAsStreamAsync(query))
{
    await ProcessProduct(product);
}
```

---

## API Reference

### PaginationQuery

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Paginate` | `bool` | `false` | Enable/disable pagination |
| `Page` | `int` | `1` | Page number (1-indexed) |
| `Limit` | `int` | `10` | Items per page |
| `OrderBy` | `string?` | `null` | Property name to sort by |
| `OrderDirection` | `string` | `"asc"` | Sort direction (`"asc"` or `"desc"`) |

### PaginationModel\<T\>

| Property | Type | Description |
|----------|------|-------------|
| `PageSize` | `int` | Items per page (max 100) |
| `CurrentPage` | `int` | Current page number |
| `TotalItems` | `int` | Total item count |
| `TotalPages` | `int` | Total page count |
| `Items` | `List<T>` | Items for current page |
| `Prev` | `string?` | Previous page URL |
| `Next` | `string?` | Next page URL |
| `SortedBy` | `string?` | Applied sort property |
| `SortDirection` | `string?` | Applied sort direction |

### Extension Methods

| Method | Description |
|--------|-------------|
| `PaginateAsync<T>()` | Async pagination with sorting |
| `Paginate<T>()` | Sync pagination with sorting |
| `ApplySort<T>()` | Apply dynamic sorting |
| `AsStreamAsync<T>()` | Stream all items |
| `PaginateAsStreamAsync<T>()` | Stream paginated items |
| `AddRouteLink<T>()` | Add navigation URLs |

---

## Complete Example

### Repository
```csharp
using Miccore.Pagination;
using Microsoft.Extensions.DependencyInjection;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public async Task<PaginationModel<Notification>> GetAllAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => !n.IsDeleted)
            .PaginateAsync(query, cancellationToken);
    }
}
```

### Service
```csharp
using Miccore.Pagination;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public async Task<PaginationModel<NotificationDto>> GetAllAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _repository.GetAllAsync(query, cancellationToken);
        return _mapper.Map<PaginationModel<NotificationDto>>(notifications);
    }
}
```

### Controller
```csharp
using Miccore.Pagination;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly IMapper _mapper;

    [HttpGet(Name = nameof(GetNotifications))]
    public async Task<ActionResult<PaginationModel<NotificationViewModel>>> GetNotifications(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var notifications = await _service.GetAllAsync(query, cancellationToken);
        var response = _mapper.Map<PaginationModel<NotificationViewModel>>(notifications);

        if (!query.Paginate)
        {
            return Ok(response.Items);
        }

        response.AddRouteLink(Url.RouteUrl(nameof(GetNotifications))!, query);
        return Ok(response);
    }
}
```

### AutoMapper Profile
```csharp
using Miccore.Pagination;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>().ReverseMap();
        CreateMap<NotificationDto, NotificationViewModel>().ReverseMap();
        CreateMap<PaginationModel<Notification>, PaginationModel<NotificationDto>>().ReverseMap();
        CreateMap<PaginationModel<NotificationDto>, PaginationModel<NotificationViewModel>>().ReverseMap();
    }
}
```

---

## License

MIT License - see [LICENSE](LICENSE) for details.
