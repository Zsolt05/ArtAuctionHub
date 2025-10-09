# ArtAuctionHub – Full Project Description

## Overview

**ArtAuctionHub** is a modular **ASP.NET Core 8 Web API** solution that provides a complete backend for an online **art auction platform**.  
It allows **artists** to upload artworks and manage auctions, while **buyers** can place real-time bids and manage favorites.  
The system demonstrates a **clean architecture approach**, combining **Entity Framework Core**, **ASP.NET Identity**, **JWT authentication**, **SignalR**, **WebSockets**, and **custom middleware** for a scalable, production-grade solution.

---

## Architectural Overview

```
├── ArtAuctionHub.API
├── ArtAuctionHub.Application
├── ArtAuctionHub.Domain
├── ArtAuctionHub.Infrastructure
├── ArtAuctionHub.Shared
└── ArtAuctionHub.SignalRClientTest
```

Each layer serves a distinct purpose:

| Project | Purpose |
|----------|----------|
| **ArtAuctionHub.API** | Exposes RESTful endpoints and real-time hubs; handles authentication, middleware, and request routing |
| **ArtAuctionHub.Application** | Contains service interfaces, DTOs, and business logic implementations |
| **ArtAuctionHub.Domain** | Holds core entities and domain models with business rules |
| **ArtAuctionHub.Infrastructure** | Implements persistence (EF Core), repositories, and external service integrations |
| **ArtAuctionHub.Shared** | Provides constants, utilities, custom exceptions, and cross-layer extensions |
| **ArtAuctionHub.SignalRClientTest** | Serves as a local testing tool for SignalR client interactions |

---

## User Roles

| Role | Description | Permissions |
|------|--------------|-------------|
| **Artist** | Registered creator | Upload artworks, manage auctions |
| **Buyer** | Registered collector | Place bids, view auctions, manage favorites |

---

## Core Functional Modules

### 1. Authentication & Authorization
**Controller:** `AuthController`:contentReference[oaicite:0]{index=0}

- `POST /api/auth/register` – Registers a new user  
- `POST /api/auth/login` – Authenticates and issues JWT  
- `GET /api/auth/me` – Returns info about the authenticated user  

**Implementation Details:**
- Built using **ASP.NET Core Identity** and **JWT Bearer Authentication**
- Role-based authorization using `[Authorize(Roles = "...")]`
- Token claims include user ID and roles
- Uses `IAuthService` for registration, login, and user retrieval

---

### 2. Artwork Management
**Controller:** `ArtworksController`:contentReference[oaicite:1]{index=1}

- `POST /api/artworks` – Create a new artwork *(Artist only)*  
- `PUT /api/artworks/{id}` – Update an existing artwork *(Artist only)*  
- `DELETE /api/artworks/{id}` – Delete artwork *(Artist only)*  
- `GET /api/artworks` – Retrieve all artworks  
- `GET /api/artworks/my` – Retrieve artworks of the current artist  

**Highlights:**
- Supports image upload via `CreateArtworkForm`
- Uses **AutoMapper** for DTO ↔ Entity conversion
- Integrates **ICacheService** to cache artwork listings
- Logs user actions with injected `ILogger`
- Restricts modification endpoints to `Artist` role

---

### 3. Auction Management
**Controller:** `AuctionsController`:contentReference[oaicite:2]{index=2}

- `POST /api/auctions` – Create new auction *(Artist only)*  
- `PUT /api/auctions/{auctionId}` – Update auction *(Artist only)*  
- `DELETE /api/auctions/{auctionId}` – Delete auction *(Artist only)*  
- `GET /api/auctions/active` – List all active auctions  
- `GET /api/auctions/my` – List artist’s auctions  
- `GET /api/auctions/{auctionId}` – Retrieve auction details  

**Highlights:**
- Validates ownership before update/delete
- Returns mapped DTOs for all operations
- Logs user actions via `ILogger`
- Integrates with SignalR for real-time updates

---

### 4. Bidding System
**Controller:** `BidsController`:contentReference[oaicite:3]{index=3}

- `POST /api/bids` – Place a bid *(Buyer only)*  
- `GET /api/bids/my` – Retrieve user’s bids *(Buyer only)*  

**Highlights:**
- Validates user role and bid amount
- Relies on `IBidService` for bid creation and retrieval

---

### 5. Favorites Management
**Controller:** `FavoritesController`:contentReference[oaicite:4]{index=4}

- `POST /api/favorites/{artworkId}` – Add artwork to favorites *(Buyer only)*  
- `DELETE /api/favorites/{artworkId}` – Remove from favorites *(Buyer only)*  
- `GET /api/favorites` – Get all user favorites *(Buyer only)*  

**Highlights:**
- Includes `AdultContentAgeCheckFilter` for restricted content  
- Uses caching for performance optimization  
- Maps `Favorite` entities to artwork DTOs using **AutoMapper**

---

### 6. Categories
**Controller:** `CategoriesController`:contentReference[oaicite:5]{index=5}

- `GET /api/categories` – Retrieve all artwork categories  

**Highlights:**
- Uses **ICategoryService** and **ICacheService**  
- Implements cache expiration and sliding expiration  
- Returns lightweight category list to minimize payloads

---

## Real-Time Communication

### SignalR Hub – `AuctionBidHub`:contentReference[oaicite:6]{index=6}
- Endpoint: `/hubs/auction-bids`
- Methods:
  - `JoinAuction(int auctionId)` – Join a live auction group
  - `PlaceBid(int auctionId, decimal amount)` – Place a new bid
- Sends messages:
  - `BidPlaced` – Broadcasts new bids
  - `CurrentBid` – Sends current highest bid on join
  - `Error` – For invalid bids or authorization issues

