using Domain.Models;

namespace Domain.Interfaces;

public interface IValidating
{
    public ValidatorStatusCode ValidateArgumentCount(string[] args);
    public ValidatorStatusCode ValidateFilePath(string filePath);
    public ValidatorStatusCode ValidateFolderPath(string folderPath);
    public ValidatorStatusCode ValidateSynchronizationInterval(string interval);
}