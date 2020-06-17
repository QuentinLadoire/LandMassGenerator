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

		Debug.Log("i'm not a spy");
		Debug.Log("quentin is a spy");
		Debug.Log("this is stackoverflow code");
		Debug.Log("C'est a mon tour de faire le Spy");
	}
	private void Update()
	{
		if (m_update)
		{
			m_meshFilter.sharedMesh = m_chunkData.meshDatas[0].CreateMesh();
			m_meshRenderer.sharedMaterial.mainTexture = TextureGenerator.GenerateTexture(m_chunkData.colorMap, m_chunkData.size.x, m_chunkData.size.y);

			m_update = false;
		}
	}
}
