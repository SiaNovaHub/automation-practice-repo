# Automation Practice Repo

**Creator:** Anastasiia Zelenova

This repository contains a .NET automation framework for an event booking application, built with **xUnit** and **Microsoft Playwright**.

It is designed to demonstrate an interview-ready automation architecture with:
- **UI and API coverage in one solution**
- **clear separation of responsibilities**
- **reusable fixtures, builders, and helpers**
- **maintainable Playwright page objects**
- **tag-based execution for smoke and regression coverage**
- **GitHub Actions CI integration** for automated execution on repository pushes

---

## What This Project Demonstrates

This framework showcases several practical automation design choices:

- **UI automation** using Playwright and the Page Object Model
- **API automation** using typed request/response models and reusable API clients
- **shared fixtures** for browser and API request lifecycle management
- **API-backed setup for UI tests**, which keeps UI scenarios faster and more deterministic
- **feature-based test organization** for both UI and API layers
- **test tagging** using `Layer`, `Feature`, and `Type`
- **per-test isolation** to reduce test interdependence and flakiness

---

## Tech Stack

- **.NET** `net10.0`
- **xUnit** `2.9.3`
- **Microsoft.Playwright.Xunit** `1.59.0`
- **Microsoft.NET.Test.Sdk** `17.14.1`
- **Microsoft.Extensions.Configuration** `10.0.7`
- **coverlet.collector** `6.0.4`

---

## Repository Structure

```text
automation-practice-repo.sln
README.md
.github/
└── workflows/
    └── tests.yml
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
│   ├── BrowserFixture.cs
│   ├── Collections.cs
│   └── UiBaseTest.cs
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
└── TestResults/  (generated)
```

---

## Framework Architecture

## 1. Configuration Layer

The configuration layer lives in `Config/`.

- `ConfigLoader.cs` loads values from `appsettings.json` and environment variables
- `PlaywrightSettings.cs` defines the runtime configuration model

This keeps browser, API base URL, timeout, and launch settings centralized and easy to change.

## 2. Fixtures and Lifecycle Management

Shared lifecycle concerns are handled in `Fixtures/`.

- `BrowserFixture.cs`
  - initializes Playwright
  - launches the configured browser
  - creates browser contexts for UI tests
- `ApiClientFactory.cs`
  - creates `IAPIRequestContext` instances
  - is used by API tests and by UI setup helpers for seeding/authentication
- `Collections.cs`
  - defines the shared UI test collection
- `UiBaseTest.cs`
  - provides a common UI base class
  - creates a fresh browser context per test
  - manages context disposal and tracing

### Why this matters

This structure keeps lifecycle logic out of test methods and makes test classes easier to read and maintain.

## 3. API Automation Design

The API layer is organized around **typed clients, models, feature tests, and reusable test infrastructure**.

### API Clients

Located in `Api/Clients/`:
- `AuthApi.cs`
- `EventApi.cs`
- `BookingApi.cs`

These classes encapsulate endpoint interaction and reduce repeated request code in tests.

### API Models

Located in `Api/Models/`:
- `Queries/` for query parameters
- `Requests/` for payloads
- `Responses/` for typed response contracts

This helps keep API tests readable and explicit.

### API Test Infrastructure

Located in `Api/Tests/Infrastructure/`:

- `Auth/`
  - auth-related helpers such as `AuthHelper.cs`
- `Builders/`
  - reusable valid test data generators
  - `EventBuilder.cs`
  - `BookingBuilder.cs`
- `Contexts/`
  - lightweight wrappers that assemble the feature-specific API clients a test needs
  - `AuthTestContext.cs`
  - `EventTestContext.cs`
  - `BookingTestContext.cs`
- `Helpers/`
  - shared response parsing and test setup logic
  - `EventTestHelpers.cs`
  - `BookingTestHelpers.cs`

### Why this matters

This design keeps API feature tests focused on assertions instead of repeating:
- request context creation
- authentication setup
- payload construction
- raw JSON parsing

## 4. UI Automation Design

The UI layer follows the **Page Object Model** and uses API helpers for stable setup.

### UI Pages

Located in `Ui/Pages/`:
- `LoginPage.cs`
- `NavigationBar.cs`
- `EventsPage.cs`
- `EventDetailsPage.cs`
- `AdminEventsPage.cs`
- `MyBookingsPage.cs`

These classes encapsulate selectors and user interactions so test methods stay scenario-focused.

### UI Helpers

Located in `Ui/Helpers/`:
- `AuthHelper.cs`
- `EventSetupHelper.cs`
- `BookingSetupHelper.cs`
- `TestDataModels.cs`

