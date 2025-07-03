# ArtAuctionHub – Project Structure

This repository follows the Clean Architecture principles for a modular .NET 9 backend application.

## Folder Layout (under `src/`)

```
src/
├── ArtAuctionHub.API/              # Web API entry point (ASP.NET Core)
│   ├── Controllers/                # REST endpoints (e.g. AuthController, AuctionController)
│   ├── Filters/                    # Action filters, validation, formatting (e.g. ValidateModelFilter)
│   ├── Middlewares/                # API-specific middleware components (e.g. ErrorHandlerMiddleware, WebSocketMiddleware)
│   ├── appsettings.json            # Configuration file
│   └── Program.cs                  # Application entry point
│
├── ArtAuctionHub.Application/      # Use case logic
│   ├── DTOs/                       # Request/response data transfer objects (e.g. RegisterUserDto, AuctionResultDto)
│   ├── Interfaces/                 # Service interfaces (e.g. IAuthService, IAuctionService, ICacheService)
│   └── Services/                   # Use case implementations (e.g. AuthService, AuctionService)
│
├── ArtAuctionHub.Domain/           # Domain layer (DDD)
│   ├── Entities/                   # Business entities (e.g. User, Auction, Bid)
│   ├── Enums/                      # Enumerations (e.g. RoleType, AuctionStatus)
│   ├── Events/                     # Domain events (e.g. BidPlacedEvent, AuctionEndedEvent, UserRegisteredEvent)
│   ├── Interfaces/                 # Domain contracts (e.g. IRepository<T>)
│   └── ValueObjects/               # Non-identifiable value objects (e.g. Money, EmailAddress)
│
├── ArtAuctionHub.Infrastructure/   # Technical implementations (EF Core, Redis, SignalR)
│   ├── Logging/                    # Logging providers (e.g. FileLogger)
│   ├── Persistence/                # DbContext, EF Core migrations, configurations (e.g. AppDbContext, EntityTypeConfigurations)
│   ├── Services/                   # Infrastructure services (e.g. TokenService, DistributedCacheService, WebSocketHandler)
│   └── SignalR/                    # Real-time communication with SignalR (e.g. AuctionHub.cs)
│
└── ArtAuctionHub.Shared/           # Reusable, cross-cutting components
    ├── Constants/                  # Global constants (e.g. Roles)
    ├── Exceptions/                 # Shared exceptions (e.g. ValidationException, NotFoundException)
    ├── Extensions/                 # Extension methods
    └── Middleware/                 # Reusable middlewares
```