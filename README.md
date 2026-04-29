# Automation Practice Repo

**Creator:** Anastasiia Zelenova

This repository contains a .NET automation framework built with **xUnit** and **Microsoft Playwright**.

## Overview

The solution currently contains a single test project: `PlaywrightTests`.

The framework is split into:

- **Ui/** for UI automation
- **Api/** for API automation
- **Fixtures/** for shared setup
- **Config/** for runtime configuration

Main technologies:

- **.NET** (`net10.0`)
- **xUnit**
- **Microsoft.Playwright.Xunit**
- **Microsoft.Extensions.Configuration**

## Repository Structure

```text
automation-practice-repo.sln
README.md
PlaywrightTests/
├── PlaywrightTests.csproj
├── appsettings.json
├── Api/
│   ├── Clients/
│   │   └── AuthApi.cs
│   ├── Models/
│   │   └── AuthUser.cs
│   └── Tests/
│       └── Auth/
│           ├── AuthTestContext.cs
│           ├── Login.cs
│           └── Register.cs
├── Config/
│   ├── ConfigLoader.cs
│   └── PlaywrightSettings.cs
├── Fixtures/
│   ├── ApiClientFactory.cs
│   └── BrowserFixture.cs
├── Ui/
│   ├── Pages/
│   └── Tests/
│       └── LoginTests.cs
├── bin/
└── obj/
```

## Framework Structure

### `PlaywrightTests.csproj`
Contains the project definition, target framework, and package references.

### `Config/`
Contains the configuration layer.

- `ConfigLoader.cs` loads values from `appsettings.json` and environment variables
- `PlaywrightSettings.cs` defines the settings model used by the framework

### `Fixtures/`
Contains shared factories and fixtures.

- `BrowserFixture.cs` manages the browser lifecycle for UI tests
- `ApiClientFactory.cs` creates API request contexts for API tests

### `Ui/`
Contains UI automation structure.

- `Tests/` contains UI test classes
- `Pages/` is reserved for page objects and UI abstractions

### `Api/`
Contains API automation structure.

- `Clients/` contains API helper classes such as `AuthApi`
- `Models/` contains API data models such as `AuthUser`
- `Tests/` contains API tests, currently grouped under `Auth/`

### `appsettings.json`
Contains runtime settings for the framework, including:

- `BaseUrl`
- `ApiBaseUrl`
- browser type
- launch options
- viewport settings
- timeout

## Execution Flow

### UI flow

1. settings are loaded from `appsettings.json`
2. `BrowserFixture` starts Playwright and launches the browser
3. UI tests create a browser context and page
4. tests run browser actions and assertions
5. resources are disposed after execution

### API flow

1. settings are loaded from `appsettings.json`
2. `ApiClientFactory` creates an API request context
3. API helpers send requests through Playwright's API layer
4. tests validate the responses
5. resources are disposed after execution

## Prerequisites

Before running the project, make sure you have:

- a compatible **.NET SDK** installed for `net10.0`
- internet access to reach the configured application URLs
- Playwright browsers installed locally

## Setup

### 1. Restore dependencies

From the repository root:

```bash
dotnet restore
```

### 2. Build the project

```bash
dotnet build
```

### 3. Install Playwright browsers

From the `PlaywrightTests` directory:

#### Windows PowerShell

```powershell
pwsh .\bin\Debug\net10.0\playwright.ps1 install
```

#### macOS / Linux

```bash
./bin/Debug/net10.0/playwright.sh install
```

> If you build in `Release`, update the path accordingly.

## Running the Project

From the `PlaywrightTests` folder:

```bash
dotnet test
```

## `.gitignore`

The root `.gitignore` is intentionally minimal and currently ignores:

- `bin/`
- `obj/`
- `.vs/`
- `TestResults/`
