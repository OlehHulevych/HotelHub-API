# HOTEL RESERVATION  API

A .NET 8 RESTful API for managing booking hotel rooms, built following Clean Architecture principles.

## Project Structure
- **Server.Controllers** - REST API  controllers
- **Server.Models** - Database models
- **Server.Resposiotries** - Repository scripts for managing user and booking logic


## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)


## Getting Started

##Avalable on Azure Server
https://hotelhub-h5e8e6heewh4cvbb.polandcentral-01.azurewebsites.net/swagger

### Running Localy
1. Clone the repository:
   
```bash
https://github.com/OlehHulevych/HotelHub-API.git
```

2. In Visual studio select solution:

```bash
server.sln
```

3. Update the connection string in `appsettings.json` to point to your local MySQL instance
   
4. The API will be available at `http://localhost:7065`.

5. Run the application:
```bash
dotnet run
```

## API Documentation
Once the application is running, you can access the Swagger documentation at:

Localy:`https://localhost:7065/swagger`

Azure:`https://hotelhub-h5e8e6heewh4cvbb.polandcentral-01.azurewebsites.net/swagger`

## Project Features
- Clean Architecture implementation
- REST API endpoints for booking management
- JSON schema validation
- MySQL database integration
- Swagger documentation
- Deploying on Azure server
- Media Storage on Cloudinary Cloud

## Technologies Used
- .NET 8
- Entity Framework Core
- MySQL
- Swagger/OpenAPI


## Author
Hulevych Oleh
