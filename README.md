
# ClaySolutionsTestCase

Locks access control sysem using .NET 5 API and MSSQLServer with clean architecture and service oriented design.

## Tech Stack

**Client:** Any

**Server:** .NET5, API


## Documentation

This project built with N-Layer clear architecture with { API - Application - Domain - Infrastructure} wtih DDD design and almost SOLID principles along with Repository pattern.
### Architecture
![Arch](https://user-images.githubusercontent.com/58634897/146997101-03a8e076-aa02-486b-80b2-13a88dea2ffa.png)

### Live from VS 2019
![VS1](https://user-images.githubusercontent.com/58634897/146997124-4b125130-6c41-4117-8d48-a77c15977e5c.JPG)

### Let's talk about the layers
API:
Contains all the controllers that allow the world to interact with the application and it's consuming Application and Infrastructure layer.

Application:
Contains all the services and middlewares as well as handlers that allow the API to apply needed project.

Domain:
Contains all the domain abstructions and interfaces as well as all entities that define the code first database.

Infrastructure:
Contains all the services and data that interact with the outside sources like databases and email service or other api.

## Run Locally

Clone the project

```bash
  git clone https://github.com/MHDKHAIR/ClaySolutionsTestCase.git
```

Go to the project directory

```bash
  cd ClaySolutionsTestCase
```

Open in VS

```bash
  start MySolution.sln and hit Enter
```

Modify appsettings.json inside API project and fill the empty fields

```bash
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
  // for changing database connection
    "DefaultConnection": "Data Source=.;Initial Catalog=ClayTestDB;Integrated Security=True"
  },
  // the secret key of the token
  "JwtSecret": "273317D7-B2E1-4484-90AD-9CF7738237BE",
  "JwtExpire": 1, // per hour
  "AdminEMail": "",
  "MailSettings": {
    "Mail": "", // soarce email to send emails to admin and users
    "DisplayName": "no-replay@claytest.com",
    "Password": "",
    "Host": "smtp.gmail.com",
    "Port": 587
  },
}
```

Then select LockAPI project as startup and start the application userin Kestrel.

It will start on this link: http://localhost:5022/
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

