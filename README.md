# Automation Practice Repo

**Creator:** Anastasiia Zelenova

This repository contains a .NET automation framework built with **xUnit** and **Microsoft Playwright**.

## Overview

The solution contains a single test project: `PlaywrightTests`.

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
│   │   ├── BookingApi.cs
│   │   └── EventApi.cs
│   ├── Models/
│   │   ├── Queries/
│   │   │   ├── BookingQuery.cs
│   │   │   └── EventQuery.cs
│   │   ├── Requests/
│   │   │   ├── CreateBookingRequest.cs
│   │   │   └── UpsertEventRequest.cs
│   │   └── Responses/
│   │       ├── AuthUser.cs
│   │       ├── BookingDto.cs
│   │       ├── BookingListResponse.cs
│   │       ├── EventDto.cs
│   │       └── EventListResponse.cs
│   └── Tests/
│       ├── Features/
│       │   ├── Auth/
│       │   │   ├── Login.cs
│       │   │   ├── Me.cs
│       │   │   └── Register.cs
│       │   ├── Bookings/
│       │   │   └── Bookings.cs
│       │   └── Events/
│       │       └── Events.cs
│       └── Infrastructure/
│           ├── Auth/
│           │   └── AuthHelper.cs
│           ├── Builders/
│           │   ├── BookingBuilder.cs
│           │   └── EventBuilder.cs
│           ├── Contexts/
│           │   ├── AuthTestContext.cs
│           │   ├── BookingTestContext.cs
│           │   └── EventTestContext.cs
│           └── Helpers/
│               ├── BookingTestHelpers.cs
│               └── EventTestHelpers.cs
├── Config/
│   ├── ConfigLoader.cs
│   └── PlaywrightSettings.cs
├── Fixtures/
│   ├── ApiClientFactory.cs
│   └── BrowserFixture.cs
├── Ui/
│   ├── Helpers/
│   │   ├── AuthHelper.cs
│   │   ├── BookingSetupHelper.cs
│   │   ├── EventSetupHelper.cs
│   │   └── TestDataModels.cs
│   ├── Pages/
│   │   ├── AdminEventsPage.cs
│   │   ├── EventDetailsPage.cs
│   │   ├── EventsPage.cs
│   │   ├── LoginPage.cs
│   │   ├── MyBookingsPage.cs
│   │   └── NavigationBar.cs
│   └── Tests/
│       ├── BookingTests.cs
│       ├── EventTests.cs
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
- `ApiClientFactory.cs` creates API request contexts for API tests and UI setup helpers

### `Api/`
Contains API automation structure.

#### `Api/Clients/`
API helper classes used by API tests.

- `AuthApi.cs` handles authentication endpoints
- `EventApi.cs` handles `/events`
- `BookingApi.cs` handles `/bookings`

#### `Api/Models/`
Typed API models separated by purpose.

- `Queries/` contains query models
- `Requests/` contains request payload models
- `Responses/` contains typed response models

#### `Api/Tests/Features/`
Feature-level API tests.

- `Features/Auth/` covers authentication flows
- `Features/Events/` covers event endpoints
- `Features/Bookings/` covers booking endpoints

#### `Api/Tests/Infrastructure/`
Shared API test support.

- `Auth/` contains auth-specific support such as `AuthHelper.cs`
- `Builders/` contains request builders for reusable test data creation
  - `EventBuilder.cs` builds valid event payloads
  - `BookingBuilder.cs` builds valid booking payloads
- `Contexts/` contains test contexts that bundle one API request context with the clients needed for a feature area
  - `AuthTestContext.cs` exposes `AuthApi`
  - `EventTestContext.cs` exposes `AuthApi` and `EventApi`
  - `BookingTestContext.cs` exposes `AuthApi`, `EventApi`, and `BookingApi`
- `Helpers/` contains reusable API test setup and response parsing helpers
  - `EventTestHelpers.cs` provides event setup and shared JSON response readers like `GetRootAsync()` and `GetData()`
  - `BookingTestHelpers.cs` provides booking setup and shared JSON response readers

These infrastructure pieces act as lightweight setup/build utilities for API tests so feature tests stay focused on assertions instead of repeating payload creation, request bootstrapping, and response extraction.

### `Ui/`
Contains UI automation structure.

#### `Ui/Pages/`
Page Object Model layer for UI flows.

- `LoginPage.cs`
- `NavigationBar.cs`
- `EventsPage.cs`
- `EventDetailsPage.cs`
- `AdminEventsPage.cs`
- `MyBookingsPage.cs`

#### `Ui/Helpers/`
UI-side helpers for setup and shared test data.

- `AuthHelper.cs` handles authentication setup for UI tests
- `EventSetupHelper.cs` creates event test data for UI scenarios
- `BookingSetupHelper.cs` creates booking test data for UI scenarios
- `TestDataModels.cs` contains UI-facing setup models

#### `Ui/Tests/`
UI test suites.

- `LoginTests.cs` covers login flows
- `EventTests.cs` covers event UI flows
- `BookingTests.cs` covers booking UI flows

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
4. UI helpers prepare auth or seed data when needed
5. page objects perform UI actions and assertions
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
