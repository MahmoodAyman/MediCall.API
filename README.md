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
4. Never commit your `appsettings.development.json` file with real connection strings
