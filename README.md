# AngularCrudAppWithDotNetCoreEFCore

## Introduction

AngularCrudAppWithDotNetCoreEFCore is a comprehensive web application that serves as a CRUD (Create, Read, Update, Delete) example. The frontend of the application is built using Angular, while the backend is developed using .NET Core and Entity Framework Core for ORM.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## Features

- Full CRUD operations for managing data.
- Responsive design with Angular frontend.
- RESTful API with .NET Core backend.
- Database management with Entity Framework Core.
- User authentication and authorization.
- Error handling and validation.
- Docker support for containerization.

## Technologies Used

- **Frontend**:
  - Angular
  - TypeScript
  - HTML
  - CSS
  - JavaScript

- **Backend**:
  - .NET Core
  - C#
  - Entity Framework Core

## Getting Started

### Prerequisites

Before you begin, ensure you have met the following requirements:

- Node.js and npm installed
- .NET Core SDK installed
- SQL Server or any other database supported by EF Core
- Docker (optional, for containerization)

### Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/OneSAHDEVSINH/AngularCrudAppWithDotNetCoreEFCore.git
    cd AngularCrudAppWithDotNetCoreEFCore
    ```

2. Navigate to the `ClientApp` directory and install the Angular dependencies:

    ```bash
    cd ClientApp
    npm install
    ```

3. Navigate back to the root directory and restore the .NET Core dependencies:

    ```bash
    cd ..
    dotnet restore
    ```

4. Update the database using Entity Framework Core migrations:

    ```bash
    dotnet ef database update
    ```

## Usage

1. To run the Angular frontend, navigate to the `ClientApp` directory and start the Angular server:

    ```bash
    cd ClientApp
    ng serve
    ```

2. To run the .NET Core backend, navigate to the root directory and start the .NET Core server:

    ```bash
    dotnet run
    ```

3. Open your browser and navigate to `http://localhost:4200` to access the application.

## Project Structure

```
AngularCrudAppWithDotNetCoreEFCore/
├── ClientApp/                # Angular frontend
│   ├── src/
│   ├── angular.json
│   ├── package.json
│   └── README.md
├── Controllers/              # API controllers
├── Models/                   # Entity models
├── Migrations/               # Entity Framework migrations
├── Services/                 # Business logic
├── appsettings.json          # Application settings
├── Program.cs                # Program entry point
├── Startup.cs                # Application startup
└── README.md                 # This file
```

## Contributing

Contributions are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature-name`).
3. Make your changes.
4. Commit your changes (`git commit -m 'Add some feature'`).
5. Push to the branch (`git push origin feature/your-feature-name`).
6. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

```
