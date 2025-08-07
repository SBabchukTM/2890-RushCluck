using System;
using AndroidInstallReferrer;
using Core;
using UnityEngine;

public class InstallReferrerExample : MonoBehaviour
{
    public static InstallReferrerExample Instance { get; private set; }

    private string installReferrer;

    public event Action<string> OnInstallReferrerReceived;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InstallReferrer.GetReferrer(OnGetData);
        
        Log($"🏁 Start InstallReferrer: {installReferrer}");

#if UNITY_EDITOR
        OnGetData(new InstallReferrerData(
            "utm_source=google&utm_medium=cpc&utm_term=1&utm_content=2&utm_campaign=3&anid=admob", 
            "1.0", false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now));
#endif
    }

    private void OnGetData(InstallReferrerData data)
    {
        if (data.IsSuccess)
        {
            installReferrer = data.InstallReferrer;
            
            Log($"✅InstallReferrer: {installReferrer}");
            
            OnInstallReferrerReceived?.Invoke(installReferrer);
        }
        else
        {
            Log($"❌InstallReferrer Error: {data.Error}");
            OnInstallReferrerReceived?.Invoke(null);
        }
    }
    
    public string GetInstallReferrer()
    {
        return installReferrer;
    }
    
    private void Log(string message)
    {
        ConsoleReporter.Info($"@@@ FirebaseInit ->: {message}", new Color(0.9f, 0.1f, 0.3f));
    }
}