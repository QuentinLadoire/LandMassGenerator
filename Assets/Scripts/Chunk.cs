using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapInfo = System.Collections.Generic.List<TerrainInfo>;

public class Chunk : MonoBehaviour
{
	MeshFilter m_meshFilter = null;
	MeshRenderer m_meshRenderer = null;

	List<MeshData> m_meshDatas = new List<MeshData>();
	Texture m_texture = null;

	bool m_update = false;

	public void Init(NoiseMap noiseMap, float heightMultiplier, AnimationCurve heightCurve, int lodMax, MapInfo mapInfo)
	{
		if (noiseMap != null)
		{
			var map = noiseMap.GetMap(new Vector2(transform.position.x, -transform.position.z));

			for (int i = 0; i < lodMax; i++)
			{
				m_meshDatas.Add(new MeshData(map, heightMultiplier, heightCurve, i));
			}

			m_texture = TextureGenerator.GenerateColorMap(map, mapInfo);

			m_update = true;
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
			m_meshFilter.sharedMesh = m_meshDatas[0].CreateMesh();
			m_meshRenderer.material.mainTexture = m_texture;

			m_update = false;
		}
	}
}
