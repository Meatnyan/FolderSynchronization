namespace Domain.Models;

public enum ValidatorStatusCode
{
    Success = 0,
    ArgumentCountError = 1,
    FolderPathDoesNotExist = 2,
    SyncIntervalParsingError = 3,
    SyncIntervalTooShortError = 4,
    FilePathDoesNotExist = 5
}