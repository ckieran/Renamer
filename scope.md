# Renamer App - Project Scope

## Overview
A cross-platform desktop application built with .NET MAUI for organizing photo libraries by analyzing EXIF metadata and renaming folders based on capture date ranges.

## Tech Stack
- **Frontend**: .NET MAUI with C#
- **Backend**: .NET 8 with C#
- **Testing**: xUnit for unit tests
- **Logging**: Serilog for audit capabilities
- **Image Processing**: MetadataExtractor for EXIF data
- **UI Framework**: MAUI Community Toolkit

## Project Structure

```
Renamer/
├── src/
│   ├── Renamer.Core/                    # Core business logic
│   │   ├── Models/
│   │   │   ├── FileOperation.cs
│   │   │   ├── FolderInfo.cs
│   │   │   ├── PhotoMetadata.cs
│   │   │   └── RenamePlan.cs
│   │   ├── Services/
│   │   │   ├── IExifService.cs
│   │   │   ├── ExifService.cs
│   │   │   ├── IFileSystemService.cs
│   │   │   ├── FileSystemService.cs
│   │   │   ├── IRenameService.cs
│   │   │   ├── RenameService.cs
│   │   │   ├── ILoggingService.cs
│   │   │   └── LoggingService.cs
│   │   ├── Enums/
│   │   │   ├── FileOperationType.cs
│   │   │   └── OperationStatus.cs
│   │   └── Utils/
│   │       ├── PathUtils.cs
│   │       └── DateUtils.cs
│   ├── Renamer.UI/                      # MAUI UI Layer
│   │   ├── Views/
│   │   │   ├── MainPage.xaml
│   │   │   ├── FolderPickerPage.xaml
│   │   │   ├── PlanViewPage.xaml
│   │   │   └── ErrorHandlingPage.xaml
│   │   ├── ViewModels/
│   │   │   ├── MainViewModel.cs
│   │   │   ├── FolderPickerViewModel.cs
│   │   │   ├── PlanViewViewModel.cs
│   │   │   └── ErrorHandlingViewModel.cs
│   │   ├── Services/
│   │   │   ├── IThemeService.cs
│   │   │   └── ThemeService.cs
│   │   └── Resources/
│   │       ├── Styles/
│   │       │   ├── Colors.xaml
│   │       │   └── Styles.xaml
│   │       └── Images/
│   └── Renamer.Tests/                   # Unit Tests
│       ├── Core/
│       │   ├── ExifServiceTests.cs
│       │   ├── FileSystemServiceTests.cs
│       │   ├── RenameServiceTests.cs
│       │   └── PathUtilsTests.cs
│       └── UI/
│           └── ViewModelTests.cs
├── docs/                                # Documentation
│   ├── API.md
│   └── UserGuide.md
├── samples/                             # Sample data for testing
│   └── TestPhotos/
└── Renamer.sln                          # Solution file
```

## Core Features Implementation

### 1. UI Features

#### Dark Mode Toggle
- Implement using MAUI's built-in theme support
- Store user preference in local settings
- Smooth transition between light/dark themes

#### Folder Picker
- Native folder picker dialog
- Display selected path with validation
- Show folder tree structure preview

#### Plan View
- Scrollable list of pending operations
- Color-coded operation types:
  - **Blue**: Folder creation
  - **Green**: Folder rename
  - **Yellow**: File move
  - **Red**: Error/warning
- Expandable details for each operation
- Batch selection capabilities

#### Test Run Mode
- Preview mode without actual file operations
- Metadata validation and reporting
- Corrupt file detection
- Progress indicator with detailed logging

### 2. Backend Features

#### File System Service
```csharp
public interface IFileSystemService
{
    Task<FolderTree> BuildFolderTreeAsync(string rootPath);
    Task<bool> RenameFolderAsync(string oldPath, string newPath);
    Task<IEnumerable<string>> GetImageFilesAsync(string folderPath);
    Task<bool> ValidatePathAsync(string path);
}
```

#### EXIF Service
```csharp
public interface IExifService
{
    PhotoMetadata ExtractMetadata(string filePath);
    DateTime? GetCaptureDate(string filePath);
    bool IsValidImageFile(string filePath);
    IEnumerable<string> GetSupportedExtensions();
}
```

#### Rename Service
```csharp
public interface IRenameService
{
    Task<RenamePlan> GenerateRenamePlanAsync(string rootPath);
    Task<OperationResult> ExecuteRenamePlanAsync(RenamePlan plan);
    Task<bool> ValidateOperationOrderAsync(List<FileOperation> operations);
}
```

### 3. Core Business Logic

#### Folder Analysis Algorithm
1. **Tree Building**: Recursively scan folders
2. **Metadata Extraction**: Process all image files in each folder
3. **Date Range Calculation**: Find min/max capture dates
4. **Name Pattern Matching**: Parse existing folder names for date ranges
5. **New Name Generation**: Create standardized folder names
6. **Operation Planning**: Build ordered list of operations

#### Supported File Formats
- **JPEG**: .jpg, .jpeg, .jpe
- **RAW**: .dng, .cr2, .nef, .arw, .raf
- **Other**: .tiff, .tif, .png, .heic

#### Date Format Handling
- **Input**: "YYYY-MM-DD - YYYY-MM-DD - <folder info>"
- **Output**: "YYYY-MM-DD - YYYY-MM-DD - <folder info>"
- **Single Date**: "YYYY-MM-DD - <folder info>" (when min = max)

