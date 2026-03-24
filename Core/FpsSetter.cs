using UnityEngine;
public static class FpsSetter
{
    #if UNITY_ANDROID
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        // int maxRefreshRate = (int)(Screen.currentResolution.refreshRateRatio.numerator / Screen.currentResolution.refreshRateRatio.denominator);
        SetFps(120);
    }
    #endif
    public static void SetFps(int fps)
    {
        Application.targetFrameRate = fps;
    }
}