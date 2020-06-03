using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapInfo = System.Collections.Generic.List<TerrainInfo>;

public enum DrawMode
{
	HeighMap,
	ColorMap
}

[System.Serializable]
public class TerrainInfo
{
	public string name = "NoName";
	public float height = 0.0f;
	public Color color = Color.white;
}

public class MapGenerator : MonoBehaviour
{
	[Header("Renderer")]
	[SerializeField] MapRenderer m_mapRenderer = null;
	[SerializeField] DrawMode m_drawMode = DrawMode.HeighMap;
	public DrawMode drawMode { get => m_drawMode; }

	[Header("Parameters")]
	[SerializeField] int m_width = 256;
	[SerializeField] int m_height = 256;
	[SerializeField] float m_scale = 20.0f;

	[SerializeField] float m_heightMultiplier = 1.0f;
	public float heightMultiplier { get => m_heightMultiplier; }

	[SerializeField] AnimationCurve m_heightCurve = new AnimationCurve();
	public AnimationCurve heightCurve { get => m_heightCurve; }

	[SerializeField] int m_octaves = 4;
	[SerializeField] float m_lacunarity = 2;
	[SerializeField] [Range(0.0f, 1.0f)] float m_persistance = 0.5f;

	[SerializeField] int m_seed = 0;
	[SerializeField] Vector2 m_offset = Vector2.zero;

	[SerializeField] Vector2 m_position = Vector2.zero;
	public Vector2 position { get => m_position; }

	[SerializeField] MapInfo m_mapInfo = new MapInfo();

	NoiseMap m_noiseMap = null;
	public NoiseMap noiseMap { get => m_noiseMap; }

	public void GenerateMap()
	{
		m_noiseMap = NoiseMap.GenerateMap(m_width, m_height, m_seed, m_scale, m_octaves, m_lacunarity, m_persistance, m_offset);

		if (m_mapRenderer != null) m_mapRenderer.DrawMap(this);
	}
	public float[,] GetMap()
	{
		if (m_noiseMap == null) return null;

		return m_noiseMap.GetMap(m_position);
	}
	public MapInfo GetMapInfo()
	{
		return m_mapInfo;
	}

	private void OnValidate()
	{
		if (m_width <= 0) m_width = 1;
		if (m_height <= 0) m_width = 1;
		if (m_octaves <= 0) m_octaves = 1;
		if (m_lacunarity < 1) m_lacunarity = 1;

		if (m_noiseMap != null)
		{
			m_noiseMap.width = m_width;
			m_noiseMap.height = m_height;
			m_noiseMap.scale = m_scale;
			m_noiseMap.lacunarity = m_lacunarity;
			m_noiseMap.persistance = m_persistance;
			m_noiseMap.offset = m_offset;
		}

		if (m_mapRenderer != null) m_mapRenderer.DrawMap(this);
	}
}
