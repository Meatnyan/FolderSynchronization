using Domain.Models;
using Domain.Models.Constants;

namespace Tests.Domain.Models;

public class ValidatorTests
{
    private Validator CreateValidator()
    {
        return new Validator();
    }

    [Fact]
    public void ValidateArgumentCount_CorrectArgumentCount_ShouldReturnSuccess()
    {
        // Arrange
        var validator = CreateValidator();
        string[] args = new string[ValidationConstants.CORRECT_PROGRAM_ARGUMENT_COUNT];

        // Act
        var result = validator.ValidateArgumentCount(
            args);

        // Assert
        Assert.True(result == ValidatorStatusCode.Success);
    }

    [Fact]
    public void ValidateArgumentCount_TooFewArguments_ShouldReturnArgumentCountError()
    {
        // Arrange
        var validator = CreateValidator();
        string[] args = new string[ValidationConstants.CORRECT_PROGRAM_ARGUMENT_COUNT - 1];

        // Act
        var result = validator.ValidateArgumentCount(
            args);

        // Assert
        Assert.True(result == ValidatorStatusCode.ArgumentCountError);
    }

    [Fact]
    public void ValidateArgumentCount_TooManyArguments_ShouldReturnArgumentCountError()
    {
        // Arrange
        var validator = CreateValidator();
        string[] args = new string[ValidationConstants.CORRECT_PROGRAM_ARGUMENT_COUNT + 1];

        // Act
        var result = validator.ValidateArgumentCount(
            args);

        // Assert
        Assert.True(result == ValidatorStatusCode.ArgumentCountError);
    }

    [Fact]
    public void ValidateArgumentCount_NullArguments_ShouldReturnArgumentCountError()
    {
        // Arrange
        var validator = CreateValidator();
        string[] args = null;

        // Act
        var result = validator.ValidateArgumentCount(
            args);

        // Assert
        Assert.True(result == ValidatorStatusCode.ArgumentCountError);
    }

    [Fact]
    public void ValidateFolderPath_PathToSystemDrive_ShouldReturnSuccess()
    {
        // Arrange
        var validator = CreateValidator();
        string folderPath = $"{Environment.GetEnvironmentVariable("SYSTEMDRIVE")}";

        // Act
        var result = validator.ValidateFolderPath(
            folderPath);

        // Assert
        Assert.True(result == ValidatorStatusCode.Success);
    }

    [Fact]
    public void ValidateFolderPath_EmptyStringPath_ShouldReturnFolderPathDoesNotExist()
    {
        // Arrange
        var validator = CreateValidator();
        string folderPath = string.Empty;

        // Act
        var result = validator.ValidateFolderPath(
            folderPath);

        // Assert
        Assert.True(result == ValidatorStatusCode.FolderPathDoesNotExist);
    }

    [Fact]
    public void ValidateFolderPath_NullStringPath_ShouldReturnFolderPathDoesNotExist()
    {
        // Arrange
        var validator = CreateValidator();
        string folderPath = null;

        // Act
        var result = validator.ValidateFolderPath(
            folderPath);

        // Assert
        Assert.True(result == ValidatorStatusCode.FolderPathDoesNotExist);
    }

    [Fact]
    public void ValidateSynchronizationInterval_IntervalEqualToMinSyncInterval_ShouldReturnSuccess()
    {
        // Arrange
        var validator = CreateValidator();
        string interval = $"{ValidationConstants.MIN_SYNCHRONIZATION_INTERVAL}";

        // Act
        var result = validator.ValidateSynchronizationInterval(
            interval);

        // Assert
        Assert.True(result == ValidatorStatusCode.Success);
    }

    [Fact]
    public void ValidateSynchronizationInterval_IntervalLargerThanMinSyncInterval_ShouldReturnSuccess()
    {
        // Arrange
        var validator = CreateValidator();
        string interval = $"{ValidationConstants.MIN_SYNCHRONIZATION_INTERVAL + 1}";

        // Act
        var result = validator.ValidateSynchronizationInterval(
            interval);

        // Assert
        Assert.True(result == ValidatorStatusCode.Success);
    }

    [Fact]
    public void ValidateSynchronizationInterval_IntervalSmallerThanMinSyncInterval_ShouldReturnSyncIntervalTooShortError()
    {
        // Arrange
        var validator = CreateValidator();
        string interval = $"{ValidationConstants.MIN_SYNCHRONIZATION_INTERVAL - 1}";

        // Act
        var result = validator.ValidateSynchronizationInterval(
            interval);

        // Assert
        Assert.True(result == ValidatorStatusCode.SyncIntervalTooShortError);
    }

    [Fact]
    public void ValidateSynchronizationInterval_NullStringInterval_ShouldReturnSyncIntervalParsingError()
    {
        // Arrange
        var validator = CreateValidator();
        string interval = null;

        // Act
        var result = validator.ValidateSynchronizationInterval(
            interval);

        // Assert
        Assert.True(result == ValidatorStatusCode.SyncIntervalParsingError);
    }

    [Fact]
    public void ValidateSynchronizationInterval_EmptyStringInterval_ShouldReturnSyncIntervalParsingError()
    {
        // Arrange
        var validator = CreateValidator();
        string interval = string.Empty;

        // Act
        var result = validator.ValidateSynchronizationInterval(
            interval);

        // Assert
        Assert.True(result == ValidatorStatusCode.SyncIntervalParsingError);
    }

    [Fact]
    public void ValidateSynchronizationInterval_NonNumberStringInterval_ShouldReturnSyncIntervalParsingError()
    {
        // Arrange
        var validator = CreateValidator();
        string interval = "a";

        // Act
        var result = validator.ValidateSynchronizationInterval(
            interval);

        // Assert
        Assert.True(result == ValidatorStatusCode.SyncIntervalParsingError);
    }

    [Fact]
    public void ValidateSynchronizationInterval_NonIntegerNumberStringInterval_ShouldReturnSyncIntervalParsingError()
    {
        // Arrange
        var validator = CreateValidator();
        string interval = "10.5";

        // Act
        var result = validator.ValidateSynchronizationInterval(
            interval);

        // Assert
        Assert.True(result == ValidatorStatusCode.SyncIntervalParsingError);
    }

    [Fact]
    public void ValidateFilePath_SystemRootExplorerDotExe_ShouldReturnSuccess()
    {
        // Arrange
        var validator = CreateValidator();
        string filePath = $"{Environment.GetEnvironmentVariable("SYSTEMROOT")}\\explorer.exe";

        // Act
        var result = validator.ValidateFilePath(
            filePath);

        // Assert
        Assert.True(result == ValidatorStatusCode.Success);
    }

    [Fact]
    public void ValidateFilePath_NullStringPath_ShouldReturnFilePathDoesNotExist()
    {
        // Arrange
        var validator = CreateValidator();
        string filePath = null;

        // Act
        var result = validator.ValidateFilePath(
            filePath);

        // Assert
        Assert.True(result == ValidatorStatusCode.FilePathDoesNotExist);
    }

    [Fact]
    public void ValidateFilePath_EmptyStringPath_ShouldReturnFilePathDoesNotExist()
    {
        // Arrange
        var validator = CreateValidator();
        string filePath = string.Empty;

        // Act
        var result = validator.ValidateFilePath(
            filePath);

        // Assert
        Assert.True(result == ValidatorStatusCode.FilePathDoesNotExist);
    }
}