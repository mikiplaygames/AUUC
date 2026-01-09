using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class GameDataManager<T> where T : new()
{
    private static T gameData;
    private static FileDataHandler<T> dataHandler;
    private static IEnumerable<IDataPersistence<T>> sceneDataPersistentObjects;
    public readonly static List<IDataPersistence<T>> offSceneDataPersistentObjects = new();
    
    public static UnityEvent<T> OnDataLoaded = new();
    public static UnityEvent<T> OnDataSaved = new();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        dataHandler = new FileDataHandler<T>(Application.persistentDataPath, "gameData.json");
        SceneManager.sceneLoaded += SceneLoaded;
    }
    private static void SceneLoaded(Scene sc, LoadSceneMode loadSceneMode)
    {
        LoadGame();
    }
    public static void NewGame()
    {
        gameData = new T();
        dataHandler.Save(gameData);
    }
    public static void LoadGame()
    {
        RefreshSceneDataPersistenceObjects();
        
        gameData = dataHandler.Load();
        
        if (gameData == null)
            NewGame();
        
        var objectsToSave = sceneDataPersistentObjects.Concat(offSceneDataPersistentObjects);
        foreach (IDataPersistence<T> dataPersistenceObject in objectsToSave)
        {
            dataPersistenceObject.LoadData(gameData);
        }
        OnDataLoaded?.Invoke(gameData);
    }
    public static T RetrieveSavedGameData()
    {
        gameData ??= dataHandler.Load();
        return gameData;
    }
    public static T RetrieveCurrentGameData()
    {
        RefreshSceneDataPersistenceObjects();

        gameData = new();

        var objectsToSave = sceneDataPersistentObjects.Concat(offSceneDataPersistentObjects);
        foreach (IDataPersistence<T> dataPersistenceObject in objectsToSave)
        {
            dataPersistenceObject.SaveData(gameData);
        }
        return gameData;
    }
    public static void AlterSavedGameData(T gameData)
    {
        dataHandler.Save(gameData);
    }
    public static void SaveGame()
    {
        RefreshSceneDataPersistenceObjects();

        gameData = new();

        var objectsToSave = sceneDataPersistentObjects.Concat(offSceneDataPersistentObjects);
        foreach (IDataPersistence<T> dataPersistenceObject in objectsToSave)
        {
            dataPersistenceObject.SaveData(gameData);
        }
        
        // gameData.lastPlayTime = DateTime.Now.ToBinary();
        // gameData.gameVersion = Application.version;

        dataHandler.Save(gameData);
        OnDataSaved?.Invoke(gameData);
    }
    private static void RefreshSceneDataPersistenceObjects()
    {
        sceneDataPersistentObjects = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<IDataPersistence<T>>(true));
    }
}