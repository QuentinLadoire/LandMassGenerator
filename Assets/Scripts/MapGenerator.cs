using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BiomeInfo = System.Collections.Generic.List<TerrainInfo>;

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

public struct MapInfo
{
	public NoiseMap noiseMap;
	public float heightMultiplier;
	public AnimationCurve heightCurve;
	public int lodMax;
	public BiomeInfo biomeInfo;

	public MapInfo(NoiseMap noiseMap, float heightMultiplier, AnimationCurve heightCurve, int lodMax, BiomeInfo biomeInfo)
	{
		this.noiseMap = noiseMap;
		this.heightMultiplier = heightMultiplier;
		this.heightCurve = heightCurve;
		this.lodMax = lodMax;
		this.biomeInfo = biomeInfo;
	}
}

public class MapGenerator : MonoBehaviour
{
	[Header("Parameters")]
	public const int chunkSize = 241;

	[SerializeField] float m_scale = 20.0f;

	[SerializeField] int m_lodMax = 1;
	public int lodMax { get => m_lodMax; }

	[SerializeField] [Range(0.0f, 1.0f)] float m_lodPercent = 0;
	public int lod { get => (int)((m_lodMax - 1) * m_lodPercent); }

	[SerializeField] float m_heightMultiplier = 1.0f;
	public float heightMultiplier { get => m_heightMultiplier; }

	[SerializeField] AnimationCurve m_heightCurve = new AnimationCurve();
	public AnimationCurve heightCurve { get => m_heightCurve; }

	[SerializeField] int m_octaves = 4;
	[SerializeField] float m_lacunarity = 2;
	[SerializeField] [Range(0.0f, 1.0f)] float m_persistance = 0.5f;

	[SerializeField] int m_seed = 0;
	[SerializeField] Vector2 m_offset = Vector2.zero;

	[SerializeField] BiomeInfo m_biomeInfo = new BiomeInfo();
	public BiomeInfo biomeInfo { get => m_biomeInfo; }

	NoiseMap m_noiseMap = null;
	public NoiseMap noiseMap { get => m_noiseMap; }

	public MapInfo mapInfo { get; private set; }

	private void Start()
	{
		m_noiseMap = new NoiseMap(m_seed, m_scale, m_octaves, m_lacunarity, m_persistance, m_offset);
		mapInfo = new MapInfo(noiseMap, heightMultiplier, heightCurve, lodMax, biomeInfo);
	}
}
