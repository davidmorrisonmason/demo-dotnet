# demo
Demo code base for example architectures.

The API and populator use SQL Server with a local SQL Express instance and Windows authentication:

```text
Server=localhost\SQLEXPRESS;Database=Demo;Trusted_Connection=True;TrustServerCertificate=True
```

Apply the SQL Server schema before running the API in development or the populator:

```powershell
dotnet ef database update --project Demo.Infrastructure --startup-project Demo.Api
```

Production startup applies migrations and repopulates the demo database. The new `InitialCreate` migration creates the complete schema in a fresh SQL Server database; it replaces the SQLite migration history and does not transfer existing SQLite data. Tests continue to use SQLite.