These helpers are used to:
- register/authenticate users
- seed events/bookings through the API
- avoid expensive or repetitive UI-only setup flows when setup can be done faster through backend calls

### Why this matters

Using API-assisted setup in UI tests is a practical tradeoff:
- tests are faster
- tests are less brittle
- scenarios can start closer to the real assertion point

---

## Test Organization Strategy

Tests are organized by **feature area** rather than by technical action.

### UI tests
Located in `Ui/Tests/`:
- `LoginTests.cs`
- `EventTests.cs`
- `BookingTests.cs`

### API tests
Located in `Api/Tests/Features/`:
- `Auth/`
- `Events/`
- `Bookings/`

### Why this matters

A feature-based structure scales better in real projects because it matches how product functionality is discussed by teams.

---

## Test Tagging Strategy

The framework now uses three tags:

| Tag | Values | Purpose |
|---|---|---|
| `Layer` | `UI`, `API` | separates test level |
| `Feature` | `Auth`, `Events`, `Bookings` | separates functional area |
| `Type` | `Smoke`, `Regression` | separates execution intent |

### Tag placement

- `Layer` → class level
- `Feature` → class level
- `Type` → test method level

This keeps the broad classification at the suite level and allows smoke/regression mixing inside the same feature class.

### Example filters

Run all UI tests:

```bash
dotnet test --filter "Layer=UI"
```

Run all API tests:

```bash
dotnet test --filter "Layer=API"
```

Run only auth tests:

```bash
dotnet test --filter "Feature=Auth"
```

Run only smoke tests:

```bash
dotnet test --filter "Type=Smoke"
```

Run UI smoke tests only:

```bash
dotnet test --filter "Layer=UI&Type=Smoke"
```

Run API bookings regression tests:

```bash
dotnet test --filter "Layer=API&Feature=Bookings&Type=Regression"
```

---

## GitHub Actions CI Integration

CI is already configured in **`.github/workflows/tests.yml`**.

### Current trigger

The workflow runs on:
- **push to `main`**

### Current pipeline coverage

The workflow currently includes:
- **API Regression** job
- **UI Smoke Tests** job

### What the workflow does

For each run, the pipeline:
- checks out the repository
- sets up **.NET 10**
- restores and builds the solution
- installs Playwright and required browser dependencies
- executes the test commands through the CI scripts
- uploads test results as artifacts
- publishes `.trx` results using a test reporter
- uploads Playwright traces/logs on failure

---

## Execution Flow

### UI flow

1. settings are loaded from `appsettings.json`
2. `BrowserFixture` starts Playwright and launches the configured browser
3. `UiBaseTest` creates a fresh browser context for the test
4. UI helpers seed users/events/bookings when needed
5. page objects perform actions and assertions
6. tracing and browser context are cleaned up after the test

### API flow

1. settings are loaded from `appsettings.json`
2. `ApiClientFactory` creates an API request context
3. a feature test context initializes the required API clients
4. builders/helpers prepare valid test data
5. tests execute requests and assert status codes and response content
6. request contexts are disposed after execution

---

## Prerequisites

Before running the project, make sure you have:

- a compatible **.NET SDK** installed for `net10.0`
- internet access to reach the configured application URLs
- Playwright browsers installed locally

---

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

---

## Running the Tests

From the `PlaywrightTests` folder:

Run everything:

```bash
dotnet test
```

Run only UI tests:

```bash
dotnet test --filter "Layer=UI"
```

Run only API tests:

```bash
dotnet test --filter "Layer=API"
```

Run smoke tests only:

```bash
dotnet test --filter "Type=Smoke"
```

Run regression tests only:

```bash
dotnet test --filter "Type=Regression"
```

---

## Suggested Next Improvements

To take this framework further, good next steps would be:

- extend CI with pull request triggers and scheduled/nightly runs
- publish richer Playwright artifacts or execution summaries for easier triage
- add richer reporting (for example, HTML reporting dashboards)
- introduce environment-specific configuration profiles
- add contract/schema validation for selected API responses
- add nightly regression execution with tag-based filtering

These are intentionally listed as next steps rather than required complexity, which helps keep the current framework practical and interview-friendly.

---

## Summary

This project demonstrates a clean automation framework that combines:
- **UI testing with Playwright page objects**
- **API testing with typed clients and reusable infrastructure**
- **fixture-based lifecycle management**
- **tagged execution for smoke and regression suites**

The goal is not just to have tests, but to show a framework structure that is easy to explain, extend, and maintain.
