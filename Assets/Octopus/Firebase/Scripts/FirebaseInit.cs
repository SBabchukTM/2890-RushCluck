using Core;
using Firebase.Crashlytics;
using Firebase.Extensions;
using Firebase.Messaging;
using Octopus.Client;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    private void Start()
    {
        Log("Start");

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
        {
            var dependencyStatus = task.Result;
            
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                            
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                
                Init();
            }
            else
            {
                ConsoleReporter.Fail(System.String.Format("Could not resolve all Firebase dependencies: {0}",
                    dependencyStatus));
            }
        });
    }

    private void Init()
    {
        Log("Init"); 
        
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        
        //var token = Firebase.Messaging.FirebaseMessaging.GetTokenAsync().Result;
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Log("Received Registration Token: " + token.Token);
        
        if (PlayerPrefs.GetString("updateFirebase", "") == token.Token) return;
        
        PlayerPrefs.SetInt("newToken", 1);
        PlayerPrefs.SetString("updateFirebase", token.Token);
        PlayerPrefs.Save();
        
        GameSettings.SetValue(Constants.FcmTokenKey, token.Token);
    }

    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) 
    {
        Log("-------------------------notification--------------------------");
        Log("Received a new message from: " + e.Message.From);
        
        var notification = e.Message.Notification;
        
        if (notification != null) 
        {
            Log("title: " + notification.Title);
            Log("body: " + notification.Body);
            
            var android = notification.Android;
            if (android != null)
            {
                Log("android channel_id: " + android.ChannelId);
            }
        }
        else
        {
            Log("notification == null");
        }
        
        if (e.Message.From.Length > 0)
        {
            Log("from: " + e.Message.From);
        }
        if (e.Message.Link != null)
        {
            Log("link: " + e.Message.Link.ToString());
        }
        
        if (e.Message.Data.Count > 0) 
        {
            Log("data:");

            foreach (var (key, value) in e.Message.Data) 
            {
                Log(" - " + key + ": " + value);

                GameSettings.SetValue(key, value);
            }
            
            StartCoroutine(SenderRequest.Send(new TrackRequest(),null));
        }
        
        Log("-------------------------_______________--------------------------");
    }
    
    public static void DeleteFcmToken()
    {
        FirebaseMessaging.DeleteTokenAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                //Debug.LogWarning("❗️ Видалення токена скасовано.");
            }
            else if (task.IsFaulted)
            {
                //Debug.LogError($"❌ Помилка при видаленні токена: {task.Exception}");
            }
            else if (task.IsCompletedSuccessfully)
            {
                //Debug.Log("✅ Токен успішно видалено.");
                
                PlayerPrefs.DeleteKey("updateFirebase");
                PlayerPrefs.Save();
            }
        });
    }
    
    public void OnDestroy()
    {
        FirebaseMessaging.MessageReceived -= OnMessageReceived;
        FirebaseMessaging.TokenReceived -= OnTokenReceived;
    }
    
    private void Log(string message)
    {
        ConsoleReporter.Info($"@@@ FirebaseInit ->: {message}", new Color(0.9f, 0.1f, 0.3f));
    }
}
