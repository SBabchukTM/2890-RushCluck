using Core;
using Runtime.Application.UserAccountSystem;
using UnityEngine;
using Zenject;

namespace Runtime.Game
{
    [CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/GameInstaller")]
    public class GameInstaller : ScriptableObjectInstaller<GameInstaller>
    {
        [SerializeField] private BalloonController _balloonController;
        public override void InstallBindings()
        {
            Container.Bind<MenuStateController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ShopStateController>().AsSingle();
            Container.Bind<StartSettingsController>().AsSingle();
            Container.Bind<LevelSelectionStateController>().AsSingle();
            Container.Bind<UserAccountService>().AsSingle();
            Container.Bind<ImageProcessingService>().AsSingle();
            Container.Bind<AvatarSelectionService>().AsSingle();

            Container.Bind<AccountStateController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplayStateController>().AsSingle();
            Container.BindInterfacesAndSelfTo<LeaderboardStateController>().AsSingle();
            Container.Bind<SettingsStateController>().AsSingle();
            Container.Bind<BaseSimplePool>().AsTransient();
            Container.BindInterfacesAndSelfTo<Spawner>().AsSingle();
            Container.BindInterfacesAndSelfTo<ItemController>().AsSingle();
            Container.Bind<BalloonController>().FromComponentInNewPrefab(_balloonController).AsSingle().NonLazy();
        }
    }
}