### 4. Error Handling & Logging

#### Error Categories
- **File Access Errors**: Permission denied, file in use
- **Metadata Errors**: Corrupt EXIF data, unsupported format
- **Path Errors**: Invalid characters, length limits
- **Operation Errors**: Rename conflicts, disk space

#### Logging Strategy
- **Audit Trail**: All operations logged with timestamps
- **Error Details**: Full stack traces and context
- **Progress Tracking**: Operation completion percentages
- **User Actions**: Skip/abort decisions logged

#### Error Recovery
- **Skip Option**: Continue with next operation
- **Abort Option**: Stop all remaining operations
- **Retry Option**: Attempt operation again
- **Manual Override**: Allow user to specify new path

### 5. Multi-Platform Considerations

#### Path Handling
- **Windows**: Handle backslashes and reserved characters
- **macOS**: Handle forward slashes and case sensitivity
- **Linux**: Handle forward slashes and permissions

#### File Naming
- **Length Limits**: Respect OS-specific path length limits
- **Reserved Characters**: Filter invalid characters per platform
- **Case Sensitivity**: Handle case differences appropriately

## Development Phases

### Phase 1: Core Infrastructure 
- [x] Set up MAUI project structure
- [x] Implement basic UI with dark mode
- [x] Create core service interfaces
- [x] Set up logging infrastructure
- [x] Implement basic file system operations

### Phase 2: EXIF Processing 
- [x] Implement EXIF metadata extraction
    - Implemented `ExifService` using `MetadataExtractor` to read DateTimeOriginal and return `PhotoMetadata`.
- [x] Add support for multiple image formats
    - Added a supported extensions list (.jpg, .jpeg, .jpe, .png, .tiff, .tif, .heic, .dng, .cr2, .nef, .arw, .raf) and `IsValidImageFileAsync`.
- [x] Create date range calculation logic
    - Basic capture date extraction implemented; full folder-level min/max calculation is scaffolded in `FolderInfo` and will be used by rename planning.
- [x] Write unit tests for EXIF functionality
    - Added `ExifServiceTests` (checks supported extensions and IsValidImageFileAsync). Tests pass.
- [x] Add error handling for corrupt files
    - `ExifService` safely catches exceptions and returns metadata with filename only when parsing fails.

### Phase 3: Rename Logic 
- [x] Implement folder name parsing
    - Basic folder name generation implemented in `RenameService.GenerateRenamePlanAsync`.
- [x] Create rename plan generation
    - Implemented `RenameService` which builds `RenamePlan` from `FolderTree` using photo capture dates.
- [x] Add operation ordering logic
    - Simple validation implemented in `RenameService.ValidateOperationOrderAsync` to detect duplicate destinations.
- [x] Implement test run functionality
    - `RenameService.ExecuteRenamePlanAsync` performs renames via `IFileSystemService.RenameFolderAsync` and returns `OperationResult`.
- [x] Write unit tests for rename logic
    - Added `RenameServiceTests` with fake `IFileSystemService` and `IExifService`; tests passed.

### Phase 4: UI Implementation
- [x] Build folder picker interface
- [x] Create plan view with color coding
- [x] Implement progress indicators
- [x] Add error handling dialogs
- [x] Polish dark mode implementation

### Phase 5: Testing & Polish 
- [ ] Comprehensive unit test coverage
- [ ] Integration testing
- [ ] Cross-platform testing
- [ ] Performance optimization
- [ ] Documentation completion

## Testing Strategy

### Unit Tests
- **EXIF Service**: Metadata extraction, date parsing
- **File System Service**: Path validation, tree building
- **Rename Service**: Plan generation, operation ordering
- **Utility Functions**: Date formatting, path handling

### Integration Tests
- **End-to-End**: Complete rename workflow
- **Error Scenarios**: Corrupt files, permission issues
- **Cross-Platform**: Path handling on different OS

### Performance Tests
- **Large Libraries**: 10,000+ files
- **Deep Folder Structures**: 10+ levels deep
- **Memory Usage**: Monitor during large operations

## Success Criteria

### Functional Requirements
- [ ] Successfully rename folders based on EXIF dates
- [ ] Handle all supported image formats
- [ ] Provide accurate operation preview
- [ ] Implement robust error handling
- [ ] Support dark mode toggle

### Non-Functional Requirements
- [ ] Process 1000+ files in under 30 seconds
- [ ] 90%+ unit test coverage
- [ ] Cross-platform compatibility
- [ ] Comprehensive audit logging
- [ ] Intuitive user interface

## Risk Mitigation

### Technical Risks
- **EXIF Library Compatibility**: Research and test multiple libraries
- **MAUI Learning Curve**: Allocate extra time for UI development
- **Cross-Platform Issues**: Early testing on all target platforms

### Project Risks
- **Scope Creep**: Stick to core requirements initially
- **Testing Complexity**: Start with simple test cases
- **Performance Issues**: Profile early and optimize as needed

## Future Enhancements
- **Batch Processing**: Process multiple root folders
- **Custom Naming Patterns**: User-defined folder name formats
- **Image Preview**: Thumbnail generation for verification
- **Cloud Storage**: Support for cloud photo libraries
- **Backup Integration**: Automatic backup before operations