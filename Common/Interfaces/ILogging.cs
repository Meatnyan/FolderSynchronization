namespace Common.Interfaces;

public interface ILogging
{
    public void LogFileAddedToReplica(string fileName, bool performedBySynchronizer);
    public void LogFileAddedToSource(string fileName, bool performedBySynchronizer);
    public void LogFileDeletedFromReplica(string fileName, bool performedBySynchronizer);
    public void LogFileDeletedFromSource(string fileName, bool performedBySynchronizer);
    public void LogFileModifiedInReplica(string fileName, bool performedBySynchronizer);
    public void LogFileModifiedInSource(string fileName, bool performedBySynchronizer);
    public void LogFileOperationError(string errorMessage);
    public void LogFileRenamedInReplica(string originalFileName, string newFileName, bool performedBySynchronizer);
    public void LogFileRenamedInSource(string originalFileName, string newFileName, bool performedBySynchronizer);
}