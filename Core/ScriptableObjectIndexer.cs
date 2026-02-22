using UnityEngine;

public static class ScriptableObjectIndexer
{
    private static ScriptableObjectIndexerAsset _indexer;
    public static ScriptableObjectIndexerAsset Indexer //todo jak bedzie full wrsja to mozna usunac i dac zwykly getter bez ifa
    {
        get
        {
            if (_indexer == null)
                Initialization();
            return _indexer;
        }
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialization()
    {
        _indexer = Resources.Load<ScriptableObjectIndexerAsset>(ScriptableObjectIndexerAsset.FileName);
    }
    public static T GetObject<T>(int id) where T : AdvancedScriptableObject => Indexer.GetObject<T>(id);
    public static int GetIdOfObject<T>(T obj) where T : AdvancedScriptableObject => Indexer.GetIdOfObject(obj);
}
