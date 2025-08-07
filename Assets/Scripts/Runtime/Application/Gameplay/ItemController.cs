using Runtime.Services;
using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

public class ItemController : IInitializable
{
    private Spawner _spawner;
    private IAssetProvider _assetProvider;

    private GameConfig _gameConfig;

    private List<GameObject> _spikesInGame = new();
    private List<GameObject> _collectiblesInGame = new();

    private LevelConfig _levelConfig;

    public ItemController(Spawner spawner, IAssetProvider assetProvider)
    {
        _spawner = spawner;
        _assetProvider = assetProvider;
    }

    public async void Initialize()
    {
        _gameConfig = await _assetProvider.Load<GameConfig>(ConstConfigs.GameConfig);
    }

    public void StartGame(LevelConfig levelConfig, CancellationToken token)
    {
        _levelConfig = levelConfig;

        _spawner.StartGameAsync(levelConfig, token);
        _spawner.OnSpikeSpawned += AddSpikeToList;
        _spawner.OnCollectibleSpawned += AddCollectibleToList;

        MoveObjectsLoop(token).Forget();
    }

    public void ReturnCollectibleToPool(GameObject obj)
    {
        _collectiblesInGame.Remove(obj);
        _spawner.ReturnCollectible(obj);
    }

    public void EndGame()
    {
        _spawner.EndGame();

        _spawner.OnSpikeSpawned -= AddSpikeToList;
        _spawner.OnCollectibleSpawned -= AddCollectibleToList;

        for (int i = _collectiblesInGame.Count - 1; i >= 0; i--)
            _spawner.ReturnCollectible(_collectiblesInGame[i]);
        _collectiblesInGame.Clear();

        for (int i = _spikesInGame.Count - 1; i >= 0; i--)
            _spawner.ReturnSpike(_spikesInGame[i]);
        _spikesInGame.Clear();
    }

    private void AddCollectibleToList(GameObject obj) => _collectiblesInGame.Add(obj);
    private void AddSpikeToList(GameObject obj) => _spikesInGame.Add(obj);

    private async UniTaskVoid MoveObjectsLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            token.ThrowIfCancellationRequested();

            Vector3 moveDir = new Vector3(0, -1 * Time.deltaTime * _levelConfig.ScrollSpeed, 0);

            MoveObjects(_spikesInGame, moveDir, (gameObject) => _spawner.ReturnSpike(gameObject));
            MoveObjects(_collectiblesInGame, moveDir, (gameObject) => _spawner.ReturnCollectible(gameObject));

            await UniTask.NextFrame();
            token.ThrowIfCancellationRequested();
        }
    }

    private void MoveObjects(List<GameObject> objects, Vector3 moveAmount, Action<GameObject> returnToPoolAction)
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            GameObject obj = objects[i];

            if (obj == null)
                continue;

            Vector3 pos = obj.transform.position;
            pos += moveAmount;

            obj.transform.position = pos;
            if (pos.y < _gameConfig.BottomDespawnPos.y)
            {
                objects.RemoveAt(i);
                returnToPoolAction(obj);
            }
        }
    }
}
