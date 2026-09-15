using UnityEditor;

public static class ReserializeAll
{
    [MenuItem("Tools/Reserialize All Assets")]
    public static void Run()
    {
        AssetDatabase.ForceReserializeAssets();
    }
}
