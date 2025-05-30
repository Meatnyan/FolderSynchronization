using Common.Interfaces;
using Domain.Interfaces;
using Domain.Models.Extensions;
using System.Security.Cryptography;
using static Domain.Models.Extensions.NameHashDictionaryExtensions;

namespace Domain.Models;

public class Synchronizer : ISynchronizing
{
    public Synchronizer(string sourceFolderPath, string replicaFolderPath, int syncInterval, ILogging logger)
    {
        if (string.IsNullOrWhiteSpace(sourceFolderPath))
        {
            throw new ArgumentException($"'{nameof(sourceFolderPath)}'" +
                $" cannot be null or whitespace.", nameof(sourceFolderPath));
        }
        if (string.IsNullOrWhiteSpace(replicaFolderPath))
        {
            throw new ArgumentException($"'{nameof(replicaFolderPath)}'" +
                $" cannot be null or whitespace.", nameof(replicaFolderPath));
        }
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }


        _sourceFolderPath = sourceFolderPath;
        _replicaFolderPath = replicaFolderPath;
        _synchronizationInterval = syncInterval;
        _logger = logger;
    }


    private readonly string _sourceFolderPath;
    private readonly string _replicaFolderPath;
    private readonly int _synchronizationInterval;
    private readonly ILogging _logger;

    private Dictionary<string, byte[]> _previousSourceNameHashDict = [];
    private Dictionary<string, byte[]> _previousReplicaNameHashDict = [];


    public void BeginSynchronization()
    {
        // synchronization only stops when the program is terminated
        while (true)
        {
            Dictionary<string, byte[]> sourceNameHashDict = GetFolderNameHashDictionary(_sourceFolderPath);
            Dictionary<string, byte[]> replicaNameHashDict = GetFolderNameHashDictionary(_replicaFolderPath);

            if (!sourceNameHashDict.DictionaryEquals(_previousSourceNameHashDict))
            {
                LogSourceFilesChangeFromExternalMeans(sourceNameHashDict);
            }

            if (!replicaNameHashDict.DictionaryEquals(_previousReplicaNameHashDict))
            {
                LogReplicaFilesChangeFromExternalMeans(replicaNameHashDict);
            }

            if (!sourceNameHashDict.DictionaryEquals(replicaNameHashDict))
            {
                DeleteOrRenameExcessReplicaFiles();

                CopyOrRenameFromExcessSourceFiles();
            }

            _previousSourceNameHashDict = GetFolderNameHashDictionary(_sourceFolderPath);
            _previousReplicaNameHashDict = GetFolderNameHashDictionary(_replicaFolderPath);

            Thread.Sleep(_synchronizationInterval);
        }
    }

    private Dictionary<string, byte[]> GetFolderNameHashDictionary(string folderPath)
    {
        FileInfo[] fileInfos = new DirectoryInfo(folderPath).GetFiles();

        Dictionary<string, byte[]> hashes = [];

        using (SHA256 sha256 = SHA256.Create())
        {
            foreach (var fileInfo in fileInfos)
            {
                using (FileStream fileStream = fileInfo.Open(FileMode.Open))
                {
                    try
                    {
                        fileStream.Position = 0;

                        byte[] hashValue = sha256.ComputeHash(fileStream);

                        hashes.Add(fileInfo.Name, hashValue);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogFileOperationError(ex.Message);
                    }
                }
            }
        }

        return hashes;
    }

    private void DeleteOrRenameExcessReplicaFiles()
    {
        Dictionary<string, byte[]> sourceNameHashDict = GetFolderNameHashDictionary(_sourceFolderPath);
        Dictionary<string, byte[]> replicaNameHashDict = GetFolderNameHashDictionary(_replicaFolderPath);

        Dictionary<string, byte[]> excessHashesInReplicaFolder
            = replicaNameHashDict.ExceptDuplicateNameHashPairs(sourceNameHashDict);

        foreach (var nameHashPair in excessHashesInReplicaFolder)
        {
            bool amountOfThisHashInReplicaIsGreaterThanInSource
                = GetFolderNameHashDictionary(_replicaFolderPath)
                .Count(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(nameHashPair.Value))
                > GetFolderNameHashDictionary(_sourceFolderPath)
                .Count(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(nameHashPair.Value));

            if (!amountOfThisHashInReplicaIsGreaterThanInSource && sourceNameHashDict.ContainsHash(nameHashPair.Value))
            {
                // replica file got renamed, should rename replica file back
                string oldFileName = sourceNameHashDict.GetNameFromHash(nameHashPair.Value);

                File.Move($"{_replicaFolderPath}\\{nameHashPair.Key}",
                    $"{_replicaFolderPath}\\{oldFileName}", overwrite: true);

                _logger.LogFileRenamedInReplica(nameHashPair.Key, oldFileName, performedBySynchronizer: true);
            }
            else
            {
                // just delete the file if either the hash or name + hash changed
                File.Delete($"{_replicaFolderPath}\\{nameHashPair.Key}");

                _logger.LogFileDeletedFromReplica(nameHashPair.Key, performedBySynchronizer: true);
            }
        }
    }

    private void CopyOrRenameFromExcessSourceFiles()
    {
        Dictionary<string, byte[]> sourceNameHashDict = GetFolderNameHashDictionary(_sourceFolderPath);
        Dictionary<string, byte[]> replicaNameHashDict = GetFolderNameHashDictionary(_replicaFolderPath);

        Dictionary<string, byte[]> excessInSourceNameHashDict
            = sourceNameHashDict.ExceptDuplicateNameHashPairs(replicaNameHashDict);

        foreach (var nameHashPair in excessInSourceNameHashDict)
        {
            bool amountOfThisHashInSourceIsGreaterThanInReplica
                = sourceNameHashDict
                .Count(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(nameHashPair.Value))
                > GetFolderNameHashDictionary(_replicaFolderPath)
                .Count(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(nameHashPair.Value));

            if (replicaNameHashDict.ContainsHash(nameHashPair.Value))
            {
                // source file got renamed, should rename replica file
                string oldFileName = replicaNameHashDict.GetNameFromHash(nameHashPair.Value);

                // handle edge case where file got deleted then recreated with a different name
                if (!amountOfThisHashInSourceIsGreaterThanInReplica && File.Exists($"{_replicaFolderPath}\\{oldFileName}"))
                {
                    File.Move($"{_replicaFolderPath}\\{oldFileName}",
                        $"{_replicaFolderPath}\\{nameHashPair.Key}", overwrite: true);

                    _logger.LogFileRenamedInReplica(oldFileName, nameHashPair.Key, performedBySynchronizer: true);
                }
                else
                {
                    File.Copy($"{_sourceFolderPath}\\{nameHashPair.Key}",
                        $"{_replicaFolderPath}\\{nameHashPair.Key}", overwrite: true);

                    _logger.LogFileAddedToReplica(nameHashPair.Key, performedBySynchronizer: true);
                }
            }
            else
            {
                // just copy the file if either the hash or name + hash changed
                File.Copy($"{_sourceFolderPath}\\{nameHashPair.Key}",
                    $"{_replicaFolderPath}\\{nameHashPair.Key}", overwrite: true);

                _logger.LogFileAddedToReplica(nameHashPair.Key, performedBySynchronizer: true);
            }
        }
    }

    private void LogSourceFilesChangeFromExternalMeans(Dictionary<string, byte[]> sourceNameHashDict)
    {
        Dictionary<string, byte[]> excessInNewSourceNameHashDict
            = sourceNameHashDict.ExceptDuplicateNameHashPairs(_previousSourceNameHashDict);

        foreach (var nameHashPair in excessInNewSourceNameHashDict)
        {
            bool amountOfThisHashInSourceNowIsGreaterThanPreviously
                = sourceNameHashDict
                .Count(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(nameHashPair.Value))
                > _previousReplicaNameHashDict
                .Count(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(nameHashPair.Value));

            // log file renaming, modification and creation
            if (!amountOfThisHashInSourceNowIsGreaterThanPreviously)
            {
                if (_previousSourceNameHashDict.ContainsHash(nameHashPair.Value))
                {
                    // file got renamed
                    string oldFileName = _previousSourceNameHashDict.GetNameFromHash(nameHashPair.Value);

                    _logger.LogFileRenamedInSource(oldFileName, nameHashPair.Key, performedBySynchronizer: false);
                }
                else if (_previousSourceNameHashDict.ContainsName(nameHashPair.Key))
                {
                    // file got modified
                    _logger.LogFileModifiedInSource(nameHashPair.Key, performedBySynchronizer: false);
                }
                else
                {
                    _logger.LogFileAddedToSource(nameHashPair.Key, performedBySynchronizer: false);
                }
            }
            else
            {
                _logger.LogFileAddedToSource(nameHashPair.Key, performedBySynchronizer: false);
            }
        }

        Dictionary<string, byte[]> excessInPreviousSourceHashDict
            = _previousSourceNameHashDict.ExceptDuplicateNameHashPairs(sourceNameHashDict);

        foreach (var nameHashPair in excessInPreviousSourceHashDict)
        {
            // log file deletions
            if (!excessInNewSourceNameHashDict.ContainsName(nameHashPair.Key)
                && !excessInNewSourceNameHashDict.ContainsHash(nameHashPair.Value))
            {
                _logger.LogFileDeletedFromSource(nameHashPair.Key, performedBySynchronizer: false);
            }
        }
    }

    private void LogReplicaFilesChangeFromExternalMeans(Dictionary<string, byte[]> replicaNameHashDict)
    {
        Dictionary<string, byte[]> excessInNewReplicaNameHashDict
            = replicaNameHashDict.ExceptDuplicateNameHashPairs(_previousReplicaNameHashDict);

        foreach (var nameHashPair in excessInNewReplicaNameHashDict)
        {
            bool amountOfThisHashInReplicaNowIsGreaterThanPreviously
                = replicaNameHashDict
                .Count(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(nameHashPair.Value))
                > _previousReplicaNameHashDict
                .Count(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(nameHashPair.Value));

            // log file renaming, modification and creation
            if (!amountOfThisHashInReplicaNowIsGreaterThanPreviously)
            {
                if (_previousReplicaNameHashDict.ContainsHash(nameHashPair.Value))
                {
                    // file got renamed
                    string oldFileName = _previousReplicaNameHashDict.GetNameFromHash(nameHashPair.Value);

                    _logger.LogFileRenamedInReplica(oldFileName, nameHashPair.Key, performedBySynchronizer: false);
                }
                else if (_previousReplicaNameHashDict.ContainsName(nameHashPair.Key))
                {
                    // file got modified
                    _logger.LogFileModifiedInReplica(nameHashPair.Key, performedBySynchronizer: false);
                }
                else
                {
                    _logger.LogFileAddedToReplica(nameHashPair.Key, performedBySynchronizer: false);
                }
            }
            else
            {
                _logger.LogFileAddedToReplica(nameHashPair.Key, performedBySynchronizer: false);
            }
        }

        Dictionary<string, byte[]> excessInPreviousReplicaNameHashDict
            = _previousReplicaNameHashDict.ExceptDuplicateNameHashPairs(replicaNameHashDict);

        foreach (var nameHashPair in excessInPreviousReplicaNameHashDict)
        {
            // log file deletions
            if (!excessInNewReplicaNameHashDict.ContainsName(nameHashPair.Key)
                && !excessInNewReplicaNameHashDict.ContainsHash(nameHashPair.Value))
            {
                _logger.LogFileDeletedFromReplica(nameHashPair.Key, performedBySynchronizer: false);
            }
        }
    }
}