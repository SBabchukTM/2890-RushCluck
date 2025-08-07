using Core;
using UnityEngine;

public class GAIDHelper : MonoBehaviour
{
    public static string GetGAID()
    {
#if UNITY_EDITOR
        return "16f3ddfa-0808-4bb8-88e6-a1362cd7bd1a";
#endif
        
        if (Application.platform != RuntimePlatform.Android)
        {
            Log("⚠️ Platform not supported (need Android)");
            return null;
        }
        
        try
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var adClient = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
            using var info = adClient.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", activity);

            return info.Call<string>("getId");
        }
        catch (System.Exception e)
        {
            Log($"❌ Failed to fetch GAID: {e.Message}");
            return null;
        }
    }
    
    private static void Log(string message)
    {
        ConsoleReporter.Info($"@@@ Content ->: {message}", new Color(0.9f, 0.1f, 0.5f));
    }
}