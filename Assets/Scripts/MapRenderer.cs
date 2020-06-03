using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using MapInfo = System.Collections.Generic.List<TerrainInfo>;

public class MapRenderer : MonoBehaviour
{
	[Header("Intern reference")]
	[SerializeField] GameObject m_plane = null;
	Renderer m_planeRenderer = null;

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

	Texture2D GenerateMapTexture(MapGenerator mapGenerator)
	{
		Texture2D texture = null;

		if (mapGenerator.drawMode == DrawMode.HeighMap) texture = GenerateHeightMap(mapGenerator.GetMap());
		else if (mapGenerator.drawMode == DrawMode.ColorMap) texture = GenerateColorMap(mapGenerator.GetMap(), mapGenerator.GetMapInfo());

		return texture;
	}
	Mesh GenerateMapMesh(MapGenerator mapGenerator)
	{
		Mesh mesh = null;

		if (mapGenerator != null)
		{
			var meshMap = MeshMap.GenerateMap(mapGenerator.noiseMap, mapGenerator.heightMultiplier, mapGenerator.heightCurve, mapGenerator.position);
			if (meshMap != null) mesh = meshMap.mesh;
		}

		return mesh;
	}

	public void DrawMap(MapGenerator mapGenerator)
	{
		if (m_plane != null)
		{
			m_planeRenderer.sharedMaterial.mainTexture = GenerateMapTexture(mapGenerator);
		}
		if (m_mesh != null)
		{
			m_meshFilter.sharedMesh = GenerateMapMesh(mapGenerator);
			m_meshRenderer.sharedMaterial.mainTexture = GenerateMapTexture(mapGenerator);
		}
	}

	private void OnValidate()
	{
		if (m_plane != null)
		{
			m_planeRenderer = m_plane.GetComponent<MeshRenderer>();
		}
		if (m_mesh != null)
		{
			m_meshFilter = m_mesh.GetComponent<MeshFilter>();
			m_meshRenderer = m_mesh.GetComponent<MeshRenderer>();
		}
	}
}
