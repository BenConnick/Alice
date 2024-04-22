using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct ObstacleSpawnInfo
{
    public GameObject Prefab;
    public int Lane;
    public float YPos;
}

[Obsolete]
public abstract class ObstacleSpawner
{
    public abstract ObstacleSpawnInfo[] Update(float deltaTime);

    public ObstacleSpawnersConfig Config => Resources.Load<ObstacleSpawnersConfig>("GlobalObstacleSpawnersConfig");
}

[Obsolete]
public abstract class ShuffledObstacleSpawner : ObstacleSpawner
{
    protected GameObject[] obstaclePrefabs;
    protected GameObject[] shuffledObstacleQueue = new GameObject[0];
    protected int shuffledObstacleIndex = int.MaxValue;

    public ShuffledObstacleSpawner(GameObject[] prefabs)
    {
        obstaclePrefabs = prefabs;
    }

    public virtual GameObject GetNextRandomObstacle()
    {
        // spawn random obstacle
        if (shuffledObstacleIndex >= shuffledObstacleQueue.Length)
        {
            obstaclePrefabs.CopyTo(shuffledObstacleQueue, 0);
            Util.Shuffle(shuffledObstacleQueue);
            shuffledObstacleIndex = 0;
        }
        var prefab = shuffledObstacleQueue[shuffledObstacleIndex];
        shuffledObstacleIndex++;
        return prefab;
    }
}

[Obsolete]
public class RabbitHoleObstacleSpawner : ShuffledObstacleSpawner
{
    private static ObstacleSpawnInfo[] Empty = new ObstacleSpawnInfo[0];
    protected float lastObstableSpawnTime;
    private float secondsBetweenObstacles => Config.RabbitHoleCurve.ComputeSpawnInterval(elapsedTime);
    private int obstacleXMax => Config.obstacleLaneMax;
    private float elapsedTime = 0;

    public RabbitHoleObstacleSpawner(GameObject[] prefabs) : base(prefabs)
    {
        // unused
    }

    public override ObstacleSpawnInfo[] Update(float dt)
    {
        elapsedTime += dt;

        // spawn new obstacles
        //Debug.Log("SecondsBetweenObstacles: " + secondsBetweenObstacles);
        if (Time.time - lastObstableSpawnTime > secondsBetweenObstacles)
        {
            lastObstableSpawnTime = Time.time;
            var prefab = GetNextRandomObstacle();

            var gameCam = World.Get<GameplayCameraBehavior>().GetComponent<Camera>();
            var yPos = -gameCam.orthographicSize * 2; // below bottom of the screen
            var lane = Random.Range(0, obstacleXMax); // at a random position

            return new ObstacleSpawnInfo[] {
                new ObstacleSpawnInfo {
                    Prefab = prefab,
                    Lane = lane,
                    YPos = yPos
                }
            };
        }
        else
        {
            return Empty;
        }
    }
}