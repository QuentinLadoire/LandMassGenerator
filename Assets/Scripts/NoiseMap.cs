using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap
{
	public int seed { get; set; }

	public float scale { get; set; }

	public int octaves { get; set; }
	public float lacunarity { get; set; }
	public float persistance { get; set; }

	Vector2[] octavesRandomOffset = null;

	public float[,] GetMap(Vector2 position, Vector2Int size)
	{
		float[,] noiseMap = new float[size.x, size.y];

		var halfWidth = size.x / 2.0f;
		var halfHeight = size.y / 2.0f;

		//Make height with a perlin noise
		for (int i = 0; i < size.x; i++)
		{
			for (int j = 0; j < size.y; j++)
			{
				var frequency = 1.0f;
				var amplitude = 1.0f;
				var noiseHeight = 0.0f;

				for (int k = 0; k < octaves; k++)
				{
					var x = (i + position.x + octavesRandomOffset[k].x - halfWidth) / scale * frequency;
					var y = (j + position.y + octavesRandomOffset[k].y - halfHeight) / scale * frequency;

					var perlinValue = Mathf.PerlinNoise(x, y);
					noiseHeight += perlinValue * amplitude;

					frequency *= lacunarity;
					amplitude *= persistance;
				}

				noiseMap[i, j] = noiseHeight;
			}
		}

		//Make height between 0 and 1.
		for (int i = 0; i < size.x; i++)
		{
			for (int j = 0; j < size.y; j++)
			{
				noiseMap[i, j] = Mathf.InverseLerp(0.0f, 1.5f, noiseMap[i, j]);
			}
		}

		return noiseMap;
	}

	public NoiseMap(int seed, float scale, int octaves, float lacunarity, float persistance, Vector2 offset)
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

		this.seed = seed;

		this.scale = scale;

		this.octaves = octaves;
		this.lacunarity = lacunarity;
		this.persistance = persistance;

		this.octavesRandomOffset = octavesRandomOffset;
	}
}
