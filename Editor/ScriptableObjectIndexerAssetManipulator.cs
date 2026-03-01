using UnityEditor;

namespace MikiHeadDev.Helpers.Editor
{
    public class ScriptableObjectIndexerAssetManipulator : EditorWindow
    {
        [MenuItem("SO Indexer/Check For Nulls", priority = 1)]
        private static void CheckForNulls()
        {
            ScriptableObjectIndexerAssetCustomEditor.CheckForNulls(ScriptableObjectIndexer.Indexer);   
        }
        [MenuItem("SO Indexer/Check For UnId-ied Objects", priority = 2)]
        private static void CheckForUnIdiedObjects()
        {
            ScriptableObjectIndexerAssetCustomEditor.CheckForUnIdObjects(ScriptableObjectIndexer.Indexer);   
        }
        [MenuItem("SO Indexer/Check For Unlisted Objects", priority = 3)]
        private static void CheckForUnlistedObjects()
        {
            ScriptableObjectIndexerAssetCustomEditor.CheckForUnlistedObjects(ScriptableObjectIndexer.Indexer);   
        }
        [MenuItem("SO Indexer/Check For Same-ID Objects", priority = 4)]
        private static void CheckForSameIdObjects()
        {
            ScriptableObjectIndexerAssetCustomEditor.CheckForSameIdObjects(ScriptableObjectIndexer.Indexer);
        }
        [MenuItem("SO Indexer/Warning Zone/Update Types Base IDs", priority = 5)]
        private static void UpdateTypesBaseIDs_SOFT()
        {
            ScriptableObjectIndexerAssetCustomEditor.UpdateTypesBaseIDs_SOFT(ScriptableObjectIndexer.Indexer);
        }
        [MenuItem("SO Indexer/Warning Zone/Regenerate Main Dict IDs", priority = 6)]
        private static void RegenerateMainDict_SOFT()
        {
            ScriptableObjectIndexerAssetCustomEditor.RegenerateMainDict_SOFT(ScriptableObjectIndexer.Indexer);
        }
        [MenuItem("SO Indexer/Danger Zone/Regenerate Main Dict IDs", priority = 7)]
        private static void RegenerateMainDict_HARD()
        {
            ScriptableObjectIndexerAssetCustomEditor.RegenerateMainDict_HARD(ScriptableObjectIndexer.Indexer);
        }
    }
}