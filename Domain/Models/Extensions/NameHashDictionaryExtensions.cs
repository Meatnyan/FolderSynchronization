namespace Domain.Models.Extensions;

public static class NameHashDictionaryExtensions
{
    public static bool ContainsHash(this Dictionary<string, byte[]> nameHashDictionary, byte[] hash)
        => nameHashDictionary.Any(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(hash));

    public static bool ContainsName(this Dictionary<string, byte[]> nameHashDictionary, string name)
        => nameHashDictionary.ContainsKey(name);

    public static bool ContainsNameHashPair(this Dictionary<string, byte[]> nameHashDictionary,
        KeyValuePair<string, byte[]> nameHashPair)
            => nameHashDictionary.ContainsName(nameHashPair.Key) && nameHashDictionary.ContainsHash(nameHashPair.Value);

    public static string GetNameFromHash(this Dictionary<string, byte[]> nameHashDictionary, byte[] hash)
        => nameHashDictionary.First(innerNameHashPair => innerNameHashPair.Value.SequenceEqual(hash)).Key;

    public static Dictionary<string, byte[]> ExceptDuplicateNameHashPairs(
        this Dictionary<string, byte[]> nameHashDictionary, Dictionary<string, byte[]> nameHashDictionaryToCompare)
            => nameHashDictionary.Where(nameHashPair => !nameHashDictionaryToCompare.ContainsNameHashPair(nameHashPair))
                .ToDictionary();

    public static bool DictionaryEquals(this Dictionary<string, byte[]> firstDictionary,
        Dictionary<string, byte[]> secondDictionary)
    {
        if (firstDictionary is null)
        {
            return secondDictionary is null;
        }
        else if (secondDictionary is null)
        {
            return false;
        }

        if (firstDictionary.Count != secondDictionary.Count)
        {
            return false;
        }

        if (firstDictionary.Count == 0)
        {
            return true;
        }

        foreach (var nameHashPair in firstDictionary)
        {
            if (!secondDictionary.ContainsName(nameHashPair.Key))
            {
                return false;
            }
            else
            {
                if (!nameHashPair.Value.SequenceEqual(secondDictionary[nameHashPair.Key]))
                {
                    return false;
                }
            }
        }

        return true;
    }
}