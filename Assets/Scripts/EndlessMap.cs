using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EndlessMap : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject m_chunkPrefab = null;
    [SerializeField] Player m_player = null;

    [Header("Parameters")]
    [SerializeField] int m_radius = 1;

    MapGenerator m_mapGenerator = null;

    Dictionary<Vector2Int, Chunk> m_chunkDictionary = new Dictionary<Vector2Int, Chunk>();
    Dictionary<Vector2Int, ChunkData> m_chunkDataDictionary = new Dictionary<Vector2Int, ChunkData>();

    Queue<Vector2Int> m_chunkKeyToLoadQueue = new Queue<Vector2Int>();
    Queue<Vector2Int> m_chunkKeyToUnloadQueue = new Queue<Vector2Int>();

    Vector2Int m_currentKey = new Vector2Int(int.MinValue, int.MinValue);

    Thread m_chunkLoaderthread = null;
    volatile bool m_requestOnChunkEnter = false;

    readonly object m_keyLock = new object();
    readonly object m_chunkKeyToLoadLock = new object();
    readonly object m_chunkKeyToUnloadLock = new object();
    readonly object m_chunkDataDictionnaryLock = new object();

    Vector2Int GetChunkKey(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / (MapGenerator.chunkSize - 1));
        int y = Mathf.RoundToInt(position.y / (MapGenerator.chunkSize - 1));

        return new Vector2Int(x, y);
    }

    void RemoveChunk(Vector2Int key)
    {
        if (m_chunkDictionary.ContainsKey(key))
        {
            var chunk = m_chunkDictionary[key];
            m_chunkDictionary.Remove(key);

            Destroy(chunk.gameObject);
        }
    }
    Chunk CreateChunk(Vector2Int key, ChunkData chunkData)
    {
        var chunk = Instantiate(m_chunkPrefab).GetComponent<Chunk>();
        chunk.transform.position = new Vector3(key.x * (MapGenerator.chunkSize - 1), 0.0f, key.y * (MapGenerator.chunkSize - 1));
        chunk.chunkData = chunkData;
        
        return chunk;
    }

    void RemoveChunkData(Vector2Int key)
    {
        if (m_chunkDataDictionary.ContainsKey(key))
        {
            m_chunkDataDictionary.Remove(key);
        }
    }
    ChunkData CreateChunkData(Vector2Int key)
    {
        return new ChunkData(m_mapGenerator.mapInfo, new Vector2(key.x * (MapGenerator.chunkSize - 1), key.y * (MapGenerator.chunkSize - 1)));
    }

    //ChunkLoader Thread Function
    void UnloadChunks(Vector2Int key)
    {
        lock (m_chunkKeyToUnloadLock)
        {
            lock (m_chunkDataDictionnaryLock)
            {
                foreach (var tmp in m_chunkDataDictionary)
                {
                    if (tmp.Key.x > key.x + m_radius || tmp.Key.y > key.y + m_radius ||
                        tmp.Key.x < key.x - m_radius || tmp.Key.y < key.y - m_radius)
                    {
                        m_chunkKeyToUnloadQueue.Enqueue(tmp.Key);
                    }
                }
            }
        }
    }
    void LoadChunkDatas(Vector2Int key)
    {
        lock (m_chunkKeyToLoadLock)
        {
            lock (m_chunkDataDictionnaryLock)
            {
                for (int i = -m_radius; i < m_radius + 1; i++)
                {
                    for (int j = -m_radius; j < m_radius + 1; j++)
                    {
                        var tmp = key + new Vector2Int(i, j);
                        if (!m_chunkDataDictionary.ContainsKey(tmp))
                        {
                            m_chunkDataDictionary.Add(tmp, CreateChunkData(tmp));
                            m_chunkKeyToLoadQueue.Enqueue(tmp);
                        }
                    }
                }
            }
        }
    }
    void OnChunkEnter(Vector2Int key)
    {
        LoadChunkDatas(key);

        UnloadChunks(key);
    }

    void ChunkLoaderThreadUpdate()
    {
        while (true)
        {
            if (m_requestOnChunkEnter)
            {
                lock (m_keyLock)
                {
                    OnChunkEnter(m_currentKey);

                    m_requestOnChunkEnter = false;
                }
            }
        }
    }

    //Main Thread Function
    void CheckKey()
    {
        if (Monitor.TryEnter(m_keyLock))
        {
            var key = GetChunkKey(m_player.position);
            if (key != m_currentKey)
            {
                m_currentKey = key;

                m_requestOnChunkEnter = true;
            }

            Monitor.Exit(m_keyLock);
        }
    }
    void ResolveUnloadQueue()
    {
        if (Monitor.TryEnter(m_chunkKeyToUnloadLock))
        {
            if (Monitor.TryEnter(m_chunkDataDictionnaryLock))
            {
                if (m_chunkKeyToUnloadQueue.Count > 0)
                {
                    var key = m_chunkKeyToUnloadQueue.Dequeue();
                    RemoveChunkData(key);
                    RemoveChunk(key);
                }

                Monitor.Exit(m_chunkDataDictionnaryLock);
            }

            Monitor.Exit(m_chunkKeyToUnloadLock);
        }
    }
    void ResolveLoadQueue()
    {
        if (Monitor.TryEnter(m_chunkKeyToLoadLock))
        {
            if (Monitor.TryEnter(m_chunkDataDictionnaryLock))
            {
                if (m_chunkKeyToLoadQueue.Count > 0)
                {
                    var key = m_chunkKeyToLoadQueue.Dequeue();
                    if (m_chunkDataDictionary.ContainsKey(key))
                    {
                        if (!m_chunkDictionary.ContainsKey(key))
                        {
                            var chunkData = m_chunkDataDictionary[key];
                            var chunk = CreateChunk(key, chunkData);
                            m_chunkDictionary.Add(key, chunk);
                        }
                    }
                }

                Monitor.Exit(m_chunkDataDictionnaryLock);
            }

            Monitor.Exit(m_chunkKeyToLoadLock);
        }
    }

    private void Awake()
    {
        m_mapGenerator = GetComponent<MapGenerator>();
        m_chunkLoaderthread = new Thread(ChunkLoaderThreadUpdate);
    }
    private void Start()
    {
        m_chunkLoaderthread.Start();
    }
    private void Update()
    {
        CheckKey();

        ResolveUnloadQueue();

        ResolveLoadQueue();
    }
    private void OnDestroy()
    {
        m_chunkLoaderthread.Abort();
    }
}
