using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BiomeInfo = System.Collections.Generic.List<TerrainInfo>;

public class TextureGenerator
{
	public static Color[] GenerateHeightMap(float[,] noiseMap)
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

		return heightMap;
	}
	public static Color[] GenerateColorMap(float[,] noiseMap, BiomeInfo biomeInfo)
	{
		if (noiseMap == null) return null;

		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		var colorMap = new Color[width * height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < biomeInfo.Count; k++)
				{
					if (noiseMap[i, j] <= biomeInfo[k].height)
					{
						colorMap[width * j + i] = biomeInfo[k].color;
						break;
					}
				}
			}
		}
		
		return colorMap;
	}

	public static Texture2D GenerateTexture(Color[] colorMap, int width, int height)
	{
		Texture2D texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colorMap);
		texture.Apply();

		return texture;
	}
}
