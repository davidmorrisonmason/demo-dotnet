# demo
A demo .NET code base with example web API architecture illustrating SOLID, Clean Architecture and CQRS.

When running locally, the API and populator use SQL Server with a local SQL Express instance and Windows authentication:

```text
Server=localhost\SQLEXPRESS;Database=Demo;Trusted_Connection=True;TrustServerCertificate=True
```

Apply the SQL Server schema before running the API in development or the populator:

```powershell
dotnet ef database update --project Demo.Infrastructure --startup-project Demo.Api
```

Production startup applies migrations and repopulates the demo database. This is just so that a deployment can always have known clean data.

## Build and Test
Tests can be run locally within visual studio.

# Deployment
The solution contains an Azure Devops pipeline for deploying to Azure App Service. For this to work, you need the following variables set up on your pipeline:

* $(AzureSubscription) e.g. My Azure Subscription Name(GUID ID HERE)
* $(WebAppName) e.g. The App service web app name

You should set up a .net 10 App Service Web App in advance
