using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
public static class ConfigManager
{
    static FileDataHandler<ConfigData> dataHandler;
    public static event System.Action OnConfigLoaded;
    public static bool ConfigLoaded => _configData != null;
    static bool ShouldFallback => ConfigLoaded && _configData.UseHardcoded;
    static readonly string FileName = $"{ConfigData.GAME_NAME}_config.json";
    #if UNITY_ANDROID
    static readonly string FilePath = System.IO.Path.Combine(Application.persistentDataPath, "Config");
    #else
    static readonly string FilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Config");
    #endif
    public static ConfigData ConfigData {
        get
        {
            if (_configData == null)
                LoadConfigData();
            return _configData;
        }
    }
    private static ConfigData _configData;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_configData == null)
            LoadConfigData();
    }
    public static async void LoadConfigData()
    {
        dataHandler ??= new(FilePath, FileName);
        _configData = dataHandler.Load();

        if ((!ConfigLoaded || _configData.UseOnlineConfig) && Application.isPlaying)
            await TryLoadOnlineConfig(ShouldFallback);
        else if (!_configData.UseHardcoded)
            return;
        _configData = new();
        dataHandler.Save(_configData);
        OnConfigLoaded?.Invoke();
    }
    static async Task TryLoadOnlineConfig(bool useDefaultIfNotFetch)
    {
        var fetcher = new OnlineJsonFetcher<ConfigData>();
        var onlineData = await fetcher.FetchJson();
        if (onlineData == null || onlineData.UseHardcoded)
        {
            if (useDefaultIfNotFetch)
                return;
            _configData = new();
            dataHandler.Save(_configData);
            OnConfigLoaded?.Invoke();
            return;
        }
        _configData = onlineData;
        dataHandler.Save(_configData);
        OnConfigLoaded?.Invoke();
    }
    public static void DeleteConfigData()
    {
        dataHandler ??= new(FilePath, FileName);
        _configData = new();
        dataHandler.Save(_configData);
    }   
    public async static void RefetchOnlineConfig()
    {
        if (Application.isPlaying)
            await TryLoadOnlineConfig(true);
    }
}
public class OnlineJsonFetcher<T> where T : class
{
    public async Task<T> FetchJson()
    {
        UnityWebRequest request = UnityWebRequest.Get(ConfigData.ONLINE_CONFIG_URL);
        request.timeout = 10;
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            return JsonUtility.FromJson<T>(json);
        }
        else
            Debug.LogWarning($"Failed to fetch online config: {request.error}");
        return null;
    }
}