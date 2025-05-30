using Domain.Interfaces;
using static Domain.Models.Constants.ValidationConstants;

namespace Domain.Models;

public class Validator : IValidating
{
    public ValidatorStatusCode ValidateArgumentCount(string[] args)
        => args is null || args.Length != CORRECT_PROGRAM_ARGUMENT_COUNT
            ? ValidatorStatusCode.ArgumentCountError : ValidatorStatusCode.Success;

    public ValidatorStatusCode ValidateFolderPath(string folderPath)
        => Directory.Exists(folderPath)
        ? ValidatorStatusCode.Success : ValidatorStatusCode.FolderPathDoesNotExist;

    public ValidatorStatusCode ValidateSynchronizationInterval(string interval)
    {
        if (int.TryParse(interval, out int result))
        {
            if (result < MIN_SYNCHRONIZATION_INTERVAL)
            {
                return ValidatorStatusCode.SyncIntervalTooShortError;
            }
            else
            {
                return ValidatorStatusCode.Success;
            }
        }
        else
        {
            return ValidatorStatusCode.SyncIntervalParsingError;
        }
    }

    public ValidatorStatusCode ValidateFilePath(string filePath)
        => File.Exists(filePath)
        ? ValidatorStatusCode.Success : ValidatorStatusCode.FilePathDoesNotExist;
}