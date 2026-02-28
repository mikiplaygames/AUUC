using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
namespace MikiHeadDev.Core.Data
{
public class GameDataHandler : FileDataHandler<GameData>{public GameDataHandler() : base(Application.persistentDataPath, "gameData.json"){}}
public class GameDataManager
{
    private static GameData gameData;
    private static GameDataHandler dataHandler;
    private static IEnumerable<IDataPersistence<GameData>> sceneDataPersistentObjects;
    public readonly static List<IDataPersistence<GameData>> offSceneDataPersistentObjects = new();
    
    public static UnityEvent<GameData> OnDataLoaded = new();
    public static UnityEvent<GameData> OnDataSaved = new();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        dataHandler = new GameDataHandler();
        SceneManager.sceneLoaded += SceneLoaded;
    }
    private static void SceneLoaded(Scene sc, LoadSceneMode loadSceneMode)
    {
        LoadGame();
    }
    public static void NewGame()
    {
        gameData = new GameData();
        dataHandler.Save(gameData);
    }
    public static void LoadGame()
    {
        RefreshSceneDataPersistenceObjects();
        
        gameData = dataHandler.Load();
        
        if (gameData == null)
            NewGame();
        
        var objectsToSave = sceneDataPersistentObjects.Concat(offSceneDataPersistentObjects);
        foreach (IDataPersistence<GameData> dataPersistenceObject in objectsToSave)
        {
            dataPersistenceObject.LoadData(gameData);
        }
        OnDataLoaded?.Invoke(gameData);
    }
    public static GameData RetrieveSavedGameData()
    {
        gameData ??= dataHandler.Load();
        return gameData;
    }
    public static GameData RetrieveCurrentGameData()
    {
        RefreshSceneDataPersistenceObjects();

        gameData = new();

        var objectsToSave = sceneDataPersistentObjects.Concat(offSceneDataPersistentObjects);
        foreach (IDataPersistence<GameData> dataPersistenceObject in objectsToSave)
        {
            dataPersistenceObject.SaveData(gameData);
        }
        return gameData;
    }
    public static void AlterSavedGameData(GameData gameData)
    {
        dataHandler.Save(gameData);
    }
    public static void SaveGame()
    {
        RefreshSceneDataPersistenceObjects();

        gameData = new();

        var objectsToSave = sceneDataPersistentObjects.Concat(offSceneDataPersistentObjects);
        foreach (IDataPersistence<GameData> dataPersistenceObject in objectsToSave)
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
        sceneDataPersistentObjects = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<IDataPersistence<GameData>>(true));
    }
}}