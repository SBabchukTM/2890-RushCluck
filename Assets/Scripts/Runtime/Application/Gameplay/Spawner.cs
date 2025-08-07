using Runtime.Services;
using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;
using Runtime.Services.UserData;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Spawner : IInitializable
{
    private readonly IAssetProvider _assetProvider;
    private readonly UserDataService _userDataService;
    private BaseSimplePool _spikesPool;
    private BaseSimplePool _collectiblePool;

    private GameConfig _gameConfig;
    private ShopConfig _shopConfig;
    private LevelConfig _levelConfig;
    private GameObject _gameBG;

    public event Action<GameObject> OnSpikeSpawned;
    public event Action<GameObject> OnCollectibleSpawned;

    public Spawner(IAssetProvider assetProvider, UserDataService userDataService, BaseSimplePool spikesPool, BaseSimplePool collectiblePool)
    {
        _assetProvider = assetProvider;
        _userDataService = userDataService;
        _spikesPool = spikesPool;
        _collectiblePool = collectiblePool;
    }

    public async void Initialize()
    {
        _gameConfig = await _assetProvider.Load<GameConfig>(ConstConfigs.GameConfig);
        _shopConfig = await _assetProvider.Load<ShopConfig>(ConstConfigs.ShopItemsConfig);
        var sprites = new List<Sprite>();

        await InitSpikes();
        _collectiblePool.Initialize(_gameConfig.CollectiblePoolSize, await _assetProvider.Load<GameObject>(ConstPrefabs.CurrencyPrefab));
        _gameBG = GameObject.Instantiate(await _assetProvider.Load<GameObject>(ConstPrefabs.GameplayBgPrefab));
    }

    private async Task InitSpikes()
    {
        var sprites = new List<Sprite>();
        sprites.Clear();
        for (int i = 0; i < _shopConfig.ShopItems.Count; i++)
        {
            ShopItem item = _shopConfig.ShopItems[i];
            if (_userDataService.GetUserData().UserInventory.PurchasedGameItemsIDs.Contains(i))
            {
                sprites.Add(item.ItemSprite);
            }
        }

        _spikesPool.Initialize(_gameConfig.SpikesPoolSize, await _assetProvider.Load<GameObject>(ConstPrefabs.SpikePrefab), sprites);
    }

    public async Task StartGameAsync(LevelConfig levelConfig, CancellationToken token)
    {
        await InitSpikes();
        _levelConfig = levelConfig;
        SpawnSpikesTask(token).Forget();
        SpawnCollectiblesTask(token).Forget();
        _gameBG.SetActive(true);
    }

    public void EndGame()
    {
        _gameBG.SetActive(false);
    }

    public void ReturnSpike(GameObject spike) => _spikesPool.Return(spike);
    public void ReturnCollectible(GameObject collectible) => _collectiblePool.Return(collectible);

    private async UniTaskVoid SpawnSpikesTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(UnityEngine.Random.Range(_levelConfig.MinSpikeSpawnDelay, _levelConfig.MaxSpikeSpawnDelay));
            token.ThrowIfCancellationRequested();

            SpawnSpike();
        }
    }

    private async UniTaskVoid SpawnCollectiblesTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(UnityEngine.Random.Range(_levelConfig.MinCollectibleSpawnDelay, _levelConfig.MaxCollectibleSpawnDelay));
            token.ThrowIfCancellationRequested();

            SpawnCollectible();
        }
    }

    private void SpawnSpike()
    {
        GameObject spike = _spikesPool.GetCar();
        spike.transform.position = GetSpawnPos();
        OnSpikeSpawned?.Invoke(spike);
    }

    private void SpawnCollectible()
    {
        GameObject collectible = _collectiblePool.Get();
        collectible.transform.position = GetSpawnPos();
        OnCollectibleSpawned?.Invoke(collectible);
    }

    private Vector3 GetSpawnPos()
    {
        Vector3 spawnPos = Vector3.zero;

        spawnPos.y = _gameConfig.TopLeftSpawnPos.y;
        spawnPos.x = UnityEngine.Random.Range(_gameConfig.TopLeftSpawnPos.x, -_gameConfig.TopLeftSpawnPos.x);

        return spawnPos;
    }
}