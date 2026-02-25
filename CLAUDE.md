# CLAUDE.md

## Project Overview

MountAws is a PowerShell provider that exposes AWS services as a navigable virtual filesystem. It is built on the [MountAnything](https://github.com/andyalm/mount-anything) framework. Users can `cd`, `dir`, and `Get-Content` through AWS resources using the `aws:` drive (e.g., `aws:/default/us-east-1/s3/buckets/my-bucket`).

## Tech Stack

- **Language:** C# (.NET 8.0) with file-scoped namespaces and nullable reference types enabled
- **Framework:** MountAnything 0.7.0 (PowerShell provider framework)
- **DI Container:** Autofac
- **AWS SDKs:** AWSSDK.* packages (version 3.7.*)
- **Testing:** xUnit + AwesomeAssertions
- **Publishing:** PowerShell Gallery module

## Repository Structure

```
MountAws.sln              # Solution file (2 projects)
MountAws/                 # Main project
  MountAwsProvider.cs     # Entry point - implements IMountAnythingProvider, sets up routing
  AwsItem.cs              # Base classes (AwsItem, AwsItem<T>) all items inherit from
  IServiceRoutes.cs       # Interface each service implements to register routes
  IServiceRegistrar.cs    # Interface each service implements for DI registration
  ItemNavigator.cs        # Generic navigator for hierarchical item listing
  WebUrlBuilder.cs        # Builds AWS Console URLs for items
  PagingHelper.cs         # Utility for paginated AWS API calls
  Commands.psm1           # PowerShell cmdlets (Switch-MountAwsProfile, Switch-MountAwsRegion)
  Services/
    Core/                 # Core routing: profiles → regions → services
    Acm/                  # AWS Certificate Manager
    Cloudfront/           # CloudFront distributions
    Cloudwatch/           # CloudWatch logs and metrics
    DynamoDb/             # DynamoDB tables and items
    Ec2/                  # EC2 instances, VPCs, subnets, security groups, ASGs, images
    Ecr/                  # Elastic Container Registry
    Ecs/                  # ECS clusters, services, tasks, task definitions
    Elasticache/          # ElastiCache
    Elbv2/                # Application/Network Load Balancers
    Iam/                  # IAM roles, policies, users
    Lambda/               # Lambda functions, aliases, versions, layers
    Rds/                  # RDS instances
    Route53/              # Route 53 hosted zones
    S3/                   # S3 buckets and objects
    ServiceDiscovery/     # Cloud Map service discovery
    Wafv2/                # WAFv2 web ACLs
MountAws.UnitTests/       # Unit tests (xUnit + AwesomeAssertions)
docs/                     # Service-specific documentation and developer guide
```

## Build & Test Commands

```bash
# Build the solution
dotnet build

# Build for publishing (outputs to bin/MountAws/)
dotnet publish

# Run unit tests
dotnet test

# Interactive testing (PowerShell, imports the module and navigates to aws: drive)
pwsh ./testenv.ps1
```

## Development Workflow

**After every code change**, always run:

1. `dotnet build` — verify the solution compiles without errors
2. `dotnet test` — verify all tests pass

Fix any build errors or test failures before committing. Do not commit code that fails to build or has failing tests.

## CI/CD

- **CI workflow** (`.github/workflows/ci.yml`): Runs on every push. Builds with `dotnet publish`, then verifies the module loads with `pwsh -c 'Import-Module ./bin/MountAws'`.
- **Publish workflow** (`.github/workflows/publish.yml`): Triggered on GitHub release. Publishes to PowerShell Gallery via `publish.ps1`.

## Architecture & Key Patterns

### Virtual Filesystem Structure

The path hierarchy is: `aws:/<profile>/<region>/<service>/...`

- `ProfilesHandler` → `ProfileHandler` → `RegionHandler` → per-service routes

### Adding/Modifying a Service

Each service lives in `MountAws/Services/<ServiceName>/` and consists of:

1. **Routes class** (`Routes.cs` or `<Service>Routes.cs`): Implements `IServiceRoutes`. Defines the URL-like route tree using `MapLiteral`, `Map`, and `MapRegex` from MountAnything's routing DSL.
2. **Registrar class** (`<Service>Registrar.cs`): Implements `IServiceRegistrar`. Registers the AWS SDK client in the Autofac container.
3. **Handler classes** (`*Handler.cs`): Extend `PathHandler`. Implement `GetItemImpl()` (return item for current path) and `GetChildItemsImpl()` (return child items for `dir`). Dependencies are constructor-injected.
4. **Item classes** (`*Item.cs`): Extend `AwsItem` or `AwsItem<T>`. Define `ItemName`, `ItemType`, `IsContainer`, and optionally `WebUrl`. The `ItemName` should prefer human-friendly names when unique; never use ARNs.
5. **API extension methods** (`*ApiExtensions.cs`): Synchronous wrappers around async AWS SDK calls using `.GetAwaiter().GetResult()`.
6. **Format files** (`Formats.ps1xml`): PowerShell formatting definitions for console display. TypeName must be the full C# type name (e.g., `MountAws.Services.S3.BucketItem`).

### Service discovery is automatic

`MountAwsProvider.CreateRouter()` uses reflection to find all `IServiceRoutes` and `IServiceRegistrar` implementations in the assembly. No manual registration is needed.

### Key conventions

- All items inherit from `AwsItem` or `AwsItem<T>` (not `Item` directly)
- File-scoped namespaces (`namespace MountAws.Services.S3;`)
- AWS SDK async methods are called synchronously via `.GetAwaiter().GetResult()` in API extension classes
- `GenericContainerItem` is used for virtual "directory" entries (e.g., service roots, collection containers)
- `CurrentXxx` classes (e.g., `CurrentBucket`, `CurrentCluster`) are injected context values set by the routing framework
- `ItemType` is auto-derived from the class name by stripping the "Item" suffix
- `*NotFoundException` exceptions follow a convention for missing resources
- Constants for item types are kept in `*ItemTypes` static classes
- `PagingHelper.Paginate()` is used for paginating AWS API responses
- `WebUrlBuilder` generates AWS Console links for items

### Routing DSL

```csharp
route.MapLiteral<RootHandler>("service-name", root => {
    root.MapLiteral<CollectionHandler>("items", items => {
        items.Map<ItemHandler, CurrentItem>(item => {   // dynamic segment with DI context
            item.MapLiteral<SubHandler>("sub-items");
        });
    });
});
```

- `MapLiteral`: Fixed path segment (e.g., "buckets", "clusters")
- `Map<THandler>`: Dynamic path segment (matches any value)
- `Map<THandler, TContext>`: Dynamic segment that also registers a context value for DI
- `MapRegex<THandler>`: Regex-based path matching (used for complex paths like S3 object keys)

## Testing

Tests use xUnit with AwesomeAssertions. The test project references the main MountAws project. Current tests verify routing resolution (that paths resolve to the correct handler types) and the ItemNavigator logic. Run with `dotnet test`.

## Code Style

- C# 10 features: file-scoped namespaces, implicit usings, nullable reference types
- No `.editorconfig` — follow existing patterns in the codebase
- Compact code style: prefer expression-bodied members and concise constructors
- Keep handler/item files focused — one primary class per file
