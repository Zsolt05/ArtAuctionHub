# ArtAuctionHub

## Database
First, you need to download and run Docker Desktop. \
After that, you can run the following command to start the database:
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssword1" -p 1433:1433 --name artauctionhub-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

## Database Image
![Database Image](./images/db-image.jpg)

## Entity Framework Core Migrations
### To apply migrations, you can use the following command:

#### In developer console:
```bash
dotnet ef database update --project ArtAuctionHub.Infrastructure
```

#### In package manager console:
```powershell
Update-Database -Project ArtAuctionHub.Infrastructure
```

### To create a new migration, use:

#### In developer console:
```bash
dotnet ef migrations add <MigrationName> --project ArtAuctionHub.Infrastructure --startup-project ArtAuctionHub.API
```

#### In package manager console:
```powershell
Add-Migration <MigrationName> -Project ArtAuctionHub.Infrastructure -StartupProject ArtAuctionHub.API
```