### WebSocket Middleware – `AuctionBidWebSocketMiddleware`:contentReference[oaicite:7]{index=7}
- Endpoint: `/ws/auction-bids?auctionId=<id>`
- Handles low-level WebSocket bid connections
- Maintains active socket groups per auction
- Validates user, auction, and bid logic
- Broadcasts bid updates to all connected clients
- Falls back to HTTP for unsupported clients

---

## Middleware Overview

| Middleware | Purpose |
|-------------|----------|
| **ExceptionHandlingMiddleware** | Converts unhandled exceptions into JSON problem details |
| **RequestLoggingMiddleware** | Logs requests and execution times |
| **AuctionExistenceMiddleware** | Verifies that the auction exists before processing |
| **AuctionBidWebSocketMiddleware** | Manages WebSocket connections and broadcasts |

These middlewares are configured and executed in order within `Program.cs`:contentReference[oaicite:8]{index=8}.

---

## Domain Model Summary

| Entity | Description |
|---------|-------------|
| **User** | Extends `IdentityUser<int>`; includes birthdate, artworks, and favorites |
| **Role** | Extends `IdentityRole<int>`; defines user roles (Artist, Buyer) |
| **Artwork** | Represents an uploaded art piece with title, description, image, and category |
| **Auction** | Defines start/end time, starting price, and links to an artwork |
| **Bid** | Represents a buyer’s offer with amount, timestamp, and auction reference |
| **Category** | Classifies artworks into categories |
| **UserFavorites** | Many-to-many mapping between `User` and `Artwork` |
| **UserRole** | Many-to-many mapping between `User` and `Role` |

---

## Application Configuration & Startup

`Program.cs` is the entry point configuring the app’s pipeline and DI container

---

### Key Service Registrations

**1. Logging & Configuration**
- Clears existing providers and adds console logging for clarity and performance.  
- Reads all app settings from `appsettings.json`, including **AdultContentOptions** for age-restricted features.

**2. Authentication & Identity**
- Configures **ASP.NET Identity** with custom password requirements:
  - Minimum 6 characters  
  - Must include digits  
  - Non-alphanumeric and uppercase are optional  
- Integrates **JWT Authentication** through `AddJwtAuth()` extension.  
- Enables role-based authorization (`Artist`, `Buyer`).  

**3. Database & Persistence**
- Registers database infrastructure via `AddInfrastructure()`,  
  providing **Entity Framework Core (SQL Server)**, repositories, and database context `ArtAuctionHubDbContext`.  
- Ensures automatic database migration at startup with `app.MigrateDatabase<ArtAuctionHubDbContext>()`.

**4. Controllers & Filters**
- Registers controllers using `AddControllers()` and applies **global filters**:
  - `ValidateModelFilter` → Enforces model validation and structured error responses  
  - `LogUserActionFilter` → Logs all incoming user actions for auditing  

**5. Options Pattern Usage**
- Demonstrates all three configuration patterns:
  - `IOptions<T>` → Static configuration (`AdultContentStartupLogger`)  
  - `IOptionsSnapshot<T>` → Per-request updates (`AdultContentConfigController`)  
  - `IOptionsMonitor<T>` → Live change tracking (`AdultContentChangeWatcher`)

**6. Core Services**
Registers core scoped services used across the system:
- `IAuthService`, `IAuctionService`, `IArtworkService`, `IBidService`, `IFavoriteService`, `ICategoryService`
- `ICacheService` implemented by `MemoryCacheService`
- `PasswordHasherService` for secure password operations
- `AdultContentAgeCheckFilter` for content age verification

**7. SignalR & Real-Time Features**
- `AddSignalR()` enables **real-time communication** between the server and clients.
- Maps the **AuctionBidHub** to `/hubs/auction-bids` for live bidding functionality.
- Adds **AuctionBidWebSocketMiddleware** as a parallel WebSocket-based real-time channel.

**8. Caching**
- Combines **In-Memory Cache** and **Response Caching** to improve API performance and reduce redundant queries.

**9. Exception Handling**
- Uses `AddExceptionHandling()` for global exception management.
- Converts exceptions into standardized JSON responses with meaningful messages and status codes.

**10. Swagger Documentation**
- Integrates **Swagger/OpenAPI** for interactive API exploration.  
- Adds JWT bearer security definition for authorized testing directly in Swagger UI.  
- Enabled automatically in non-production environments.

---

### Middleware Pipeline Order

The order of middleware execution in `Program.cs` ensures optimal performance, security, and clarity:contentReference[oaicite:1]{index=1}:

1. **`UseExceptionHandling()`**  
   Centralized exception middleware capturing all unhandled errors and returning structured JSON responses.  

2. **`UseWebSockets()`**  
   Enables native WebSocket support for real-time communication.  

3. **`UseRequestLoggingMiddleware()`**  
   Custom middleware that logs request details (method, path, response time, status).  

4. **`UseHttpsRedirection()`**  
   Redirects HTTP requests to HTTPS to enforce secure communication.  

5. **`UseResponseCaching()`**  
   Caches GET responses to enhance response speed and reduce server load.  

6. **`UseAuthentication()`**  
   Verifies JWT tokens and user identity for all protected routes.  

7. **`UseAuthorization()`**  
   Enforces role-based access control defined in controllers.  

8. **`AddAuctionExistenceMiddleware()`**  
   Validates that a requested auction exists before controller logic executes.  

9. **`UseAuctionBidWebSocket()`**  
   Manages WebSocket connections for auction bid streams (`/ws/auction-bids`).  

10. **`MapHub<AuctionBidHub>()`**  
    Registers SignalR hub at `/hubs/auction-bids` to broadcast bid updates in real time.  

11. **`MapControllers()`**  
    Maps all REST API endpoints (Auth, Artworks, Auctions, etc.) to the pipeline.
