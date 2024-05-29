# Table of Content

1. For Developer
   1. [Scaffold database schema](./ForDeveloper.md#Scaffold-database-context)
   2. [Update database schema](./ForDeveloper.md#Update-database-schema)
   3. [Realtime Data](./ForDeveloper.md#signalr)
   4. [Rules](./ForDeveloper.md#rules)

## Database

### Scaffold database context

In this project, we are using database-first approach. Therefore, our scaffold database using:

a. Using Entity Framework Core tools
CLI ([Installation Guide](https://docs.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools))

```shell
dotnet ef dbcontext scaffold "Host=[ip];Database=MlffWebApi;Username=[username];Password=[password]" Npgsql.EntityFrameworkCore.PostgreSQL --context MlffDbContext --context-dir "Database/DbContexts" --use-database-names --namespace MlffWebApi.Database.DbContexts --output-dir "Database/DbContexts" --no-onconfiguring --data-annotations --force
```

> Reference: https://learn.microsoft.com/en-us/ef/core/cli/dotnet#dotnet-ef-dbcontext-scaffold

b. Using Nuget package
Microsoft.EntityFrameworkCore.Tools ([Installation Guide](https://docs.microsoft.com/en-us/ef/core/cli/powershell#installing-the-tools))

```shell
Scaffold-DbContext -Connection "Host=[ip]:5432;Database=MlffWebApi;Username=[username];Password=[password]" -Provider Npgsql.EntityFrameworkCore.PostgreSQL -Context MlffDbContext -OutputDir "Database/DbContexts" -UseDatabaseName -Namespace MlffWebAPI.Database.DbContexts -NoOnConfiguring -DataAnnotations --force
```

> Reference: https://learn.microsoft.com/en-us/ef/core/cli/powershell#scaffold-dbcontext

### Update database schema

1. To update the database schema, there is a few key action to perform.

   1. Modify the table initialization [script](./sql-scripts/001-init-tables.sql).
   2. After step #1, you can manually update the database schema using:
      1. DBeaver or any other database management tools of your choice
      2. Restart the container after deleted the attached volume (Defined in [YAML](./docker-compose.yaml))
         > <small>Re-initiate table will caused data loss. Kindly use
         > on <b style="text-transform:uppercase;font-style:italic;color:red">development environment only.</b></small>

2. In order to get updated value or database generated value, example: `camera.date_modified`, `camera.date_created`,
   the `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]` is required. Scaffold database using ef core tool is
   missing that. Manual modification is required. This is only required if the value is generated with trigger.

```csharp
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime date_modified { get; set; };
```

## SignalR

In this project, we selected SignalR as the technology to fulfill the realtime data exchange between API server and
clients (Mobile App).

### Setup

You can install the SignalR client package with following approaches

#### Install via [npm](https://www.npmjs.com/package/@microsoft/signalr)

```shell
npm init -y
npm install @microsoft/signalr
```

#### Using CDN

```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/6.0.1/signalr.js"></script>
```

### Connection

You may connect to the SignalR server using url `http://{server_ip}:{server_port}/detection`

### Example

Microsoft provided plenty of example on create and start a connection. You may take a look.

1. [Connect to a hub](https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-6.0&tabs=visual-studio#connect-to-a-hub)
   - Replace the URL with the provided url in [this section](#connection).
   - Make sure this connection URL is configurable as we can easily change the SignalR server.

## Rules

This is some rules to follow for code quality.

### Database

1. Use singular as table name. Example:`camera`, `user` etc.
2. Use pluralized database context.
   ```csharp
   public DbSet<camera> cameras { get; set; }
   public DbSet<user> users { get; set; }
   ```
3. Naming convention please refer to table in [this section](#backend).

### Backend

1. Use Newtonsoft.Json instead of System.Text.Json as JsonSerializer.
2. Follow this naming convention:

| Type                     | Use case                                |
| :----------------------- | --------------------------------------- |
| **camelCase**            | JSON property name (HTTP response body) |
| **SCREAMING_SNAKE_CASE** | Constant, enum values                   |
| **PascalCase**           | Public class property, class method     |
| **\_camelCase**          | Private class property                  |
| **flatcase**             | SQL table name, SQL column name         |
