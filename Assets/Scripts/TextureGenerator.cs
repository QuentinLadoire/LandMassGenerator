using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapInfo = System.Collections.Generic.List<TerrainInfo>;

public class TextureGenerator
{
	public static Texture2D GenerateHeightMap(float[,] noiseMap)
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
	public static Texture2D GenerateColorMap(float[,] noiseMap, MapInfo mapInfo)
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
}
