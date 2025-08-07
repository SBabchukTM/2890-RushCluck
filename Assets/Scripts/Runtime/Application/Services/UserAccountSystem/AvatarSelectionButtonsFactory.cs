using System.Collections.Generic;
using Core;
using Core.Factory;
using UnityEngine;
using Zenject;

namespace Runtime.Application.UserAccountSystem
{
    public class AvatarSelectionButtonsFactory : IInitializable
    {
        private const string AvatarPrefabAddressableName = "AvatarButtonPrefab";

        private readonly IAssetProvider _assetProvider;
        private readonly ISettingProvider _settingProvider;
        private readonly GameObjectFactory _gameObjectFactory;

        private GameObject _avatarButtonPrefab;
    
        public AvatarSelectionButtonsFactory(IAssetProvider assetProvider, ISettingProvider settingProvider, GameObjectFactory gameObjectFactory)
        {
            _assetProvider = assetProvider;
            _settingProvider = settingProvider;
            _gameObjectFactory = gameObjectFactory;
        }
    
        public async void Initialize()
        {
            _avatarButtonPrefab = await _assetProvider.Load<GameObject>(AvatarPrefabAddressableName);
        }

        public List<AvatarSelectionButton> CreateAvatarSelectionButtons()
        {
            var avatars = _settingProvider.Get<DefaultAvatarsConfig>().Avatars;
        
            int size = avatars.Count;
            
            List<AvatarSelectionButton> result = new List<AvatarSelectionButton>(size);
        
            for (int i = 0; i < size; i++)
                CreateAvatarSelectionButton(avatars[i], result);
        
            return result;
        }

        private void CreateAvatarSelectionButton(Sprite avatar, List<AvatarSelectionButton> result)
        {
            var avatarSelectionButton = _gameObjectFactory.Create<AvatarSelectionButton>(_avatarButtonPrefab);
            avatarSelectionButton.Initialize(avatar);
            result.Add(avatarSelectionButton);
        }
    }
}

