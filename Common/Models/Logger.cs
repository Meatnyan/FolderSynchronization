using Common.Extensions;
using Common.Interfaces;
using static Domain.Models.Constants.TextConstants;

namespace Domain.Models;

public class Logger : ILogging
{
    public Logger(string logFilePath)
    {
        if (string.IsNullOrWhiteSpace(logFilePath))
        {
            throw new ArgumentException($"'{nameof(logFilePath)}'" +
                $" cannot be null or whitespace.", nameof(logFilePath));
        }

        _logFilePath = logFilePath;
    }


    private readonly string _logFilePath;


    public void LogFileAddedToSource(string fileName, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}'" +
                $" cannot be null or whitespace.", nameof(fileName));
        }


        LogToConsoleAndFile($"File added to source folder: {fileName}", performedBySynchronizer);
    }

    public void LogFileRenamedInSource(string originalFileName, string newFileName, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new ArgumentException($"'{nameof(originalFileName)}'" +
                $" cannot be null or whitespace.", nameof(originalFileName));
        }
        if (string.IsNullOrWhiteSpace(newFileName))
        {
            throw new ArgumentException($"'{nameof(newFileName)}'" +
                $" cannot be null or whitespace.", nameof(newFileName));
        }


        LogToConsoleAndFile($"File renamed in source folder: {originalFileName} -> {newFileName}",
            performedBySynchronizer);
    }

    public void LogFileModifiedInSource(string fileName, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}'" +
                $" cannot be null or whitespace.", nameof(fileName));
        }


        LogToConsoleAndFile($"File modified in source folder: {fileName}", performedBySynchronizer);
    }

    public void LogFileDeletedFromSource(string fileName, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}'" +
                $" cannot be null or whitespace.", nameof(fileName));
        }


        LogToConsoleAndFile($"File deleted from source folder: {fileName}", performedBySynchronizer);
    }

    public void LogFileAddedToReplica(string fileName, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}'" +
                $" cannot be null or whitespace.", nameof(fileName));
        }


        LogToConsoleAndFile($"File added to replica folder: {fileName}", performedBySynchronizer);
    }

    public void LogFileRenamedInReplica(string originalFileName, string newFileName, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new ArgumentException($"'{nameof(originalFileName)}'" +
                $" cannot be null or whitespace.", nameof(originalFileName));
        }
        if (string.IsNullOrWhiteSpace(newFileName))
        {
            throw new ArgumentException($"'{nameof(newFileName)}'" +
                $" cannot be null or whitespace.", nameof(newFileName));
        }


        LogToConsoleAndFile($"File renamed in replica folder: {originalFileName} -> {newFileName}"
            , performedBySynchronizer);
    }

    public void LogFileModifiedInReplica(string fileName, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}'" +
                $" cannot be null or whitespace.", nameof(fileName));
        }


        LogToConsoleAndFile($"File modified in replica folder: {fileName}", performedBySynchronizer);
    }

    public void LogFileDeletedFromReplica(string fileName, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}'" +
                $" cannot be null or whitespace.", nameof(fileName));
        }


        LogToConsoleAndFile($"File deleted from replica folder: {fileName}", performedBySynchronizer);
    }

    public void LogFileOperationError(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException($"'{nameof(errorMessage)}'" +
                $" cannot be null or whitespace.", nameof(errorMessage));
        }


        LogToConsoleAndFile($"{ERROR_INDICATOR}File operation error: \"{errorMessage}\"", performedBySynchronizer: true);
    }

    private void LogToConsoleAndFile(string message, bool performedBySynchronizer)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException($"'{nameof(message)}'" +
                $" cannot be null or whitespace.", nameof(message));
        }

        message = message.FormatAsLog(performedBySynchronizer);

        Console.WriteLine(message);

        try
        {
            using (StreamWriter writer = File.AppendText(_logFilePath))
            {
                writer.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ERROR_INDICATOR}Error while attempting to write to log file: \"{ex.Message}\""
                .FormatAsLog(performedBySynchronizer: true));
        }
    }
}