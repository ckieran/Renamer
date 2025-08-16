# Renamer

Renamer is a cross-platform desktop application built with .NET MAUI for organizing photo libraries. It analyzes EXIF metadata and renames folders based on the range of capture dates found in the images inside.

## Features

- Rename folders based on EXIF capture dates
- Preview planned operations before applying them
- Dark mode toggle and other common desktop UI conveniences
- Serilog-based logging for full audit trails

## Project Structure

- `src/Renamer.Core` – core business logic and services
- `src/Renamer.UI` – MAUI UI layer
- `src/Renamer.Tests` – unit tests

## Prerequisites

- .NET 8 SDK

## Build

```
dotnet build
```

## Test

```
dotnet test
```

## License

This project is licensed under the MIT License.
