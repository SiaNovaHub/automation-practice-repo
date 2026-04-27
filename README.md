# Automation Practice Repo

**Creator:** Anastasiia Zelenova

This repository contains a UI automation framework built with **.NET**, **xUnit**, and **Microsoft Playwright**.

## Overview

The solution is organized as a single test project named `PlaywrightTests`.

Main technologies used:

- **.NET** (`net10.0`)
- **xUnit** for test execution
- **Microsoft.Playwright.Xunit** for browser automation
- **Microsoft.Extensions.Configuration** for loading runtime settings

## Repository Structure

```text
automation-practice-repo.sln       Solution file
PlaywrightTests/
├── PlaywrightTests.csproj         Test project and package references
├── LoginTests.cs                  Test class file
├── appsettings.json               Framework runtime settings
├── Config/
│   ├── ConfigLoader.cs            Reads configuration from JSON and environment variables
│   └── PlaywrightSettings.cs      Strongly typed settings models
├── Fixtures/
│   └── PlaywrightFixture.cs       Shared Playwright browser fixture
├── bin/                           Generated build output
└── obj/                           Generated intermediate output
```

## Framework Structure

### `PlaywrightTests.csproj`
Defines the test project, target framework, and NuGet dependencies.

### `Fixtures/PlaywrightFixture.cs`
This is the core shared fixture for the framework. It is responsible for:

- creating the Playwright instance
- launching the configured browser
- creating browser contexts
- cleaning up browser resources after execution

### `Config/ConfigLoader.cs`
Loads framework settings from `appsettings.json` and environment variables.

### `Config/PlaywrightSettings.cs`
Contains the settings classes used by the framework, including:

- browser selection
- launch options
- viewport settings
- timeout configuration

### `appsettings.json`
Stores the default local execution settings for the framework, such as:

- browser type
- headless mode
- slow motion
- viewport size
- timeout

## Execution Flow

At a high level, the framework works like this:

1. configuration is loaded from `appsettings.json`
2. the Playwright fixture starts the browser
3. tests create a new browser context and page
4. Playwright runs browser actions and assertions
5. resources are cleaned up after execution

## Prerequisites

Before running the framework, make sure you have:

- a compatible **.NET SDK** installed for `net10.0`
- internet access to reach `https://eventhub.rahulshettyacademy.com`
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
