using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap
{
	public int seed = 0;

	public int width = 0;
	public int height = 0;
	public float scale = 0.0f;
	public float heightMultiplier = 1.0f;

	public int octaves = 1;
	public float lacunarity = 2.0f;
	public float persistance = 0.5f;

	public Vector2 offset = Vector2.zero;

	Vector2[] octavesRandomOffset = null;

	NoiseMap(int seed, int width, int height, float scale, int octaves, float lacunarity, float persistance, Vector2 offset, Vector2[] octavesRandomOffset)
	{
		this.seed = seed;

		this.width = width;
		this.height = height;
		this.scale = scale;

		this.octaves = octaves;
		this.lacunarity = lacunarity;
		this.persistance = persistance;

		this.offset = offset;
		this.octavesRandomOffset = octavesRandomOffset;
	}

	public float[,] GetMap(Vector2 position)
	{
		float[,] noiseMap = new float[width, height];

		var minHeight = float.MaxValue;
		var maxHeight = float.MinValue;

		var halfWidth = width / 2.0f;
		var halfHeight = height / 2.0f;

		//Make height with a perlin noise
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				var frequency = 1.0f;
				var amplitude = 1.0f;
				var noiseHeight = 0.0f;

				for (int k = 0; k < octaves; k++)
				{
					var x = (i + position.x - halfWidth) / scale * frequency + (octavesRandomOffset[k].x + offset.x);
					var y = (j + position.y - halfHeight) / scale * frequency + (octavesRandomOffset[k].y + offset.y);

					var perlinValue = Mathf.PerlinNoise(x, y) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					frequency *= lacunarity;
					amplitude *= persistance;
				}

				if (noiseHeight < minHeight) minHeight = noiseHeight;
				else if (noiseHeight > maxHeight) maxHeight = noiseHeight;

				noiseMap[i, j] = noiseHeight;
			}
		}

		//Make height between 0 and 1.
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				noiseMap[i, j] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[i, j]);
			}
		}

		return noiseMap;
	}

	public static NoiseMap GenerateMap(int width, int height, int seed, float scale, int octaves, float lacunarity, float persistance, Vector2 offset)
	{
		Random.InitState(seed);

		//Make a random offset for each octaves
		Vector2[] octavesRandomOffset = new Vector2[octaves];
		for (int i = 0; i < octaves; i++)
		{
			float offsetX = Random.Range(-100000.0f, 100000.0f);
			float offsetY = Random.Range(-100000.0f, 100000.0f);

			octavesRandomOffset[i] = new Vector2(offsetX, offsetY);
		}

		return new NoiseMap(seed, width, height, scale, octaves, lacunarity, persistance, offset, octavesRandomOffset);
	}
}
