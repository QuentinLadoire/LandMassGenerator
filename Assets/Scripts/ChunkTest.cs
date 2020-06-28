using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTest : MonoBehaviour
{
    [SerializeField] MapGenerator m_mapGenerator = null;
    [SerializeField] Chunk m_chunk1 = null;
    [SerializeField] Chunk m_chunk2 = null;
    [SerializeField] Vector2Int m_size = new Vector2Int(241, 241);

    Vector3 previous1 = Vector3.zero;
    Vector3 previous2 = Vector3.zero;

    private void Update()
    {
        if (m_chunk1.transform.position != previous1)
        {
            m_chunk1.chunkData = new ChunkData(m_mapGenerator.mapInfo, new Vector2(m_chunk1.transform.position.x, m_chunk1.transform.position.z), m_size);
        }
        if (m_chunk2.transform.position != previous2)
        {
            m_chunk2.chunkData = new ChunkData(m_mapGenerator.mapInfo, new Vector2(m_chunk2.transform.position.x, m_chunk2.transform.position.z), m_size);
        }

        previous1 = m_chunk1.transform.position;
        previous2 = m_chunk2.transform.position;
    }
}
