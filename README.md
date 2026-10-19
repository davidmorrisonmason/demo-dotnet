# DemoDotnet
A demo .NET code base with example web API architecture illustrating SOLID, Clean Architecture and CQRS.

When running locally, the API and populator use SQL Server with a local SQL Express instance and Windows authentication:

```text
Server=localhost\SQLEXPRESS;Database=Demo;Trusted_Connection=True;TrustServerCertificate=True
```

Set up a SQL Express database called Demo.

Apply the SQL Server schema before running the API in development or the populator:

```powershell
dotnet ef database update --project Demo.Infrastructure --startup-project Demo.Api
```

## Build and Test
Tests can be run locally within Visual Studio.

Tests use SQLite rather than SQL Server. This is to allow unit tests to run on Azure build agents without the need to connect to a SQL Server test instance (i.e. a nice cheap setup that can be
run on the free tier). The tests create local SQLite databases and then execute against them. These are ignored in the solution and created as needed.

## Build
The solution contains an Azure Devops pipeline for building and running the tests. Just create a pipeline and then link it to the yaml file in the root directory. It will then run without further setup.

## Deployment
The solution contains an Azure Devops pipeline for deploying to Azure App Service. For this to work, you need the following variables set up on your pipeline:

* $(AzureSubscription) e.g. My Azure Subscription Name(GUID ID HERE)
* $(WebAppName) e.g. DemoDotnet, or whatever you call it

You should set up a .net 10 App Service Web App in advance and populate these variables in your azure pipeline appropriately. 

The deployed solution expects a SQL Server database to exist somewhere - how this is managed is up to you. Easiest is to use Azure SQL Database free tier. Add the connection string to your
App Service environment variables. You can apply migrations and run the populator by putting the connection string to Azure into your user secrets in the solution and migrating and populating 
from your dev environment. 
