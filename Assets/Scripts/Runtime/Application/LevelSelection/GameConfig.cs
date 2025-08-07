using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private int _spikesPoolSize;
    [SerializeField] private int _collectiblePoolSize;
    [SerializeField] private List<LevelConfig> _levelConfigs;
    [SerializeField] private int _collectibleReward = 1;
    [SerializeField] private Vector3 _balloonSpawnPos;
    [SerializeField] private Vector3 _topLeftSpawnPos;
    [SerializeField] private Vector3 _bottomDespawnPos;

    public int SpikesPoolSize => _spikesPoolSize;
    public int CollectiblePoolSize => _collectiblePoolSize;
    public List<LevelConfig> LevelConfigs => _levelConfigs;
    public int CollectibleReward => _collectibleReward;
    public Vector3 BalloonSpawnPos => _balloonSpawnPos;
    public Vector3 TopLeftSpawnPos => _topLeftSpawnPos;
    public Vector3 BottomDespawnPos => _bottomDespawnPos;
}
