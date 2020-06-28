using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
	public MeshData[] meshDatas { get; private set; }
	public Color[] colorMap { get; private set; }
	public Vector2Int size { get; private set; }

	public ChunkData(ChunkData copy)
	{
		meshDatas = copy.meshDatas.Clone() as MeshData[];
		colorMap = copy.colorMap.Clone() as Color[];
		size = copy.size;
	}
	public ChunkData(MapInfo mapInfo, Vector2 position, Vector2Int size)
	{
		if (mapInfo.noiseMap != null)
		{
			var map = mapInfo.noiseMap.GetMap(new Vector2(position.x, -position.y), size);
			
			meshDatas = new MeshData[mapInfo.lodMax];
			for (int i = 0; i < mapInfo.lodMax; i++)
			{
				meshDatas[i] = new MeshData(map, mapInfo.heightMultiplier, mapInfo.heightCurve, i);
			}

			this.size = size;
			colorMap = TextureGenerator.GenerateColorMap(map, mapInfo.biomeInfo);
		}
	}
}
