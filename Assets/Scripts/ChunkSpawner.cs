using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner
{
    protected LevelChunk[] chunkPrefabs;

    protected float lastObstableSpawnTime;
    private float secondsBetweenObstacles => LevelChunk.height/ContextualInputSystem.ActiveGameInstance.fallSpeed;
    private float elapsedTime = 0;
    private LevelChunk prevChunk;

    protected LevelChunk[] shuffledChunkQueue;
    protected int shuffledChunkIndex = int.MaxValue;

    public ChunkSpawner(LevelChunk[] prefabs)
    {
        chunkPrefabs = prefabs;
        prevChunk = prefabs[0];
        shuffledChunkQueue = new LevelChunk[prefabs.Length];
        chunkPrefabs.CopyTo(shuffledChunkQueue, 0);
    }

    public virtual LevelChunk GetNextRandomChunk()
    {
        // spawn random obstacle
        if (shuffledChunkIndex >= shuffledChunkQueue.Length)
        {
            // record the last chunk in order
            LevelChunk prevChunk = shuffledChunkQueue[shuffledChunkQueue.Length - 1];

            // shuffle the queue
            Util.Shuffle(shuffledChunkQueue);
            // reshuffle if there's two of the same chunk next to each other
            int breakCondition = 0;
            while (shuffledChunkQueue.Length > 1 && shuffledChunkQueue[0] == prevChunk)
            {
                if (breakCondition > 10) break;
                breakCondition++;
                Util.Shuffle(shuffledChunkQueue);
            }
            shuffledChunkIndex = 0;
        }
        var prefab = chunkPrefabs[shuffledChunkIndex];
        shuffledChunkIndex++;
        return prefab;
    }

    public virtual LevelChunk GetNextValidChunk()
    {
        // spawn random obstacle
        var exitPoints = prevChunk.BottomSlots;
        List<LevelChunk> validNextChunks = new List<LevelChunk>();
        foreach (var chunk in chunkPrefabs)
        {
            // search for two adjacent points
            int prevMatch = -1;
            for (int i = 0; i < LaneUtils.NumLanes; i++)
            {
                if (System.Array.IndexOf(exitPoints,i) >= 0 && System.Array.IndexOf(chunk.TopSlots, i) >= 0)
                {
                    if (prevMatch < 0) prevMatch = i;
                    else
                    {
                        validNextChunks.Add(chunk);
                        break;
                    }
                }
                else
                {
                    prevMatch = -1;
                }
            }
        }
        // check for the (error) case where there are no valid next chunks
        if (validNextChunks.Count == 0)
        {
            Debug.LogError($"Level Error: There is no valid next level chunk for chunk {prevChunk.name}, game will be unwinnable!");
            prevChunk = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
            return prevChunk;
        }
        // pick a random chunk among the valid chunks
        var prefab = validNextChunks[Random.Range(0,validNextChunks.Count)];
        // update prev chunk, note that if this doesn't spawn the chunk, undesired behavior will occur
        prevChunk = prefab;
        return prefab;
    }

    public LevelChunk Update(float dt)
    {
        elapsedTime += dt;

        // spawn new obstacles
        //Debug.Log("SecondsBetweenObstacles: " + secondsBetweenObstacles);
        if (Time.time - lastObstableSpawnTime > secondsBetweenObstacles)
        {
            lastObstableSpawnTime = Time.time;
            var prefab = GetNextRandomChunk();
            return prefab;
        }
        else
        {
            return null;
        }
    }

    public LevelChunk Force()
    {
        return GetNextRandomChunk();
    }
}