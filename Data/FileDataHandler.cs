using UnityEngine;
using System.IO;
using System;

public class FileDataHandler<T>
{
    protected string dataPath;
    protected string dataFileName;
    public FileDataHandler(string path, string fileName)
    {
        dataPath = path;
        dataFileName = fileName;
    }
    public virtual T Load()
    {
        string fullPath = Path.Combine(dataPath, dataFileName);
        T loadedData = default;

        if (!File.Exists(fullPath))
            return loadedData;

        try
        {
            string dataToLoad = File.ReadAllText(fullPath);
            if (string.IsNullOrWhiteSpace(dataToLoad))
                return loadedData;

            loadedData = JsonUtility.FromJson<T>(dataToLoad);
            if (loadedData == null)
                Debug.LogWarning($"Snapshot file {fullPath} deserialized to null.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not load game data from {fullPath}: {e.Message}");
        }

        return loadedData;
    }
    public virtual void Save(T data)
    {
        string fullPath = Path.Combine(dataPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string tempPath = fullPath + ".tmp";
            string dataToStore = JsonUtility.ToJson(data, true);

            File.WriteAllText(tempPath, dataToStore);
            if (File.Exists(fullPath))
                File.Replace(tempPath, fullPath, fullPath + ".bak");
            else
                File.Move(tempPath, fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data to file: {fullPath}. {e.Message}");
        }
    }
    public virtual void Delete()
    {
        string fullPath = Path.Combine(dataPath, dataFileName);
        try
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            string backupPath = fullPath + ".bak";
            if (File.Exists(backupPath))
                File.Delete(backupPath);

            string tempPath = fullPath + ".tmp";
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete data file: {fullPath}. {e.Message}");
        }
    }
}
