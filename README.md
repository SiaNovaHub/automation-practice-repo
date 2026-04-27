# Automation Practice Repo

**Creator:** Anastasiia Zelenova

This repository contains a .NET automation framework built with **xUnit** and **Microsoft Playwright**.

## Overview

The solution has one test project: `PlaywrightTests`.

It is currently structured into:

- **UI automation** under `Ui/`
- **API automation** under `Api/`
- **shared fixtures** under `Fixtures/`
- **configuration classes** under `Config/`

Main technologies:

- **.NET** (`net10.0`)
- **xUnit**
- **Microsoft.Playwright.Xunit**
- **Microsoft.Extensions.Configuration**

## Repository Structure

```text
automation-practice-repo.sln
PlaywrightTests/
├── PlaywrightTests.csproj
├── appsettings.json
├── Api/
│   ├── Clients/
│   └── Tests/
│       └── APILoginTests.cs
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
Contains the project definition, target framework, and NuGet package references.

### `Config/`
Holds the configuration layer for the framework.

- `ConfigLoader.cs` loads settings from `appsettings.json` and environment variables
- `PlaywrightSettings.cs` defines strongly typed settings models

### `Fixtures/`
Holds the shared setup used by tests.

- `BrowserFixture.cs` creates and manages the Playwright browser for UI tests
- `ApiClientFactory.cs` creates Playwright API request contexts for API tests

### `Ui/`
Contains UI automation code.

- `Tests/` stores UI test classes
- `Pages/` is available for page objects or other UI abstractions

### `Api/`
Contains API automation code.

- `Tests/` stores API test classes
- `Clients/` is available for API client abstractions

### `appsettings.json`
Stores runtime settings for the framework, including:

- UI base URL
- API base URL
- browser type
- launch options
- viewport settings
- timeout

## Execution Flow

### UI layer

1. settings are loaded from `appsettings.json`
2. `BrowserFixture` starts Playwright and launches the browser
3. tests create a browser context and page
4. Playwright performs browser actions and assertions
5. resources are disposed after execution

### API layer

1. settings are loaded from `appsettings.json`
2. `ApiClientFactory` creates an API request context
3. tests send HTTP requests through Playwright's API features
4. responses are validated in xUnit tests
5. resources are disposed after execution

## Prerequisites

Before running the framework, make sure you have:

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
