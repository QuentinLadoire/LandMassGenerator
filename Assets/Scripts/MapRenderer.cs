using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using MapInfo = System.Collections.Generic.List<TerrainInfo>;

public class MapRenderer : MonoBehaviour
{
	[SerializeField] MapGenerator m_mapGenerator = null;

	[SerializeField] GameObject m_mesh = null;
	Renderer m_meshRenderer = null;
	MeshFilter m_meshFilter = null;

	Texture2D GenerateHeightMap(float[,] noiseMap)
	{
		if (noiseMap == null) return null;

		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		var heightMap = new Color[width * height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				heightMap[width * j + i] = Color.Lerp(Color.black, Color.white, noiseMap[i, j]);
			}
		}

		var texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;
		texture.SetPixels(heightMap);
		texture.Apply();

		return texture;
	}
	Texture2D GenerateColorMap(float[,] noiseMap, MapInfo mapInfo)
	{
		if (noiseMap == null) return null;

		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		var colorMap = new Color[width * height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < mapInfo.Count; k++)
				{
					if (noiseMap[i, j] <= mapInfo[k].height)
					{
						colorMap[width * j + i] = mapInfo[k].color;
						break;
					}
				}
			}
		}

		Texture2D texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colorMap);
		texture.Apply();

		return texture;
	}

	Texture2D GenerateMapTexture()
	{
		Texture2D texture = null;

		if (m_mapGenerator != null)
		{
			if (m_mapGenerator.drawMode == DrawMode.HeighMap) texture = GenerateHeightMap(m_mapGenerator.GetMap());
			else if (m_mapGenerator.drawMode == DrawMode.ColorMap) texture = GenerateColorMap(m_mapGenerator.GetMap(), m_mapGenerator.GetMapInfo());
		}

		return texture;
	}
	Mesh GenerateMapMesh()
	{
		Mesh mesh = null;

		if (m_mapGenerator != null)
		{
			var meshMap = MeshMap.GenerateMap(m_mapGenerator.noiseMap, m_mapGenerator.heightMultiplier, m_mapGenerator.heightCurve, m_mapGenerator.lodMax, m_mapGenerator.position);
			if (meshMap != null) mesh = meshMap.GetMesh(m_mapGenerator.lod);
		}

		return mesh;
	}

	public void DrawMap()
	{
		if (m_mesh != null)
		{
			m_meshFilter.sharedMesh = GenerateMapMesh();
			m_meshRenderer.sharedMaterial.mainTexture = GenerateMapTexture();
		}
	}

	private void OnValidate()
	{
		if (m_mesh != null)
		{
			m_meshFilter = m_mesh.GetComponent<MeshFilter>();
			m_meshRenderer = m_mesh.GetComponent<MeshRenderer>();
		}
	}
}
