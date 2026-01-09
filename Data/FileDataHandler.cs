using UnityEngine;
using System.IO;
using System;

public class FileDataHandler<T> {
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

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new(fullPath, FileMode.Open))
                {
                    using StreamReader reader = new(stream);
                    dataToLoad = reader.ReadToEnd();
                }
                loadedData = JsonUtility.FromJson<T>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Could not load game data: " + e.Message);
            }
        }

        return loadedData;
    }
    public virtual void Save(T data)
    {
        string fullPath = Path.Combine(dataPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using FileStream stream = new(fullPath, FileMode.Create);
            using StreamWriter writer = new(stream);
            writer.Write(dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load data from file: " + fullPath + e.Message);
        }
    }
}
