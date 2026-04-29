# Automation Practice Repo

**Creator:** Anastasiia Zelenova

This repository contains a .NET automation framework built with **xUnit** and **Microsoft Playwright**.

## Overview

The solution currently contains a single test project: `PlaywrightTests`.

The framework is organized into:

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
│   │   ├── AuthApi.cs
│   │   └── EventApi.cs
│   ├── Models/
│   │   ├── Queries/
│   │   │   └── EventQuery.cs
│   │   ├── Requests/
│   │   │   └── UpsertEventRequest.cs
│   │   └── Responses/
│   │       ├── AuthUser.cs
│   │       ├── EventDto.cs
│   │       └── EventListResponse.cs
│   └── Tests/
│       ├── Features/
│       │   ├── Auth/
│       │   │   ├── Login.cs
│       │   │   ├── Me.cs
│       │   │   └── Register.cs
│       │   └── Events/
│       │       └── Events.cs
│       └── Infrastructure/
│           ├── Contexts/
│           │   ├── AuthTestContext.cs
│           │   └── EventTestContext.cs
│           └── Helpers/
│               └── EventTestHelpers.cs
├── Config/
│   ├── ConfigLoader.cs
│   └── PlaywrightSettings.cs
├── Fixtures/
│   ├── ApiClientFactory.cs
│   └── BrowserFixture.cs
├── Ui/
│   ├── Helpers/
│   │   └── AuthHelper.cs
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

### `Api/`
Contains API automation structure.

#### `Api/Clients/`
API helper classes used by tests.

- `AuthApi.cs` handles authentication-related API calls
- `EventApi.cs` handles `/events` API operations and deserializes typed responses

#### `Api/Models/`
Typed API models separated by purpose.

- `Queries/` contains query models such as `EventQuery`
- `Requests/` contains request payload models such as `UpsertEventRequest`
- `Responses/` contains response models such as `AuthUser`, `EventDto`, `EventResponse`, `EventListResponse`, and `PaginationMeta`

#### `Api/Tests/Features/`
Feature-level API tests.

- `Features/Auth/` contains auth API tests for register, login, and `/auth/me`
- `Features/Events/` contains tests for the `/events` endpoints

#### `Api/Tests/Infrastructure/`
Shared API test support.

- `Contexts/` contains test contexts that bundle request context + API clients
- `Helpers/` contains reusable helper methods for API test setup and response parsing

### `Ui/`
Contains UI automation structure.

- `Helpers/` contains reusable UI-side helpers such as API-based login support
- `Pages/` is reserved for page objects and UI abstractions
- `Tests/` contains UI test classes

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
4. UI helpers can prepare authenticated state when needed
5. tests run browser actions and assertions
6. resources are disposed after execution

### API flow

1. settings are loaded from `appsettings.json`
2. `ApiClientFactory` creates an API request context
3. test contexts initialize the needed API clients
4. API clients send requests and deserialize typed responses
5. tests validate status codes and response content
6. resources are disposed after execution

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
