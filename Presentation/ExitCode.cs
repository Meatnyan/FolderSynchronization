namespace Presentation;

internal enum ExitCode
{
    Success = 0,
    ArgumentCountError = 1,
    FolderCreationError = 2,
    SyncIntervalParsingError = 3,
    SyncIntervalTooShortError = 4,
    LogFileCreationError = 5
}