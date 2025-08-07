using System.Threading;
using Core.StateMachine;
using Runtime.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;
using Runtime.Services.UserData;
using Zenject;
using Core;
using Core.Factory;
using UnityEngine;
using Runtime.Services;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Game
{
    public class LeaderboardStateController : StateController, IInitializable
    {
        private readonly IUiService _uiService;
        private readonly IAssetProvider _assetProvider;
        private readonly UserDataService _userDataService;
        private readonly GameObjectFactory _factory;

        private LeaderboardScreen _screen;
        private GameObject _recordDisplayPrefab;

        public LeaderboardStateController(ILogger logger, IUiService uiService, IAssetProvider assetProvider, UserDataService userDataService, GameObjectFactory factory) : base(logger)
        {
            _uiService = uiService;
            _assetProvider = assetProvider;
            _userDataService = userDataService;
            _factory = factory;
        }

        public async void Initialize()
        {
            _recordDisplayPrefab = await _assetProvider.Load<GameObject>(ConstPrefabs.RecordDisplayPrefab);
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            CreateScreen();
            SubscribeToEvents();
            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            await _uiService.HideScreen(ConstScreens.LeaderboardScreen);
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<LeaderboardScreen>(ConstScreens.LeaderboardScreen);
            _screen.Initialize(CreateRecordDisplayList());
            _screen.ShowAsync().Forget();
        }

        private void SubscribeToEvents()
        {
            _screen.OnBackPressed += async () => await GoTo<MenuStateController>();
        }

        private List<RecordDisplay> CreateRecordDisplayList()
        {
            var records = CreateGameRecords();
            var result = new List<RecordDisplay>(records.Count);

            for (int i = 0; i < records.Count; i++)
            {
                var recordDisplay = _factory.Create<RecordDisplay>(_recordDisplayPrefab);
                recordDisplay.Initialize(i + 1, records[i].Name, records[i].Score);
                result.Add(recordDisplay);
            }

            return result;
        }

        private List<Record> CreateGameRecords()
        {
            var records = CreateFakeRecords();
            var userData = _userDataService.GetUserData();

            records.Add(new() { Name = userData.UserAccountData.Username, Score = userData.UserProgressData.Score });

            records = records.OrderByDescending(x => x.Score).ToList();
            return records;
        }

        private List<Record> CreateFakeRecords() => new()
        {
            new () { Name = "Mike", Score = 1203},
            new () { Name = "Tyson", Score = 1155},
            new () { Name = "Susie", Score = 1012},
            new () { Name = "Margerie", Score = 932},
            new () { Name = "Lois", Score = 983},
            new () { Name = "Arthur", Score = 896},
            new () { Name = "Alex", Score = 832},
            new () { Name = "Sophia", Score = 778},
            new () { Name = "Danny", Score = 623},
            new () { Name = "Daniel", Score = 554},
            new () { Name = "Irena", Score = 461},
            new () { Name = "Bob", Score = 312},
            new () { Name = "Betty", Score = 227},
            new () { Name = "Patrick", Score = 152},
            new () { Name = "Michael", Score = 92},
            new () { Name = "Rob", Score = 73},
            new () { Name = "Lary", Score = 32},
            new () { Name = "Daniel", Score = 16},
        };
    }

    public class Record
    {
        public string Name;
        public int Score;
    }
}