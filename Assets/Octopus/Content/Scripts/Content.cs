using Core;
using Octopus.Client;
using Octopus.VerifyInternet;
using Octopus.Preloader;
using Octopus.SceneLoaderCore.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Octopus.EntryPoint
{
   public class EntryPoint : MonoBehaviour
   {
      private void Start()
      { 
         Log("Application initialized");

         if (Loading.Instance != null)
         {
            Loading.Instance.Visible(true);
         }

         EvaluateConnection(ConnectivityManager.Instance.IsConnected);
      }

      private void EvaluateConnection(bool? status)
      {
         if (!status.HasValue)
         {
            ConnectivityManager.Instance?.OnChangedInternetConnection.AddListener(EvaluateConnection);
            return;
         }

         ConnectivityManager.Instance?.OnChangedInternetConnection.RemoveListener(EvaluateConnection);

         if (status.Value)
         {
            var whiteModeEnabled = PlayerPrefs.GetInt(Constants.IsOnlyWhiteRunApp, 0) != 0;
            Log($"WhiteApp flag: {whiteModeEnabled}");

            if (whiteModeEnabled)
            {
               LoadWhiteScene();
            }
            else
            {
               Client.Client.Instance?.Initialize();
            }
         }
         else
         {
            Log("Connectivity lost");

            var isFirstStart = GameSettings.HasKey(Constants.IsFirstRunApp);
            
            var whiteModeForced = PlayerPrefs.GetInt(Constants.IsOnlyWhiteRunApp, 0) == 1;

            if (isFirstStart && whiteModeForced)
            {
               LoadWhiteScene();
            }
            else
            {
               ConnectivityManager.Instance?.OnChangedInternetConnection.AddListener(EvaluateConnection);
            }
         }
      }
      
      private void LoadWhiteScene()
      {
         var mainSceneId = SceneLoader.Instance?.mainScene;

         if (SceneLoader.Instance != null && !string.IsNullOrEmpty(mainSceneId))
         {
            SceneLoader.Instance.SwitchToScene(mainSceneId);
         }
         else
         {
            SceneManager.LoadScene("MainMenu");
         }
      }
      
      private void Log(string content)
      {
         var tone = new Color(0.8f, 0.5f, 0f);
         
         ConsoleReporter.Info($"### EntryPoint: {content}", tone);
      }
   }
}
