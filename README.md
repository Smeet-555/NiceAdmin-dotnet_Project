# NiceAdmin

A modern ASP.NET Core MVC web application for administrative management with a clean and intuitive interface.

## Overview

NiceAdmin is a comprehensive administrative dashboard built with ASP.NET Core that provides management capabilities for:

- **Staff Management** - Employee information and administration
- **Department Management** - Organizational structure management
- **Meeting Management** - Meeting scheduling and coordination
- **Meeting Types** - Categorization of different meeting types
- **Meeting Venues** - Venue management and booking
- **Meeting Members** - Participant management for meetings

## Features

- Clean and responsive admin interface
- MVC architecture with separation of concerns
- Modern ASP.NET Core framework
- Static asset management
- Error handling and logging
- Extensible controller-based architecture

## Technology Stack

- **Framework**: ASP.NET Core (.NET 10.0)
- **Architecture**: Model-View-Controller (MVC)
- **Language**: C#
- **Frontend**: HTML, CSS, JavaScript
- **Development Environment**: Compatible with Visual Studio, VS Code, and JetBrains Rider

## Project Structure

```
NiceAdmin/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs
│   ├── StaffController.cs
│   ├── DepartmentController.cs
│   ├── MeetingController.cs
│   ├── MeetingMemberController.cs
│   ├── MeetingTypeController.cs
│   └── MeetingVenueController.cs
├── Models/               # Data models and view models
├── Views/                # Razor views and templates
│   ├── Home/
│   ├── Staff/
│   ├── Department/
│   ├── Meeting/
│   └── Shared/
├── wwwroot/              # Static files (CSS, JS, images)
│   ├── assets/
│   ├── css/
│   ├── js/
│   └── lib/
└── Properties/           # Launch settings and configuration
```

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- A code editor (Visual Studio, VS Code, or JetBrains Rider)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd NiceAdmin
```

### 2. Navigate to Project Directory

```bash
cd NiceAdmin/NiceAdmin
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

The application will start and be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## Development

### Running in Development Mode

```bash
dotnet run --environment Development
```

### Building for Production

```bash
dotnet publish -c Release -o ./publish
```

### Project Configuration

The application uses standard ASP.NET Core configuration:

- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development-specific settings
- `launchSettings.json` - Development server configuration

## Available Controllers

| Controller | Purpose |
|------------|---------|
| `HomeController` | Main dashboard and home page |
| `StaffController` | Staff management operations |
| `DepartmentController` | Department administration |
| `MeetingController` | Meeting scheduling and management |
| `MeetingMemberController` | Meeting participant management |
| `MeetingTypeController` | Meeting type categorization |
| `MeetingVenueController` | Venue management and booking |

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Development Guidelines

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public methods
- Write unit tests for new features
- Ensure responsive design for all views

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support and questions, please open an issue in the repository or contact the development team.

---

**NiceAdmin** - Making administration nice and simple! 🚀