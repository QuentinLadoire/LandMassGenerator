using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMap
{
	Mesh m_mesh = null;
	public Mesh mesh { get => m_mesh; }

	Vector3[] m_vertices = null;
	int[] m_triangles = null;
	Vector2[] m_uvs = null;

	int m_triangleIndex = 0;

	MeshMap(int width, int height)
	{
		m_mesh = null;

		m_vertices = new Vector3[width * height];
		m_triangles = new int[(width - 1) * (height - 1) * 6];
		m_uvs = new Vector2[width * height];

		m_triangleIndex = 0;
	}

	void AddTriangle(int a, int b, int c)
	{
		m_triangles[m_triangleIndex] = a;
		m_triangles[m_triangleIndex + 1] = b;
		m_triangles[m_triangleIndex + 2] = c;

		m_triangleIndex += 3;
	}

	void CreateMesh(float[,] noiseMap, float heightMultiplier, AnimationCurve heightCurve)
	{
		if (noiseMap != null)
		{
			int width = noiseMap.GetLength(0);
			int height = noiseMap.GetLength(1);

			float topLeftX = -((width - 1) / 2.0f);
			float topLeftZ = (height - 1) / 2.0f;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					int index = width * j + i;

					m_vertices[index] = new Vector3(topLeftX + i, heightCurve.Evaluate(noiseMap[i, j]) * heightMultiplier, topLeftZ - j);
					m_uvs[index] = new Vector2(i / (float)width, j / (float)height);

					if (i < width - 1 && j < height - 1)
					{
						AddTriangle(index, index + width + 1, index + width);
						AddTriangle(index + width + 1, index, index + 1);
					}
				}
			}
		}

		m_mesh = new Mesh();
		m_mesh.vertices = m_vertices;
		m_mesh.triangles = m_triangles;
		m_mesh.uv = m_uvs;
		m_mesh.RecalculateNormals();
	}

	public static MeshMap GenerateMap(NoiseMap noiseMap, float heightMultiplier, AnimationCurve heightCurve, Vector2 position)
	{
		if (noiseMap != null)
		{
			var meshMap = new MeshMap(noiseMap.width, noiseMap.height);
			meshMap.CreateMesh(noiseMap.GetMap(position), heightMultiplier, heightCurve);

			return meshMap;
		}

		return null;
	}
}
