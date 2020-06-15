using System;
using System.Collections;
using System.Collections.Generic;
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

    Vector2Int m_currentKey = Vector2Int.zero;

    Vector2Int GetChunkKey(Vector2 position)
    {
        int x = Mathf.RoundToInt(m_player.position.x / (MapGenerator.chunkSize - 1));
        int y = Mathf.RoundToInt(m_player.position.y / (MapGenerator.chunkSize - 1));

        return new Vector2Int(x, y);
    }

    Chunk CreateChunk(Vector2Int key)
    {
        var chunk = Instantiate(m_chunkPrefab).GetComponent<Chunk>();
        chunk.transform.position = new Vector3(key.x * (MapGenerator.chunkSize - 1), 0.0f, key.y * (MapGenerator.chunkSize - 1));
        chunk.Init(m_mapGenerator.noiseMap, m_mapGenerator.heightMultiplier, m_mapGenerator.heightCurve, m_mapGenerator.lodMax, m_mapGenerator.mapInfo);

        return chunk;
    }
    void RemoveChunk(Vector2Int key)
    {
        var chunk = m_chunkDictionary[key];
        m_chunkDictionary.Remove(key);

        Destroy(chunk.gameObject);
    }
    void LoadChunks(Vector2Int key)
    {
        for (int i = -m_radius; i < m_radius + 1; i++)
        {
            for (int j = -m_radius; j < m_radius + 1; j++)
            {
                var tmp = key + new Vector2Int(i, j);
                if (!m_chunkDictionary.ContainsKey(tmp))
                {
                    var chunk = CreateChunk(tmp);
                    m_chunkDictionary.Add(tmp, chunk);
                }
            }
        }
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
            RemoveChunk(keyToRemove);
    }

    void OnChunkEnter(Vector2Int key)
    {
        UnloadChunks(key);
        LoadChunks(key);
    }

    private void Awake()
    {
        m_mapGenerator = GetComponent<MapGenerator>();
    }
    private void Start()
    {
        m_currentKey = GetChunkKey(m_player.position);
        OnChunkEnter(m_currentKey);
    }
    private void Update()
    {
        var key = GetChunkKey(m_player.position);
        if (key != m_currentKey)
        {
            OnChunkEnter(key);

            m_currentKey = key;
        }
    }
}
