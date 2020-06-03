using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
	MapGenerator m_mapGenerator = null;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		m_mapGenerator = target as MapGenerator;

		if (GUILayout.Button("Generate Map"))
		{
			m_mapGenerator.GenerateMap();
		}
	}
}
