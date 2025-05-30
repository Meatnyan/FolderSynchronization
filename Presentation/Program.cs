using static Domain.Models.Constants.TextConstants;
using static Domain.Models.Constants.ValidationConstants;
using Validator = Domain.Models.Validator;
using Domain.Models;

namespace Presentation;

internal static class Program
{
    private static void Main(string[] args)
    {
        Validator validator = new();

        HandleArgumentCount(validator, args);
        HandleFolderPaths(validator, [args[0], args[1]]);
        HandleSyncInterval(validator, args[2]);
        HandleLogFilePath(validator, args[3]);

        string sourceFolderPath = args[0];
        string replicaFolderPath = args[1];
        int syncInterval = int.Parse(args[2]);
        string logFilePath = args[3];

        DisplayConfigurationSummary(sourceFolderPath, replicaFolderPath, syncInterval,
            logFilePath);

        Synchronizer synchronizer = new(sourceFolderPath, replicaFolderPath, syncInterval,
            new Logger(logFilePath));

        synchronizer.BeginSynchronization();
    }

    private static void DisplayConfigurationSummary(string sourceFolderPath,
        string replicaFolderPath, int syncInterval, string logFilePath)
    {
        if (string.IsNullOrWhiteSpace(sourceFolderPath))
        {
            throw new ArgumentException($"'{nameof(sourceFolderPath)}' cannot be null or whitespace.",
                nameof(sourceFolderPath));
        }
        if (string.IsNullOrWhiteSpace(replicaFolderPath))
        {
            throw new ArgumentException($"'{nameof(replicaFolderPath)}' cannot be null or whitespace.",
                nameof(replicaFolderPath));
        }
        if (string.IsNullOrWhiteSpace(logFilePath))
        {
            throw new ArgumentException($"'{nameof(logFilePath)}' cannot be null or whitespace.",
                nameof(logFilePath));
        }

        Console.WriteLine(
@$"
{LINE_SEPARATOR}
Configuration summary:
Source folder: ""{sourceFolderPath}""
Replica folder: ""{replicaFolderPath}""
Synchronization interval: {syncInterval}ms
Log file path: ""{logFilePath}""
            
Beginning folder synchronization.
{LINE_SEPARATOR}
"
        );
    }

    private static void HandleSyncInterval(Validator validator, string syncInterval)
    {
        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }
        if (string.IsNullOrWhiteSpace(syncInterval))
        {
            throw new ArgumentException($"'{nameof(syncInterval)}' cannot be null or whitespace.",
                nameof(syncInterval));
        }

        ValidatorStatusCode syncIntervalValidationResult =
            validator.ValidateSynchronizationInterval(syncInterval);

