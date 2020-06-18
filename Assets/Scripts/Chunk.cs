using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	MeshFilter m_meshFilter = null;
	MeshRenderer m_meshRenderer = null;

	bool m_update = false;

	ChunkData m_chunkData = null;
	public ChunkData chunkData
	{
		get => m_chunkData;
		set
		{
			m_update = true;
			m_chunkData = value;
		}
	}

	private void Awake()
	{
		m_meshFilter = GetComponent<MeshFilter>();
		m_meshRenderer = GetComponent<MeshRenderer>();
	}
	private void Update()
	{
		if (m_update)
		{
			m_meshFilter.mesh = m_chunkData.meshDatas[0].CreateMesh();
			m_meshRenderer.material.mainTexture = TextureGenerator.GenerateTexture(m_chunkData.colorMap, m_chunkData.size.x, m_chunkData.size.y);

			m_update = false;
		}
	}
}
