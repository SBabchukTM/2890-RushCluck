using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Octopus.NetworkUI
{
    public class OfflineOverlay : MonoBehaviour
    {
        public static OfflineOverlay Instance;
        
        private GameObject _contentRoot;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            _contentRoot = GetComponentInChildren<Panel>(true).GetGameObject();
        }

        public void Show()
        {
            _contentRoot.SetActive(true);
        }
        
        public void Hide()
        {
            _contentRoot.SetActive(false);
        }
    }
}