        if (syncIntervalValidationResult == ValidatorStatusCode.SyncIntervalParsingError)
        {
            Console.WriteLine($"Error: Provided synchronization interval \"{syncInterval}\"" +
                $" cannot be parsed as an integer. Exiting program.");

            Environment.Exit((int)ExitCode.SyncIntervalParsingError);
        }
        else if (syncIntervalValidationResult == ValidatorStatusCode.SyncIntervalTooShortError)
        {
            Console.WriteLine($"Error: Provided synchronization interval \"{syncInterval}\"" +
                $" is too short. Minimum expected value is {MIN_SYNCHRONIZATION_INTERVAL}." +
                $" Exiting program.");

            Environment.Exit((int)ExitCode.SyncIntervalTooShortError);
        }
        else
        {
            Console.WriteLine($"Synchronization interval of {syncInterval}ms is valid" +
                $" and will be used.");
        }
    }

    private static void HandleFolderPaths(Validator validator, string[] folderPaths)
    {
        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }
        if (folderPaths is null)
        {
            throw new ArgumentNullException(nameof(folderPaths));
        }

        for (int i = 0; i < folderPaths.Length; i++)
        {
            if (validator.ValidateFolderPath(folderPaths[i])
                == ValidatorStatusCode.FolderPathDoesNotExist)
            {
                Console.WriteLine($"Folder path \"{folderPaths[i]}\" does not exist." +
                    $" Attempt to create it? ({RESPONSE_POSITIVE}/{RESPONSE_NEGATIVE})");

                HandleUserResponse(folderPaths[i], ResponseTryCreateFolder, ResponseDoNotCreateFolder);
            }
            else
            {
                Console.WriteLine($"Folder \"{folderPaths[i]}\" already exists and will be used.");
            }
        }
    }

    private static void HandleArgumentCount(Validator validator, string[] args)
    {
        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        if (validator.ValidateArgumentCount(args) == ValidatorStatusCode.ArgumentCountError)
        {
            Console.WriteLine(@"Error: Incorrect argument count.
                Provide 4 arguments (enclosed in quotes and separated by spaces):
                1. Source folder path;
                2. Replica folder path;
                3. Synchronization interval (in ms);
                4. Log file path.
                Exiting program.");

            Environment.Exit((int)ExitCode.ArgumentCountError);
        }
    }

    private static void HandleLogFilePath(Validator validator, string logFilePath)
    {
        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }
        if (string.IsNullOrWhiteSpace(logFilePath))
        {
            throw new ArgumentException($"'{nameof(logFilePath)}' cannot be null or whitespace.",
                nameof(logFilePath));
        }

        if (validator.ValidateFilePath(logFilePath) == ValidatorStatusCode.FilePathDoesNotExist)
        {
            Console.WriteLine($"Log file path \"{logFilePath}\" does not exist." +
                $" Attempt to create it? ({RESPONSE_POSITIVE}/{RESPONSE_NEGATIVE})");

            HandleUserResponse(logFilePath, ResponseTryCreateLogFile, ResponseDoNotCreateLogFile);
        }
        else
        {
            Console.WriteLine($"Log file \"{logFilePath}\" already exists and will be used.");
        }
    }

    private static void HandleUserResponse(string positiveResponseTextParam,
        PositiveResponseCallback positiveResponseCallback,
        NegativeResponseCallback negativeResponseCallback)
    {
        if (string.IsNullOrWhiteSpace(positiveResponseTextParam))
        {
            throw new ArgumentException($"'{nameof(positiveResponseTextParam)}'" +
                $" cannot be null or whitespace.", nameof(positiveResponseTextParam));
        }
        if (positiveResponseCallback is null)
        {
            throw new ArgumentNullException(nameof(positiveResponseCallback));
        }
        if (negativeResponseCallback is null)
        {
            throw new ArgumentNullException(nameof(negativeResponseCallback));
        }

        bool validResponse = false;
        while (!validResponse)
        {
            string? userResponse = Console.ReadLine()?.ToLower();
            validResponse = userResponse == RESPONSE_POSITIVE
                || userResponse == RESPONSE_NEGATIVE;

            if (!validResponse)
            {
                Console.WriteLine($"Provided response is not valid." +
                    $" Enter \"{RESPONSE_POSITIVE}\" for \"Yes\"" +
                    $" or \"{RESPONSE_NEGATIVE}\" for \"No\".");
            }
            else if (userResponse == RESPONSE_POSITIVE)
            {
                positiveResponseCallback(positiveResponseTextParam);
            }
            else if (userResponse == RESPONSE_NEGATIVE)
            {
                negativeResponseCallback();
            }
        }
    }


    private delegate void PositiveResponseCallback(string textParam);

    private delegate void NegativeResponseCallback();

    private static void ResponseTryCreateFolder(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            throw new ArgumentException($"'{nameof(folderPath)}'" +
                $" cannot be null or whitespace.", nameof(folderPath));
        }

        try
        {
            Directory.CreateDirectory(folderPath);

            Console.WriteLine($"Successfully created folder \"{folderPath}\".");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Folder creation did not succeed" +
                $" due to exception \"{ex.Message}\". Exiting program.");

            Environment.Exit((int)ExitCode.FolderCreationError);
        }
    }

    private static void ResponseDoNotCreateFolder()
    {
        Console.WriteLine($"Folder will not be created. Exiting program.");

        Environment.Exit((int)ExitCode.Success);
    }

    private static void ResponseTryCreateLogFile(string logFilePath)
    {
        if (string.IsNullOrWhiteSpace(logFilePath))
        {
            throw new ArgumentException($"'{nameof(logFilePath)}'" +
                $" cannot be null or whitespace.", nameof(logFilePath));
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)
                ?? throw new ArgumentException($"Could not get directory name from" +
                $" \"{logFilePath}\".", nameof(logFilePath)));

            File.Create(logFilePath);

            Console.WriteLine($"Successfully created log file \"{logFilePath}\".");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Log file creation did not succeed" +
                $" due to exception \"{ex.Message}\". Exiting program.");

            Environment.Exit((int)ExitCode.LogFileCreationError);
        }
    }

    private static void ResponseDoNotCreateLogFile()
    {
        Console.WriteLine($"Log file will not be created. Exiting program.");

        Environment.Exit((int)ExitCode.Success);
    }
}