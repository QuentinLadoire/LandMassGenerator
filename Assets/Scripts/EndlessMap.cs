using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct ChunkDataKeyInfo
{
    public ChunkData chunkData;
    public Vector2Int key;

    public ChunkDataKeyInfo(ChunkData chunkData, Vector2Int key)
    {
        this.chunkData = chunkData;
        this.key = key;
    }
}

public class EndlessMap : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject m_chunkPrefab = null;
    [SerializeField] Player m_player = null;

    [Header("Parameters")]
    [SerializeField] int m_radius = 1;

    MapGenerator m_mapGenerator = null;
    Dictionary<Vector2Int, Chunk> m_chunkDictionary = new Dictionary<Vector2Int, Chunk>();
    Queue<ChunkDataKeyInfo> m_chunkDataInfoQueue = new Queue<ChunkDataKeyInfo>();
    Vector2Int m_currentKey = new Vector2Int(int.MinValue, int.MinValue);

    readonly object m_chunkDataInfoQueueLock = new object();

    Vector2Int GetChunkKey(Vector2 position)
    {
        int x = Mathf.RoundToInt(m_player.position.x / (MapGenerator.chunkSize - 1));
        int y = Mathf.RoundToInt(m_player.position.y / (MapGenerator.chunkSize - 1));

        return new Vector2Int(x, y);
    }

    void RemoveChunk(Vector2Int key)
    {
        var chunk = m_chunkDictionary[key];
        m_chunkDictionary.Remove(key);

        Destroy(chunk.gameObject);
    }
    Chunk CreateChunk(Vector2Int key, ChunkData chunkData)
    {
        var chunk = Instantiate(m_chunkPrefab).GetComponent<Chunk>();
        chunk.transform.position = new Vector3(key.x * (MapGenerator.chunkSize - 1), 0.0f, key.y * (MapGenerator.chunkSize - 1));
        chunk.chunkData = new ChunkData(chunkData);
        
        return chunk;
    }
    ChunkData CreateChunkData(Vector2Int key)
    {
        return new ChunkData(m_mapGenerator.mapInfo, new Vector2(key.x * (MapGenerator.chunkSize - 1), key.y * (MapGenerator.chunkSize - 1)));
    }

    void UnloadChunks(Vector2Int key)
    {
        List<Vector2Int> keysToRemove = new List<Vector2Int>();

        foreach (var tmp in m_chunkDictionary)
        {
            if (tmp.Key.x > key.x + m_radius || tmp.Key.y > key.y + m_radius ||
                tmp.Key.x < key.x - m_radius || tmp.Key.y < key.y - m_radius)
            {
                keysToRemove.Add(tmp.Key);
            }
        }

        foreach (var keyToRemove in keysToRemove)
        {
            RemoveChunk(keyToRemove);
        }
    }
    void LoadChunkDatas(Vector2Int key)
    {
        lock (m_chunkDataInfoQueueLock)
        {
            for (int i = -m_radius; i < m_radius + 1; i++)
            {
                for (int j = -m_radius; j < m_radius + 1; j++)
                {
                    var tmp = key + new Vector2Int(i, j);
                    if (!m_chunkDictionary.ContainsKey(tmp))
                    {
                        var chunkData = CreateChunkData(tmp);
                        m_chunkDataInfoQueue.Enqueue(new ChunkDataKeyInfo(chunkData, tmp));
                    }
                }
            }
        }
    }

    void OnChunkEnter(Vector2Int key)
    {
        //UnloadChunks(key);

        new Thread(new ThreadStart(delegate { LoadChunkDatas(key); })).Start();
    }

    private void Awake()
    {
        m_mapGenerator = GetComponent<MapGenerator>();
    }
    private void Update()
    {
        var key = GetChunkKey(m_player.position);
        if (key != m_currentKey)
        {
            OnChunkEnter(key);

            m_currentKey = key;
        }

        if (Monitor.TryEnter(m_chunkDataInfoQueueLock))
        {
            if (m_chunkDataInfoQueue.Count > 0)
            {
                var chunkDataInfo = m_chunkDataInfoQueue.Dequeue();
                var chunk = CreateChunk(chunkDataInfo.key, chunkDataInfo.chunkData);
                m_chunkDictionary.Add(chunkDataInfo.key, chunk);
            }

            Monitor.Exit(m_chunkDataInfoQueueLock);
        }
    }
}
