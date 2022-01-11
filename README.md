
# Clay Solutions TestCase

This is a TestCase solution for creating ASP.NET Core following the principles of Clean Architecture.
The domain for this solution is to allow mobile or web users clients to interact with an API interface which allows users to open doors and show historical events to extend
our user experience beyond classical tags.

## Technologies

* [ASP.NET Core 5](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-5.0)
* [Entity Framework Core 5](https://docs.microsoft.com/en-us/ef/core/)
* [JwtBearer](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer?view=aspnetcore-5.0)
* [AutoMapper](https://automapper.org/)
* [FluentValidation](https://fluentvalidation.net/)
* [XUnit](https://nunit.org/), [Moq](https://github.com/moq)
* [Docker](https://www.docker.com/)

## Documentation

This project built with N-Layer clear architecture with { API - Application - Domain - Infrastructure} wtih DDD design and almost SOLID principles along with Repository pattern.

### Database Diagram

![DB](https://github.com/MHDKHAIR/ClaySolutionsTestCase/blob/master/Database%20Diagram.png)

### Architecture

![Arch](https://user-images.githubusercontent.com/58634897/146997101-03a8e076-aa02-486b-80b2-13a88dea2ffa.png)

## Overview

### Domain

This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

## API

This layer is an API application based on ASP.NET Core 5. This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.

## Getting Started

The easiest way to get started:

1. Install the latest [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Install the latest [DOCKER](https://www.docker.com/get-started)
3. Modify **Presentation/LookAPIs/appsettings.json** fill the empty fields
```json
  "UseInMemoryDatabase": true,
  "AdminEMail": "",
  "MailSettings": {
    "Mail": "",
    "DisplayName": "no-replay@claytest.com",
    "Password": "",
    "Host": "smtp.gmail.com",
    "Port": 587
  },
  "HostDomain": "https://localhost:5022/",
```

Then select LockAPI project as startup and start the application using Kestrel or Docker.

</br>
Admin user:</br>
Email: clay@admin.com
Password: SuperAdmin@2022

## API Reference
Postman collection:
https://github.com/MHDKHAIR/ClaySolutionsTestCase/blob/master/Clay%20LocksAPI.postman_collection.json

## Demo

Guest user flow:https://youtu.be/AyrqBvgugPY
Employee user flow: https://youtu.be/tjcyTJlhDgo

## License

No License Needed :)
