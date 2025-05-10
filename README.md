# MediCall - On-Demand Home Healthcare Service


https://github.com/user-attachments/assets/c8fb4c57-d455-4a68-82bb-08d885578584


## Project Overview

MediCall is a comprehensive on-demand home healthcare service platform that connects patients with qualified nurses for in-home medical services. The application enables patients to request medical services at their location, find nearby available nurses, and coordinate visits seamlessly through a robust booking system.

## Business Idea

The core business concept of MediCall is to bridge the gap between patients needing medical care and qualified healthcare professionals. By leveraging modern technology, the platform facilitates:

- On-demand healthcare services at the patient's location
- Real-time nurse discovery based on proximity and qualifications
- Secure payment processing
- Comprehensive visit management
- Quality assurance through reviews and ratings

This solution addresses the growing need for accessible healthcare, especially for individuals with mobility limitations, busy schedules, or those who prefer receiving care in the comfort of their homes.

## Key Features

- **User Authentication**: Secure registration and login for patients and nurses
- **Geolocation Services**: Matching patients with nearby available nurses
- **Visit Management**: Complete lifecycle management of healthcare visits
- **Real-time Notifications**: Using SignalR for instant communication
- **Payment Processing**: Integration with payment gateways for secure transactions
- **Review System**: Quality assurance through patient feedback
- **Admin Dashboard**: Comprehensive management interface

## Architecture & Project Structure

MediCall follows a clean architecture pattern with clear separation of concerns:

### Core Layer
Contains domain models, DTOs, interfaces, and business logic abstractions:
- **Models**: Domain entities (Patient, Nurse, Visit, Service, etc.)
- **DTOs**: Data transfer objects for API communication
- **Interfaces**: Service contracts and repository interfaces
- **Enums**: Type definitions and constants

### Infrastructure Layer
Implements the interfaces defined in the Core layer:
- **Data**: Database context, configurations, and seed data
- **Repositories**: Data access implementations
- **Services**: Business logic implementations
- **Identity**: Authentication and authorization
- **SignalR**: Real-time communication hubs

### API Layer
Web API controllers and endpoints for client applications:
- **Controllers**: RESTful API endpoints
- **Middleware**: Request processing pipelines
- **Extensions**: Application service configurations

### AdminDashboard
Web application for administrative functions:
- **Controllers**: MVC controllers for admin views
- **Views**: Razor views for the admin interface
- **Models**: View models specific to the admin dashboard

## Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: ASP.NET Core Identity with JWT
- **Real-time Communication**: SignalR
- **Payment Processing**: Integration with third-party payment gateway (Paymob)
- **Location Services**: Geospatial calculations for proximity matching
- **Frontend**: Angular (developed by Abdelrahman Fathy)

## Contributors

### Mahmoud Ayman
- Database configurations
- Visit service implementation
- Payment processing integration
- SignalR real-time notifications
- Service management
- **Core Feature**: Implemented the nearest available and verified nurse discovery system using the Haversine Equation for geolocation matching

### Abdulaziz Taha
- Authentication and authorization
- Admin dashboard development
- Data seeding and management
- User management
- Advanced validation system for National ID, birth data, and gender verification
- Email confirmation system using MailSender service

### Ahmed Ashraf
- Data Transfer Objects (DTOs)
- Review system implementation
- API documentation

### Abdelrahman Fathy
- Angular frontend development
- User interface design and implementation
- Frontend-backend integration
- access it from [GitHub Repo](https://github.com/abdofathy883/MediCall)

## Development Setup

1. Clone the repository
2. Copy the template settings file:
   ```
   cp API/appsettings.development.template.json API/appsettings.development.json
   ```
3. Update the connection string in your local copy of `appsettings.development.json` or use user secrets:
   ```
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING" --project API/API.csproj
   ```
4. Configure payment gateway settings in appsettings or user secrets
5. Build and run the solution:
   ```
   dotnet build
   dotnet run --project API/API.csproj
   ```

## API Documentation

The API provides endpoints for:
- User authentication and registration
- Service discovery
- Visit booking and management
- Payment processing
- Reviews and ratings

Full API documentation is available when running the application by navigating to `/swagger`.

