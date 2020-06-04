using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public Mesh mesh = null;

	public Vector3[] vertices = null;
	public int[] triangles = null;
	public Vector2[] uvs = null;

	int m_triangleIndex = 0;

	void AddTriangles(int a, int b, int c)
	{
		triangles[m_triangleIndex] = a;
		triangles[m_triangleIndex + 1] = b;
		triangles[m_triangleIndex + 2] = c;

		m_triangleIndex += 3;
	}

	public void GenerateMesh(float[,] noiseMap, float heightMultiplier, AnimationCurve heightCurve, int lod)
	{
		if (noiseMap != null)
		{
			int width = noiseMap.GetLength(0);
			int height = noiseMap.GetLength(1);

			float topLeftX = -((width - 1) / 2.0f);
			float topLeftZ = (height - 1) / 2.0f;

			int lodIncrement = (lod == 0) ? 1 : (lod * 2);
			int size = ((width - 1) / lodIncrement) + 1;

			vertices = new Vector3[size * size];
			triangles = new int[(size - 1) * (size - 1) * 6];
			uvs = new Vector2[size * size];

			int vertexIndex = 0;
			for (int j = 0; j < height; j += lodIncrement)
			{
				for (int i = 0; i < width; i += lodIncrement)
				{
					vertices[vertexIndex] = new Vector3(topLeftX + i, heightCurve.Evaluate(noiseMap[i, j]) * heightMultiplier, topLeftZ - j);
					uvs[vertexIndex] = new Vector2(i / (float)width, j / (float)height);

					if (i < (width - 1) && j < (height - 1))
					{
						AddTriangles(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
						AddTriangles(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
					}

					vertexIndex++;
				}
			}

			mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;
			mesh.RecalculateNormals();
		}
	}
}

public class MeshMap
{
	List<MeshData> m_meshDatas = new List<MeshData>();

	public Mesh GetMesh(int lod)
	{
		if (lod < m_meshDatas.Count && lod >= 0)
			return m_meshDatas[lod].mesh;

		return null;
	}

	public static MeshMap GenerateMap(NoiseMap noiseMap, float heightMultiplier, AnimationCurve heightCurve, int lodMax, Vector2 position)
	{
		if (noiseMap != null)
		{
			var meshMap = new MeshMap();

			for (int i = 0; i < lodMax; i++)
			{
				var meshData = new MeshData();
				meshData.GenerateMesh(noiseMap.GetMap(position), heightMultiplier, heightCurve, i);

				meshMap.m_meshDatas.Add(meshData);
			}

			return meshMap;
		}

		return null;
	}
}
