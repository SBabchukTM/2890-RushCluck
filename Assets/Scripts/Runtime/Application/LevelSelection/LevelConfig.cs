using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Config 0", menuName = "Config/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private float _scrollSpeed;
    [SerializeField] private float _targetDistance;
    [SerializeField] private float _minSpikeSpawnDelay;
    [SerializeField] private float _maxSpikeSpawnDelay;
    [SerializeField] private float _minCollectibleSpawnDelay;
    [SerializeField] private float _maxCollectibleSpawnDelay;

    public float ScrollSpeed => _scrollSpeed;
    public float TargetDistance => _targetDistance;
    public float MinSpikeSpawnDelay => _minSpikeSpawnDelay;
    public float MaxSpikeSpawnDelay => _maxSpikeSpawnDelay;
    public float MinCollectibleSpawnDelay => _minCollectibleSpawnDelay;
    public float MaxCollectibleSpawnDelay => _maxCollectibleSpawnDelay;
}
