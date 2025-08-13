using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		MapGenerator generator = (MapGenerator)target;
		if(GUILayout.Button("Build Map"))
		{
			generator.GenerateTileMap();
			generator.GenerateOther ();
		}
	}
}
